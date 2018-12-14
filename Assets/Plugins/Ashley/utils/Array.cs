using System;
using System.Collections;
using System.Collections.Generic;

namespace anygames.ashley.utils
{
	/** A resizable, ordered or unordered array of objects. If unordered, this class avoids a memory copy when removing elements (the
	 * last element is moved to the removed element's position).
	 * @author Nathan Sweet */
	public class Array<T> : IEnumerable<T>
	{
		public T [] items;

		public int size;
		public bool ordered;

		private ArrayIterable iterable;

		public Array ()
		{
			this.ordered = true;
			items = new T [16];
		}

		/** Creates an ordered array with the specified capacity. */
		public Array (int capacity)
		{
			this.ordered = true;
			items = new T [capacity];
		}

		public Array (Type arrayType)
		{
			this.ordered = true;
			items = (T [])Activator.CreateInstance (arrayType, 16);
		}

		public Array (T [] array)
		{
			this.ordered = true;
			items = (T [])Activator.CreateInstance (array.GetType (), array.Length);
			size = array.Length;
			Array.Copy (array, 0, items, 0, size);
		}

		public Array (Array<T> array)
		{
			this.ordered = array.ordered;
			items = (T [])Activator.CreateInstance (array.items.GetType (), array.size);
			size = array.size;
			Array.Copy (array.items, 0, items, 0, size);
		}

		public Array (bool ordered, T [] array, int start, int count)
		{
			this.ordered = ordered;
			items = (T [])Activator.CreateInstance (array.GetType (), count);
			size = count;
			Array.Copy (array, start, items, 0, size);
		}

		/** @param ordered If false, methods that remove elements may change the order of other elements in the array, which avoids a
		 *           memory copy.
		 * @param capacity Any elements added beyond this will cause the backing array to be grown. */
		public Array (bool ordered, int capacity)
		{
			this.ordered = ordered;
			items = new T [capacity];
		}

		public Array (bool ordered, int capacity, Type arrayType)
		{
			this.ordered = ordered;
			items = (T [])Activator.CreateInstance (arrayType, capacity);
		}

		public void add (T value)
		{
			T [] items = this.items;
			if (size == items.Length) items = resize (Math.Max (8, (int)(size * 1.75f)));
			items [size++] = value;
		}

		public T get (int index)
		{
			if (index >= size) throw new IndexOutOfRangeException ("index can't be >= size: " + index + " >= " + size);
			return items [index];
		}

		/** Returns if this array contains value.
		 * @param value May be null.
		 * @param identity If true, == comparison will be used. If false, .equals() comparison will be used.
		 * @return true if array contains value, false if it doesn't */
		public bool contains (T value, bool identity)
		{
			T [] items = this.items;
			int i = size - 1;
			if (identity || value.Equals (null)) {
				while (i >= 0)
					if (items [i--].Equals (value)) return true;
			} else {
				while (i >= 0)
					if (value.Equals (items [i--])) return true;
			}
			return false;
		}

		/** Returns the index of first occurrence of value in the array, or -1 if no such value exists.
		 * @param value May be null.
		 * @param identity If true, == comparison will be used. If false, .equals() comparison will be used.
		 * @return An index of first occurrence of value in array or -1 if no such value exists */
		public int indexOf (T value, bool identity)
		{
			T [] items = this.items;
			if (identity || value.Equals (null)) {
				for (int i = 0, n = size; i < n; i++)
					if (items [i].Equals (value)) return i;
			} else {
				for (int i = 0, n = size; i < n; i++)
					if (value.Equals (items [i])) return i;
			}
			return -1;
		}

		/** Returns an index of last occurrence of value in array or -1 if no such value exists. Search is started from the end of an
		 * array.
		 * @param value May be null.
		 * @param identity If true, == comparison will be used. If false, .equals() comparison will be used.
		 * @return An index of last occurrence of value in array or -1 if no such value exists */
		public int lastIndexOf (T value, bool identity)
		{
			T [] items = this.items;
			if (identity || value.Equals (null)) {
				for (int i = size - 1; i >= 0; i--)
					if (items [i].Equals (value)) return i;
			} else {
				for (int i = size - 1; i >= 0; i--)
					if (value.Equals (items [i])) return i;
			}
			return -1;
		}

