using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace anygames.ashley.utils
{
	/** An unordered set where the keys are objects. This implementation uses cuckoo hashing using 3 hashes, random walking, and a
	 * small stash for problematic keys. Null keys are not allowed. No allocation is done except when growing the table size. <br>
	 * <br>
	 * This set performs very fast contains and remove (typically O(1), worst case O(log(n))). Add may be a bit slower, depending on
	 * hash collisions. Load factors greater than 0.91 greatly increase the chances the set will have to rehash to the next higher POT
	 * size.
	 * @author Nathan Sweet */
	public class ObjectSet<T> : IEnumerable<T>
	{
		private static readonly int PRIME1 = (int)Convert.ToInt64 (0xbe1f14b1);
		private static readonly int PRIME2 = (int)Convert.ToInt64 (0xb4b82e39);
		private static readonly int PRIME3 = (int)Convert.ToInt64 (0xced1c241);

		public int size;

		T [] keyTable;
		int capacity, stashSize;

		private float _loadFactor;
		private int hashShift, mask, threshold;
		private int stashCapacity;
		private int pushIterations;

		private ObjectSetIterator<T> iterator1, iterator2;

		/** Creates a new set with an initial capacity of 51 and a load factor of 0.8. */
		public ObjectSet ()
		{
			init (51, 0.8f);
		}

		/** Creates a new set with a load factor of 0.8.
		 * @param initialCapacity If not a power of two, it is increased to the next nearest power of two. */
		public ObjectSet (int initialCapacity)
		{
			init (initialCapacity, 0.8f);
		}

		/** Creates a new set with the specified initial capacity and load factor. This set will hold initialCapacity items before
		 * growing the backing table.
		 * @param initialCapacity If not a power of two, it is increased to the next nearest power of two. */
		public ObjectSet (int initialCapacity, float loadFactor)
		{
			init (initialCapacity, loadFactor);
		}

		/** Creates a new set identical to the specified set. */
		public ObjectSet (ObjectSet<T> set)
		{
			init ((int)Math.Floor (set.capacity * set._loadFactor), set._loadFactor);
			stashSize = set.stashSize;
			Array.Copy (set.keyTable, 0, keyTable, 0, set.keyTable.Length);
			size = set.size;
		}

		private void init (int initialCapacity, float loadFactor)
		{
			if (initialCapacity < 0) throw new ArgumentException ("initialCapacity must be >= 0: " + initialCapacity);
			initialCapacity = MathUtils.NextPowerOfTwo ((int)Math.Ceiling (initialCapacity / loadFactor));
			if (initialCapacity > 1 << 30) throw new ArgumentException ("initialCapacity is too large: " + initialCapacity);
			capacity = initialCapacity;

			if (loadFactor <= 0) throw new ArgumentException ("loadFactor must be > 0: " + loadFactor);
			this._loadFactor = loadFactor;

			threshold = (int)(capacity * loadFactor);
			mask = capacity - 1;
			hashShift = 31 - MathUtils.NumberOfTrailingZeros (capacity);
			stashCapacity = Math.Max (3, (int)Math.Ceiling (Math.Log (capacity)) * 2);
			pushIterations = Math.Max (Math.Min (capacity, 8), (int)Math.Sqrt (capacity) / 8);

			keyTable = new T [capacity + stashCapacity];
		}

		/** Returns true if the key was not already in the set. If this set already contains the key, the call leaves the set unchanged
		 * and returns false. */
		public bool add (T key)
		{
			if (key == null) throw new ArgumentException ("key cannot be null.");
			T [] keyTable = this.keyTable;

			// Check for existing keys.
			int hashCode = key.GetHashCode ();
			int index1 = hashCode & mask;

			T key1 = keyTable [index1];
			//UnityEngine.Debug.Log ("keyTable size:  " + keyTable.Length + ", index1: " + index1);
			if (key.Equals (key1)) return false;

			int index2 = hash2 (hashCode);
			T key2 = keyTable [index2];
			if (key.Equals (key2)) return false;

			int index3 = hash3 (hashCode);
			T key3 = keyTable [index3];
			if (key.Equals (key3)) return false;

			// Find key in the stash.
			for (int i = capacity, n = i + stashSize; i < n; i++)
				if (key.Equals (keyTable [i])) return false;

			// Check for empty buckets.
			if (key1 == null) {
				keyTable [index1] = key;
				if (size++ >= threshold) resize (capacity << 1);
				return true;
			}

			if (key2 == null) {
				keyTable [index2] = key;
				if (size++ >= threshold) resize (capacity << 1);
				return true;
			}

			if (key3 == null) {
				keyTable [index3] = key;
				if (size++ >= threshold) resize (capacity << 1);
				return true;
			}

			push (key, index1, key1, index2, key2, index3, key3);
			return true;
		}

		public void addAll (Array<T> array)
		{
			addAll (array, 0, array.size);
		}

		public void addAll (Array<T> array, int offset, int length)
		{
			if (offset + length > array.size)
				throw new ArgumentException ("offset + length must be <= size: " + offset + " + " + length + " <= " + array.size);
			addAll ((T [])array.items, offset, length);
		}

		public void addAll (T [] array)
		{
			addAll (array, 0, array.Length);
		}

		public void addAll (T [] array, int offset, int length)
		{
			ensureCapacity (length);
			for (int i = offset, n = i + length; i < n; i++)
				add (array [i]);
		}

		public void addAll (ObjectSet<T> set)
		{
			ensureCapacity (set.size);
			foreach (T key in set)
				add (key);
		}

		/** Skips checks for existing keys. */
		private void addResize (T key)
		{
			// Check for empty buckets.
			int hashCode = key.GetHashCode ();
			int index1 = hashCode & mask;
			T key1 = keyTable [index1];
			if (key1 == null) {
				keyTable [index1] = key;
				if (size++ >= threshold) resize (capacity << 1);
				return;
			}

			int index2 = hash2 (hashCode);
			T key2 = keyTable [index2];
			if (key2 == null) {
				keyTable [index2] = key;
				if (size++ >= threshold) resize (capacity << 1);
				return;
			}

			int index3 = hash3 (hashCode);
			T key3 = keyTable [index3];
			if (key3 == null) {
				keyTable [index3] = key;
				if (size++ >= threshold) resize (capacity << 1);
				return;
			}

			push (key, index1, key1, index2, key2, index3, key3);
		}

		private void push (T insertKey, int index1, T key1, int index2, T key2, int index3, T key3)
		{
			T [] keyTable = this.keyTable;
			int mask = this.mask;

			// Push keys until an empty bucket is found.
			T evictedKey;
			int i = 0, pushIterations = this.pushIterations;
			do {
				// Replace the key and value for one of the hashes.
				switch (MathUtils.Random (2)) {
				case 0:
					evictedKey = key1;
					keyTable [index1] = insertKey;
					break;
				case 1:
					evictedKey = key2;
					keyTable [index2] = insertKey;
					break;
				default:
					evictedKey = key3;
					keyTable [index3] = insertKey;
					break;
				}

				// If the evicted key hashes to an empty bucket, put it there and stop.
				int hashCode = evictedKey.GetHashCode ();
				index1 = hashCode & mask;
				key1 = keyTable [index1];
				if (key1 == null) {
					keyTable [index1] = evictedKey;
					if (size++ >= threshold) resize (capacity << 1);
					return;
				}

				index2 = hash2 (hashCode);
				key2 = keyTable [index2];
				if (key2 == null) {
					keyTable [index2] = evictedKey;
					if (size++ >= threshold) resize (capacity << 1);
					return;
				}

				index3 = hash3 (hashCode);
				key3 = keyTable [index3];
				if (key3 == null) {
					keyTable [index3] = evictedKey;
					if (size++ >= threshold) resize (capacity << 1);
					return;
				}

				if (++i == pushIterations) break;

				insertKey = evictedKey;
			} while (true);

			addStash (evictedKey);
		}

		private void addStash (T key)
		{
			if (stashSize == stashCapacity) {
				// Too many pushes occurred and the stash is full, increase the table size.
				resize (capacity << 1);
				add (key);
				return;
			}
			// Store key in the stash.
			int index = capacity + stashSize;
			keyTable [index] = key;
			stashSize++;
			size++;
		}

		/** Returns true if the key was removed. */
		public bool remove (T key)
		{
			int hashCode = key.GetHashCode ();
			int index = hashCode & mask;
			if (key.Equals (keyTable [index])) {
				keyTable [index] = default (T);
				size--;
				return true;
			}

			index = hash2 (hashCode);
			if (key.Equals (keyTable [index])) {
				keyTable [index] = default (T);
				size--;
				return true;
			}

			index = hash3 (hashCode);
			if (key.Equals (keyTable [index])) {
				keyTable [index] = default (T);
				size--;
				return true;
			}

			return removeStash (key);
		}

		bool removeStash (T key)
		{
			T [] keyTable = this.keyTable;
			for (int i = capacity, n = i + stashSize; i < n; i++) {
				if (key.Equals (keyTable [i])) {
					removeStashIndex (i);
					size--;
					return true;
				}
			}
			return false;
		}

		void removeStashIndex (int index)
		{
			// If the removed location was not last, move the last tuple to the removed location.
			stashSize--;
			int lastIndex = capacity + stashSize;
			if (index < lastIndex) keyTable [index] = keyTable [lastIndex];
		}

		/** Reduces the size of the backing arrays to be the specified capacity or less. If the capacity is already less, nothing is
		 * done. If the set contains more items than the specified capacity, the next highest power of two capacity is used instead. */
		public void shrink (int maximumCapacity)
		{
			if (maximumCapacity < 0) throw new ArgumentException ("maximumCapacity must be >= 0: " + maximumCapacity);
			if (size > maximumCapacity) maximumCapacity = size;
			if (capacity <= maximumCapacity) return;
			maximumCapacity = MathUtils.NextPowerOfTwo (maximumCapacity);
			resize (maximumCapacity);
		}

		/** Clears the set and reduces the size of the backing arrays to be the specified capacity if they are larger. */
		public void clear (int maximumCapacity)
		{
			if (capacity <= maximumCapacity) {
				clear ();
				return;
			}
			size = 0;
			resize (maximumCapacity);
		}

		public void clear ()
		{
			if (size == 0) return;
			T [] keyTable = this.keyTable;
			for (int i = capacity + stashSize; i-- > 0;)
				keyTable [i] = default (T);
			size = 0;
			stashSize = 0;
		}

		public bool contains (T key)
		{
			int hashCode = key.GetHashCode ();
			int index = hashCode & mask;
			if (!key.Equals (keyTable [index])) {
				index = hash2 (hashCode);
				if (!key.Equals (keyTable [index])) {
					index = hash3 (hashCode);
					if (!key.Equals (keyTable [index])) return containsKeyStash (key);
				}
			}
			return true;
		}

		private bool containsKeyStash (T key)
		{
			T [] keyTable = this.keyTable;
			for (int i = capacity, n = i + stashSize; i < n; i++)
				if (key.Equals (keyTable [i])) return true;
			return false;
		}

		public T first ()
		{
			T [] keyTable = this.keyTable;
			for (int i = 0, n = capacity + stashSize; i < n; i++)
				if (keyTable [i] != null) return keyTable [i];
			throw new Exception ("ObjectSet is empty.");
		}

		/** Increases the size of the backing array to accommodate the specified number of additional items. Useful before adding many
		 * items to avoid multiple backing array resizes. */
		public void ensureCapacity (int additionalCapacity)
		{
			int sizeNeeded = size + additionalCapacity;
			if (sizeNeeded >= threshold) resize (MathUtils.NextPowerOfTwo ((int)Math.Ceiling (sizeNeeded / _loadFactor)));
		}

		private void resize (int newSize)
		{
			int oldEndIndex = capacity + stashSize;

			capacity = newSize;
			threshold = (int)(newSize * _loadFactor);
			mask = newSize - 1;
			hashShift = 31 - MathUtils.NumberOfTrailingZeros (newSize);
			stashCapacity = Math.Max (3, (int)Math.Ceiling (Math.Log (newSize)) * 2);
			pushIterations = Math.Max (Math.Min (newSize, 8), (int)Math.Sqrt (newSize) / 8);

			T [] oldKeyTable = keyTable;

			keyTable = new T [newSize + stashCapacity];

			int oldSize = size;
			size = 0;
			stashSize = 0;
			if (oldSize > 0) {
				for (int i = 0; i < oldEndIndex; i++) {
					T key = oldKeyTable [i];
					if (key != null) addResize (key);
				}
			}
		}

		private int hash2 (int h)
		{
			h *= PRIME2;

			return (h ^ MathUtils.RightBitMove (h, hashShift)) & mask;
		}

		private int hash3 (int h)
		{
			h *= PRIME3;
			return (h ^ MathUtils.RightBitMove (h, hashShift)) & mask;
		}

		public int hashCode ()
		{
			int h = 0;
			for (int i = 0, n = capacity + stashSize; i < n; i++)
				if (keyTable [i] != null) h += keyTable [i].GetHashCode ();
			return h;
		}

		public bool equals (Object obj)
		{
			if (!(obj is ObjectSet<T>)) return false;
			ObjectSet<T> other = (ObjectSet<T>)obj;
			if (other.size != size) return false;
			for (int i = 0, n = capacity + stashSize; i < n; i++)
				if (keyTable [i] != null && !other.contains (keyTable [i])) return false;
			return true;
		}

		public String toString ()
		{
			return '{' + toString (", ") + '}';
		}

		public String toString (String separator)
		{
			if (size == 0) return "";
			StringBuilder buffer = new StringBuilder (32);
			T [] keyTable = this.keyTable;
			int i = keyTable.Length;
			while (i-- > 0) {
				T key = keyTable [i];
				if (key == null) continue;
				buffer.Append (key);
				break;
			}
			while (i-- > 0) {
				T key = keyTable [i];
				if (key == null) continue;
				buffer.Append (separator);
				buffer.Append (key);
			}
			return buffer.ToString ();
		}

		public IEnumerator<T> GetEnumerator ()
		{
			if (iterator1 == null) {
				iterator1 = new ObjectSetIterator<T> (this);
				iterator2 = new ObjectSetIterator<T> (this);
			}
			if (!iterator1.valid) {
				iterator1.reset ();
				iterator1.valid = true;
				iterator2.valid = false;
				return iterator1;
			}
			iterator2.reset ();
			iterator2.valid = true;
			iterator1.valid = false;
			return iterator2;
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator();
		}

		//static public <T> ObjectSet<T> with (T...array)
		//{
		//	ObjectSet<T> set = new ObjectSet<T> ();
		//	set.addAll (array);
		//	return set;
		//}

		private class ObjectSetIterator<K> : IEnumerable<K>, IEnumerator<K>
		{
			public bool hasNext;

			readonly ObjectSet<K> set;
			public int nextIndex, currentIndex;
			public bool valid = true;

			public K Current {
				get {
					return default (K);
				}
			}

			object IEnumerator.Current {
				get {
					return this;
				}
			}

			public ObjectSetIterator (ObjectSet<K> set)
			{
				this.set = set;
				reset ();
			}

			public void reset ()
			{
				currentIndex = -1;
				nextIndex = -1;
				findNextIndex ();
			}

			void findNextIndex ()
			{
				hasNext = false;
				K [] keyTable = set.keyTable;
				for (int n = set.capacity + set.stashSize; ++nextIndex < n;) {
					if (keyTable [nextIndex] != null) {
						hasNext = true;
						break;
					}
				}
			}

			public void remove ()
			{
				if (currentIndex < 0) throw new Exception ("next must be called before remove.");
				if (currentIndex >= set.capacity) {
					set.removeStashIndex (currentIndex);
					nextIndex = currentIndex - 1;
					findNextIndex ();
				} else {
					set.keyTable [currentIndex] = default(K);
				}
				currentIndex = -1;
				set.size--;
			}

			public bool GetHasNext ()
			{
				if (!valid) throw new Exception ("#iterator() cannot be used nested.");
				return hasNext;
			}

			public K next ()
			{
				if (!hasNext) throw new Exception ("No such element");
				if (!valid) throw new Exception ("#iterator() cannot be used nested.");
				K key = set.keyTable [nextIndex];
				currentIndex = nextIndex;
				findNextIndex ();
				return key;
			}

			public ObjectSetIterator<K> iterator ()
			{
				return this;
			}

			/** Adds the remaining values to the array. */
			public Array<K> toArray (Array<K> array)
			{
				while (hasNext)
					array.add (next ());
				return array;
			}

			/** Returns a new array containing the remaining values. */
			public Array<K> toArray ()
			{
				return toArray (new Array<K> (true, set.size));
			}

			public IEnumerator<K> GetEnumerator ()
			{
				return this;
			}

			IEnumerator IEnumerable.GetEnumerator ()
			{
				return this;
			}

			public void Dispose ()
			{
			}

			public bool MoveNext ()
			{
				next ();
				return hasNext;
			}

			public void Reset ()
			{
			}
		}

	}
}
