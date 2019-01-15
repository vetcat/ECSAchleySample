using UnityEngine;
using Zenject;

namespace UiCore
{
    public static class DiContainerUiExtensions
    {
        public static void BindViewController<TView, TPresenter>(this DiContainer container, UiView view, Transform parent) where TView : UiView where TPresenter : UiPresenter
        {
            container.Bind<TView>().FromComponentInNewPrefab(view).UnderTransform(parent).AsSingle();
            container.BindController<TPresenter>();
        }
        
        public static void BindViewControllerWithArgument<TView, TPresenter, T>(this DiContainer container, GameObject viewPrefab, Transform parent, T paramArgument) where TView : UiView where TPresenter : UiPresenter
        {
            container.Bind<TView>().FromComponentInNewPrefab(viewPrefab).UnderTransform(parent).AsSingle();
            container.BindControllerWithArgument<TPresenter, T>(paramArgument);
        }
    
        private static void BindController<TPresenter>(this DiContainer container) where TPresenter : UiPresenter
        {        
            container.BindInterfacesAndSelfTo<TPresenter>().AsSingle().NonLazy();             
        }

        private static void BindControllerWithArgument<TPresenter, T>(this DiContainer container, T paramArgument) where TPresenter : UiPresenter
        {
            container.BindInterfacesAndSelfTo<TPresenter>().AsSingle().WithArguments(paramArgument).NonLazy();
        }
    }
}
