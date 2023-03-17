using UnityEngine;

using WhalePark18.HUD;
using WhalePark18.HUD.Window;

namespace WhalePark18.Character.Player
{
    public enum Window { TemporaryMenu, Trait}

    public class HUDController : MonoBehaviour
    {
        [SerializeField, Tooltip("임시 메뉴, 특성창")]
        private WindowBase[] windowGroup;

        public void OnWindowTemporaryMenuPower()
        {
            windowGroup[(int)Window.TemporaryMenu].OnWindowPower();
        }

        public void OnWindowTraitPower()
        {
            windowGroup[(int)Window.Trait].OnWindowPower();
        }
    }
}