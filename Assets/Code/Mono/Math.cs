public class Math
{
    /// <summary>
    /// 반올림
    /// </summary>
    /// <param name="value"></param>
    /// <returns>소수점 첫째자리에서 반환한 값</returns>
    public static double Round(double value)
    {
        return System.Math.Round(value, System.MidpointRounding.AwayFromZero);
    }
}
