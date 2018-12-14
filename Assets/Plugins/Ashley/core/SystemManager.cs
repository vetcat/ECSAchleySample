using System;
using anygames.ashley.utils;
using System.Collections.Generic;

namespace anygames.ashley.core
{
	public class SystemManager
	{
		private SystemComparator systemComparator = new SystemComparator ();
		private List<EntitySystem> systems = new List<EntitySystem> ();
		private ImmutableArray<EntitySystem> immutableSystems;
		private Dictionary<Type, EntitySystem> systemsByClass = new Dictionary<Type, EntitySystem> ();
		private SystemListener listener;

		public SystemManager (SystemListener listener)
		{
			this.listener = listener;
			immutableSystems = new ImmutableArray<EntitySystem> (systems);
		}

		public void addSystem (EntitySystem system)
		{
			Type systemType = system.GetType ();
			EntitySystem oldSytem = getSystem(systemType);
			if (oldSytem != null) {
				removeSystem (oldSytem);
			}

			systems.Add (system);
			systemsByClass.Add (systemType, system);
			systems.Sort (systemComparator);
			listener.systemAdded (system);
		}

		public void removeSystem (EntitySystem system)
		{
			if (systems.Remove (system)) {//, true)) {
				systemsByClass.Remove (system.GetType ());
				listener.systemRemoved (system);
			}
		}


		public EntitySystem getSystem (Type type)
		{
			return systemsByClass.ContainsKey (type) ?  systemsByClass[type] : null;
		}

		public ImmutableArray<EntitySystem> getSystems ()
		{
			return immutableSystems;
		}

		public class SystemComparator : IComparer<EntitySystem>{
			public int Compare (EntitySystem a, EntitySystem b)
			{
				return a.priority > b.priority ? 1 : (a.priority == b.priority) ? 0 : -1;
			}
		}

		public interface SystemListener
		{
			void systemAdded (EntitySystem system);
			void systemRemoved (EntitySystem system);
		}
	}

}
