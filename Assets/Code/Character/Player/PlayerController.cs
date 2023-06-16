using UnityEngine;

using WhalePark18.UI;
using WhalePark18.UI.Window;
using WhalePark18.Weapon;
using WhalePark18.Manager;

namespace WhalePark18.Character.Player
{
    /// <summary>
    /// �ֻ��� �÷��̾� ��Ʈ�ѷ� Ŭ����
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        /****************************************
         * PlayerController
         ****************************************/
        private PlayerStatus                status;             // �̵��ӵ� ���� �÷��̾� ����
        private RotateToMouse               rotateToMouse;      // ���콺 �̵����� ī�޶� ȸ��
        private MovementCharacterController movement;           // Ű���� �Է����� �÷��̾� �̵�, ����
        private AudioData                   audioData;          // ����� ���� ����
        private WeaponBase                  weapon;             // ���� �ֻ��� Ŭ������ �̿��� ���� ����
        private PlayerHUDController         hudController;
        private bool                        active = true;

        [Header("Input KeyCodes")]
        [SerializeField]
        private KeyCode keyCodeRun                  = KeyCode.LeftShift;    // �޸��� Ű
        [SerializeField]
        private KeyCode keyCodeJump                 = KeyCode.Space;        // ���� Ű
        [SerializeField]
        private KeyCode keyCodeReload               = KeyCode.R;            // ź ������ Ű
        [SerializeField]
        private KeyCode keyCodeWindowTrait          = KeyCode.Tab;          // Ư�� â Ű
        [SerializeField]
        private KeyCode keyCodeWindowTemporaryMenu  = KeyCode.Escape;       // �޴� â Ű

        [Header("Audio Clips")]
        [SerializeField]
        private AudioClip audioClipWalk;    // �ȱ� ����
        [SerializeField]
        private AudioClip audioClipRun;     // �޸��� ����

        [Header("HUD")]
        [SerializeField]
        private PlayerHUD playerHUD;        // �÷��̾� HUD

        private void Awake()
        {
            /// ���콺 Ŀ�� ����
            /// FPS ���� ���� ȯ�濡�� Ŀ�� �������� ���Ǹ� �������� �׻� ȭ�� ���߾��̴�.
            /// ��, Ŀ�� ǥ�ø� ��Ȱ��ȭ �ϰ�, ���� ��ġ�� ������Ų��.
            /// 
            /// * ���� �Ŵ������� ���¸� �����ϸ� ������ ���̱� ������ ���� ����
            //GameManager.Instance.SetCursorActive(false);

            /// ������Ʈ �ʱ�ȭ
            status              = GetComponent<PlayerStatus>();
            rotateToMouse       = GetComponent<RotateToMouse>();
            movement            = GetComponent<MovementCharacterController>();
            audioData           = new AudioData(GetComponent<AudioSource>(), Manager.AudioType.Player);
            hudController       = GetComponent<PlayerHUDController>();

            //SetActive(false);
        }

        private void Start()
        {
            SoundManager.Instance.AddAudioSource(audioData);
        }

        private void OnDestroy()
        {
            SoundManager.Instance.RemoveAudioSource(audioData);
        }

        private void Update()
        {
            if (active == false || GameManager.Instance.DeactivePlayer)
                return;

            UpdateRotate();
            UpdateMove();
            UpdateJump();
            UpdateWeaponAction();
            UpdateWindow();
        }

        /// <summary>
        /// PlayerController�� �۵� ���θ� �����ϴ� �޼ҵ�
        /// </summary>
        /// <param name="active">�۵� ����</param>
        /// <remarks>
        /// PlayerController ������Ʈ�� Active�� �����ϴ� ���� �ƴ� 
        /// PlayerController�� �߻������� Ȱ���� ���� ���� �ǹ���
        /// </remarks>
        public void SetActive(bool active)
        {
            this.active = active; 

            /// �÷��̾��� �������� ����ϴ� ������Ʈ�� ��Ȱ��ȭ �Ѵ�.
            movement.enabled        = active;
            rotateToMouse.enabled   = active;
            
            audioData.audioSource.Stop();
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

                /// �޸��Ⱑ �Է� �Ǿ��� ���Ⱑ ���� ��尡 �ƴ϶�� �޸��� �ӵ��� �̵��Ѵ�.
                movement.MoveSpeed = isRun && weapon.IsAimMode == false ? status.RunSpeed : status.WalkSpeed;
                weapon.Animator.MoveSpeed = isRun && weapon.IsAimMode == false ? 1 : 0.5f;
                audioData.audioSource.clip = isRun && weapon.IsAimMode == false ? audioClipRun : audioClipWalk;

                /// ����Ű �Է� ���δ� �� ������ Ȯ���ϱ� ������
                /// ������� ���� �ٽ� ������� �ʵ��� isPlaying���� üũ�ؼ� ���
                if (audioData.audioSource.isPlaying == false)
                {
                    audioData.audioSource.loop = true;
                    audioData.audioSource.Play();
                }
            }
            else
            {
                movement.MoveSpeed = 0;
                weapon.Animator.MoveSpeed = 0;

                /// ������ �� ���尡 ������̸� ����
                if (audioData.audioSource.isPlaying == true)
                {
                    audioData.audioSource.Stop();
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
            if (GameManager.Instance.IsPause || active == false) return;

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

        private void UpdateWindow()
        {
            if (Input.GetKeyDown(keyCodeWindowTrait))
            {
                hudController.OnWindowTraitPower();
            }

            if (Input.GetKeyDown(keyCodeWindowTemporaryMenu))
            {
                hudController.OnWindowGameMenuPower();
            }
        }

        /// <summary>
        /// ���� ó�� �޼ҵ�
        /// </summary>
        /// <param name="damge">���ط�</param>
        public void TakeDamage(int damge)
        {
            if(status.ShieldActive)
            {
                int hpDamage = status.DecreaseShield(damge);

                bool isShieldDestroy = hpDamage > 0;
                if(isShieldDestroy)
                {
                    status.DecreaseHp(hpDamage);
                }
            }
            else
            {
                bool isDie = status.DecreaseHp(damge);

                if (isDie == true)
                {
                    GameManager.Instance.GameOver();
                    StopAllCoroutines();
                }
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
