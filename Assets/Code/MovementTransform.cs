using UnityEngine;

public class MovementTransform : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private Vector3 moveDirection = Vector3.zero;

    /// <summary>
    /// 이동 방향이 설정도면 알아서 이동하도록 Update() 메소드에 작성
    /// </summary>
    private void Update()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    /// <summary>
    /// 외부에서 매개변수로 이동 방향을 설정
    /// </summary>
    /// <param name="direction">이동 방향</param>
    public void MoveTo(Vector3 direction)
    {
        moveDirection = direction;
    }
}
