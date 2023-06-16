using UnityEngine;

using WhalePark18.UI;
using WhalePark18.UI.Window;
using WhalePark18.Weapon;
using WhalePark18.Manager;

namespace WhalePark18.Character.Player
{
    /// <summary>
    /// 최상위 플레이어 컨트롤러 클래스
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        /****************************************
         * PlayerController
         ****************************************/
        private PlayerStatus                status;             // 이동속도 등의 플레이어 정보
        private RotateToMouse               rotateToMouse;      // 마우스 이동으로 카메라 회전
        private MovementCharacterController movement;           // 키보드 입력으로 플레이어 이동, 점프
        private AudioData                   audioData;          // 오디오 관련 정보
        private WeaponBase                  weapon;             // 무기 최상위 클래스를 이용항 공격 제어
        private PlayerHUDController         hudController;
        private bool                        active = true;

        [Header("Input KeyCodes")]
        [SerializeField]
        private KeyCode keyCodeRun                  = KeyCode.LeftShift;    // 달리기 키
        [SerializeField]
        private KeyCode keyCodeJump                 = KeyCode.Space;        // 점프 키
        [SerializeField]
        private KeyCode keyCodeReload               = KeyCode.R;            // 탄 재장전 키
        [SerializeField]
        private KeyCode keyCodeWindowTrait          = KeyCode.Tab;          // 특성 창 키
        [SerializeField]
        private KeyCode keyCodeWindowTemporaryMenu  = KeyCode.Escape;       // 메뉴 창 키

        [Header("Audio Clips")]
        [SerializeField]
        private AudioClip audioClipWalk;    // 걷기 사운드
        [SerializeField]
        private AudioClip audioClipRun;     // 달리기 사운드

        [Header("HUD")]
        [SerializeField]
        private PlayerHUD playerHUD;        // 플레이어 HUD

        private void Awake()
        {
            /// 마우스 커서 설정
            /// FPS 게임 실행 환경에선 커서 조준점이 사용되며 조준점은 항상 화면 정중앙이다.
            /// 즉, 커서 표시를 비활성화 하고, 현재 위치에 고정시킨다.
            /// 
            /// * 게임 매니저에서 상태를 변경하며 관리할 것이기 떄문에 삭제 예정
            //GameManager.Instance.SetCursorActive(false);

            /// 컴포넌트 초기화
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
        /// PlayerController의 작동 여부를 설정하는 메소드
        /// </summary>
        /// <param name="active">작동 여부</param>
        /// <remarks>
        /// PlayerController 오브젝트의 Active를 설정하는 것이 아닌 
        /// PlayerController가 추상적으로 활동을 멈춘 것을 의미함
        /// </remarks>
        public void SetActive(bool active)
        {
            this.active = active; 

            /// 플레이어의 움직임을 담당하는 컴포넌트를 비활성화 한다.
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

            /// 이동 중일 때(걷기 or 달리기)
            if (x != 0 || z != 0)
            {
                bool isRun = false;

                /// 옆이나 뒤로 이동할 때는 달릴 수 없다.
                if (z > 0) isRun = Input.GetKey(keyCodeRun);

                /// 달리기가 입력 되었고 무기가 에임 모드가 아니라면 달리기 속도로 이동한다.
                movement.MoveSpeed = isRun && weapon.IsAimMode == false ? status.RunSpeed : status.WalkSpeed;
                weapon.Animator.MoveSpeed = isRun && weapon.IsAimMode == false ? 1 : 0.5f;
                audioData.audioSource.clip = isRun && weapon.IsAimMode == false ? audioClipRun : audioClipWalk;

                /// 방향키 입력 여부는 매 프레임 확인하기 때문에
                /// 재새중일 때는 다시 재생하지 않도록 isPlaying으로 체크해서 재생
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

                /// 멈췄을 때 사운드가 재생중이면 정지
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
        /// 피해 처리 메소드
        /// </summary>
        /// <param name="damge">피해량</param>
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
