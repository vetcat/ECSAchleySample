using Battle.InputControllers;
using Battle.Systems;
using Battle.Views;
using Uniject;
using Uniject.Impl;
using UnityEngine;
using Zenject;

namespace Battle
{
    public class BattleInstaller : MonoInstaller
    {
        public GameObject TankEntity;
        
        public override void InstallBindings()
        {
            //controllers
            Container.BindInterfacesAndSelfTo<StandardInputController>().AsSingle().NonLazy();
            //systems
            Container.BindInterfacesAndSelfTo<PlayerControlSystem>().AsSingle().NonLazy();
            
            Container.Bind<ITestableGameObject>().FromMethod(_ => GetTestableGameObject(TankEntity, "Pool_Tanks"))
                .WhenInjectedInto<ITankView>();
                                    
            //pools                                    
            Container.BindMemoryPool<TankView, TankView.Pool>().WithInitialSize(20);
                        

            Container.BindInterfacesAndSelfTo<Core>().AsSingle().NonLazy();
        }

        private ITestableGameObject GetTestableGameObject(GameObject prefab, string root)
        {                           
            var go = Instantiate(prefab);
            go.transform.parent = GetRoot(root).transform;
            return new UnityGameObject(go);
        }

        private ITestableGameObject GetTestableGameObject(string prefabPath, string root)
        {               
            var prefab = Resources.Load(prefabPath);
            var go = (GameObject)Instantiate(prefab);
            go.transform.parent = GetRoot(root).transform;
            return new UnityGameObject(go);
        }
        
        private GameObject GetRoot(string root)
        {
            var rootGo = GameObject.Find("/" + root);
            if (!rootGo)
                rootGo = new GameObject(){name = root};
            return rootGo;
        }
    }        
}
