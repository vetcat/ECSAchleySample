using System;
using System.Reflection;

namespace anygames.ashley.utils
{
	/** Pool that creates new instances of a type using reflection. The type must have a zero argument constructor.
	 * {@link Constructor#setAccessible(boolean)} will be used if the class and/or constructor is not visible.
	 * @author Nathan Sweet */
	public class ReflectionPool<T> : Pool<T> 
	{ 
		
		private readonly ConstructorInfo constructor;
		private readonly Type _type;

		public ReflectionPool (Type type) : base (16, int.MaxValue)
		{
			constructor = findConstructor (type);
			_type = type;
		}

		public ReflectionPool (Type type, int initialCapacity) : base(initialCapacity, int.MaxValue)
		{
			constructor = findConstructor (type);
			_type = type;
		}

		public ReflectionPool (Type type, int initialCapacity, int max) : base (initialCapacity, max)
		{
			constructor = findConstructor (type);
			if (constructor == null) {
				throw new Exception ("Class cannot be created (missing no-arg constructor): " + typeof (T).Name);
			}
			_type = type;
		}

		private ConstructorInfo findConstructor (Type type)
		{
			try {
					return type.GetConstructor (Type.EmptyTypes);
			} catch (Exception ex1) {
				try {
					//ConstructorInfo constructor = ClassReflection.getDeclaredConstructor (type, (Class [])null);
					//constructor.setAccessible (true);
					//return constructor;
					} catch (Exception ex2) {
					return null;
				}
			}
			return null;
		}

		protected override T newObject ()
		{
			try {
					return (T)Activator.CreateInstance (_type);
			} catch (Exception ex) {
					throw new Exception ("Unable to create new instance: " + constructor.GetType ().Name, ex);
			}
		}
	}

}
