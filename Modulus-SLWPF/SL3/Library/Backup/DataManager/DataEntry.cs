using System;
using System.Collections.Generic;

namespace ModulusFE.Data
{
    /// <summary>
    /// Provides information about values stored in a series
    /// </summary>
    public partial class DataEntry
    {
        internal DataEntryCollection _collectionOwner;
        internal DataManager.DataManager _dataManager;

        ///<summary>
        /// Initializes a new instance of the <seealso cref="DataEntry"/> class.
        ///</summary>
        ///<param name="value">Value</param>
        public DataEntry(double? value)
        {
            _value = value;
        }

        ///<summary>
        /// Gets the index of data entry
        ///</summary>
        public int Index { get; set; }

        ///<summary>
        /// Gets the reference to the owener-series
        ///</summary>
        public Series SeriesOwner { get; internal set; }

        private double? _value;
        ///<summary>
        /// Gets or sets the value
        ///</summary>
        public double? Value
        {
            get { return _value; }
            set
            {
                if (_value == value) return;
                _value = value;
            }
        }

        ///<summary>
        /// Gets the timestamp 
        ///</summary>
        public DateTime TimeStamp
        {
            get { return _dataManager.GetTimeStampByIndex(Index); }
        }
    }

    ///<summary>
    /// Collection of data<seealso cref="DataEntry"/>
    ///</summary>
    public class DataEntryCollection : List<DataEntry>
    {

    }

    internal class PriceStyleValue
    {
        public DateTime TimeStamp;
        public double Value;
        public PriceStyleValue(DateTime timeStamp, double value)
        {
            TimeStamp = timeStamp;
            Value = value;
        }
    }

    internal class PriceStyleValuesCollection : List<PriceStyleValue>
    {

    }
}

