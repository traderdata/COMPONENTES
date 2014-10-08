using System;

namespace ModulusFE.Exceptions
{
    /// <summary>
    /// The exception that is thrown when a key is not unique
    /// </summary>
    public class KeyNotUniqueException : Exception
    {
        /// <summary>
        /// Key that is not unique
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <seealso cref="KeyNotUniqueException"/> class.
        /// </summary>
        /// <param name="key">Key that is not unique</param>
        public KeyNotUniqueException(string key)
            : base("Key not unique. Value: " + key)
        {
            Key = key;
        }
    }

    ///<summary>
    /// The exception that is thrown when a given indicator type is invalid.
    ///</summary>
    public class InvalidIndicatorException : Exception
    {
        ///<summary>
        /// Indicator type that is not valid
        ///</summary>
        public IndicatorType IndicatorType { get; private set; }

        ///<summary>
        /// Initializes a new instance of the <seealso cref="InvalidIndicatorException"/> class.
        ///</summary>
        ///<param name="indicatorType">Indicator type that is not valid</param>
        public InvalidIndicatorException(IndicatorType indicatorType)
            : base("Invalid technical indicator")
        {
            IndicatorType = indicatorType;
        }
    }

    ///<summary>
    /// The exception that is thrown when a series name is not unique
    ///</summary>
    public class SeriesNameNotUniqueException : Exception
    {
        ///<summary>
        /// Series name that is not unique
        ///</summary>
        public string SeriesName { get; private set; }
        ///<summary>
        /// Initializes a new instance of the <seealso cref="SeriesNameNotUniqueException"/> class.
        ///</summary>
        ///<param name="seriesName">Series name that is not unique</param>
        public SeriesNameNotUniqueException(string seriesName)
            : base("Series name " + seriesName + " already exists")
        {
            SeriesName = seriesName;
        }
    }

    ///<summary>
    /// The exception that is thrown when a given series name doesn't exist.
    ///</summary>
    public class SeriesDoesNotExistsException : Exception
    {
        ///<summary>
        /// Series name that doesn't exists
        ///</summary>
        public string SeriesName { get; private set; }
        ///<summary>
        /// Initializes a new instance of the <seealso cref="SeriesDoesNotExistsException"/> class.
        ///</summary>
        ///<param name="seriesName">Series name that doesn't exists</param>
        public SeriesDoesNotExistsException(string seriesName)
            : base("Series with name " + seriesName + " does not exists in current chart.")
        {
            SeriesName = seriesName;
        }
    }

    ///<summary>
    /// The exception that is thrown when the chart needs to be updated but <see cref="StockChartX.Symbol"/> is not set.
    ///</summary>
    public class SymbolNotSetException : Exception
    {
        ///<summary>
        /// Initializes a new instance of the <seealso cref="SymbolNotSetException"/> class.
        ///</summary>
        public SymbolNotSetException()
            : base("Symbol not set for chart.")
        {
        }
    }

    ///<summary>
    /// The exception that is thrown when a file is read and has a wrong serialization type or is corupt.
    ///</summary>
    public class ChartFileCorruptException : Exception
    {
        ///<summary>
        /// Initializes a new instance of the <seealso cref="ChartFileCorruptException"/> class.
        ///</summary>
        public ChartFileCorruptException()
            : base("Chart file that is being loaded was found corrupt.")
        {
        }
    }

    ///<summary>
    ///</summary>
    public class ChartException : Exception
    {
        ///<summary>
        ///</summary>
        ///<param name="message"></param>
        public ChartException(string message)
            : base(message)
        {

        }
    }
}