		/** Returns the last item. */
		public T peek ()
		{
			if (size == 0) throw new Exception ("Array is empty.");
			return items [size - 1];
		}

		/** Returns the first item. */
		public T first ()
		{
			if (size == 0) throw new Exception ("Array is empty.");
			return items [0];
		}

		/** Returns a random item from the array, or null if the array is empty. */
		public T random ()
		{
			if (size == 0) return default(T);
			return items [MathUtils.Random (0, size - 1)];
		}

		/** Returns the items as an array. Note the array is typed, so the {@link #Array(Class)} constructor must have been used.
 		* Otherwise use {@link #toArray(Class)} to specify the array type. */
		public T [] toArray ()
		{
			return toArray<T> (items.GetType ());
		}

		public V[] toArray<V> (Type type) 
		{
			var result = (V [])Activator.CreateInstance (type, size);
			Array.Copy (items, 0, result, 0, size);
			return result;
		}

		public virtual void clear ()
		{
			T [] items = this.items;
			for (int i = 0, n = size; i < n; i++)
				items [i] = default(T);
			size = 0;
		}


		/** Removes and returns the item at the specified index. */
		public virtual T removeIndex (int index)
		{
			if (index >= size) throw new IndexOutOfRangeException ("index can't be >= size: " + index + " >= " + size);
			T [] items = this.items;
			T value = (T)items [index];
			size--;
			if (ordered)
				Array.Copy (items, index + 1, items, index, size - index);
			else
				items [index] = items [size];
			items [size] = default(T);
			return value;
		}

		public virtual void set (int index, T value)
		{
			if (index >= size) throw new IndexOutOfRangeException ("index can't be >= size: " + index + " >= " + size);
			items [index] = value;
		}

		public virtual void insert (int index, T value)
		{
			if (index > size) throw new IndexOutOfRangeException ("index can't be > size: " + index + " > " + size);
			T [] items = this.items;
			if (size == items.Length) items = resize (Math.Max (8, (int)(size * 1.75f)));
			if (ordered)
				Array.Copy (items, index, items, index + 1, size - index);
			else
				items [size] = items [index];
			size++;
			items [index] = value;
		}

		public virtual void swap (int first, int second)
		{
			if (first >= size) throw new IndexOutOfRangeException ("first can't be >= size: " + first + " >= " + size);
			if (second >= size) throw new IndexOutOfRangeException ("second can't be >= size: " + second + " >= " + size);
			T [] items = this.items;
			T firstValue = items [first];
			items [first] = items [second];
			items [second] = firstValue;
		}

		/** Removes the first instance of the specified value in the array.
		 * @param value May be null.
		 * @param identity If true, == comparison will be used. If false, .equals() comparison will be used.
		 * @return true if value was found and removed, false otherwise */
		public virtual bool removeValue (T value, bool identity)
		{
			T [] items = this.items;
			if (identity || value.Equals (null)) {
				for (int i = 0, n = size; i < n; i++) {
					if (items [i].Equals (value)) {
						removeIndex (i);
						return true;
					}
				}
			} else {
				for (int i = 0, n = size; i < n; i++) {
					if (value.Equals (items [i])) {
						removeIndex (i);
						return true;
					}
				}
			}
			return false;
		}

		/** Removes the items between the specified indices, inclusive. */
		public virtual void removeRange (int start, int end)
		{
			if (end >= size) throw new IndexOutOfRangeException ("end can't be >= size: " + end + " >= " + size);
			if (start > end) throw new IndexOutOfRangeException ("start can't be > end: " + start + " > " + end);
			T [] items = this.items;
			int count = end - start + 1;
			if (ordered)
				Array.Copy (items, start + count, items, start, size - (start + count));
			else {
				int lastIndex = this.size - 1;
				for (int i = 0; i < count; i++)
					items [start + i] = items [lastIndex - i];
			}
			size -= count;
		}

