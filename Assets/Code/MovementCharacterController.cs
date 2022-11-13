using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovementCharacterController : MonoBehaviour
{
    [SerializeField]
    private float   moveSpeed = 4f;     // �̵��ӵ�
    private Vector3 moveForce;          // �̵� ��(����Ƽ ��ǥ���� �ٴ��� x, z���̱� ������ x, z�� y���� ������ ����� ���� �̵��� ����)

    [SerializeField]
    private float   jumpForce;          // ���� ��
    
    /// <summary>
    /// Tip.
    ///  Physics.gravity ������ ����� rigidbody�� ������ �߷��� ������ ���� ������
    ///  �� �� ū �߷� ���� �����ϱ� ���� ������ ���� ����
    /// </summary>
    [SerializeField]
    private float   gravity = -9.81f;    // �߷� ���

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

        // 1�ʴ� moveForce �ӷ����� �̵�
        characterController.Move(moveForce * Time.deltaTime);
    }

    public void MoveTo(Vector3 direction)
    {
        /// Tip.
        ///  ���� �Ʒ��� �ٶ󺸰� �̵��� ��� ĳ���Ͱ� �������� �߰ų� �Ʒ��� ��������� �ϱ� ������
        ///  direction�� �״�� ������� �ʰ�, moveForce ������ x, z ���� �־ ���
        ///  
        /// �� ȸ�� ���� �����ϴ°�?
        ///  ī�޶� ȸ������ ���� ������ ���ϱ� ������ ȸ�� ���� ���ؼ� �����ؾ� ��
        
        // �̵� ���� = ĳ���� ȸ�� �� * ���� ��
        direction = transform.rotation * new Vector3(direction.x, 0, direction.z);

        // �̵� �� = �̵� ���� * �ӵ�
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
