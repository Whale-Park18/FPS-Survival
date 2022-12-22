using UnityEngine;

using WhalePark18.Weapon;

namespace WhalePark18.Character.Player
{
    /// <summary>
    /// 최상위 플레이어 컨트롤러 클래스
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [Header("Input KeyCodes")]
        [SerializeField]
        private KeyCode keyCodeRun = KeyCode.LeftShift;     // 달리기 키
        [SerializeField]
        private KeyCode keyCodeJump = KeyCode.Space;        // 점프 키
        [SerializeField]
        private KeyCode keyCodeReload = KeyCode.R;          // 탄 재장전 키
        [SerializeField]
        private KeyCode keyCodeIdentity = KeyCode.Tab;      // 특성 창 키

        [Header("Audio Clips")]
        [SerializeField]
        private AudioClip audioClipWalk;                    // 걷기 사운드
        [SerializeField]
        private AudioClip audioClipRun;                     // 달리기 사운드

        private RotateToMouse rotateToMouse;                // 마우스 이동으로 카메라 회전
        private MovementCharacterController movement;       // 키보드 입력으로 플레이어 이동, 점프
        private Status status;                              // 이동속도 등의 플레이어 정보
        private AudioSource audioSource;                    // 사운드 재생 제어
        private WeaponBase weapon;                          // 무기 최상위 클래스를 이용항 공격 제어
        private PlayerHUD playerHUD;                        // 플레이어 HUD

        private void Awake()
        {
            /// 마우스 커서 설정
            /// FPS 게임 실행 환경에선 커서 조준점이 사용되며 조준점은 항상 화면 정중앙이다.
            /// 즉, 커서 표시를 비활성화 하고, 현재 위치에 고정시킨다.
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            rotateToMouse = GetComponent<RotateToMouse>();
            movement = GetComponent<MovementCharacterController>();
            status = GetComponent<Status>();
            audioSource = GetComponent<AudioSource>();
            playerHUD = GetComponentInChildren<PlayerHUD>();
        }

        private void Update()
        {
            UpdateRotate();
            UpdateMove();
            UpdateJump();
            UpdateWeaponAction();
            UpdateIdentityPanel();
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

            /// 이동 중일 때(걷기 or 달리기)
            if (x != 0 || z != 0)
            {
                bool isRun = false;

                /// 옆이나 뒤로 이동할 때는 달릴 수 없다.
                if (z > 0) isRun = Input.GetKey(keyCodeRun);

                movement.MoveSpeed = isRun ? status.RunSpeed : status.WalkSpeed;
                weapon.Animator.MoveSpeed = isRun ? 1 : 0.5f;
                audioSource.clip = isRun ? audioClipRun : audioClipWalk;

                /// 방향키 입력 여부는 매 프레임 확인하기 때문에
                /// 재새중일 때는 다시 재생하지 않도록 isPlaying으로 체크해서 재생
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

                /// 멈췄을 때 사운드가 재생중이면 정지
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

        private void UpdateIdentityPanel()
        {
            if (Input.GetKeyDown(keyCodeIdentity))
            {
                playerHUD.OnPanelTraits();
            }
        }

        /// <summary>
        /// 피해 처리 메소드
        /// </summary>
        /// <param name="damge">피해량</param>
        public void TakeDamage(int damge)
        {
            // TODO: 실드 활성화 여부 변수 or 메소드 추가
            if(status.MaxShield > 0)
            {
                bool isShieldDestroy = status.DecreaseShield(damge);
                if(isShieldDestroy)
                {
                    status.StopShieldResilience();
                    status.StartShieldResilience();
                }
            }
            else
            {
                bool isDie = status.DecreaseHP(damge);
                status.StopHPResilience();
                status.StartHPResilience();

                if (isDie == true)
                {
                    print("Game Over");
                }
            }

        }

        /// <summary>
        /// 무기 변경 메소드
        /// </summary>
        /// <param name="newWeapon">변경할 무기</param>
        /// <remarks>
        /// PlayerController에서 관리하는 무기 멤벼변수를 변경함
        /// </remarks>
        public void SwitchingWeapon(WeaponBase newWeapon)
        {
            weapon = newWeapon;
        }
    }
}
