using anygames.ashley.utils;
using System;
using System.Collections.Generic;

namespace anygames.ashley.core
{
	class EntityManager
	{
		private EntityListener listener;
		private readonly List<Entity> entities = new List<Entity> ();
		private readonly List<Entity> entitySet = new List<Entity> ();
		private readonly ImmutableArray<Entity> immutableEntities;
		private readonly List<EntityOperation> pendingOperations = new List<EntityOperation> ();
		private readonly EntityOperationPool entityOperationPool = new EntityOperationPool ();

		public EntityManager (EntityListener listener)
		{
			this.listener = listener;
			immutableEntities = new ImmutableArray<Entity> (entities);
		}

		public void addEntity (Entity entity)
		{
			addEntity (entity, false);
		}

		public void addEntity (Entity entity, bool delayed)
		{
			if (delayed) {
				EntityOperation operation = entityOperationPool.obtain ();
				operation.entity = entity;
				operation.type = EntityOperation.Type.Add;
				pendingOperations.Add (operation);
			} else {
				addEntityInternal (entity);
			}
		}

		public void removeEntity (Entity entity)
		{
			removeEntity (entity, false);
		}

		public void removeEntity (Entity entity, bool delayed)
		{
			if (delayed) {
				if (entity.isScheduledForRemoval ()) {
					return;
				}
				entity.ScheduledForRemoval = true;
				EntityOperation operation = entityOperationPool.obtain ();
				operation.entity = entity;
				operation.type = EntityOperation.Type.Remove;
				pendingOperations.Add (operation);
			} else {
				removeEntityInternal (entity);
			}
		}

		public void removeAllEntities ()
		{
			removeAllEntities (false);
		}

		public void removeAllEntities (bool delayed)
		{
			if (delayed) {
				foreach (var entity in entities) {
					entity.ScheduledForRemoval = true;
				}
				EntityOperation operation = entityOperationPool.obtain ();
				operation.type = EntityOperation.Type.RemoveAll;
				pendingOperations.Add (operation);
			} else {
				while (entities.Count > 0) {
					removeEntity (entities[0], false);
				}
			}
		}

		public ImmutableArray<Entity> getEntities ()
		{
			return immutableEntities;
		}

		public void processPendingOperations ()
		{
			for (int i = 0; i < pendingOperations.Count; ++i) {
				EntityOperation operation = pendingOperations[i];

				switch (operation.type) {
					case EntityOperation.Type.Add: addEntityInternal (operation.entity); break;
					case EntityOperation.Type.Remove: removeEntityInternal (operation.entity); break;
					case EntityOperation.Type.RemoveAll:
					while (entities.Count > 0) {
						removeEntityInternal (entities[0]);
					}
					break;
				default:
					throw new ArgumentException ("Unexpected EntityOperation type");
				}

				entityOperationPool.free (operation);
			}

			pendingOperations.Clear ();
		}

		protected void removeEntityInternal (Entity entity)
		{
			var removed = entitySet.Remove (entity);

			if (removed) {
				entity.ScheduledForRemoval = false;
				entity.Removing = true;
				entities.Remove (entity);//, true);
				listener.entityRemoved (entity);
				entity.Removing = false;
			}
		}

		protected void addEntityInternal (Entity entity)
		{
			if (entitySet.Contains (entity)) {
				throw new ArgumentException ("Entity is already registered " + entity);
			}

			entities.Add (entity);
			entitySet.Add (entity);

			listener.entityAdded (entity);
		}

		private class EntityOperation : Poolable {
			public enum Type
			{
				Add,
				Remove,
				RemoveAll
			}

			public Type type;
			public Entity entity;

			public void reset ()
			{
				entity = null;
			}
		}

		private class EntityOperationPool : Pool<EntityOperation> {
			protected override EntityOperation newObject ()
			{
				return new EntityOperation ();
			}
		}
	}
}
