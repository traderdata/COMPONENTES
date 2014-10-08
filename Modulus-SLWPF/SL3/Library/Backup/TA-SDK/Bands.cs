using System;

namespace ModulusFE
{
    namespace Tasdk
    {
        /// <summary>
        /// Bands
        /// </summary>
        internal class Bands
        {
            /// <summary>
            /// Bollinger bands
            /// </summary>
            /// <param name="pNav">Navigator</param>
            /// <param name="pSource">Field source</param>
            /// <param name="periods">Periods</param>
            /// <param name="StandardDeviations">Standard deviation</param>
            /// <param name="MAType">Moving Average Type</param>
            /// <returns>Recordset</returns>
            public Recordset BollingerBands(Navigator pNav, Field pSource, int periods, int StandardDeviations, IndicatorType MAType)
            {
                MovingAverage MA = new MovingAverage();
                Recordset Results = null;
                int Record;

                int RecordCount = pNav.RecordCount;

                if (MAType < Constants.MA_START || MAType > Constants.MA_END)
                    return null;

                if (periods < 1 || periods > RecordCount)
                    return null;

                if (StandardDeviations < 0 || StandardDeviations > 100)
                    return null;

                Field Field1 = new Field(RecordCount, "Bollinger Band Bottom");
                Field Field2 = new Field(RecordCount, "Bollinger Band Top");

                switch (MAType)
                {
                    case IndicatorType.SimpleMovingAverage:
                        Results = MA.SimpleMovingAverage(pNav, pSource, periods, "Bollinger Band Median");
                        break;
                    case IndicatorType.ExponentialMovingAverage:
                        Results = MA.ExponentialMovingAverage(pNav, pSource, periods, "Bollinger Band Median");
                        break;
                    case IndicatorType.TimeSeriesMovingAverage:
                        Results = MA.TimeSeriesMovingAverage(pNav, pSource, periods, "Bollinger Band Median");
                        break;
                    case IndicatorType.TriangularMovingAverage:
                        Results = MA.TriangularMovingAverage(pNav, pSource, periods, "Bollinger Band Median");
                        break;
                    case IndicatorType.VariableMovingAverage:
                        Results = MA.VariableMovingAverage(pNav, pSource, periods, "Bollinger Band Median");
                        break;
                    case IndicatorType.WeightedMovingAverage:
                        Results = MA.WeightedMovingAverage(pNav, pSource, periods, "Bollinger Band Median");
                        break;
                    case IndicatorType.VIDYA:
                        Results = MA.VIDYA(pNav, pSource, periods, 0.65, "Bollinger Band Median");
                        break;
                }
                if (Results == null)
                    return null;

                int Start = periods + 1;
                pNav.Position = Start;

                for (Record = Start; Record < RecordCount + 1; Record++)
                {
                    double Sum = 0;
                    double Value = Results.ValueEx("Bollinger Band Median", pNav.Position);

                    int Period;
                    for (Period = 1; Period < periods + 1; Period++)
                    {
                        Sum += (pSource.Value(pNav.Position).Value - Value) *
                               (pSource.Value(pNav.Position).Value - Value);
                        pNav.MovePrevious();
                    }//Period

                    pNav.Position = pNav.Position + periods;

                    Value = StandardDeviations * Math.Sqrt(Sum / periods);
                    Field1.Value(pNav.Position,
                    Results.Value("Bollinger Band Median", pNav.Position) - Value);
                    Field2.Value(pNav.Position,
                    Results.Value("Bollinger Band Median", pNav.Position) + Value);

                    pNav.MoveNext();

                }//Record

                //Append fields to recordset
                Results.AddField(Field1);
                Results.AddField(Field2);
                return Results;
            }

            ///<summary>
            /// Moving Average Enveloper
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource">Field Source</param>
            ///<param name="periods">Periods</param>
            ///<param name="MAType">Moving Average Type</param>
            ///<param name="Shift">Shift</param>
            ///<returns>Recordset</returns>
            public Recordset MovingAverageEnvelope(Navigator pNav, Field pSource, int periods, IndicatorType MAType, double Shift)
            {
                MovingAverage MA = new MovingAverage();
                Recordset Results = null;
                int Record;

                int RecordCount = pNav.RecordCount;

                if (MAType < Constants.MA_START || MAType > Constants.MA_END)
                    return null;

                if (periods < 1 || periods > RecordCount)
                    return null;

                if (Shift < 0 || Shift > 100)
                    return null;

                Field Field1 = new Field(RecordCount, "Envelope Top");
                Field Field2 = new Field(RecordCount, "Envelope Bottom");

                switch (MAType)
                {
                    case IndicatorType.SimpleMovingAverage:
                        Results = MA.SimpleMovingAverage(pNav, pSource, periods, "Temp");
                        break;
                    case IndicatorType.ExponentialMovingAverage:
                        Results = MA.ExponentialMovingAverage(pNav, pSource, periods, "Temp");
                        break;
                    case IndicatorType.TimeSeriesMovingAverage:
                        Results = MA.TimeSeriesMovingAverage(pNav, pSource, periods, "Temp");
                        break;
                    case IndicatorType.TriangularMovingAverage:
                        Results = MA.TriangularMovingAverage(pNav, pSource, periods, "Temp");
                        break;
                    case IndicatorType.VariableMovingAverage:
                        Results = MA.VariableMovingAverage(pNav, pSource, periods, "Temp");
                        break;
                    case IndicatorType.WeightedMovingAverage:
                        Results = MA.WeightedMovingAverage(pNav, pSource, periods, "Temp");
                        break;
                    case IndicatorType.VIDYA:
                        Results = MA.VIDYA(pNav, pSource, periods, 0.65, "Temp");
                        break;
                }
                if (Results == null)
                    return null;

                pNav.MoveFirst();
                Shift = Shift / 100;

                for (Record = 1; Record < RecordCount + 1; Record++)
                {
                    double Value = Results.ValueEx("Temp", pNav.Position);
                    Field1.Value(pNav.Position, Value + (Value * Shift));

                    Value = Results.ValueEx("Temp", pNav.Position);
                    Field2.Value(pNav.Position, Value - (Value * Shift));

                    pNav.MoveNext();
                }//Record

                //Append fields to recordset
                Results.AddField(Field1);
                Results.AddField(Field2);

                return Results;
            }

