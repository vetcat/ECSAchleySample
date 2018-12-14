using System.Collections;

namespace Uniject
{
    public interface ITestableGameObject
    {
        ITransform Transform { get; }
        bool Destroyed { get; }
		bool LateUpdateEnabled { get; set; }
        int ComponentAmount { get; }
        bool Active { get; set; }
        string Name { get; set; }
        int InstanceId { get; }
        string Tag { get; set; }
        int Layer { get; set; }
        void RegisterComponent(TestableComponent component);
        void Destroy();
        void Update();
        T GetComponent<T>() where T : class;
        void OnCollisionEnter(Collision c);
        void OnCollisionEnter2D(Collision2D c);
        void OnMouseDown();
        void StartCoroutine(IEnumerator coroutine);
        void StopCoroutine(IEnumerator coroutine);
        void StopAllCoroutines();
        void StartInvokeRepeating(float intTime, float repeatTime);
        void StartInvokeRepeating(IInvokeRepeatable repeatable, float inTime, float repeatTime);
        void AddInvokeRepeatable(IInvokeRepeatable repeatable);
        bool RemoveInvokeRepeatable(IInvokeRepeatable repeatable);
        void CancelInvoke();
        void SetActiveRecursively(bool active);
        void SetActive(bool active);
    }
}