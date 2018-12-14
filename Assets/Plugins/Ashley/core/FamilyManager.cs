using System;
using anygames.ashley.utils;
using System.Collections.Generic;

namespace anygames.ashley.core
{
	class FamilyManager
	{
		private ImmutableArray<Entity> entities;
		private Dictionary<Family, List<Entity>> families = new Dictionary<Family, List<Entity>> ();
		private Dictionary<Family, ImmutableArray<Entity>> immutableFamilies = new Dictionary<Family, ImmutableArray<Entity>> ();
		private SnapshotArray<EntityListenerData> entityListeners = new SnapshotArray<EntityListenerData> ();
		private Dictionary<Family, Bits> entityListenerMasks = new Dictionary<Family, Bits> ();
		private BitsPool bitsPool = new BitsPool ();
		private bool notifying = false;

		public FamilyManager (ImmutableArray<Entity> entities)
		{
			this.entities = entities;
		}

		public ImmutableArray<Entity> getEntitiesFor (Family family)
		{
			return registerFamily (family);
		}

		public bool Notifying ()
		{
			return notifying;
		}

		public void addEntityListener (Family family, int priority, EntityListener listener)
		{
			registerFamily (family);

			int insertionIndex = 0;
			while (insertionIndex < entityListeners.size) {
				if (entityListeners.get (insertionIndex).priority <= priority) {
					insertionIndex++;
				} else {
					break;
				}
			}

			// Shift up bitmasks by one step
			foreach (var mask in entityListenerMasks.Values) {
				//var mask = entry.value;
				for (int k = mask.length (); k > insertionIndex; k--) {
					if (mask.get (k - 1)) {
						mask.set (k);
					} else {
						mask.clear (k);
					}
				}
				mask.clear (insertionIndex);
			}

			entityListenerMasks[family].set (insertionIndex);

			EntityListenerData entityListenerData = new EntityListenerData ();
			entityListenerData.listener = listener;
			entityListenerData.priority = priority;
			entityListeners.insert (insertionIndex, entityListenerData);
		}

		public void removeEntityListener (EntityListener listener)
		{
			for (int i = 0; i < entityListeners.size; i++) {
				EntityListenerData entityListenerData = entityListeners.get (i);
				if (entityListenerData.listener == listener) {
					// Shift down bitmasks by one step
					foreach (var mask in entityListenerMasks.Values) {
						//var mask = entry.value;
						for (int k = i, n = mask.length (); k < n; k++) {
							if (mask.get (k + 1)) {
								mask.set (k);
							} else {
								mask.clear (k);
							}
						}
					}

					entityListeners.removeIndex (i--);
				}
			}
		}

		public void updateFamilyMembership (Entity entity)
		{
			// Find families that the entity was added to/removed from, and fill
			// the bitmasks with corresponding listener bits.
			Bits addListenerBits = bitsPool.obtain ();
			Bits removeListenerBits = bitsPool.obtain ();

			foreach (var family in entityListenerMasks.Keys) {
				//var family = entry.key;
				int familyIndex = family.getIndex ();
				Bits entityFamilyBits = entity.getFamilyBits ();

				bool belongsToFamily = entityFamilyBits.get (familyIndex);
				bool matches = family.matches (entity) && !entity.Removing;

				if (belongsToFamily != matches) {
					Bits listenersMask = entityListenerMasks[family];
					List<Entity> familyEntities = families[family];
					if (matches) {
						addListenerBits.or (listenersMask);
						familyEntities.Add (entity);
						entityFamilyBits.set (familyIndex);
					} else {
						removeListenerBits.or (listenersMask);
						familyEntities.Remove (entity);//, true);
						entityFamilyBits.clear (familyIndex);
					}
				}
			}

			// Notify listeners; set bits match indices of listeners
			notifying = true;
			Object [] items = entityListeners.begin ();

			try {
				for (int i = removeListenerBits.nextSetBit (0); i >= 0; i = removeListenerBits.nextSetBit (i + 1)) {
					((EntityListenerData)items [i]).listener.entityRemoved (entity);
				}

				for (int i = addListenerBits.nextSetBit (0); i >= 0; i = addListenerBits.nextSetBit (i + 1)) {
					((EntityListenerData)items [i]).listener.entityAdded (entity);
				}
			} finally {
				addListenerBits.clear ();
				removeListenerBits.clear ();
				bitsPool.free (addListenerBits);
				bitsPool.free (removeListenerBits);
				entityListeners.end ();
				notifying = false;
			}
		}

		private ImmutableArray<Entity> registerFamily (Family family)
		{
			ImmutableArray<Entity> entitiesInFamily = immutableFamilies.ContainsKey (family) ? immutableFamilies[family] : null;

			if (entitiesInFamily == null) {
				
				List<Entity> familyEntities = new List<Entity> ();
				entitiesInFamily = new ImmutableArray<Entity> (familyEntities);
				families.Add (family, familyEntities);
				immutableFamilies.Add (family, entitiesInFamily);
				entityListenerMasks.Add (family, new Bits ());

				foreach (Entity entity in entities) {
					updateFamilyMembership (entity);
				}
			}

			return entitiesInFamily;
		}

		public class EntityListenerData
		{
			public EntityListener listener;
			public int priority;
		}

		public class BitsPool : Pool<Bits> 
		{
			protected override Bits newObject ()
			{
				return new Bits ();
			}
		}
	}

}