            ///<summary>
            /// High Low Bands
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="HighPrice">Field High Price</param>
            ///<param name="LowPrice">Field Low Price</param>
            ///<param name="ClosePrice">Field ClosePrice</param>
            ///<param name="periods">Periods</param>
            ///<returns>Recordset</returns>
            public Recordset HighLowBands(Navigator pNav, Field HighPrice, Field LowPrice, Field ClosePrice, int periods)
            {
                MovingAverage MA = new MovingAverage();
                Recordset Results = new Recordset();

                if (periods < 6 || periods > pNav.RecordCount)
                    return null;

                Recordset RS1 = MA.VIDYA(pNav, HighPrice, periods, 0.8, "High Low Bands Top");
                Recordset RS2 = MA.VIDYA(pNav, ClosePrice, periods / 2, 0.8, "High Low Bands Median");
                Recordset RS3 = MA.VIDYA(pNav, LowPrice, periods, 0.8, "High Low Bands Bottom");

                Results.AddField(RS1.GetField("High Low Bands Top"));
                Results.AddField(RS2.GetField("High Low Bands Median"));
                Results.AddField(RS3.GetField("High Low Bands Bottom"));

                // Remove fields so recordset can be deleted
                RS1.RemoveField("High Low Bands Top");
                RS2.RemoveField("High Low Bands Median");
                RS3.RemoveField("High Low Bands Bottom");

                pNav.MoveFirst();
                return Results;
            }

            ///<summary>
            /// Fractal Chaos bands
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pOHLCV">OHLCV Recordset</param>
            ///<param name="periods">Periods</param>
            ///<returns>Records</returns>
            public Recordset FractalChaosBands(Navigator pNav, Recordset pOHLCV, int periods)
            {
                MovingAverage MA = new MovingAverage();
                Recordset Results = new Recordset();

                int RecordCount = pNav.RecordCount;
                int Record;

                if (periods < 1)
                    periods = 100;

                Field fHiFractal = new Field(RecordCount, "Fractal High");
                Field fLoFractal = new Field(RecordCount, "Low High");
                Field fH = pOHLCV.GetField("High");
                Field fL = pOHLCV.GetField("Low");
                Field fFR = new Field(RecordCount, "FR");

                Field fH1 = new Field(RecordCount, "High 1");
                Field fH2 = new Field(RecordCount, "High 2");
                Field fH3 = new Field(RecordCount, "High 3");
                Field fH4 = new Field(RecordCount, "High 4");

                Field fL1 = new Field(RecordCount, "Low 1");
                Field fL2 = new Field(RecordCount, "Low 2");
                Field fL3 = new Field(RecordCount, "Low 3");
                Field fL4 = new Field(RecordCount, "Low 4");

                for (Record = 5; Record < RecordCount + 1; ++Record)
                {
                    fH1.Value(Record, fH.ValueEx(Record - 4));
                    fL1.Value(Record, fL.ValueEx(Record - 4));

                    fH2.Value(Record, fH.ValueEx(Record - 3));
                    fL2.Value(Record, fL.ValueEx(Record - 3));

                    fH3.Value(Record, fH.ValueEx(Record - 2));
                    fL3.Value(Record, fL.ValueEx(Record - 2));

                    fH4.Value(Record, fH.ValueEx(Record - 1));
                    fL4.Value(Record, fL.ValueEx(Record - 1));
                }

                for (Record = 1; Record < RecordCount + 1; ++Record)
                    fHiFractal.Value(Record, (fH.ValueEx(Record) + fL.ValueEx(Record)) / 3);

                Recordset rsFractals = MA.SimpleMovingAverage(pNav, fHiFractal, periods, "Fractal High");
                fHiFractal = rsFractals.GetField("Fractal High");
                rsFractals.RemoveField("Fractal High");

                rsFractals = MA.SimpleMovingAverage(pNav, fLoFractal, periods, "Fractal Low");
                fLoFractal = rsFractals.GetField("Fractal Low");
                rsFractals.RemoveField("Fractal Low");

                for (Record = 1; Record < RecordCount + 1; ++Record)
                {
                    fHiFractal.Value(Record, fH3.ValueEx(Record) + fHiFractal.ValueEx(Record));
                    fLoFractal.Value(Record, fL3.ValueEx(Record) - fLoFractal.ValueEx(Record));
                }

                for (Record = 2; Record < RecordCount + 1; ++Record)
                {

                    if ((fH3.Value(Record) > fH1.Value(Record)) &&
                        (fH3.Value(Record) > fH2.Value(Record)) &&
                        (fH3.Value(Record) >= fH4.Value(Record)) &&
                        (fH3.Value(Record) >= fH.Value(Record)))
                    {
                        fFR.Value(Record, fHiFractal.Value(Record).Value);
                    }
                    else
                    {
                        fFR.Value(Record, 0);
                    }

                    if (fFR.Value(Record) == 0)
                    {
                        if ((fL3.Value(Record) < fL1.Value(Record)) &&
                            (fL3.Value(Record) < fL2.Value(Record)) &&
                            (fL3.Value(Record) <= fL4.Value(Record)) &&
                            (fL3.Value(Record) <= fL.Value(Record)))
                        {
                            fFR.Value(Record, fLoFractal.Value(Record));
                        }
                        else
                        {
                            fFR.Value(Record, 0);
                        }
                    }

                    if (fHiFractal.Value(Record) == fFR.Value(Record))
                    {
                        fHiFractal.Value(Record, fH3.Value(Record));
                    }
                    else
                    {
                        fHiFractal.Value(Record, fHiFractal.Value(Record - 1));
                    }

                    if (fLoFractal.Value(Record) == fFR.Value(Record))
                    {
                        fLoFractal.Value(Record, fL3.Value(Record));
                    }
                    else
                    {
                        fLoFractal.Value(Record, fLoFractal.Value(Record - 1));
                    }

                }

                // Added 12/19/2005 TW
                for (Record = 2; Record < RecordCount + 1; ++Record)
                {
                    if (fLoFractal.Value(Record) == 0)
                        fLoFractal.Value(Record, null);
                    if (fHiFractal.Value(Record) == 0)
                        fHiFractal.Value(Record, null);
                }

                Results.AddField(fHiFractal);
                Results.AddField(fLoFractal);

                return Results;
            }