		/** Removes from this array all of elements contained in the specified array.
	 * @param identity True to use ==, false to use .equals().
	 * @return true if this array was modified. */
		public virtual bool removeAll (Array<T> array, bool identity)
		{
			int size = this.size;
			int startSize = size;
			T [] items = this.items;
			if (identity) {
				for (int i = 0, n = array.size; i < n; i++) {
					T item = array.get (i);
					for (int ii = 0; ii < size; ii++) {
						if (item.Equals (items [ii])) {
							removeIndex (ii);
							size--;
							break;
						}
					}
				}
			} else {
				for (int i = 0, n = array.size; i < n; i++) {
					T item = array.get (i);
					for (int ii = 0; ii < size; ii++) {
						if (item.Equals (items [ii])) {
							removeIndex (ii);
							size--;
							break;
						}
					}
				}
			}
			return size != startSize;
		}

		/** Removes and returns the last item. */
		public virtual T pop ()
		{
			if (size == 0) throw new Exception ("Array is empty.");
			--size;
			T item = items [size];
			items [size] = default(T);
			return item;
		}

		/** Creates a new backing array with the specified size containing the current items. */
		protected T [] resize (int newSize)
		{
			T [] items = this.items;
			T [] newItems = (T [])Activator.CreateInstance (items.GetType (), newSize);
			Array.Copy (items, 0, newItems, 0, Math.Min (size, newItems.Length));
			this.items = newItems;
			return newItems;
		}

		public IEnumerator<T> GetEnumerator ()
		{
			if (iterable == null) iterable = new ArrayIterable (this);
			return iterable.GetEnumerator ();
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}

		private class ArrayIterator : IEnumerator<T>, IEnumerable<T> {
			private readonly Array<T> array;
			private readonly bool allowRemove;
			public int index;

			public bool valid = true;


			// ArrayIterable<T> iterable;

			public ArrayIterator (Array<T> array)
			{
				this.array = array;
				this.allowRemove = true;
			}

			public ArrayIterator (Array<T> array, bool allowRemove)
			{
				this.array = array;
				this.allowRemove = allowRemove;
			}

			public bool hasNext ()
			{
				if (!valid) {
					// System.out.println(iterable.lastAcquire);
					throw new Exception ("#iterator() cannot be used nested.");
				}
				return index < array.size;
			}

			public void remove ()
			{
				if (!allowRemove) throw new Exception ("Remove not allowed.");
				index--;
				array.removeIndex (index);
			}


			public bool MoveNext ()
			{
				if (index > array.size) throw new Exception ("no such element");
				if (!valid) {
					// System.out.println(iterable.lastAcquire);
					throw new Exception ("#iterator() cannot be used nested.");
				}
				return array.size < index++;
			}

			public void Dispose ()
			{
	
			}

			public void Reset ()
			{
				index = 0;
			}

			public IEnumerator<T> GetEnumerator ()
			{
				return this;
			}

			IEnumerator IEnumerable.GetEnumerator ()
			{
				return this;
			}

			public T Current {
				get {
					return default(T);
				}
			}

			object IEnumerator.Current {
				get {
					return this;
				}
			}
		}	


		private class ArrayIterable : IEnumerable<T> {
			
			private readonly Array<T> array;
			private readonly bool allowRemove;
			private ArrayIterator iterator1, iterator2;

			// java.io.StringWriter lastAcquire = new java.io.StringWriter();

			public ArrayIterable (Array<T> array)
			{
				this.array = array;
				this.allowRemove = true;
			}

			public ArrayIterable (Array<T> array, bool allowRemove)
			{
				this.array = array;
				this.allowRemove = allowRemove;
			}


			public IEnumerator<T> GetEnumerator ()
			{
				// lastAcquire.getBuffer().setLength(0);
				// new Throwable().printStackTrace(new java.io.PrintWriter(lastAcquire));
				if (iterator1 == null) {
					iterator1 = new ArrayIterator (array, allowRemove);
					iterator2 = new ArrayIterator (array, allowRemove);
					// iterator1.iterable = this;
					// iterator2.iterable = this;
				}
				if (!iterator1.valid) {
					iterator1.index = 0;
					iterator1.valid = true;
					iterator2.valid = false;
					return iterator1;
				}
				iterator2.index = 0;
				iterator2.valid = true;
				iterator1.valid = false;
				return iterator2;
			}

			IEnumerator IEnumerable.GetEnumerator ()
			{
				return GetEnumerator();
			}
		}

	}
}
