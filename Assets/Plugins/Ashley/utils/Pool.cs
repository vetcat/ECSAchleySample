using System;
using System.Collections.Generic;

namespace anygames.ashley.utils
{
	public abstract class Pool<T>
	{
		/** The maximum number of objects that will be pooled. */
		public readonly int max;
		/** The highest number of free objects. Can be reset any time. */
		public int peak;

		private readonly Stack<T> freeObjects;

		/** Creates a pool with an initial capacity of 16 and no maximum. */
		public Pool ()
		{
			freeObjects = new Stack<T> (16);
			this.max = int.MaxValue;
		}

		/** Creates a pool with the specified initial capacity and no maximum. */
		public Pool (int initialCapacity)
		{
			freeObjects = new Stack<T> (initialCapacity);
			this.max = int.MaxValue;
		}

		/** @param max The maximum number of free objects to store in this pool. */
		public Pool (int initialCapacity, int max)
		{
			freeObjects = new Stack<T> (initialCapacity);
			this.max = max;
		}

		abstract protected T newObject ();

		/** Returns an object from this pool. The object may be new (from {@link #newObject()}) or reused (previously
		 * {@link #free(Object) freed}). */
		public T obtain ()
		{
			return freeObjects.Count == 0 ? newObject () : freeObjects.Pop ();
		}

		/** Puts the specified object in the pool, making it eligible to be returned by {@link #obtain()}. If the pool already contains
		 * {@link #max} free objects, the specified object is reset but not added to the pool. */
		public void free (T obj)
		{
			if (obj.Equals (null)) throw new ArgumentException ("object cannot be null.");
			if (freeObjects.Count < max) {
				freeObjects.Push (obj);
				peak = Math.Max (peak, freeObjects.Count);
			}
			reset (obj);
		}

		/** Called when an object is freed to clear the state of the object for possible later reuse. The default implementation calls
		 * {@link Poolable#reset()} if the object is {@link Poolable}. */
		protected void reset (T obj)
		{
			if (obj is Poolable) ((Poolable)obj).reset ();
		}

		/** Puts the specified objects in the pool. Null objects within the array are silently ignored.
		 * @see #free(Object) */
		public void freeAll (IList<T> objects)
		{
			if (objects == null) throw new ArgumentException ("objects cannot be null.");
			Stack<T> fo = this.freeObjects;
			int max = this.max;
			for (int i = 0; i < objects.Count; i++) {
				T obj = objects[i];
				if (obj.Equals (null)) continue;
				if (fo.Count < max) fo.Push (obj);
				reset (obj);
			}
			peak = Math.Max (peak, fo.Count);
		}

		/** Removes all free objects from this pool. */
		public void clear ()
		{
			freeObjects.Clear ();
		}

		/** The number of objects available to be obtained. */
		public int getFree ()
		{
			return freeObjects.Count;
		}
	}

	/** Objects implementing this interface will have {@link #reset()} called when passed to {@link #free(Object)}. */
	public interface Poolable
	{
		/** Resets the object for reuse. Object references should be nulled and fields may be set to default values. */
		void reset ();
	}
}