            ///<summary>
            /// Prime Number Bands
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="HighPrice">Field High Price</param>
            ///<param name="LowPrice">Field Low Price</param>
            ///<returns>Recordset</returns>
            public Recordset PrimeNumberBands(Navigator pNav, Field HighPrice, Field LowPrice)
            {
                Recordset Results = new Recordset();
                int RecordCount = pNav.RecordCount;
                int Record;
                Field fTop = new Field(RecordCount, "Prime Bands Top");
                Field fBottom = new Field(RecordCount, "Prime Bands Bottom");
                General GN = new General();
                long Top = 0, Bottom = 0;

                for (Record = 1; Record != RecordCount + 1; ++Record)
                {
                    long Value = (long)(LowPrice.Value(Record));
                    if (Value < 10)
                        Value = Value * 10;

                    long N;
                    for (N = Value; N != 1; --N)
                    {
                        if (General.IsPrime(N))
                        {
                            Bottom = N;
                            break;
                        }
                    }
                    fBottom.Value(Record, Bottom);

                    Value = (long)(HighPrice.Value(Record));
                    if (Value < 10)
                        Value = Value * 10;

                    for (N = Value; N != Value * 2; ++N)
                    {
                        if (General.IsPrime(N))
                        {
                            Top = N;
                            break;
                        }
                    }
                    fTop.Value(Record, Top);
                } // Record
                Results.AddField(fTop);
                Results.AddField(fBottom);

                return Results;
            }

            public Recordset Keltner(Navigator pNav, Recordset pOHLCV, int Periods, double Factor, IndicatorType MAType, string Alias)
            {   // Same as STARC
                Recordset Results = new Recordset();
                int recordCount = pOHLCV.GetField("Close").RecordCount;
                Field top = new Field();
                top.Initialize(recordCount, Alias + " Top");
                Field bottom = new Field();
                bottom.Initialize(recordCount, Alias + " Bottom");
                Oscillator os = new Oscillator();
                Field tr = os.TrueRange(pNav, pOHLCV, "atr").GetField("atr");
                MovingAverage ma = new MovingAverage();
                Field atr = ma.SimpleMovingAverage(pNav, tr, Periods, "atr").GetField("atr");
                Field median = ma.MovingAverageSwitch(pNav, pOHLCV.GetField("Close"), Periods, MAType, Alias + " Median").GetField(Alias + " Median");

                for (int record = 1; record < recordCount + 1; record++)
                {
                    double shift = Factor * atr.ValueEx(record);
                    top.SetValue(record, median.Value(record) + shift);
                    bottom.SetValue(record, median.Value(record) - shift);
                }

                Results.AddField(top);
                Results.AddField(median);
                Results.AddField(bottom);

                return Results;
            }
        }
    }
}
