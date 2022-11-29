using UnityEngine;

namespace WhalePark18.Item
{
    public abstract class ItemBase : MonoBehaviour
    {
        public abstract void Use(GameObject entity);
    }
}