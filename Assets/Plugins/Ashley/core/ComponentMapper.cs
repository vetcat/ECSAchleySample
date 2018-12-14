using System;

namespace anygames.ashley.core
{
	/**
	 * Provides super fast {@link Component} retrieval from {@Link Entity} objects.
	 * @param <T> the class type of the {@link Component}.
	 * @author David Saltares
	 */
	public sealed class ComponentMapper<T> where T : Component
	{
		private readonly ComponentType componentType;

		/**
		 * @param componentClass Component class to be retrieved by the mapper.
		 * @return New instance that provides fast access to the {@link Component} of the specified class.
		 */
		public static ComponentMapper<T> getFor ()
		{
			return new ComponentMapper<T> (typeof(T));
		}

		/** @return The {@link Component} of the specified class belonging to entity. */
		public T get (Entity entity)
		{
			return entity.getComponent<T> ();
		}

		/** @return Whether or not entity has the component of the specified class. */
		public bool has (Entity entity)
		{
			return entity.hasComponent (componentType);
		}

		private ComponentMapper (Type componentClass)
		{
			componentType = ComponentType.getFor (componentClass);
		}
	}
}
