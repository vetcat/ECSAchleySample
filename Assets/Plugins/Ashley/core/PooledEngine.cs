using System;
using anygames.ashley.utils;
using System.Collections.Generic;

namespace anygames.ashley.core
{
	/**
	 * Supports {@link Entity} and {@link Component} pooling. This improves performance in environments where creating/deleting
	 * entities is frequent as it greatly reduces memory allocation.
	 * <ul>
	 * <li>Create entities using {@link #createEntity()}</li>
	 * <li>Create components using {@link #createComponent(Class)}</li>
	 * <li>Components should implement the {@link Poolable} interface when in need to reset its state upon removal</li>
	 * </ul>
	 * @author David Saltares
	 */
	public class PooledEngine : Engine {

		private EntityPool entityPool;
		private ComponentPools componentPools;

		/**
		 * Creates a new PooledEngine with a maximum of 100 entities and 100 components of each type. Use
		 * {@link #PooledEngine(int, int, int, int)} to configure the entity and component pools.
		 */
		public PooledEngine ()
		{
			componentPools = new ComponentPools (10, 100);
			entityPool = new EntityPool (componentPools, 10, 100);
		}

		/**
		 * Creates new PooledEngine with the specified pools size configurations.
		 * @param entityPoolInitialSize initial number of pre-allocated entities.
		 * @param entityPoolMaxSize maximum number of pooled entities.
		 * @param componentPoolInitialSize initial size for each component type pool.
		 * @param componentPoolMaxSize maximum size for each component type pool.
		 */
		public PooledEngine (int entityPoolInitialSize, int entityPoolMaxSize, int componentPoolInitialSize, int componentPoolMaxSize)
		{
			componentPools = new ComponentPools (componentPoolInitialSize, componentPoolMaxSize);
			entityPool = new EntityPool (componentPools, entityPoolInitialSize, entityPoolMaxSize);
		}

		/** @return Clean {@link Entity} from the Engine pool. In order to add it to the {@link Engine}, use {@link #addEntity(Entity)}. @{@link Override {@link Engine#createEntity()}} */
		public override Entity createEntity ()
		{
			return entityPool.obtain ();
		}

		/**
		 * Retrieves a new {@link Component} from the {@link Engine} pool. It will be placed back in the pool whenever it's removed
		 * from an {@link Entity} or the {@link Entity} itself it's removed.
		 * Overrides the default implementation of Engine (creating a new Object)
		 */
		public override T createComponent<T> ()
		{
			return componentPools.obtain<T> (typeof(T));
		}

		/**
		 * Removes all free entities and components from their pools. Although this will likely result in garbage collection, it will
		 * free up memory.
		 */
		public void clearPools ()
		{
			entityPool.clear ();
			componentPools.clear ();
		}

		internal override void removeEntityInternal (Entity entity)
		{
			base.removeEntityInternal (entity);

			if (entity is PooledEntity) {
				entityPool.free ((PooledEntity)entity);
			}
		}

		private class PooledEntity : Entity, Poolable {

			private readonly ComponentPools _componentPools;

			public PooledEntity (ComponentPools componentPools)
			{
				this._componentPools = componentPools;
			}

			public override Component remove (Type componentClass)
			{
				Component component = base.remove (componentClass);

				if (component != null) {
					_componentPools.free (component);
				}

				return component;
			}

			public void reset ()
			{
				removeAll ();
				flags = 0;
				componentAdded.removeAllListeners ();
				componentRemoved.removeAllListeners ();
				ScheduledForRemoval = false;
				Removing = false;
			}
		}

		private class EntityPool : Pool<PooledEntity> {

			private readonly ComponentPools _componentPools;

			public EntityPool (ComponentPools componentPools, int initialSize, int maxSize) : base (initialSize, maxSize)
			{
				this._componentPools = componentPools;
			}

			protected override PooledEntity newObject ()
			{
				return new PooledEntity (_componentPools);
			}
		}

		private class ComponentPools
		{
			private Dictionary<Type, utils.ReflectionPool<Component>> pools;
			private int initialSize;
			private int maxSize;

			public ComponentPools (int initialSize, int maxSize)
			{
				this.pools = new Dictionary<Type, ReflectionPool<Component>> ();
				this.initialSize = initialSize;
				this.maxSize = maxSize;
			}

			public  T obtain<T> (Type type)
			{
				ReflectionPool<Component> pool = pools.ContainsKey (type) ? pools[type] : null;

				if (pool == null) {
					pool = new ReflectionPool<Component> (type, initialSize, maxSize);
					pools.Add (typeof(T), pool);
				}

				return (T)pool.obtain ();
			}

			public void free (Component obj)
			{
				if (obj == null) {
						throw new ArgumentException ("object cannot be null.");
				}

				ReflectionPool<Component> pool = pools[obj.GetType ()];

				if (pool == null) {
					return; // Ignore freeing an object that was never retained.
				}

				pool.free (obj);
			}

			public void freeAll (Component[] objects)
			{
				if (objects == null) throw new ArgumentException ("objects cannot be null.");

					for (int i = 0, n = objects.Length; i < n; i++) {
					Component obj = objects[i];
					if (obj == null) continue;
					free (obj);
				}
			}

			public void clear ()
			{
				foreach (var pool in pools.Values) {
					pool.clear ();
				}
			}
		}
	}
}
