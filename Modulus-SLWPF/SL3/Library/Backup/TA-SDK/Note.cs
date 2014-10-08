namespace ModulusFE
{
    namespace Tasdk
    {
        ///<summary>
        /// Note
        ///</summary>
        internal class Note
        {
            ///<summary>
            /// Constructor
            ///</summary>
            public Note()
            {
                Period = 0;
                Value = 0.0;
                Note_ = "";
            }

            ///<summary>
            /// Period
            ///</summary>
            public int Period { get; set; }
            ///<summary>
            /// Value
            ///</summary>
            public double Value { get; set; }
            ///<summary>
            /// Note
            ///</summary>
            public string Note_ { get; set; }
        }
    }
}

