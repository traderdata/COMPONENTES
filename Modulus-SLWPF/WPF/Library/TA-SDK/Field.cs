using System.Collections.Generic;

namespace ModulusFE
{
    namespace Tasdk
    {
        ///<summary>
        /// Field class used to store values that are used in calculations
        ///</summary>
        internal class Field
        {
            private int m_iRecordCount;
            private List<string> m_strpNav = new List<string>();
            private List<double?> m_dblpNav = new List<double?>();

            ///<summary>
            /// Constructor
            ///</summary>
            ///<param name="iRecordCount">Record count that will be stored</param>
            ///<param name="sName">Field Name</param>
            public Field(int iRecordCount, string sName)
            {
                Name = sName;
                m_iRecordCount = iRecordCount;
                Note_ = new Note();
                m_strpNav.AddRange(new string[iRecordCount + 2]); //resize list to needed size. There is no method Resize like in STL
                m_dblpNav.AddRange(new double?[iRecordCount + 2]);
            }

            public Field()
                : this(0, "fake")
            {
            }

            ///<summary>
            /// A note about this field
            ///</summary>
            public Note Note_ { get; set; }

            ///<summary>
            /// Gets RecordCount currently stored
            ///</summary>
            public int RecordCount
            {
                get { return m_iRecordCount; }
                set { m_iRecordCount = value; }
            }

            ///<summary>
            /// Field Name
            ///</summary>
            public string Name { get; set; }

            ///<summary>
            /// Returns a value by index
            ///</summary>
            ///<param name="iIndex">Index</param>
            ///<returns>Value</returns>
            public double? Value(int iIndex)
            {
                return m_dblpNav[iIndex];
            }

            ///<summary>
            /// Returns a value by index or 0.0 if the value is null
            ///</summary>
            ///<param name="iIndex">Index</param>
            ///<returns>Value or 0.0</returns>
            public double ValueEx(int iIndex)
            {
                return m_dblpNav[iIndex].HasValue ? m_dblpNav[iIndex].Value : 0.0;
            }

            ///<summary>
            /// Sets the value at a given index
            ///</summary>
            ///<param name="iIndex">Index</param>
            ///<param name="dValue">New Value</param>
            public void Value(int iIndex, double? dValue)
            {
                m_dblpNav[iIndex] = dValue;
            }

            public void SetValue(int iIndex, double? value)
            {
                m_dblpNav[iIndex] = value;
            }

            ///<summary>
            /// Gets a string value from a specified index
            ///</summary>
            ///<param name="iIndex">Index</param>
            ///<returns>String value</returns>
            public string ValueStr(int iIndex)
            {
                return m_strpNav[iIndex];
            }

            ///<summary>
            /// Sets a string value at a specified index
            ///</summary>
            ///<param name="iIndex">Index</param>
            ///<param name="sValue">New string value</param>
            public void ValueStr(int iIndex, string sValue)
            {
                m_strpNav[iIndex] = sValue;
            }

            public void Initialize(int iRecordCount, string sName)
            {
                Name = sName;
                RecordCount = iRecordCount;
                m_strpNav.AddRange(new string[iRecordCount + 2]); //resize list to needed size. There is no method Resize like in STL
                m_dblpNav.AddRange(new double?[iRecordCount + 2]);

                for (int n = 0; n < m_dblpNav.Count; n++)
                    m_dblpNav[n] = 0;

            }

        }
    }
}
