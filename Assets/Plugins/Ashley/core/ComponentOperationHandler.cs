using System.Collections.Generic;
using anygames.ashley.utils;

namespace anygames.ashley.core
{
	class ComponentOperationHandler
	{
		private BooleanInformer delayed;
		private ComponentOperationPool operationPool = new ComponentOperationPool ();
		private List<ComponentOperation> operations = new List<ComponentOperation> ();
			
 		public ComponentOperationHandler (BooleanInformer delayed)
		{
			this.delayed = delayed;
		}

		public void add (Entity entity)
		{
			if (delayed.value ()) {
				ComponentOperation operation = operationPool.obtain ();
				operation.makeAdd (entity);
				operations.Add (operation);
			} else {
				entity.notifyComponentAdded ();
			}
		}

		public void remove (Entity entity)
		{
			if (delayed.value ()) {
				ComponentOperation operation = operationPool.obtain ();
				operation.makeRemove (entity);
				operations.Add (operation);
			} else {
				entity.notifyComponentRemoved ();
			}
		}

		public void processOperations ()
		{
			for (int i = 0; i < operations.Count; ++i) {
				ComponentOperation operation = operations[i];

				switch (operation.type) {
					case ComponentOperation.Type.Add:
					operation.entity.notifyComponentAdded ();
					break;
					case ComponentOperation.Type.Remove:
					operation.entity.notifyComponentRemoved ();
					break;
				default: break;
				}

				operationPool.free (operation);
			}

			operations.Clear ();
		}

		private class ComponentOperation : Poolable {
			
			public enum Type
			{
				Add,
				Remove,
			}

			public Type type;
			public Entity entity;

			public void makeAdd (Entity entity)
			{
				this.type = Type.Add;
				this.entity = entity;
			}

			public void makeRemove (Entity entity)
			{
				this.type = Type.Remove;
				this.entity = entity;
			}

			public void reset ()
			{
				entity = null;
			}
		}

		private class ComponentOperationPool : Pool<ComponentOperation> {
			protected override ComponentOperation newObject ()
			{
				return new ComponentOperation ();
			}
		}
	}

	interface BooleanInformer
	{
		bool value ();
	}
}
