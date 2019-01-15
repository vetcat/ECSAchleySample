using System.Collections.Generic;
using UnityEngine;

namespace UiCore
{
    public abstract class UiCollecttion<TMono, TView> : MonoBehaviour where TMono : UiCollecttion<TMono, TView> where TView: UiView
    {
        [SerializeField] 
        private Transform _collectionRoot;

        [SerializeField] 
        private TView _collectionPrefab;

        private readonly List<TView> _items = new List<TView>();

        public TView AddItem()
        {
            TView item = Instantiate(_collectionPrefab, _collectionRoot).GetComponent<TView>();
        
            if (!item.gameObject.activeInHierarchy)
                item.gameObject.SetActive(true);
        
            _items.Add(item);
            return item;
        }
        
        public List<TView> GetItems()
        {
            return _items;
        }

        public void Clear()
        {
            foreach (var item in _items)            
                Destroy(item.gameObject);
        
            _items.Clear();
        }

        public int Count()
        {
            return _items.Count;
        }
    }
}
