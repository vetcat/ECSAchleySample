using System;
using Zenject;

namespace UiCore
{
    public interface IUiPresenter
    {
        void Show();
        void Hide();
        bool IsShow();
    }

    public abstract class UiPresenter : IInitializable, IDisposable
    {
        public virtual void Initialize()
        {        
        }

        public virtual void Dispose()
        {        
        }
    }

    public abstract class UiPresenter<T> : UiPresenter, IUiPresenter
        where T : UiView
    {
        [Inject]
        private T _view;

        public T View { get { return _view; } }
    
        public virtual void Show()
        {
            View.Show();
        }

        public virtual void Hide()
        {
            View.Hide();
        }

        public bool IsShow()
        {
            return View.IsShow();
        }
    }
}