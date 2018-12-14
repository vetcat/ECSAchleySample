using System;
using System.Collections.Generic;
using anygames.ashley.utils;

namespace anygames.ashley.core
{
	/// <summary>
	///	Uniquely identifies a {@link Component}
	/// sub-class. It assigns them an index which is used internally for fast comparison and
	/// retrieval.See { @link Family }
	/// and {@link Entity}. ComponentType is a package protected class. You cannot instantiate a
	///	 ComponentType.They can only be accessed via {
	///	@link #getIndexFor(Class<? extends Component>)}. Each component class will always
	///	 return the same instance of ComponentType.
	/// </summary>
	public sealed class ComponentType
	{
		private static Dictionary<Type, ComponentType> assignedComponentTypes = new Dictionary<Type, ComponentType> ();
		private static int typeIndex = 0;

		private readonly int index;

		private ComponentType ()
		{
			index = typeIndex++;
		}

		/// <summary>
		/// Gets the index.
		/// </summary>
		/// <returns>This ComponentType's unique index.</returns>
		public int getIndex ()
		{
			return index;
		}

		/// <summary>
		/// Gets for.
		/// </summary>
		/// <returns>A ComponentType matching the Component Class</returns>
		/// <param name="componentType">componentType The {@link Component} class</param>
		public static ComponentType getFor (Type componentType)
		{
			ComponentType type = assignedComponentTypes.ContainsKey (componentType) ? assignedComponentTypes [componentType]
			                                           : null;
			if (type == null) {
				type = new ComponentType ();
				assignedComponentTypes.Add (componentType, type);
			}

			return type;
		}

		/// <summary>
		/// Quick helper method. The same could be done via {@link ComponentType.getFor(Class<? extends Component>)}.
		/// </summary>
		/// <returns>The index for the specified {@link Component} Class</returns>
		/// <param name="componentType">componentType The {@link Component} class</param>
		public static int getIndexFor (Type componentType)
		{
			return getFor (componentType).getIndex ();
		}

		/// <summary>
		/// Gets the bits for.
		/// </summary>
		/// <returns>Bits representing the collection of components for quick comparison and matching. See {@link Family#getFor(Bits, Bits, Bits)}.</returns>
		/// <param name="">componentTypes list of {@link Component} classes</param>
		public static Bits getBitsFor (Type[] componentTypes)
		{
			Bits bits = new Bits ();

			int typesLength = componentTypes.Length;
			for (int i = 0; i < typesLength; i++) {
				bits.set (ComponentType.getIndexFor (componentTypes [i]));
			}

			return bits;
		}

		public override int GetHashCode ()
		{
			return index;
		}


		public override bool Equals (Object obj)
		{
			if (this == obj) return true;
			if (obj == null) return false;
			if (GetType () != obj.GetType ()) return false;
			ComponentType other = (ComponentType)obj;
			return index == other.index;
		}
	}

}
