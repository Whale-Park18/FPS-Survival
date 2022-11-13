using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovementCharacterController : MonoBehaviour
{
    [SerializeField]
    private float   moveSpeed = 4f;     // 이동속도
    private Vector3 moveForce;          // 이동 힘(유니티 좌표에서 바닥은 x, z축이기 때문에 x, z와 y축은 별도로 계산해 실제 이동에 적용)

    [SerializeField]
    private float   jumpForce;          // 점프 힘
    
    /// <summary>
    /// Tip.
    ///  Physics.gravity 변수를 사용해 rigidbody와 동일한 중력을 설정할 수도 있지만
    ///  좀 더 큰 중력 값을 적용하기 위해 변수를 따로 선언
    /// </summary>
    [SerializeField]
    private float   gravity = -9.81f;    // 중력 계수

    public float MoveSpeed
    {
        set => moveSpeed = Mathf.Max(0, value);
        get => moveSpeed;
    }

    private CharacterController characterController;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();       
    }

    private void Update()
    {
        if(characterController.isGrounded == false)
        {
            moveForce.y += gravity * Time.deltaTime;
        }

        // 1초당 moveForce 속력으로 이동
        characterController.Move(moveForce * Time.deltaTime);
    }

    public void MoveTo(Vector3 direction)
    {
        /// Tip.
        ///  위나 아래를 바라보고 이동할 경우 캐릭터가 공중으로 뜨거나 아래로 가라앉으려 하기 때문에
        ///  direction을 그대로 사용하지 않고, moveForce 변수에 x, z 값만 넣어서 사용
        ///  
        /// 왜 회전 값을 연산하는가?
        ///  카메라 회전으로 전방 방향이 변하기 때문에 회전 값을 곱해서 연산해야 함
        
        // 이동 방향 = 캐릭터 회전 값 * 방향 값
        direction = transform.rotation * new Vector3(direction.x, 0, direction.z);

        // 이동 힘 = 이동 방향 * 속도
        moveForce = new Vector3(direction.x * moveSpeed, moveForce.y, direction.z * moveSpeed);
    }

    public void Jump()
    {
        if(characterController.isGrounded)
        {
            moveForce.y = jumpForce;
        }
    }
}
