using UnityEngine;

namespace UiCore
{
    public class UiView : MonoBehaviour
    {
        private GameObject GameObject
        {
            get { return gameObject; }
        }
    
        public void Show(bool isShow = true)
        {        
            GameObject.SetActive(isShow);
        }
    
        public void Hide()
        {
            if (IsShow())
                GameObject.SetActive(false);
        }
    
        public bool IsShow()
        {
            return GameObject.activeInHierarchy;
        }    
    }
}
