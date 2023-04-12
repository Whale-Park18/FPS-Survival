using UnityEngine;

namespace WhalePark18.Character.Enemy.Event
{
    public abstract class EventBase<T> : MonoBehaviour where T : EnemyBase
    {
        protected T owner;

        public abstract void Setup(T owner);
    }
}