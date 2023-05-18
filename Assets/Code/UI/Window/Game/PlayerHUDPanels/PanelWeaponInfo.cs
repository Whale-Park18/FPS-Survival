using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WhalePark18.Weapon;

namespace WhalePark18.UI.Window.Game.PlayerHUDPanels
{
    public class PanelWeaponInfo : MonoBehaviour
    {
        [Header("Weaon Info")]
        [SerializeField]
        private TextMeshProUGUI     textWeaponName;
        [SerializeField]
        private Image               imageWeaponIcon;
        [SerializeField]
        private Sprite[]            spriteWeaponIcons;
        [SerializeField]
        private Vector2[]           sizeWeaponIcions;

        [Header("Magazine")]
        [SerializeField]
        private GameObject          magazineUIPrefab;
        [SerializeField]
        private Transform           magazineParent;
        [SerializeField]
        private int                 maxMagazineCount;
        private List<GameObject>    magazineList;

        [Header("Ammo")]
        [SerializeField]
        private TextMeshProUGUI     textAmmo;

        public void SwitchingWeapon(WeaponBase newWeapon)
        {
            OnSetWeapon(newWeapon);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="weapon"></param>
        private void OnSetWeapon(WeaponBase weapon)
        {
            textWeaponName.text = weapon.WeaponName.ToString();
            imageWeaponIcon.sprite = spriteWeaponIcons[(int)weapon.WeaponName];
            imageWeaponIcon.rectTransform.sizeDelta = sizeWeaponIcions[(int)weapon.WeaponName];
        }

        /// <summary>
        /// źâ �ʱ�ȭ �޼ҵ�
        /// </summary>
        public void OnInitializedMagazine()
        {
            magazineList = new List<GameObject>();
            for (int i = 0; i < maxMagazineCount; i++) 
            {
                GameObject clone = Instantiate(magazineUIPrefab);
                clone.transform.SetParent(magazineParent);
                clone.SetActive(false);

                magazineList.Add(clone);
            }
        }

        /// <summary>
        /// źâ UI ������Ʈ �޼ҵ�
        /// </summary>
        /// <param name="currentMagazine">���� źâ ��</param>
        public void UpdateMagazine(int currentMagazine)
        {
            /// ��� źâ�� ��Ȱ��ȭ ��Ų ��, ���� źâ ��ŭ �ٽ� Ȱ��ȭ ��Ų��.
            for (int i = 0; i < magazineList.Count; i++)
            {
                magazineList[i].SetActive(false);
            }
            for (int i = 0; i < currentMagazine; i++)
            {
                magazineList[i].SetActive(true);
            }
        }

        /// <summary>
        /// ź�� UI ������Ʈ �޼ҵ�
        /// </summary>
        /// <param name="currentAmmo">���� ź�� ��</param>
        /// <param name="maxAmmo">�ִ� ź�� ��</param>
        public void UpdateAmmo(int currentAmmo, int maxAmmo)
        {
            textAmmo.text = $"<size=40>{currentAmmo}/<size>{maxAmmo}";
        }
    }
}