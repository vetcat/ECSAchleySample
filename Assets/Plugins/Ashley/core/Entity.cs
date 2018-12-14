using anygames.ashley.signals;
using anygames.ashley.utils;
using System.Collections.Generic;
using System;

namespace anygames.ashley.core
{
	/**
	 * Simple containers of {@link Component}s that give them "data". The component's data is then processed by {@link EntitySystem}s.
	 * @author Stefan Bachmann
	 */
	public class Entity
	{
		/** A flag that can be used to bit mask this entity. Up to the user to manage. */
		public int flags;
		/** Will dispatch an event when a component is added. */
		public readonly Signal<Entity> componentAdded;
		/** Will dispatch an event when a component is removed. */
		public readonly Signal<Entity> componentRemoved;

		public bool ScheduledForRemoval { set; private get; }
		public bool Removing { set; get; }
		internal ComponentOperationHandler componentOperationHandler;

		private Bag<Component> components;
		private List<Component> componentsArray;
		private ImmutableArray<Component> immutableComponentsArray;
		private Bits componentBits;
		private Bits familyBits;

		/** Creates an empty Entity. */
		public Entity ()
		{
			components = new Bag<Component> ();
			componentsArray = new List<Component> ();
			immutableComponentsArray = new ImmutableArray<Component>(componentsArray);
			componentBits = new Bits ();
			familyBits = new Bits ();
			flags = 0;

			componentAdded = new Signal<Entity> ();
			componentRemoved = new Signal<Entity> ();
		}

		/**
		 * Adds a {@link Component} to this Entity. If a {@link Component} of the same type already exists, it'll be replaced.
		 * @return The Entity for easy chaining
		 */
		public Entity add (Component component)
		{
			if (addInternal (component)) {
				if (componentOperationHandler != null) {
					componentOperationHandler.add (this);
				} else {
					notifyComponentAdded ();
				}
			}

			return this;
		}

		/**
		 * Adds a {@link Component} to this Entity. If a {@link Component} of the same type already exists, it'll be replaced.
		 * @return The Component for direct component manipulation (e.g. PooledComponent)
		 */
		public Component addAndReturn (Component component)
		{
			add (component);
			return component;
		}

		/**
		 * Removes the {@link Component} of the specified type. Since there is only ever one component of one type, we don't need an
		 * instance reference.
		 * @return The removed {@link Component}, or null if the Entity did no contain such a component.
		 */
		public virtual Component remove (Type componentClass)
		{
			ComponentType componentType = ComponentType.getFor (componentClass);
			int componentTypeIndex = componentType.getIndex ();
			Component removeComponent = components.get (componentTypeIndex);

			if (removeComponent != null && removeInternal (componentClass)) {
				if (componentOperationHandler != null) {
					componentOperationHandler.remove (this);
				} else {
					notifyComponentRemoved ();
				}
			}

			return removeComponent;
		}

		/** Removes all the {@link Component}'s from the Entity. */
		public void removeAll ()
		{
			while (componentsArray.Count > 0) {
				remove (componentsArray[0].GetType ());
			}
		}

		/** @return immutable collection with all the Entity {@link Component}s. */
		public ImmutableArray<Component> getComponents ()
		{
			return immutableComponentsArray;
		}

		/**
		 * Retrieve a component from this {@link Entity} by class. <em>Note:</em> the preferred way of retrieving {@link Component}s is
		 * using {@link ComponentMapper}s. This method is provided for convenience; using a ComponentMapper provides O(1) access to
		 * components while this method provides only O(logn).
		 * @param componentClass the class of the component to be retrieved.
		 * @return the instance of the specified {@link Component} attached to this {@link Entity}, or null if no such
		 *         {@link Component} exists.
		 */
		public T getComponent<T> () where T : Component
		{
			var t = ComponentType.getFor (typeof(T));
			return getComponent<T> (t);
		}

	/**
	 * Internal use.
	 * @return The {@link Component} object for the specified class, null if the Entity does not have any components for that class.
	 */
		T getComponent<T> (ComponentType componentType) where T : Component
		{
			int componentTypeIndex = componentType.getIndex ();

			if (componentTypeIndex < components.getCapacity ()) {
				return (T)components.get (componentType.getIndex ());
			} else {
				return default(T);
			}
		}

		/**
		 * @return Whether or not the Entity has a {@link Component} for the specified class.
		 */
		public bool hasComponent (ComponentType componentType)
		{
			return componentBits.get (componentType.getIndex ());
		}

		/**
		 * @return This Entity's component bits, describing all the {@link Component}s it contains.
		 */
		public Bits getComponentBits ()
		{
			return componentBits;
		}

		/** @return This Entity's {@link Family} bits, describing all the {@link EntitySystem}s it currently is being processed by. */
		public Bits getFamilyBits ()
		{
			return familyBits;
		}

		/**
		 * @param component
		 * @return whether or not the component was added.
		 */
		bool addInternal (Component component)
		{
			//System.Type componentClass = component.GetType ();
			Component oldComponent = getComponent<Component> ();

			if (component == oldComponent) {
				return false;
			}

			if (oldComponent != null) {
				removeInternal (component.GetType ());
			}

			int componentTypeIndex = ComponentType.getIndexFor (component.GetType ());
			components.set (componentTypeIndex, component);

			componentsArray.Add (component);
			componentBits.set (componentTypeIndex);

			return true;
		}
		/**
		 * @param componentClass
		 * @return whether or not a component with the specified class was found and removed.
		 */
		bool removeInternal (Type componentClass)
		{
			ComponentType componentType = ComponentType.getFor (componentClass);
			int componentTypeIndex = componentType.getIndex ();
			Component removeComponent = components.get(componentTypeIndex);

			if (removeComponent != null) {
				components.set (componentTypeIndex, null);
				componentsArray.Remove (removeComponent);//, true);
				componentBits.clear (componentTypeIndex);

				return true;
			}

			return false;
		}

		public void notifyComponentAdded ()
		{
			componentAdded.dispatch (this);
		}

		public void notifyComponentRemoved ()
		{
			componentRemoved.dispatch (this);
		}

		/** @return true if the entity is scheduled to be removed */
		public bool isScheduledForRemoval ()
		{
			return ScheduledForRemoval;
		}
	}

}
