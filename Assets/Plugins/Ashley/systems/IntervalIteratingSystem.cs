using anygames.ashley.core;
using anygames.ashley.utils;

namespace anygames.ashley.systems
{
	/**
	 * A simple {@link EntitySystem} that processes a {@link Family} of entities not once per frame, but after a given interval.
	 * Entity processing logic should be placed in {@link IntervalIteratingSystem#processEntity(Entity)}.
	 * @author David Saltares
	 */
	public abstract class IntervalIteratingSystem : IntervalSystem {
		private Family family;
		private ImmutableArray<Entity> entities;

		/**
		 * @param family represents the collection of family the system should process
		 * @param interval time in seconds between calls to {@link IntervalIteratingSystem#updateInterval()}.
		 */
		public IntervalIteratingSystem (Family family, float interval) : base (interval)
		{
			this.family = family;
		}

		/**
		 * @param family represents the collection of family the system should process
		 * @param interval time in seconds between calls to {@link IntervalIteratingSystem#updateInterval()}.
		 * @param priority
		 */
		public IntervalIteratingSystem (Family family, float interval, int priority) : base(interval, priority)
		{
			this.family = family;
		}

		public override void addedToEngine (Engine engine)
		{
			entities = engine.getEntitiesFor (family);
		}

		protected override void updateInterval ()
		{
			for (int i = 0; i < entities.size (); ++i) {
				processEntity (entities.get (i));
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
		 * The user should place the entity processing logic here.
		 * @param entity
		 */
		protected abstract void processEntity (Entity entity);
	}

}
