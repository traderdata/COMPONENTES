using System;

namespace ModulusFE
{
    /// <summary>
    /// Common extentions
    /// </summary>
    public static class Ex
    {
        /// <summary>
        /// Checks whether a value is between two given values
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool Between<T>(this T value, T min, T max) where T : IComparable<T>
        {
            return value.CompareTo(min) >= 0 &&
                   value.CompareTo(max) <= 0;
        }
    }
}
