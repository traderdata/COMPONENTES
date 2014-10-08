using System.Collections.Generic;

namespace ModulusFE
{
    namespace Tasdk
    {
        /// <summary>
        /// Recorset class, keeps multiple Fields inside
        /// </summary>
        internal class Recordset
        {
            private readonly List<Field> m_FieldNav = new List<Field>();

            /// <summary>
            /// Reference to the Navigator
            /// </summary>
            public Navigator Navigator_ { get; set; }

            ///<summary>
            /// Adds a new field
            ///</summary>
            ///<param name="newField">new field</param>
            public void AddField(Field newField)
            {
                if (m_FieldNav.Count == 0)
                    m_FieldNav.Add(new Field(0, "fake"));//cause everything is based in 1- index
                m_FieldNav.Add(newField);
            }

            ///<summary>
            /// Returns the index of a field
            ///</summary>
            ///<param name="FieldName">Field Name</param>
            ///<returns>Field index</returns>
            public int GetIndex(string FieldName)
            {
                for (int i = 0; i < m_FieldNav.Count; i++)
                    if (m_FieldNav[i].Name == FieldName)
                        return i;
                return -1;
            }

            ///<summary>
            /// Renames a field
            ///</summary>
            ///<param name="OldFieldName">Old name</param>
            ///<param name="NewFieldName">New name</param>
            public void RenameField(string OldFieldName, string NewFieldName)
            {
                int iIndex = GetIndex(OldFieldName);
                if (iIndex != -1)
                    m_FieldNav[iIndex].Name = NewFieldName;
            }

            ///<summary>
            /// Removes a field from field list
            ///</summary>
            ///<param name="FieldName">Field name</param>
            public void RemoveField(string FieldName)
            {
                int iIndex = GetIndex(FieldName);
                if (iIndex != -1)
                    m_FieldNav.RemoveAt(iIndex);
            }

            ///<summary>
            /// Returns a value from a field by record index
            ///</summary>
            ///<param name="FieldName">Field name</param>
            ///<param name="RecordIndex">Record Index</param>
            ///<returns>Value</returns>
            public double? Value(string FieldName, int RecordIndex)
            {
                int iIndex = GetIndex(FieldName);
                return iIndex != -1 ? m_FieldNav[iIndex].Value(RecordIndex) : null;
            }

            ///<summary>
            /// Returna a value from field by record index or 0.0 if such a field doesn't exists
            ///</summary>
            ///<param name="FieldName">Field name</param>
            ///<param name="RecordIndex">Record index</param>
            ///<returns>Value</returns>
            public double ValueEx(string FieldName, int RecordIndex)
            {
                int iIndex = GetIndex(FieldName);
                return iIndex != -1 ? m_FieldNav[iIndex].ValueEx(RecordIndex) : 0.0;
            }

            ///<summary>
            /// Sets field's value at a specified record index
            ///</summary>
            ///<param name="FieldName">Field name</param>
            ///<param name="RowIndex">Record index</param>
            ///<param name="Value">New value</param>
            public void Value(string FieldName, int RowIndex, double Value)
            {
                int iIndex = GetIndex(FieldName);
                if (iIndex != -1)
                    m_FieldNav[iIndex].Value(RowIndex, Value);
            }

            ///<summary>
            /// Returns a field by its name
            ///</summary>
            ///<param name="FieldName">Field name</param>
            ///<returns>Reference to a Field object</returns>
            public Field GetField(string FieldName)
            {
                int iIndex = GetIndex(FieldName);
                return iIndex != -1 ? m_FieldNav[iIndex] : new Field(0, "");
            }

            public Field GetFieldByIndex(int iIndex)
            {
                if (iIndex != -1 && iIndex < m_FieldNav.Count)
                    return m_FieldNav[iIndex];
                return new Field();
            }

            ///<summary>
            /// Copies value into given Field from a Field given by its name
            ///</summary>
            ///<param name="FieldDestination">Field to copy values to.</param>
            ///<param name="SourceFieldName">Field name from copy values from</param>
            public void CopyField(Field FieldDestination, string SourceFieldName)
            {
                int iIndex = GetIndex(SourceFieldName);
                if (iIndex == -1)
                    return;
                Field src = m_FieldNav[iIndex];
                int iRecordCount = src.RecordCount;
                for (int iRec = 1; iRec < iRecordCount + 1; iRec++)
                    FieldDestination.Value(iRec, src.Value(iRec));
            }

            ///<summary>
            /// Retuns field name by its index
            ///</summary>
            ///<param name="FieldIndex">Field index</param>
            ///<returns>Field name</returns>
            public string GetName(int FieldIndex)
            {
                if (FieldIndex >= 0 && FieldIndex < m_FieldNav.Count)
                    return m_FieldNav[FieldIndex].Name;
                return "";
            }

            ///<summary>
            /// Checks is a field with given name exists
            ///</summary>
            ///<param name="FieldName">Needed name</param>
            ///<returns>true if such a name exists, false otherwise</returns>
            public bool IsFieldValid(string FieldName)
            {
                foreach (Field f in m_FieldNav)
                    if (f.Name == FieldName)
                        return true;
                return false;
            }

            ///<summary>
            /// Gets the number of fields currently in recordset
            ///</summary>
            public int FieldCount
            {
                get { return m_FieldNav.Count - 1; }
            }
        }
    }
}
