using UnityEngine;

using WhalePark18.Item;

namespace WhalePark18.Character.Player
{
    public class PlayerCollider : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Item"))
            {
                other.GetComponent<ItemBase>().Use(transform.parent.gameObject);
            }
        }
    }
}