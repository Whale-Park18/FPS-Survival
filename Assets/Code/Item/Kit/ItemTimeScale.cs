using System.Collections;
using UnityEngine;
using WhalePark18.Character;

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
            StartCoroutine("TimeScaleTimer", entity.GetComponent<Status>());
            mesh.SetActive(false);
        }

        private IEnumerator TimeScaleTimer(Status status)
        {
            StartTimeScale();
            yield return new WaitForSeconds(effectTime * status.CurrentItemEfficiency);
            StopTimeScale();
        }

        private void StartTimeScale()
        {
            Time.timeScale = timeScale;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }

        private void StopTimeScale()
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }
    }
}