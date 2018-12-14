using anygames.ashley.core;
using anygames.ashley.utils;
using Uniject;

namespace Battle.Components
{
    public class Transformable : Component, Poolable
    {
        public ITransform Value;

        public void reset()
        {
            Value = null;
        }
    }
}