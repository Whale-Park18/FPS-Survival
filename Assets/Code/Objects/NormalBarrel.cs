namespace WhalePark18.Objects
{
    /// <summary>
    /// �Ϲ� �巳�� Ŭ����
    /// </summary>
    public class NormalBarrel : InteractionObject
    {
        public override void TakeDamage(int damage)
        {
            /// �±װ� "InteractionObject"�� ��� ��ȣ�ۿ� ������Ʈ��
            /// InteractionObejct.TakeDamage()�� ȣ���ϱ� ������
            /// �⺻ �巳�뵵 INteractionObject Ŭ������ ��� �޴� Ŭ������ �����Ѵ�.
            /// �ٸ� �⺻ �巳���� ������ ü���� �����ؼ� �μ����� �ʱ� ������ ü���� ����
            /// �ʵ��� TakeDamage() ���ΰ� ����ִ�.
        }
    }
}