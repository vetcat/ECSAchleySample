using anygames.ashley.core;
using anygames.ashley.utils;

namespace anygames.ashley.systems
{
	/**
	 * A simple EntitySystem that iterates over each entity and calls processEntity() for each entity every time the EntitySystem is
	 * updated. This is really just a convenience class as most systems iterate over a list of entities.
	 * @author Stefan Bachmann
	 */
	public abstract class IteratingSystem : EntitySystem {
		private Family family;
		private ImmutableArray<Entity> entities;

		/**
		 * Instantiates a system that will iterate over the entities described by the Family.
		 * @param family The family of entities iterated over in this System
		 */
		public IteratingSystem (Family family) : base(0)
		{
			this.family = family;
		}

		/**
		 * Instantiates a system that will iterate over the entities described by the Family, with a specific priority.
		 * @param family The family of entities iterated over in this System
		 * @param priority The priority to execute this system with (lower means higher priority)
		 */
		public IteratingSystem (Family family, int priority) : base(priority)
		{
			this.family = family;
		}

		public override void addedToEngine (Engine engine)
		{
			entities = engine.getEntitiesFor (family);
		}

		public override void removedFromEngine (Engine engine)
		{
			entities = null;
		}

		public override void update (float deltaTime)
		{
			for (int i = 0; i < entities.size (); ++i) {
				ProcessEntity (entities.get (i), deltaTime);
			}
		}

		/**
		 * @return set of entities processed by the system
		 */
		public ImmutableArray<Entity> getEntities ()
		{
			return entities;
		}

		/**
		 * @return the Family used when the system was created
		 */
		public Family getFamily ()
		{
			return family;
		}

		/**
		 * This method is called on every entity on every update call of the EntitySystem. Override this to implement your system's
		 * specific processing.
		 * @param entity The current Entity being processed
		 * @param deltaTime The delta time between the last and current frame
		 */
		protected abstract void ProcessEntity (Entity entity, float deltaTime);
	}

}
