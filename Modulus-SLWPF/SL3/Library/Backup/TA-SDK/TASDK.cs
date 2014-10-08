using System;

namespace ModulusFE
{
    namespace Tasdk
    {
        ///<summary>
        /// Common functions for the TA-SDK library
        ///</summary>
        internal class TASDK
        {
            ///<summary>
            /// Returns maximum from 2 given numbers. If they are equal returns defaultValue
            ///</summary>
            ///<param name="Value1">Value1</param>
            ///<param name="Value2">Value2</param>
            ///<param name="defValue">Default value in case values are equal</param>
            ///<typeparam name="T">Parameter type</typeparam>
            ///<returns>Maximum or default value</returns>
            public T max<T>(T Value1, T Value2, T defValue) where T : IComparable
            {
                if (Value1.CompareTo(Value2) > 0)
                    return Value1;
                return Value1.CompareTo(Value2) < 0 ? Value2 : defValue;
            }

            ///<summary>
            /// Returns minimum from 2 given numbers. If they are equal returns defaultValue
            ///</summary>
            ///<param name="Value1">Value1</param>
            ///<param name="Value2">Value2</param>
            ///<param name="defValue">Default value in case values are equal</param>
            ///<typeparam name="T">Parameter type</typeparam>
            ///<returns>Minimum or default value</returns>
            public T min<T>(T Value1, T Value2, T defValue) where T : IComparable
            {
                if (Value1.CompareTo(Value2) > 0)
                    return Value2;
                return Value1.CompareTo(Value2) < 0 ? Value1 : defValue;
            }

            ///<summary>
            /// Returns a normalized value, between 0..1
            ///</summary>
            ///<param name="MaxValue">Max Value</param>
            ///<param name="MinValue">Min Value</param>
            ///<param name="ValueToBeNormalized">Value to be normalized</param>
            ///<returns>Normalized value</returns>
            public double Normalize(double MaxValue, double MinValue, double ValueToBeNormalized)
            {
                if (MaxValue == MinValue)
                    return 0.0;
                return (ValueToBeNormalized - MinValue) / (MaxValue - MinValue);
            }
        }
    }
}
