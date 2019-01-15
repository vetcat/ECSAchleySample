using Battle.Views;

namespace Battle.Signals
{
    public class SignalShotFXDestroy
    {
        public readonly ShotFXView View;

        public SignalShotFXDestroy(ShotFXView view)
        {
            View = view;
        }
    }
}