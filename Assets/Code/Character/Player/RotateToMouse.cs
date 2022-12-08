using UnityEngine;

namespace WhalePark18.Character.Player
{
    public class RotateToMouse : MonoBehaviour
    {
        [SerializeField]
        private float rotateCamXAixsSpeed = 5f; // 카메라 x축 회전속도
        [SerializeField]
        private float rotateCamYAixsSpeed = 5f; // 카메라 y축 회전 속도

        private float limitMinX = -80f;         // 카메라 x축 최소 회전 범위
        private float limitMaxX = 80f;          // 카메라 x축 최대 회전 범위
        private float eulerAngleX;
        private float eulerAngleY;

        public void UpdateRotate(float mouseX, float mouseY)
        {
            eulerAngleY += mouseX * rotateCamYAixsSpeed;    // 마우스 좌/우 이동으로 카메라 y축 회전
            eulerAngleX -= mouseY * rotateCamXAixsSpeed;    // 마우스 위/아래 이동으로 카메라 x축 회전

            //카메라 x축 회전의 경우 회전 범위를 설정
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