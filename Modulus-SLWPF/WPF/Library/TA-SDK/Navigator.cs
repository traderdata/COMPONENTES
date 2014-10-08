namespace ModulusFE
{
    namespace Tasdk
    {

        ///<summary>
        /// Class used to navigate through a recordset
        ///</summary>
        internal class Navigator
        {
            private Recordset m_recordset;
            private int m_iIndex;

            /// <summary>
            /// Sets/Gets the recordset associated with it
            /// </summary>
            public Recordset Recordset_
            {
                get { return m_recordset; }
                set
                {
                    m_recordset = value;
                    string sName = m_recordset.GetName(1);
                    RecordCount = m_recordset.GetField(sName).RecordCount;
                    m_iIndex = 1;
                }
            }

            ///<summary>
            /// Record Count
            ///</summary>
            public int RecordCount { get; private set; }

            ///<summary>
            /// Current cursor position
            ///</summary>
            public int Position
            {
                get { return m_iIndex; }
                set
                {
                    if (value > 0)
                        m_iIndex = value;
                }
            }

            ///<summary>
            /// Move to next record
            ///</summary>
            public void MoveNext()
            {
                Position++;
            }

            ///<summary>
            /// Move to previous record
            ///</summary>
            public void MovePrevious()
            {
                Position--;
            }

            ///<summary>
            /// Move to first record
            ///</summary>
            public void MoveFirst()
            {
                Position = 1;
            }

            ///<summary>
            /// Move to last record
            ///</summary>
            public void MoveLast()
            {
                Position = RecordCount;
            }
        }
    }
}
