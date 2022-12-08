using UnityEngine;

namespace WhalePark18.Character.Player
{
    public class RotateToMouse : MonoBehaviour
    {
        [SerializeField]
        private float rotateCamXAixsSpeed = 5f; // ī�޶� x�� ȸ���ӵ�
        [SerializeField]
        private float rotateCamYAixsSpeed = 5f; // ī�޶� y�� ȸ�� �ӵ�

        private float limitMinX = -80f;         // ī�޶� x�� �ּ� ȸ�� ����
        private float limitMaxX = 80f;          // ī�޶� x�� �ִ� ȸ�� ����
        private float eulerAngleX;
        private float eulerAngleY;

        public void UpdateRotate(float mouseX, float mouseY)
        {
            eulerAngleY += mouseX * rotateCamYAixsSpeed;    // ���콺 ��/�� �̵����� ī�޶� y�� ȸ��
            eulerAngleX -= mouseY * rotateCamXAixsSpeed;    // ���콺 ��/�Ʒ� �̵����� ī�޶� x�� ȸ��

            //ī�޶� x�� ȸ���� ��� ȸ�� ������ ����
            eulerAngleX = ClampAngle(eulerAngleX, limitMinX, limitMaxX);

            transform.rotation = Quaternion.Euler(eulerAngleX, eulerAngleY, 0);
        }

        private float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360) angle += 360;
            if (angle > 360) angle -= 360;

            return Mathf.Clamp(angle, min, max);
        }
    }
}