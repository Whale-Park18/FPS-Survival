using UnityEngine;

public class MovementTransform : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private Vector3 moveDirection = Vector3.zero;

    /// <summary>
    /// �̵� ������ �������� �˾Ƽ� �̵��ϵ��� Update() �޼ҵ忡 �ۼ�
    /// </summary>
    private void Update()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    /// <summary>
    /// �ܺο��� �Ű������� �̵� ������ ����
    /// </summary>
    /// <param name="direction">�̵� ����</param>
    public void MoveTo(Vector3 direction)
    {
        moveDirection = direction;
    }
}
