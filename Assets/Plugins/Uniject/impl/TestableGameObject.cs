using System.Collections;
using System.Collections.Generic;

namespace Uniject
{
    /// <summary>
    ///     A testable equivalent of <c>UnityEngine.GameObject</c>.
    /// </summary>
    public abstract class TestableGameObject : ITestableGameObject
    {
        private readonly List<TestableComponent> _components = new List<TestableComponent>();

        protected TestableGameObject(ITransform transform)
        {
            Transform = transform;
        }

        public ITransform Transform { get; private set; }

        public bool Destroyed { get; private set; }

        public int ComponentAmount
        {
            get { return _components.Count; }
        }

        public abstract bool Active { get; set; }
        public abstract string Name { get; set; }
        public abstract int InstanceId { get; }

        public abstract string Tag { get; set; }
        public abstract int Layer { get; set; }

        public void RegisterComponent(TestableComponent component)
        {
            _components.Add(component);
        }

        public virtual void Destroy()
        {
            if (Destroyed) return;
            foreach (var component in _components) component.OnDestroy();
            Destroyed = true;
        }

        public void Update()
        {
            if (Active)
                for (var t = 0; t < _components.Count; t++)
                {
                    var component = _components[t];
                    component.OnUpdate();
                }
        }

		public void LateUpdate ()
		{
			if (LateUpdateEnabled)
				for (var t = 0; t < _components.Count; t++) {
					var component = _components [t];
					component.OnLateUpdate ();
				}
		}

        public T GetComponent<T>() where T : class
        {
            foreach (var component in _components)
            {
                if (component is T) return component as T;
            }

            return null;
        }

        public void OnCollisionEnter(Collision c)
        {
            foreach (var component in _components)
                component.OnCollisionEnter(c);
        }

        public void OnCollisionEnter2D(Collision2D c)
        {
            foreach (var component in _components)
                component.OnCollisionEnter2D(c);
        }

        public void OnMouseDown()
        {
            foreach (var component in _components)
                component.OnMouseDown();
        }

		public bool LateUpdateEnabled { get; set; }

        public abstract void StartCoroutine(IEnumerator coroutine);
        public abstract void StopCoroutine(IEnumerator coroutine);
        public abstract void StopAllCoroutines();

        public abstract void StartInvokeRepeating(float intTime, float repeatTime);
        public abstract void StartInvokeRepeating(IInvokeRepeatable repeatable, float inTime, float repeatTime);
        public abstract void AddInvokeRepeatable(IInvokeRepeatable repeatable);
        public abstract bool RemoveInvokeRepeatable(IInvokeRepeatable repeatable);
        public abstract void CancelInvoke();

        public abstract void SetActiveRecursively(bool active);
        public abstract void SetActive(bool active);
    }
}