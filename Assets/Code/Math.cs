public class Math
{
    /// <summary>
    /// �ݿø�
    /// </summary>
    /// <param name="value"></param>
    /// <returns>�Ҽ��� ù°�ڸ����� ��ȯ�� ��</returns>
    public static double Round(double value)
    {
        return System.Math.Round(value, System.MidpointRounding.AwayFromZero);
    }
}
