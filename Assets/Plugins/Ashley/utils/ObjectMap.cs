using System;
using System.Collections;
using System.Collections.Generic;

namespace anygames.ashley.utils
{
	public class ObjectMap<K, V> : IEnumerable<Entry<K, V>> 
	{
		private static readonly int PRIME1 = (int)Convert.ToInt64(0xbe1f14b1);
		private static readonly int PRIME2 = (int)Convert.ToInt64(0xb4b82e39);
		private static readonly int PRIME3 = (int)Convert.ToInt64(0xced1c241);

		public int size;

		public K [] keyTable;
		public V [] valueTable;
		public int capacity, stashSize;

		private float _loadFactor;
		private int hashShift, mask, threshold;
		private int stashCapacity;
		private int pushIterations;

		private Entries<K,V> entries1, entries2;
		private Values<V> values1, values2;
		private Keys<K> keys1, keys2;

		/** Creates a new map with an initial capacity of 51 and a load factor of 0.8. */
		public ObjectMap ()
		{
			Init (51, 0.8f);
		}

		/** Creates a new map with a load factor of 0.8.
 		* @param initialCapacity If not a power of two, it is increased to the next nearest power of two. */
		public ObjectMap (int initialCapacity)
		{
			Init (initialCapacity, 0.8f);
		}

		public ObjectMap (int initialCapacity, float loadFactor)
		{
			Init (initialCapacity, loadFactor);
		}

		public ObjectMap (ObjectMap<K, V> map)
		{
			Init (map);
		}

		private void Init (int initialCapacity, float loadFactor)
		{
	
			if (initialCapacity < 0) throw new ArgumentException ("initialCapacity must be >= 0: " + initialCapacity);
			initialCapacity =  MathUtils.NextPowerOfTwo ((int)Math.Ceiling (initialCapacity / loadFactor));
			if (initialCapacity > 1 << 30) throw new ArgumentException ("initialCapacity is too large: " + initialCapacity);
			capacity = initialCapacity;

			if (loadFactor <= 0) throw new ArgumentException ("loadFactor must be > 0: " + loadFactor);
			this._loadFactor = loadFactor;

			threshold = (int)(capacity * loadFactor);
			mask = capacity - 1;
			hashShift = 31 - MathUtils.NumTrailingBinaryZeros (capacity);
			stashCapacity = Math.Max (3, (int)Math.Ceiling (Math.Log (capacity)) * 2);
			pushIterations = Math.Max (Math.Min (capacity, 8), (int)Math.Sqrt (capacity) / 8);
		
			keyTable = new K [capacity + stashCapacity];

			valueTable = new V [keyTable.Length];
		}

		/** Creates a new map identical to the specified map. */
		private void Init (ObjectMap<K,V> map)
		{
			Init ((int)Math.Floor (map.capacity * map._loadFactor), map._loadFactor);
			stashSize = map.stashSize;
			Array.Copy (map.keyTable, 0, keyTable, 0, map.keyTable.Length);
			Array.Copy (map.valueTable, 0, valueTable, 0, map.valueTable.Length);
			size = map.size;
		}



		public V get (K key)
		{
			int hashCode = key.GetHashCode ();
			int index = hashCode & mask;

			if (!key.Equals (keyTable [index])) {
				index = hash2 (hashCode);
				if (!key.Equals (keyTable [index])) {
					index = hash3 (hashCode);
					if (!key.Equals (keyTable [index])) return getStash (key);
				}
			}
			return valueTable [index];
		}


		private V getStash (K key)
		{
			K [] keyTable = this.keyTable;
			for (int i = capacity, n = i + stashSize; i < n; i++)
				if (key.Equals (keyTable [i])) return valueTable [i];
			return default (V);
		}

		/** Returns the value for the specified key, or the default value if the key is not in the map. */
		public V get (K key, V defaultValue)
		{
			int hashCode = key.GetHashCode ();
			int index = hashCode & mask;
			if (!key.Equals (keyTable [index])) {
				index = hash2 (hashCode);
				if (!key.Equals (keyTable [index])) {
					index = hash3 (hashCode);
					if (!key.Equals (keyTable [index])) return getStash (key, defaultValue);
				}
			}
			return valueTable [index];
		}

		private V getStash (K key, V defaultValue)
		{
			K [] keyTable = this.keyTable;
			for (int i = capacity, n = i + stashSize; i < n; i++)
				if (key.Equals (keyTable [i])) return valueTable [i];
			return defaultValue;
		}

		/** Returns the old value associated with the specified key, or null. */
		public V put (K key, V value)
		{
			if (key == null) throw new ArgumentException ("key cannot be null.");
			return put_internal (key, value);
		}

