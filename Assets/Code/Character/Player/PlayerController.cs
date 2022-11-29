using UnityEngine;

using WhalePark18.Weapon;

namespace WhalePark18.Character.Player
{
    /// <summary>
    /// �ֻ��� �÷��̾� ��Ʈ�ѷ� Ŭ����
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [Header("Input KeyCodes")]
        [SerializeField]
        private KeyCode keyCodeRun = KeyCode.LeftShift;     // �޸��� Ű
        [SerializeField]
        private KeyCode keyCodeJump = KeyCode.Space;        // ���� Ű
        [SerializeField]
        private KeyCode keyCodeReload = KeyCode.R;          // ź ������ Ű

        [Header("Audio Clips")]
        [SerializeField]
        private AudioClip audioClipWalk;                    // �ȱ� ����
        [SerializeField]
        private AudioClip audioClipRun;                     // �޸��� ����

        private RotateToMouse rotateToMouse;                // ���콺 �̵����� ī�޶� ȸ��
        private MovementCharacterController movement;       // Ű���� �Է����� �÷��̾� �̵�, ����
        private Status status;                              // �̵��ӵ� ���� �÷��̾� ����
        private AudioSource audioSource;                    // ���� ��� ����
        private WeaponBase weapon;                          // ���� �ֻ��� Ŭ������ �̿��� ���� ����

        private void Awake()
        {
            /// ���콺 Ŀ�� ����
            /// FPS ���� ���� ȯ�濡�� Ŀ�� �������� ���Ǹ� �������� �׻� ȭ�� ���߾��̴�.
            /// ��, Ŀ�� ǥ�ø� ��Ȱ��ȭ �ϰ�, ���� ��ġ�� ������Ų��.
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            rotateToMouse = GetComponent<RotateToMouse>();
            movement = GetComponent<MovementCharacterController>();
            status = GetComponent<Status>();
            audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            UpdateRotate();
            UpdateMove();
            UpdateJump();
            UpdateWeaponAction();
        }

        private void UpdateRotate()
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            rotateToMouse.UpdateRotate(mouseX, mouseY);
        }

        private void UpdateMove()
        {
            float x = Input.GetAxisRaw("Horizontal");
            float z = Input.GetAxisRaw("Vertical");

            /// �̵� ���� ��(�ȱ� or �޸���)
            if (x != 0 || z != 0)
            {
                bool isRun = false;

                /// ���̳� �ڷ� �̵��� ���� �޸� �� ����.
                if (z > 0) isRun = Input.GetKey(keyCodeRun);

                movement.MoveSpeed = isRun ? status.RunSpeed : status.WalkSpeed;
                weapon.Animator.MoveSpeed = isRun ? 1 : 0.5f;
                audioSource.clip = isRun ? audioClipRun : audioClipWalk;

                /// ����Ű �Է� ���δ� �� ������ Ȯ���ϱ� ������
                /// ������� ���� �ٽ� ������� �ʵ��� isPlaying���� üũ�ؼ� ���
                if (audioSource.isPlaying == false)
                {
                    audioSource.loop = true;
                    audioSource.Play();
                }
            }
            else
            {
                movement.MoveSpeed = 0;
                weapon.Animator.MoveSpeed = 0;

                /// ������ �� ���尡 ������̸� ����
                if (audioSource.isPlaying == true)
                {
                    audioSource.Stop();
                }
            }

            movement.MoveTo(new Vector3(x, 0, z));
        }

        private void UpdateJump()
        {
            if (Input.GetKeyDown(keyCodeJump))
            {
                movement.Jump();
            }
        }

        private void UpdateWeaponAction()
        {
            if (Input.GetMouseButtonDown(0))
            {
                weapon.StartWeaponAction();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                weapon.StopWeaponAction();
            }

            if (Input.GetMouseButtonDown(1))
            {
                weapon.StartWeaponAction(1);
            }
            else if (Input.GetMouseButtonUp(1))
            {
                weapon.StopWeaponAction(1);
            }

            if (Input.GetKeyDown(keyCodeReload))
            {
                weapon.StartReload();
            }
        }

        /// <summary>
        /// ���� ó�� �޼ҵ�
        /// </summary>
        /// <param name="damge">���ط�</param>
        public void TakeDamage(int damge)
        {
            bool isDie = status.DecreaseHP(damge);

            if (isDie == true)
            {
                print("Game Over");
            }
        }

        /// <summary>
        /// ���� ���� �޼ҵ�
        /// </summary>
        /// <param name="newWeapon">������ ����</param>
        /// <remarks>
        /// PlayerController���� �����ϴ� ���� �⺭������ ������
        /// </remarks>
        public void SwitchingWeapon(WeaponBase newWeapon)
        {
            weapon = newWeapon;
        }
    }
}
