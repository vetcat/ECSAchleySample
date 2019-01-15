using Battle.Views;
using UnityEngine;

namespace Battle.Signals
{
    public class SignalShotDestroy
    {
        public readonly ShotOneView OneView;

        public SignalShotDestroy(ShotOneView oneView)
        {
            OneView = oneView;
        }
    }
}