		private V put_internal (K key, V value)
		{
			K [] keyTable = this.keyTable;

			// Check for existing keys.
			int hashCode = key.GetHashCode ();
			int index1 = hashCode & mask;
			K key1 = keyTable [index1];
			if (key.Equals (key1)) {
				V oldValue = valueTable [index1];
				valueTable [index1] = value;
				return oldValue;
			}

			int index2 = hash2 (hashCode);
			K key2 = keyTable [index2];
			if (key.Equals (key2)) {
				V oldValue = valueTable [index2];
				valueTable [index2] = value;
				return oldValue;
			}

			int index3 = hash3 (hashCode);
			K key3 = keyTable [index3];
			if (key.Equals (key3)) {
				V oldValue = valueTable [index3];
				valueTable [index3] = value;
				return oldValue;
			}

			// Update key in the stash.
			for (int i = capacity, n = i + stashSize; i < n; i++) {
				if (key.Equals (keyTable [i])) {
					V oldValue = valueTable [i];
					valueTable [i] = value;
					return oldValue;
				}
			}

			// Check for empty buckets.
			if (key1 == null) {
				keyTable [index1] = key;
				valueTable [index1] = value;
				if (size++ >= threshold) resize (capacity << 1);
				return default(V);
			}

			if (key2 == null) {
				keyTable [index2] = key;
				valueTable [index2] = value;
				if (size++ >= threshold) resize (capacity << 1);
				return default (V);
			}

			if (key3 == null) {
				keyTable [index3] = key;
				valueTable [index3] = value;
				if (size++ >= threshold) resize (capacity << 1);
				return default (V);
			}

			push (key, value, index1, key1, index2, key2, index3, key3);
			return default (V);
		}

		public void removeStashIndex (int index)
		{
			// If the removed location was not last, move the last tuple to the removed location.
			stashSize--;
			int lastIndex = capacity + stashSize;
			if (index < lastIndex) {
				keyTable [index] = keyTable [lastIndex];
				valueTable [index] = valueTable [lastIndex];
				valueTable [lastIndex] = default(V);
			} else
				valueTable [index] = default(V);
		}

		public V remove (K key)
		{
			int hashCode = key.GetHashCode ();
			int index = hashCode & mask;
			if (key.Equals (keyTable [index])) {
				keyTable [index] = default(K);
				V oldValue = valueTable [index];
				valueTable [index] = default(V);
				size--;
				return oldValue;
			}

			index = hash2 (hashCode);
			if (key.Equals (keyTable [index])) {
				keyTable [index] = default (K);
				V oldValue = valueTable [index];
				valueTable [index] = default (V);
				size--;
				return oldValue;
			}

			index = hash3 (hashCode);
			if (key.Equals (keyTable [index])) {
				keyTable [index] = default (K);
				V oldValue = valueTable [index];
				valueTable [index] = default (V);
				size--;
				return oldValue;
			}

			return removeStash (key);
		}

		private V removeStash (K key)
		{
			K [] keyTable = this.keyTable;
			for (int i = capacity, n = i + stashSize; i < n; i++) {
				if (key.Equals (keyTable [i])) {
					V oldValue = valueTable [i];
					removeStashIndex (i);
					size--;
					return oldValue;
				}
			}
			return default(V);
		}




		/** Returns an iterator for the entries in the map. Remove is supported. Note that the same iterator instance is returned each
 		* time this method is called. Use the {@link Entries} constructor for nested or multithreaded iteration. */
		public Entries<K,V> entries ()
		{
			if (entries1 == null) {
				entries1 = new Entries <K,V>(this);
				entries2 = new Entries<K, V> (this);
			}
			if (!entries1.valid) {
				entries1.reset ();
				entries1.valid = true;
				entries2.valid = false;
				return entries1;
			}
			entries2.reset ();
			entries2.valid = true;
			entries1.valid = false;
			return entries2;
		}



		public IEnumerator<Entry<K, V>> GetEnumerator ()
		{
			return entries ();
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return entries ();
		}

