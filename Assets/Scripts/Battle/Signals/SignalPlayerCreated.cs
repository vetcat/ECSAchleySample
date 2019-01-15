using anygames.ashley.core;
namespace Battle.Signals
{
    public class SignalPlayerCreated
    {
        public readonly Entity Entity;

        public SignalPlayerCreated(Entity entity)
        {
            Entity = entity;
        }
    }
}