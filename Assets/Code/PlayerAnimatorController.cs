using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public float MoveSpeed
    {
        set => animator.SetFloat("movementSpeed", value);
        get => animator.GetFloat("movementSpeed");
    }

    public void OnReload()
    {
        animator.SetTrigger("onReload");
    }

    public bool AimModeIs
    {
        set => animator.SetBool("isAimMode", value);
        get => animator.GetBool("isAimMode");
    }

    public void Play(string stateName, int layer, float normalizedTime)
    {
        animator.Play(stateName, layer, normalizedTime);
    }

    /// <summary>
    /// �Ű������� ���� name �ִϸ��̼��� ���� ��������� Ȯ���ϰ� �� ����� ��ȯ��
    /// </summary>
    /// <param name="name"></param>
    /// <returns>name �ִϸ��̼� ��� ����</returns>
    public bool CurrentAnimationIs(string name)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(name);
    }

    public void SetFloat(string name, float value)
    {
        animator.SetFloat(name, value);
    }
}
