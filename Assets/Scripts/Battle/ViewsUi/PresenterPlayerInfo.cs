using anygames.ashley.core;
using Battle.Components;
using Battle.Signals;
using UiCore;
using UniRx;
using UnityEngine;
using Zenject;

namespace Battle.ViewsUi
{
    public class PresenterPlayerInfo : UiPresenter<ViewUiPlayerInfo> 
    {
        private readonly SignalBus _signalBus;
        readonly CompositeDisposable _disposables = new CompositeDisposable();


        public PresenterPlayerInfo(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        public override void Initialize()
        {
            _signalBus.GetStream<SignalPlayerCreated>()
                .Subscribe(x=>OnPlayerCreated(x.Entity)).AddTo(_disposables);                           
        }

        public override void Dispose()
        {
            _disposables.Dispose();
        }        

        private void OnPlayerCreated(Entity player)
        {
            var healthComponent = player.getComponent<HealthComponent>();
            var armorComponent = player.getComponent<ArmorComponent>();
            var movementComponent = player.getComponent<MovementComponent>();
            //data.Health.SubscribeToText(View.TextHealth);
            healthComponent.Health.Subscribe(x => View.TextHealth.text = "Health : " + x).AddTo(View.TextHealth);
            armorComponent.Armor.Subscribe(x => View.TextArmor.text = "Armor : " + x).AddTo(View.TextArmor);
            movementComponent.Speed.Subscribe(x => View.TextSpeed.text = "Speed : " + x).AddTo(View.TextSpeed);
        }
    }
}
