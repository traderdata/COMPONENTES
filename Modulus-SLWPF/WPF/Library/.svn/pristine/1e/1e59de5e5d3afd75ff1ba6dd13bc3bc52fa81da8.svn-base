namespace ModulusFE
{
    namespace Tasdk
    {

        /// <summary>
        /// Moving Averages
        /// </summary>
        internal class MovingAverage
        {

            ///<summary>
            /// Simple Moving Average
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource">Field Source</param>
            ///<param name="periods">Periods</param>
            ///<returns>Recordset</returns>
            public Recordset SimpleMovingAverage(Navigator pNav, Field pSource, int periods)
            { return SimpleMovingAverage(pNav, pSource, periods, "Simple Moving Average"); }
            ///<summary>
            /// Simple Moving Average
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource">Field Source</param>
            ///<param name="periods">Periods</param>
            ///<param name="Alias">Alias</param>
            ///<returns>Recordset</returns>
            public Recordset SimpleMovingAverage(Navigator pNav, Field pSource, int periods, string Alias)
            {
                int iRecord;

                Recordset Results = new Recordset();
                int iRecordCount = pNav.RecordCount;
                Field Field1 = new Field(iRecordCount, Alias);
                int iStart = periods + 1;
                pNav.Position = iStart;

                //Loop through each record in recordset
                for (iRecord = iStart; iRecord < iRecordCount + 1; iRecord++)
                {
                    double dAvg = 0.0;

                    //Loop backwards through each period
                    int iPeriod;
                    for (iPeriod = 1; iPeriod < periods + 1; iPeriod++)
                    {
                        dAvg += pSource.ValueEx(pNav.Position);
                        pNav.MovePrevious();
                    }//Period

                    //Jump forward to last position
                    pNav.Position = pNav.Position + periods;

                    //Calculate moving average
                    dAvg /= periods;
                    Field1.Value(pNav.Position, dAvg);

                    pNav.MoveNext();
                }//Record

                pNav.MoveFirst();
                Results.AddField(Field1);
                return Results;
            }

            ///<summary>
            /// Exponential Moving Average
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource">Field Source</param>
            ///<param name="periods">Periods</param>
            ///<returns>Recordset</returns>
            public Recordset ExponentialMovingAverage(Navigator pNav, Field pSource, int periods)
            { return ExponentialMovingAverage(pNav, pSource, periods, "Exponential Moving Average"); }
            ///<summary>
            /// Exponential Moving Average
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource">Field Source</param>
            ///<param name="periods">Periods</param>
            ///<param name="Alias">Alias</param>
            ///<returns>Recordset</returns>
            public Recordset ExponentialMovingAverage(Navigator pNav, Field pSource, int periods, string Alias)
            {
                double dPrime = 0;
                int iRecord;

                Recordset Results = new Recordset();

                int iRecordCount = pNav.RecordCount;
                Field Field1 = new Field(iRecordCount, Alias);

                double dExp = 2.0 / (periods + 1);

                // To prime the EMA, get an average for the first n periods
                for (iRecord = 1; iRecord < periods + 1; iRecord++)
                    dPrime += pSource.ValueEx(iRecord);
                dPrime /= periods;

                double dValue = (pSource.ValueEx(iRecord) * (1 - dExp)) + (dPrime * dExp);
                Field1.Value(periods, dValue);

                //Loop through each record in recordset
                for (iRecord = periods + 1; iRecord < iRecordCount + 1; iRecord++)
                {
                    dValue = (Field1.ValueEx(iRecord - 1) * (1 - dExp)) + (pSource.ValueEx(iRecord) * dExp);
                    Field1.Value(iRecord, dValue);
                }

                Results.AddField(Field1);
                return Results;
            }

            ///<summary>
            /// Time Series Moving Average
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource">Field Source</param>
            ///<param name="periods">Periods</param>
            ///<returns>Recordset</returns>
            public Recordset TimeSeriesMovingAverage(Navigator pNav, Field pSource, int periods)
            { return TimeSeriesMovingAverage(pNav, pSource, periods, "Time Series Moving Average"); }
            ///<summary>
            /// Time Series Moving Average
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource">Field Source</param>
            ///<param name="periods">Periods</param>
            ///<param name="Alias">Alias</param>
            ///<returns>Recordset</returns>
            public Recordset TimeSeriesMovingAverage(Navigator pNav, Field pSource, int periods, string Alias)
            {
                LinearRegression LR = new LinearRegression();
                int Record;

                int RecordCount = pNav.RecordCount;

                Field Field1 = new Field(RecordCount, Alias);

                Recordset Results = LR.Regression(pNav, pSource, periods);

                pNav.MoveFirst();
                for (Record = pNav.Position; Record < RecordCount + 1; Record++)
                {
                    double Value = Results.ValueEx("Forecast", pNav.Position);
                    Field1.Value(pNav.Position, Value);
                    pNav.MoveNext();
                }

                pNav.MoveFirst();
                Results.AddField(Field1);

                return Results;
            }

            ///<summary>
            /// Variable Moving Average
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource">Field Source</param>
            ///<param name="periods">Periods</param>
            ///<returns>Recordset</returns>
            public Recordset VariableMovingAverage(Navigator pNav, Field pSource, int periods)
            { return VariableMovingAverage(pNav, pSource, periods, "Variable Moving Average"); }
            ///<summary>
            /// Variable Moving Average
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource">Field Source</param>
            ///<param name="periods">Periods</param>
            ///<param name="Alias">Alias</param>
            ///<returns>Recordset</returns>
            public Recordset VariableMovingAverage(Navigator pNav, Field pSource, int periods, string Alias)
            {
                Oscillator OS = new Oscillator();
                int Record;

                int RecordCount = pNav.RecordCount;
                Field Field1 = new Field(RecordCount, Alias);

                //CMO must be overwritten    
                Recordset Results = OS.ChandeMomentumOscillator(pNav, pSource, 9, "CMO");

                const int Start = 2;
                pNav.Position = Start;
                for (Record = Start; Record < RecordCount + 1; Record++)
                {
                    pNav.MovePrevious();
                    double PrevVMA = Field1.ValueEx(pNav.Position);
                    pNav.MoveNext();
                    double CMO = Results.ValueEx("CMO", pNav.Position) / 100;
                    double Price = pSource.ValueEx(pNav.Position);
                    if (CMO < 0)
                        CMO = -1 * CMO;
                    double VMA = (CMO * Price) + (1 - CMO) * PrevVMA;
                    Field1.Value(pNav.Position, VMA);
                    pNav.MoveNext();
                }//Record

                pNav.MoveFirst();
                Results.AddField(Field1);

                return Results;
            }

            ///<summary>
            /// Triangular Moving Average
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource">Field Source</param>
            ///<param name="periods">Periods</param>
            ///<returns>Recordset</returns>
            public Recordset TriangularMovingAverage(Navigator pNav, Field pSource, int periods)
            { return TriangularMovingAverage(pNav, pSource, periods, "Triangular Moving Average"); }
            ///<summary>
            /// Triangular Moving Average
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource">Field Source</param>
            ///<param name="periods">Periods</param>
            ///<param name="Alias">Alias</param>
            ///<returns>Recordset</returns>
            public Recordset TriangularMovingAverage(Navigator pNav, Field pSource, int periods, string Alias)
            {

                int Record;
                int Period;
                double MA1;
                double MA2;
                double Avg;
                Recordset Results = new Recordset();

                int RecordCount = pNav.RecordCount;

                Field Field1 = new Field(RecordCount, "MA1");
                Field Field2 = new Field(RecordCount, Alias);

                if ((periods % 2) > 0)
                { //Odd number
                    MA1 = (int)((double)periods / 2) + 1;
                    MA2 = MA1;
                }
                else
                { //Even number
                    MA1 = (double)periods / 2;
                    MA2 = MA1 + 1;
                }

                int Start = periods + 1;
                pNav.Position = Start;

                //Loop through each record in recordset
                for (Record = Start; Record < RecordCount + 1; Record++)
                {

                    Avg = 0;

                    //Loop backwards through each period
                    for (Period = 1; Period < MA1 + 1; Period++)
                    {
                        Avg += pSource.ValueEx(pNav.Position);
                        pNav.MovePrevious();
                    }//Period

                    //Jump forward to last position
                    pNav.Position = pNav.Position + (int)MA1;

                    //Calculate moving average
                    Avg = Avg / MA1;
                    Field1.Value(pNav.Position, Avg);

                    pNav.MoveNext();

                }//Record

                pNav.Position = Start;

                //Loop through each record in recordset
                for (Record = Start; Record < RecordCount + 1; Record++)
                {

                    Avg = 0;

                    //Loop backwards through each period
                    for (Period = 1; Period < MA2 + 1; Period++)
                    {
                        Avg += Field1.ValueEx(pNav.Position);
                        pNav.MovePrevious();
                    }//Period

                    //Jump forward to last position
                    pNav.Position = pNav.Position + (int)MA2;

                    //Calculate moving average
                    Avg = Avg / MA2;
                    Field2.Value(pNav.Position, Avg);

                    pNav.MoveNext();

                }//Record

                pNav.MoveFirst();
                Results.AddField(Field2);
                return Results;
            }

            ///<summary>
            /// Weighted Moving Average
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource">Field Source</param>
            ///<param name="periods">Periods</param>
            ///<returns>Recordset</returns>
            public Recordset WeightedMovingAverage(Navigator pNav, Field pSource, int periods)
            { return WeightedMovingAverage(pNav, pSource, periods, "Weighted Moving Average"); }
            ///<summary>
            /// Weighted Moving Average
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource">Field Source</param>
            ///<param name="periods">Periods</param>
            ///<param name="Alias">Alias</param>
            ///<returns>Recordset</returns>
            public Recordset WeightedMovingAverage(Navigator pNav, Field pSource, int periods, string Alias)
            {
                double Weight = 0;
                int Period;
                int Record;
                Recordset Results = new Recordset();

                int RecordCount = pNav.RecordCount;
                Field Field1 = new Field(RecordCount, Alias);

                for (Period = 1; Period < periods + 1; Period++)
                {
                    Weight += Period;
                }//Period

                int Start = periods + 1;
                pNav.Position = Start;

                //Loop through each record in recordset
                for (Record = Start; Record < RecordCount + 1; Record++)
                {

                    double Total = 0;

                    //Loop backwards through each period
                    for (Period = periods; Period > 0; Period--)
                    {
                        Total += Period * pSource.ValueEx(pNav.Position);
                        pNav.MovePrevious();
                    }//Period

                    //Jump forward to last position
                    pNav.Position = pNav.Position + periods;

                    //Calculate moving average
                    Total = Total / Weight;
                    Field1.Value(pNav.Position, Total);

                    pNav.MoveNext();

                }//Record

                pNav.MoveFirst();
                Results.AddField(Field1);
                return Results;
            }

            ///<summary>
            /// VIDYA
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource">Field Source</param>
            ///<param name="periods">Periods</param>
            ///<param name="R2Scale">R2 Scale</param>
            ///<returns>Recordset</returns>
            public Recordset VIDYA(Navigator pNav, Field pSource, int periods, double R2Scale)
            { return VIDYA(pNav, pSource, periods, R2Scale, "VIDYA"); }
            ///<summary>
            /// VIDYA
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource">Field Source</param>
            ///<param name="periods">Periods</param>
            ///<param name="R2Scale">R2 Scale</param>
            ///<param name="Alias">Alias</param>
            ///<returns>Recordset</returns>
            public Recordset VIDYA(Navigator pNav, Field pSource, int periods, double R2Scale, string Alias)
            {
                int Record;
                LinearRegression LR = new LinearRegression();

                int RecordCount = pNav.RecordCount;

                Field Field1 = new Field(RecordCount, Alias);

                Recordset Results = LR.Regression(pNav, pSource, periods);

                const int Start = 2;
                pNav.Position = Start;
                for (Record = Start; Record < RecordCount + 1; Record++)
                {
                    pNav.MovePrevious();
                    double PreviousValue = pSource.ValueEx(pNav.Position);
                    pNav.MoveNext();
                    double R2Scaled = Results.ValueEx("RSquared", pNav.Position) * R2Scale;
                    Field1.Value(pNav.Position, R2Scaled *
                    pSource.Value(pNav.Position) + (1 - R2Scaled) * PreviousValue);
                    pNav.MoveNext();
                }//Record

                Results.AddField(Field1);
                return Results;
            }

            ///<summary>
            /// Welles Wilder Smoothing
            ///</summary>
            ///<param name="pSource">Field Source</param>
            ///<param name="periods">Periods</param>
            ///<returns>Recordset</returns>
            public Recordset WellesWilderSmoothing(Field pSource, int periods)
            { return WellesWilderSmoothing(pSource, periods, "Welles Wilder Smoothing"); }
            ///<summary>
            /// Welles Wilder Smoothing
            ///</summary>
            ///<param name="pSource">Field Source</param>
            ///<param name="periods">Periods</param>
            ///<param name="Alias">Alias</param>
            ///<returns>Recordset</returns>
            public Recordset WellesWilderSmoothing(Field pSource, int periods, string Alias)
            {
                Recordset Results = new Recordset();
                int Record;

                int RecordCount = pSource.RecordCount;
                Field Field1 = new Field(RecordCount, Alias);

                for (Record = 2; Record < RecordCount + 1; ++Record)
                {
                    double Value = Field1.ValueEx(Record - 1) + 1 / (double)periods * (pSource.ValueEx(Record) - Field1.ValueEx(Record - 1));
                    Field1.Value(Record, Value);
                } //Record
                Results.AddField(Field1);

                return Results;
            }

            // Moving Average Switch
            public Recordset MovingAverageSwitch(Navigator pNav, Field field, int periods, IndicatorType maType, string alias)
            {
                Recordset ret = null;
                switch (maType)
                {
                    case IndicatorType.SimpleMovingAverage:
                        ret = SimpleMovingAverage(pNav, field, periods, alias);
                        break;
                    case IndicatorType.ExponentialMovingAverage:
                        ret = ExponentialMovingAverage(pNav, field, periods, alias);
                        break;
                    case IndicatorType.TimeSeriesMovingAverage:
                        ret = TimeSeriesMovingAverage(pNav, field, periods, alias);
                        break;
                    case IndicatorType.TriangularMovingAverage:
                        ret = TriangularMovingAverage(pNav, field, periods, alias);
                        break;
                    case IndicatorType.VariableMovingAverage:
                        ret = VariableMovingAverage(pNav, field, periods, alias);
                        break;
                    case IndicatorType.WeightedMovingAverage:
                        ret = WeightedMovingAverage(pNav, field, periods, alias);
                        break;
                    case IndicatorType.VIDYA:
                        ret = VIDYA(pNav, field, periods, 0.65, alias);
                        break;
                    case IndicatorType.WellesWilderSmoothing:
                        ret = WellesWilderSmoothing(field, periods, alias);
                        break;
                }
                return ret;
            }
        }
    }
}
