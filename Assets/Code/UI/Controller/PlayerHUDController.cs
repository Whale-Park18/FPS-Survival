using UnityEngine;

using WhalePark18.UI;
using WhalePark18.UI.Window;

namespace WhalePark18.Character.Player
{
    public enum Window { TemporaryMenu, Trait }

    public class PlayerHUDController : MonoBehaviour
    {
        [SerializeField, Tooltip("임시 메뉴, 특성창")]
        private WindowBase[] windowGroup;

        public void OnWindowGameMenuPower()
        {
            windowGroup[(int)Window.TemporaryMenu].OnWindowPower();
        }

        public void OnWindowTraitPower()
        {
            windowGroup[(int)Window.Trait].OnWindowPower();
        }
    }
}