		private void resize (int newSize)
		{
			int oldEndIndex = capacity + stashSize;

			capacity = newSize;
			threshold = (int)(newSize * _loadFactor);
			mask = newSize - 1;
			hashShift = 31 - MathUtils.NumTrailingBinaryZeros (newSize);
			stashCapacity = Math.Max (3, (int)Math.Ceiling (Math.Log (newSize)) * 2);
			pushIterations = Math.Max (Math.Min (newSize, 8), (int)Math.Sqrt (newSize) / 8);

			K [] oldKeyTable = keyTable;
			V [] oldValueTable = valueTable;

			keyTable = (new K [newSize + stashCapacity]);
			valueTable = new V [newSize + stashCapacity];

			int oldSize = size;
			size = 0;
			stashSize = 0;
			if (oldSize > 0) {
				for (int i = 0; i < oldEndIndex; i++) {
					K key = oldKeyTable [i];
					if (key != null) putResize (key, oldValueTable [i]);
				}
			}
		}


	/** Skips checks for existing keys. */
	private void putResize (K key, V value)
		{
			// Check for empty buckets.
			int hashCode = key.GetHashCode ();
			int index1 = hashCode & mask;
			K key1 = keyTable [index1];
			if (key1 == null) {
				keyTable [index1] = key;
				valueTable [index1] = value;
				if (size++ >= threshold) resize (capacity << 1);
				return;
			}

			int index2 = hash2 (hashCode);
			K key2 = keyTable [index2];
			if (key2 == null) {
				keyTable [index2] = key;
				valueTable [index2] = value;
				if (size++ >= threshold) resize (capacity << 1);
				return;
			}

			int index3 = hash3 (hashCode);
			K key3 = keyTable [index3];
			if (key3 == null) {
				keyTable [index3] = key;
				valueTable [index3] = value;
				if (size++ >= threshold) resize (capacity << 1);
				return;
			}

			push (key, value, index1, key1, index2, key2, index3, key3);
		}

		private void push (K insertKey, V insertValue, int index1, K key1, int index2, K key2, int index3, K key3)
		{
			K [] keyTable = this.keyTable;
			V [] valueTable = this.valueTable;
			int mask = this.mask;

			// Push keys until an empty bucket is found.
			K evictedKey;
			V evictedValue;
			int i = 0, pushIterations = this.pushIterations;
			do {
				// Replace the key and value for one of the hashes.
				switch (new Random().Next (2 + 1)) {
				case 0:
					evictedKey = key1;
					evictedValue = valueTable [index1];
					keyTable [index1] = insertKey;
					valueTable [index1] = insertValue;
					break;
				case 1:
					evictedKey = key2;
					evictedValue = valueTable [index2];
					keyTable [index2] = insertKey;
					valueTable [index2] = insertValue;
					break;
				default:
					evictedKey = key3;
					evictedValue = valueTable [index3];
					keyTable [index3] = insertKey;
					valueTable [index3] = insertValue;
					break;
				}

				// If the evicted key hashes to an empty bucket, put it there and stop.
				int hashCode = evictedKey.GetHashCode ();
				index1 = hashCode & mask;
				key1 = keyTable [index1];
				if (key1 == null) {
					keyTable [index1] = evictedKey;
					valueTable [index1] = evictedValue;
					if (size++ >= threshold) resize (capacity << 1);
					return;
				}

				index2 = hash2 (hashCode);
				key2 = keyTable [index2];
				if (key2 == null) {
					keyTable [index2] = evictedKey;
					valueTable [index2] = evictedValue;
					if (size++ >= threshold) resize (capacity << 1);
					return;
				}

				index3 = hash3 (hashCode);
				key3 = keyTable [index3];
				if (key3 == null) {
					keyTable [index3] = evictedKey;
					valueTable [index3] = evictedValue;
					if (size++ >= threshold) resize (capacity << 1);
					return;
				}

				if (++i == pushIterations) break;

				insertKey = evictedKey;
				insertValue = evictedValue;
			} while (true);

			putStash (evictedKey, evictedValue);
		}

		private void putStash (K key, V value)
		{
			if (stashSize == stashCapacity) {
				// Too many pushes occurred and the stash is full, increase the table size.
				resize (capacity << 1);
				put_internal (key, value);
				return;
			}
			// Store key in the stash.
			int index = capacity + stashSize;
			keyTable [index] = key;
			valueTable [index] = value;
			stashSize++;
			size++;
		}

		private int hash2 (int h)
		{
			h *= PRIME2;
			return (h ^ MathUtils.RightBitMove(h, hashShift)) & mask;
		}

		private int hash3 (int h)
		{
			h *= PRIME3;
			return (h ^ MathUtils.RightBitMove (h, hashShift)) & mask;
		}

	}

	public class Entry<K, V>
	{
		public K key;
		public V value;

		public String toString ()
		{
			return key + "=" + value;
		}
	}

	public abstract class MapIterator<K, V, I> : IEnumerable<I>, IEnumerator<I>
	{
		public bool hasNext;

