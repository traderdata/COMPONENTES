

using ModulusFE.Exceptions;

namespace ModulusFE
{
    public partial class StockChartX
    {
        /// <summary>
        /// Gets the cross over value for the EXACT record between the two points
        /// </summary>
        /// <param name="record">0-index based</param>
        /// <param name="series1">Reference to first series</param>
        /// <param name="series2">Reference to second series</param>
        /// <returns>Found value or null if such value doesn't exists</returns>
        public double? CrossOverValue(int record, Series series1, Series series2)
        {
            double? value1a = 0, value1b = 0, value2a = 0, value2b = 0;
            int recordCount = RecordCount;
            if (record >= recordCount)
                record = recordCount - 1;

            for (int rec = record; rec > 0; rec--)
            {
                value1a = series1[record].Value;
                value1b = series1[record - 1].Value;

                value2a = series2[record].Value;
                value2b = series2[record - 1].Value;

                if (value1a > value2a && value1b < value2b)
                {
                    // Series1 just crossed above series2
                    break;
                }
                if (value1a < value2a && value1b > value2b)
                {
                    // Series1 just crossed below series2
                    break;
                }
            }

            double? A1 = value1a - value1b;
            double? A2 = value2a - value2b;
            double? C1 = A1 + 1 * value1a;
            double? C2 = A2 + 1 * value2a;
            if (A2 == A1)
                return null;
            return (C1 * A2 - A1 * C2) / (A2 - A1);
        }

        /// <summary>
        /// Returns the cross over value for the EXACT record between the two points
        /// </summary>
        /// <param name="record">0-index based</param>
        /// <param name="seriesName1">Series name 1</param>
        /// <param name="seriesName2">Series name 2</param>
        /// <returns>Found value or null if such value doesn't exists</returns>
        public double? CrossOverValue(int record, string seriesName1, string seriesName2)
        {
            Series series1 = GetSeriesByName(seriesName1);
            if (series1 == null)
                throw new SeriesDoesNotExistsException(seriesName1);
            Series series2 = GetSeriesByName(seriesName2);
            if (series2 == null)
                throw new SeriesDoesNotExistsException(seriesName2);

            return CrossOverValue(record, series1, series2);
        }

        /// <summary>
        /// Finds the most recent record where a cross over occured
        /// </summary>
        /// <param name="record">0-based index</param>
        /// <param name="series1">Reference to first series</param>
        /// <param name="series2">Reference to second series</param>
        /// <returns>0-based index</returns>
        public int? CrossOverRecord(int record, Series series1, Series series2)
        {
            int recordCount = RecordCount;
            if (record >= recordCount)
                record = recordCount - 1;

            for (int rec = record; rec > 0; rec--)
            {
                double? value1a = series1[record].Value;
                double? value1b = series1[record - 1].Value;

                double? value2a = series2[record].Value;
                double? value2b = series2[record - 1].Value;

                if (value1a > value2a && value1b < value2b)
                {
                    // Series1 just crossed above series2
                    return record;
                }
                if (value1a < value2a && value1b > value2b)
                {
                    // Series1 just crossed below series2
                    return record;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the most recent record where a cross over occured
        /// </summary>
        /// <param name="record">0-based index</param>
        /// <param name="seriesName1">Series name 1</param>
        /// <param name="seriesName2">Series name 2</param>
        /// <returns>0-based index</returns>
        public int? CrossOverRecord(int record, string seriesName1, string seriesName2)
        {
            Series series1 = GetSeriesByName(seriesName1);
            if (series1 == null)
                throw new SeriesDoesNotExistsException(seriesName1);
            Series series2 = GetSeriesByName(seriesName2);
            if (series2 == null)
                throw new SeriesDoesNotExistsException(seriesName2);

            return CrossOverRecord(record, series1, series2);
        }
    }
}