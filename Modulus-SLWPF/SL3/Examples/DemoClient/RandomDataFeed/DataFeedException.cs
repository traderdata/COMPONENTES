using System;

namespace ModulusFE.OMS.Interface
{
    public class DataFeedException : Exception
    {
        public DataFeedException(string message)
            : base(message)
        {
        }
    }
}
