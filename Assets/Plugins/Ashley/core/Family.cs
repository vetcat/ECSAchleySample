using System;
using System.Collections.Generic;
using System.Text;
using anygames.ashley.utils;

namespace anygames.ashley.core
{
	/**
 * Represents a group of {@link Component}s. It is used to describe what {@link Entity} objects an {@link EntitySystem} should
 * process. Example: {@code Family.all(PositionComponent.class, VelocityComponent.class).get()} Families can't be instantiated
 * directly but must be accessed via a builder ( start with {@code Family.all()}, {@code Family.one()} or {@code Family.exclude()}
 * ), this is to avoid duplicate families that describe the same components.
 * @author Stefan Bachmann
 */
	public class Family
	{
		private static ObjectMap<String, Family> families = new ObjectMap<String, Family> ();
		private static Builder builder = Builder.Instance;
		private static int familyIndex = 0;
		private static readonly Bits zeroBits = new Bits ();

		private readonly Bits _all;
		private readonly Bits _one;
		private readonly Bits _exclude;
		private readonly int index;

		/** Private constructor, use static method Family.getFamilyFor() */
		private Family (Bits all, Bits any, Bits exclude)
		{
			this._all = all;
			this._one = any;
			this._exclude = exclude;
			this.index = familyIndex++;
		}

		/** @return This family's unique index */
		public int getIndex ()
		{
			return this.index;
		}

		/** @return Whether the entity matches the family requirements or not */
		public bool matches (Entity entity)
		{
			Bits entityComponentBits = entity.getComponentBits ();

			if (!entityComponentBits.containsAll (_all)) {
				return false;
			}

			if (!_one.isEmpty () && !_one.intersects (entityComponentBits)) {
				return false;
			}

			if (!_exclude.isEmpty () && _exclude.intersects (entityComponentBits)) {
				return false;
			}

			return true;
		}

		/**
		 * @param componentTypes entities will have to contain all of the specified components.
		 * @return A Builder singleton instance to get a family
		 */
		public static Builder all (params Type[] componentTypes) 
		{
			return builder.reset ().All (componentTypes);
		}
		

		/**
		 * @param componentTypes entities will have to contain at least one of the specified components.
		 * @return A Builder singleton instance to get a family
		 */
		public static Builder one (params Type [] componentTypes)
		{
			return builder.reset ().One (componentTypes);
		}

		/**
		 * @param componentTypes entities cannot contain any of the specified components.
		 * @return A Builder singleton instance to get a family
		 */
		public static Builder exclude (params Type [] componentTypes)
		{
			return builder.reset ().Exclude (componentTypes);
		}

		public class Builder
		{
			private static Builder _instance;

			private Builder ()
			{

			}

			public static Builder Instance {
				get {
					if (_instance == null) {
						_instance = new Builder ();
					}
					return _instance;
				}
			}

			private Bits _all = zeroBits;
			private Bits _one = zeroBits;
			private Bits _exclude = zeroBits;

			/**
			 * Resets the builder instance
			 * @return A Builder singleton instance to get a family
			 */
			public Builder reset ()
			{
				_all = zeroBits;
				_one = zeroBits;
				_exclude = zeroBits;
				return this;
			}

			/**
			 * @param componentTypes entities will have to contain all of the specified components.
			 * @return A Builder singleton instance to get a family
			 */
			public Builder All (params Type[] componentTypes)
			{
				_all = ComponentType.getBitsFor (componentTypes);
				return this;
			}

			/**
			 * @param componentTypes entities will have to contain at least one of the specified components.
			 * @return A Builder singleton instance to get a family
			 */
			public Builder One (params Type[] componentTypes)
			{ 
				_one = ComponentType.getBitsFor (componentTypes);
				return this;
			}

			/**
			 * @param componentTypes entities cannot contain any of the specified components.
			 * @return A Builder singleton instance to get a family
			 */
			public Builder Exclude (params Type[] componentTypes)
			{
				_exclude = ComponentType.getBitsFor (componentTypes);
				return this;
			}

			/** @return A family for the configured component types */
			public Family get ()
			{
				String hash = getFamilyHash (_all, _one, _exclude);
				Family family = families.get (hash, null);
				if (family == null) {
					family = new Family (_all, _one, _exclude);
					families.put (hash, family);
				}
				return family;
			}
		}

		public  override int GetHashCode ()
		{
			return index;
		}

		public override bool Equals (Object obj)
		{
			return this == obj;
		}

		private static String getFamilyHash (Bits all, Bits one, Bits exclude)
		{
			StringBuilder stringBuilder = new StringBuilder ();
			if (!all.isEmpty ()) {
				stringBuilder.Append ("{all:").Append (getBitsString (all)).Append ("}");
			}
			if (!one.isEmpty ()) {
				stringBuilder.Append ("{one:").Append (getBitsString (one)).Append ("}");
			}
			if (!exclude.isEmpty ()) {
				stringBuilder.Append ("{exclude:").Append (getBitsString (exclude)).Append ("}");
			}
			return stringBuilder.ToString ();
		}

		private static String getBitsString (Bits bits)
		{
			StringBuilder stringBuilder = new StringBuilder ();

			int numBits = bits.length ();
			for (int i = 0; i < numBits; ++i) {
				stringBuilder.Append (bits.get (i) ? "1" : "0");
			}

			return stringBuilder.ToString ();
		}
	}

}
