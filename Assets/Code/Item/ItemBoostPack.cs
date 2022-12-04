using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WhalePark18.Character;

namespace WhalePark18.Item
{ 
    public class ItemBoostPack : ItemBase
    {
        [SerializeField]
        private float time = 30f;
        [SerializeField]
        private float attackSpeedIncrease = 0.15f;

        [SerializeField]
        private float moveDistance = 0.2f;
        [SerializeField]
        private float pingpongSpeed = 0.5f;
        [SerializeField]
        private float rotateSpeed = 50;

        private IEnumerator Start()
        {
            float y = transform.position.y;

            while (true)
            {
                /// y���� �������� ȸ��
                transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);

                /// ó�� ��ġ�� ��ġ�� �������� y ��ġ�� ��, �Ʒ��� �̵�
                Vector3 position = transform.position;
                position.y = Mathf.Lerp(y, y + moveDistance, Mathf.PingPong(Time.time * pingpongSpeed, 1));
                transform.position = position;

                yield return null;
            }
        }

        public override void Use(GameObject entity)
        {
            print(name + "use");
            StartCoroutine("OnEffect", entity.GetComponent<Status>());

            //Destroy(gameObject);
        }

        private IEnumerator OnEffect(Status status)
        {
            status.IncreaseAttackSpeed(attackSpeedIncrease);
            print(status.CurrentAttackSpeed);

            yield return new WaitForSeconds(time);

            status.DisincreaseAttackSpeed(attackSpeedIncrease);
            print(status.CurrentAttackSpeed);
        }
    }
}