		public ObjectMap<K, V> map;
		public int nextIndex, currentIndex;
		public bool valid = true;

		public I Current {
			get {
				throw new NotImplementedException ();
			}
		}

		object IEnumerator.Current {
			get {
				return this;
			}
		}

		public MapIterator (ObjectMap<K, V> map)
		{
			this.map = map;
			reset ();
		}

		public void reset ()
		{
			currentIndex = -1;
			nextIndex = -1;
			findNextIndex ();
		}

		public void findNextIndex ()
		{
			hasNext = false;
			K [] keyTable = map.keyTable;
			for (int n = map.capacity + map.stashSize; ++nextIndex < n;) {
				if (keyTable [nextIndex] != null) {
					hasNext = true;
					break;
				}
			}
		}


		public void remove ()
		{
			if (currentIndex < 0) throw new Exception ("next must be called before remove.");
			if (currentIndex >= map.capacity) {
				map.removeStashIndex (currentIndex);
				nextIndex = currentIndex - 1;
				findNextIndex ();
			} else {
				map.keyTable [currentIndex] = default (K);
				map.valueTable [currentIndex] = default (V);
			}
			currentIndex = -1;
			map.size--;
		}

		public abstract IEnumerator<I> GetEnumerator ();


		public abstract void Dispose ();

		public abstract bool MoveNext ();
		public abstract void Reset ();

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return this;
		}
	}


	public class Entries<K, V> : MapIterator<K, V, Entry<K, V>>
	{
		Entry<K, V> entry = new Entry<K, V> ();

		public Entries (ObjectMap<K, V> map) : base (map)
		{

		}

		public bool GetHasNext ()
		{
			if (!valid) throw new Exception ("#iterator() cannot be used nested.");
			return hasNext;
		}

		public override IEnumerator<Entry<K, V>> GetEnumerator ()
		{
			return this;
		}

		public override void Dispose ()
		{
		}

		public override bool MoveNext ()
		{
			if (!hasNext) throw new Exception ("No such element");
			if (!valid) throw new Exception ("#iterator() cannot be used nested.");
			K [] keyTable = map.keyTable;
			entry.key = keyTable [nextIndex];
			entry.value = map.valueTable [nextIndex];
			currentIndex = nextIndex;
			findNextIndex ();
			return hasNext;
		}

		public override void Reset ()
		{

		}
	}


	public class Values<V> : MapIterator<Object, V, V>
	{

		public Values (ObjectMap<Object, V> map) : base (map)
		{
		}

		public override void Dispose ()
		{
		}

		public override IEnumerator<V> GetEnumerator ()
		{
			return this;
		}

		public bool GetHasNext ()
		{
			if (!valid) throw new Exception ("#iterator() cannot be used nested.");
			return hasNext;
		}

		private V next ()
		{
			if (!hasNext) throw new Exception ();
			if (!valid) throw new Exception ("#iterator() cannot be used nested.");
			V value = map.valueTable [nextIndex];
			currentIndex = nextIndex;
			findNextIndex ();
			return value;
		}

		public override bool MoveNext ()
		{
			next ();
			return hasNext;
		}

		public override void Reset ()
		{
		}

		/** Returns a new array containing the remaining values. */
		public Array<V> toArray ()
		{
			return toArray (new Array<V> (true, map.size));
		}

		/** Adds the remaining values to the specified array. */
		public Array<V> toArray (Array<V> array)
		{
			while (hasNext)
				array.add (next ());
			return array;
		}
	}


	public class Keys<K> : MapIterator<K, Object, K>
	{

		public Keys (ObjectMap<K, Object> map) : base (map)
		{

		}

		public bool GetHasNext ()
		{
			if (!valid) throw new Exception ("#iterator() cannot be used nested.");
			return hasNext;
		}

		private K next ()
		{
			if (!hasNext) throw new Exception ("No such element");
			if (!valid) throw new Exception ("#iterator() cannot be used nested.");
			K key = map.keyTable [nextIndex];
			currentIndex = nextIndex;
			findNextIndex ();
			return key;
		}



		/** Returns a new array containing the remaining keys. */
		public Array<K> toArray ()
		{
			return toArray (new Array<K> (true, map.size));
		}

		/** Adds the remaining keys to the array. */
		public Array<K> toArray (Array<K> array)
		{
			while (hasNext)
				array.add (next ());
			return array;
		}

		public override IEnumerator<K> GetEnumerator ()
		{
			return this;
		}

		public override void Dispose ()
		{

		}

		public override bool MoveNext ()
		{
			next ();
			return hasNext;
		}

		public override void Reset ()
		{
		}
	}

}
