using System.Collections;
using UnityEngine;
using WhalePark18.Character.Player;

namespace WhalePark18.Item.Kit
{
    public class ItemTimeScale : ItemBase
    {
        [SerializeField]
        private float timeScale = 0.8f;
        private float effectTime = 10f;

        private GameObject mesh;

        private void Awake()
        {
            mesh = transform.GetChild(0).gameObject;
        }

        public override void Use(GameObject entity)
        {
            StartCoroutine("TimeScaleTimer", entity.GetComponent<PlayerStatus>());
            mesh.SetActive(false);
        }

        /// <summary>
        /// Time Scale�� �����ð����� �۵��ϵ��� �ϴ� �޼ҵ�
        /// </summary>
        /// <param name="status">�÷��̾� Status</param>
        /// <returns></returns>
        private IEnumerator TimeScaleTimer(PlayerStatus status)
        {
            EnableTimeScale();
            yield return new WaitForSeconds(effectTime * status.ItemEfficiency.currentAbility);
            DisableTimeScale();
        }

        /// <summary>
        /// TimeScale Ȱ��ȭ �޼ҵ�
        /// </summary>
        private void EnableTimeScale()
        {
            Time.timeScale = timeScale;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }

        /// <summary>
        /// TimeScale ��Ȱ��ȭ �޼ҵ�
        /// </summary>
        private void DisableTimeScale()
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }
    }
}