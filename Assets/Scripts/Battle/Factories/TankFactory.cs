using anygames.ashley.core;
using Battle.Components;
using Battle.Views;

namespace Battle.Factories
{
    public class TankFactory
    {
        public static Entity Create(ITankView view, Engine engine)
        {
            var entity = engine.createEntity();
            
            var transformable = engine.createComponent<Transformable>();
            transformable.Value = view.Transform;
            entity.add(transformable);
            
            var pc = engine.createComponent<PlayerControlled>();
            entity.add(pc);
            
            engine.addEntity(entity);

            return entity;
        }
    }
}