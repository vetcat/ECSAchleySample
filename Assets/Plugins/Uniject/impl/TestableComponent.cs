using System;

namespace Uniject {
    public class TestableComponent {
        private ITestableGameObject obj;

        public bool enabled { get; set; }

        public TestableComponent(ITestableGameObject obj) {
            this.enabled = true;
            this.obj = obj;
            obj.RegisterComponent(this);
        }
        
        public ITestableGameObject Obj {
            get { return obj; } 
        }

        public void OnUpdate() {
            if (enabled) {
                Update();
            }
        }

		public void OnLateUpdate ()
		{
			if (enabled) {
				LateUpdate ();
			}
		}

        public virtual void Update() {
        }

		public virtual void LateUpdate ()
		{
		}

        public virtual void OnDestroy() {
        }

        public virtual void OnCollisionEnter(Collision collision) {
        }

		public virtual void OnCollisionEnter2D (Collision2D collision)
		{
		}

		public virtual void OnMouseDown() {
		}

    }
}

