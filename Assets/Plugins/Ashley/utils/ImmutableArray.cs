using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace anygames.ashley.utils
{
	/**
	 * Wrapper class to treat {@link Array} objects as if they were immutable. However, note that the values could be modified if they
	 * are mutable.
	 * @author David Saltares
	 */
	public class ImmutableArray<T> : IEnumerable<T> {
		private readonly List<T> array;
		private IEnumerable<T> iterable;

		public ImmutableArray (List<T> array)
		{
			this.array = array;
		}

		public int size ()
		{
			return array.Count;
		}

		public T get (int index)
		{
			return array[index];
		}

		public bool contains (T value, bool identity)
		{
			return array.Contains (value);
		}

		public int indexOf (T value, bool identity)
		{
			return array.IndexOf (value);
		}

		//public int lastIndexOf (T value, bool identity)
		//{
		//	return array.lastIndexOf (value, identity);
		//}


		//public T peek ()
		//{
		//	return array.peek ();
		//}

		//public T first ()
		//{
		//	return array.first ();
		//}

		//public T random ()
		//{
		//	return array.random ();
		//}

		public T [] toArray ()
		{
			return array.ToArray ();
		}

		//public V [] toArray<V> (Type type)
		//{
		//	return array.ToArray<V> ();
		//}

		public override int GetHashCode ()
		{
			return array.GetHashCode ();
		}

		public override bool Equals (Object obj)
		{
			return array.Equals (obj);
		}

		public String toString ()
		{
			return array.ToString ();
		}

		public IEnumerator<T> GetEnumerator ()
		{
			return array.GetEnumerator ();
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return array.GetEnumerator ();
		}
	}

}
