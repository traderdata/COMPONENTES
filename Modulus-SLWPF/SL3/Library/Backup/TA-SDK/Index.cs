using System;

namespace ModulusFE
{
    namespace Tasdk
    {
        /// <summary>
        /// Index type of calculations
        /// </summary>
        internal class Index
        {
            ///<summary>
            /// Money Flow Index
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pOHLCV">OHLCV Recordset</param>
            ///<param name="periods">Periods</param>
            ///<returns>Recordset</returns>
            public Recordset MoneyFlowIndex(Navigator pNav, Recordset pOHLCV, int periods)
            { return MoneyFlowIndex(pNav, pOHLCV, periods, "Money Flow Index"); }
            ///<summary>
            /// Money Flow Index
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pOHLCV">OHLCV Recordset</param>
            ///<param name="periods">Periods</param>
            ///<param name="Alias">Alias</param>
            ///<returns>Recordset</returns>
            public Recordset MoneyFlowIndex(Navigator pNav, Recordset pOHLCV, int periods, string Alias)
            {
                Recordset Results = new Recordset();
                int Record;

                int RecordCount = pNav.RecordCount;

                if (periods < 1 || periods > RecordCount)
                    return null;

                Field Field1 = new Field(RecordCount, Alias);

                int Start = periods + 2;
                pNav.Position = Start;

                for (Record = Start; Record < RecordCount + 2; Record++)
                {
                    double PosFlow = 0;
                    double NegFlow = 0;

                    pNav.Position = Record - periods;
                    int Period;
                    for (Period = 1; Period < periods + 1; Period++)
                    {
                        pNav.MovePrevious();
                        double Price1 = (pOHLCV.GetField("High").Value(pNav.Position).Value +
                                         pOHLCV.GetField("Low").Value(pNav.Position).Value +
                                         pOHLCV.GetField("Close").Value(pNav.Position).Value) / 3;
                        pNav.MoveNext();
                        double V = pOHLCV.GetField("Volume").Value(pNav.Position).Value;
                        if (V < 1) { V = 1; }
                        double Price2 = (pOHLCV.GetField("High").Value(pNav.Position).Value +
                                         pOHLCV.GetField("Low").Value(pNav.Position).Value +
                                         pOHLCV.GetField("Close").Value(pNav.Position).Value) / 3;

                        if (Price2 > Price1)
                            PosFlow += Price2 * V;
                        else if (Price2 < Price1)
                            NegFlow += Price2 * V;

                        pNav.MoveNext();
                    }//Period

                    pNav.MovePrevious();
                    if (PosFlow != 0 && NegFlow != 0)
                    {
                        double MoneyRatio = PosFlow / NegFlow;
                        double MoneyIndex = 100 - (100 / (1 + MoneyRatio));
                        Field1.Value(pNav.Position, MoneyIndex);
                    }
                }//Record

                pNav.MoveFirst();
                Results.AddField(Field1);
                return Results;
            }

            ///<summary>
            /// Trade Volume Index
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource">Field Source</param>
            ///<param name="Volume">Field Volume</param>
            ///<param name="MinTickValue">Minimum Tick Value</param>
            ///<returns>Recordset</returns>
            public Recordset TradeVolumeIndex(Navigator pNav, Field pSource, Field Volume, double MinTickValue)
            { return TradeVolumeIndex(pNav, pSource, Volume, MinTickValue, "Trade Volume Index"); }
            ///<summary>
            /// Trade Volume Index
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource">Field Source</param>
            ///<param name="Volume">Field Volume</param>
            ///<param name="MinTickValue">Minimum Tick Value</param>
            ///<param name="Alias">Alias</param>
            ///<returns>Recordset</returns>
            public Recordset TradeVolumeIndex(Navigator pNav, Field pSource, Field Volume, double MinTickValue, string Alias)
            {
                Recordset Results = new Recordset();
                int Record;
                int Direction = 0;
                int LastDirection = 0;
                double TVI = 0;

                int RecordCount = pNav.RecordCount;
                Field Field1 = new Field(RecordCount, Alias);
                const int Start = 2;

                pNav.Position = Start;
                for (Record = Start; Record < RecordCount + 1; Record++)
                {
                    double Change = pSource.Value(pNav.Position).Value - pSource.Value(pNav.Position - 1).Value;

                    if (Change > MinTickValue)
                    {
                        Direction = 1;
                    }
                    else if (Change < -MinTickValue)
                    {
                        Direction = -1;
                    }

                    if (Change <= MinTickValue && Change >= -MinTickValue)
                    {
                        Direction = LastDirection;
                    }

                    LastDirection = Direction;

                    switch (Direction)
                    {
                        case 1:
                            TVI = TVI + Volume.Value(pNav.Position).Value;
                            break;
                        case -1:
                            TVI = TVI - Volume.Value(pNav.Position).Value;
                            break;
                    }

                    Field1.Value(pNav.Position, TVI);

                    pNav.MoveNext();
                }//Record

                pNav.MoveFirst();
                Results.AddField(Field1);
                return Results;
            }

            ///<summary>
            /// Swing Index
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pOHLCV">OHLCV Recordset</param>
            ///<param name="LimitMoveValue">Limit Move Value</param>
            ///<returns>RecordSet</returns>
            public Recordset SwingIndex(Navigator pNav, Recordset pOHLCV, double LimitMoveValue)
            { return SwingIndex(pNav, pOHLCV, LimitMoveValue, "Swing Index"); }
            ///<summary>
            /// Swing Index
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pOHLCV">OHLCV Recordset</param>
            ///<param name="LimitMoveValue">Limit Move Value</param>
            ///<param name="Alias">Alias</param>
            ///<returns>RecordSet</returns>
            public Recordset SwingIndex(Navigator pNav, Recordset pOHLCV, double LimitMoveValue, string Alias)
            {
                TASDK TASDK1 = new TASDK();
                Recordset Results = new Recordset();
                int Record;
                double R = 0;

                int RecordCount = pNav.RecordCount;

                if (LimitMoveValue <= 0)
                    return null;

                Field Field1 = new Field(RecordCount, Alias);

                const int Start = 2;
                pNav.Position = Start;
                for (Record = Start; Record < RecordCount + 1; Record++)
                {
                    double Oy = pOHLCV.GetField("Open").Value(pNav.Position - 1).Value;
                    double Ot = pOHLCV.GetField("Open").Value(pNav.Position).Value;
                    double Ht = pOHLCV.GetField("High").Value(pNav.Position).Value;
                    double Lt = pOHLCV.GetField("Low").Value(pNav.Position).Value;
                    double Cy = pOHLCV.GetField("Close").Value(pNav.Position - 1).Value;
                    double Ct = pOHLCV.GetField("Close").Value(pNav.Position).Value;

                    double K = TASDK1.max(Math.Abs(Ht - Cy), Math.Abs(Lt - Cy), 0.0);

                    double A = Math.Abs(Ht - Cy);
                    double B = Math.Abs(Lt - Cy);
                    double C = Math.Abs(Ht - Lt);

                    if (A > B && A > C)
                    {
                        R = Math.Abs(Ht - Cy) - 0.5 * Math.Abs(Lt - Cy) + 0.25 * Math.Abs(Cy - Oy);
                    }
                    else if (B > A && B > C)
                    {
                        R = Math.Abs(Lt - Cy) - 0.5 * Math.Abs(Ht - Cy) + 0.25 * Math.Abs(Cy - Oy);
                    }
                    else if (C > A && C > B)
                    {
                        R = Math.Abs(Ht - Lt) + 0.25 * Math.Abs(Cy - Oy);
                    }

                    double Value;
                    if (R > 0 && LimitMoveValue > 0)
                    {
                        Value = 50 * ((Ct - Cy) + 0.5 * (Ct - Ot) + 0.25 * (Cy - Oy)) / R * K / LimitMoveValue;
                    }
                    else
                    {
                        Value = 0;
                    }

                    Field1.Value(pNav.Position, Value);

                    pNav.MoveNext();
                }//Record

                pNav.MoveFirst();
                Results.AddField(Field1);
                return Results;
            }

            ///<summary>
            /// Accumulative Swing Index
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pOHLCV">OHLCV Recordset</param>
            ///<param name="LimitMoveValue">Limit Move Value</param>
            ///<returns>RecordSet</returns>
            public Recordset AccumulativeSwingIndex(Navigator pNav, Recordset pOHLCV, double LimitMoveValue)
            { return AccumulativeSwingIndex(pNav, pOHLCV, LimitMoveValue, "Accumulative Swing Index"); }
            ///<summary>
            /// Accumulative Swing Index
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pOHLCV">OHLCV Recordset</param>
            ///<param name="LimitMoveValue">Limit Move Value</param>
            ///<param name="Alias">Alias</param>
            ///<returns>RecordSet</returns>
            public Recordset AccumulativeSwingIndex(Navigator pNav, Recordset pOHLCV, double LimitMoveValue, string Alias)
            {
                Recordset Results = new Recordset();
                Index SI = new Index();
                int Record;

                int RecordCount = pNav.RecordCount;

                Recordset RawSI = SI.SwingIndex(pNav, pOHLCV, LimitMoveValue, "SI");
                Field Field1 = new Field(RecordCount, Alias);

                const int Start = 2;
                pNav.Position = Start;
                for (Record = Start; Record < RecordCount + 1; Record++)
                {
                    double fieldValue = Field1.ValueEx(pNav.Position - 1);
                    double Value = RawSI.ValueEx("SI", pNav.Position) + fieldValue;
                    Field1.Value(pNav.Position, Value);
                    pNav.MoveNext();
                }//Record

                pNav.MoveFirst();
                Results.AddField(Field1);

                return Results;
            }

            ///<summary>
            /// Relative Strength Index
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource">Field Source</param>
            ///<param name="periods">Periods</param>
            ///<returns>Recordset</returns>
            public Recordset RelativeStrengthIndex(Navigator pNav, Field pSource, int periods)
            { return RelativeStrengthIndex(pNav, pSource, periods, "Relative Strength Index"); }
            ///<summary>
            /// Relative Strength Index
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource">Field Source</param>
            ///<param name="periods">Periods</param>
            ///<param name="Alias">Alias</param>
            ///<returns>Recordset</returns>
            public Recordset RelativeStrengthIndex(Navigator pNav, Field pSource, int periods, string Alias)
            {
                Recordset Results = new Recordset();
                int Record;
                int Period;
                double UT = 0;
                double DT = 0;
                double UpSum = 0;
                double DownSum = 0;
                double? value = 0;

                int RecordCount = pNav.RecordCount;

                Field Field1 = new Field(RecordCount, Alias);
                Field AU = new Field(RecordCount, "AU");
                Field AD = new Field(RecordCount, "AD");

                pNav.Position = 2;
                for (Period = 1; Period < periods + 1; Period++)
                {
                    UT = 0;
                    DT = 0;

                    if (true)
                    {
                        if (value > pSource.Value(pNav.Position - 1))
                        {
                            UT = pSource.Value(pNav.Position).Value - pSource.Value(pNav.Position - 1).Value;
                            UpSum += UT;
                        }
                        else if (pSource.Value(pNav.Position) < pSource.Value(pNav.Position - 1))
                        {
                            DT = pSource.Value(pNav.Position - 1).Value - pSource.Value(pNav.Position).Value;
                            DownSum += DT;
                        }
                    }
                    pNav.MoveNext();
                }//Period

                pNav.MovePrevious();

                UpSum = UpSum / periods;
                AU.Value(pNav.Position, UpSum);
                DownSum = DownSum / periods;
                AD.Value(pNav.Position, DownSum);

                int Start = periods + 3;

                for (Record = Start; Record < RecordCount + 2; Record++)
                {
                    pNav.Position = Record - periods;

                    UpSum = 0;
                    DownSum = 0;

                    for (Period = 1; Period < periods + 1; Period++)
                    {
                        UT = 0;
                        DT = 0;
                        value = pSource.Value(pNav.Position);
                        if (value.HasValue)
                        {
                            if (value > pSource.Value(pNav.Position - 1))
                            {
                                UT = pSource.Value(pNav.Position).Value - pSource.Value(pNav.Position - 1).Value;
                                UpSum += UT;
                            }
                            else if (pSource.Value(pNav.Position) < pSource.Value(pNav.Position - 1))
                            {
                                DT = pSource.Value(pNav.Position - 1).Value - pSource.Value(pNav.Position).Value;
                                DownSum += DT;
                            }
                        }
                        pNav.MoveNext();
                    }//Period

                    pNav.MovePrevious();

                    UpSum = (((AU.Value(pNav.Position - 1).Value * (periods - 1)) + UT)) / periods;
                    DownSum = (((AD.Value(pNav.Position - 1).Value * (periods - 1)) + DT)) / periods;

                    AU.Value(pNav.Position, UpSum);
                    AD.Value(pNav.Position, DownSum);

                    if (DownSum == 0)
                        DownSum = UpSum;

                    double RS = UpSum / DownSum;
                    double RSI = 100 - (100 / (1 + RS));

                    Field1.Value(pNav.Position, RSI);
                }//Record

                pNav.MoveFirst();
                Results.AddField(Field1);

                return Results;
            }

            ///<summary>
            /// Comparative Relative Strength
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource1">Field Source1</param>
            ///<param name="pSource2">Field Source2</param>
            ///<returns>Recordset</returns>
            public Recordset ComparativeRelativeStrength(Navigator pNav, Field pSource1, Field pSource2)
            { return ComparativeRelativeStrength(pNav, pSource1, pSource2, "Comparative Relative Strength"); }
            ///<summary>
            /// Comparative Relative Strength
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource1">Field Source1</param>
            ///<param name="pSource2">Field Source2</param>
            ///<param name="Alias">Alias</param>
            ///<returns>Recordset</returns>
            public Recordset ComparativeRelativeStrength(Navigator pNav, Field pSource1, Field pSource2, string Alias)
            {
                Recordset Results = new Recordset();
                int Record;

                int RecordCount = pNav.RecordCount;
                Field Field1 = new Field(RecordCount, Alias);

                for (Record = 1; Record < RecordCount + 1; Record++)
                {
                    double? Value = pSource1.Value(pNav.Position).Value / pSource2.Value(pNav.Position).Value;
                    if (Value == 1)
                        Value = null;
                    Field1.Value(pNav.Position, Value);

                    pNav.MoveNext();
                }//Record

                pNav.MoveFirst();
                Results.AddField(Field1);

                return Results;
            }

            ///<summary>
            /// Price Volume Trend
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource">Field Source</param>
            ///<param name="Volume">Field Volume</param>
            ///<returns>RecordSet</returns>
            public Recordset PriceVolumeTrend(Navigator pNav, Field pSource, Field Volume)
            { return PriceVolumeTrend(pNav, pSource, Volume, "Price Volume Trend"); }
            ///<summary>
            /// Price Volume Trend
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource">Field Source</param>
            ///<param name="Volume">Field Volume</param>
            ///<param name="Alias">Alias</param>
            ///<returns>RecordSet</returns>
            public Recordset PriceVolumeTrend(Navigator pNav, Field pSource, Field Volume, string Alias)
            {
                Recordset Results = new Recordset();
                int Record;

                int RecordCount = pNav.RecordCount;
                Field Field1 = new Field(RecordCount, Alias);

                const int Start = 2;
                pNav.Position = Start;
                for (Record = Start; Record < RecordCount + 1; Record++)
                {
                    double Value = (((pSource.ValueEx(pNav.Position) - pSource.ValueEx(pNav.Position - 1)) /
                                     pSource.ValueEx(pNav.Position - 1)) * Volume.ValueEx(pNav.Position)) +
                                   Field1.ValueEx(pNav.Position - 1);
                    Field1.Value(pNav.Position, Value);
                    pNav.MoveNext();
                }//Record

                pNav.MoveFirst();
                Results.AddField(Field1);

                return Results;
            }

            ///<summary>
            /// Price Volume Index
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource">Field Source</param>
            ///<param name="Volume">Field Volume</param>
            ///<returns>Recordset</returns>
            public Recordset PositiveVolumeIndex(Navigator pNav, Field pSource, Field Volume)
            { return PositiveVolumeIndex(pNav, pSource, Volume, "Positive Volume Index"); }
            ///<summary>
            /// Price Volume Index
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource">Field Source</param>
            ///<param name="Volume">Field Volume</param>
            ///<param name="Alias">Alias</param>
            ///<returns>Recordset</returns>
            public Recordset PositiveVolumeIndex(Navigator pNav, Field pSource, Field Volume, string Alias)
            {
                Recordset Results = new Recordset();
                int Record;

                int RecordCount = pNav.RecordCount;
                Field Field1 = new Field(RecordCount, Alias);

                const int Start = 2;
                pNav.Position = Start;
                Field1.Value(1, 1);
                for (Record = Start; Record < RecordCount + 1; Record++)
                {
                    double? Value;
                    if (Volume.Value(pNav.Position) > Volume.Value(pNav.Position - 1))
                    {
                        Value = Field1.Value(pNav.Position - 1) + (pSource.Value(pNav.Position) -
                                pSource.Value(pNav.Position - 1)) / pSource.Value(pNav.Position - 1) *
                                Field1.Value(pNav.Position - 1);
                    }
                    else
                    {
                        Value = Field1.Value(pNav.Position - 1).Value;
                    }
                    Field1.Value(pNav.Position, Value);
                    pNav.MoveNext();
                }//Record

                pNav.MoveFirst();
                Results.AddField(Field1);

                return Results;
            }

            ///<summary>
            /// Negative Volume Index
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource">Field Source</param>
            ///<param name="Volume">Field Volume</param>
            ///<returns>Recordset</returns>
            public Recordset NegativeVolumeIndex(Navigator pNav, Field pSource, Field Volume)
            { return NegativeVolumeIndex(pNav, pSource, Volume, "Negative Volume Index"); }
            ///<summary>
            /// Negative Volume Index
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource">Field Source</param>
            ///<param name="Volume">Field Volume</param>
            ///<param name="Alias">Alias</param>
            ///<returns>Recordset</returns>
            public Recordset NegativeVolumeIndex(Navigator pNav, Field pSource, Field Volume, string Alias)
            {
                Recordset Results = new Recordset();
                int Record;

                int RecordCount = pNav.RecordCount;
                Field Field1 = new Field(RecordCount, Alias);

                const int Start = 2;
                pNav.Position = Start;
                Field1.Value(1, 1);
                for (Record = Start; Record < RecordCount + 1; Record++)
                {
                    double? Value;
                    if (Volume.Value(pNav.Position) < Volume.Value(pNav.Position - 1))
                    {
                        Value = Field1.Value(pNav.Position - 1) + (pSource.Value(pNav.Position) -
                                pSource.Value(pNav.Position - 1)) / pSource.Value(pNav.Position - 1) *
                                Field1.Value(pNav.Position - 1);
                    }
                    else
                    {
                        Value = Field1.Value(pNav.Position - 1);
                    }
                    Field1.Value(pNav.Position, Value);

                    pNav.MoveNext();
                }//Record

                pNav.MoveFirst();
                Results.AddField(Field1);

                return Results;
            }

            ///<summary>
            /// Performance
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource">Field Source</param>
            ///<returns>Recordset</returns>
            public Recordset Performance(Navigator pNav, Field pSource)
            { return Performance(pNav, pSource, "Performance"); }
            ///<summary>
            /// Performance
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource">Field Source</param>
            ///<param name="Alias">Alias</param>
            ///<returns>Recordset</returns>
            public Recordset Performance(Navigator pNav, Field pSource, string Alias)
            {
                Recordset Results = new Recordset();
                int Record;

                int RecordCount = pNav.RecordCount;
                Field Field1 = new Field(RecordCount, Alias);
                const int Start = 2;
                pNav.Position = Start;
                double? FirstPrice = pSource.Value(1);
                for (Record = Start; Record < RecordCount + 1; Record++)
                {
                    double? Value = ((pSource.Value(pNav.Position) - FirstPrice) / FirstPrice) * 100;
                    Field1.Value(pNav.Position, Value);
                    pNav.MoveNext();
                }//Record

                pNav.MoveFirst();
                Results.AddField(Field1);

                return Results;
            }

            ///<summary>
            /// On Balance Volume
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource">Field Source</param>
            ///<param name="Volume">Field Volume</param>
            ///<returns>RecordSet</returns>
            public Recordset OnBalanceVolume(Navigator pNav, Field pSource, Field Volume)
            { return OnBalanceVolume(pNav, pSource, Volume, "On Balance Volume"); }
            ///<summary>
            /// On Balance Volume
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource">Field Source</param>
            ///<param name="Volume">Field Volume</param>
            ///<param name="Alias">Alias</param>
            ///<returns>RecordSet</returns>
            public Recordset OnBalanceVolume(Navigator pNav, Field pSource, Field Volume, string Alias)
            {
                Recordset Results = new Recordset();
                int Record;

                int RecordCount = pNav.RecordCount;
                Field Field1 = new Field(RecordCount, Alias);
                const int Start = 2;
                pNav.Position = Start;
                for (Record = Start; Record < RecordCount + 1; Record++)
                {
                    double Value;
                    if (pSource.Value(pNav.Position - 1) < pSource.Value(pNav.Position))
                    {
                        Value = Field1.ValueEx(pNav.Position - 1) + Volume.ValueEx(pNav.Position);
                    }
                    else if (pSource.Value(pNav.Position) < pSource.Value(pNav.Position - 1))
                    {
                        Value = Field1.ValueEx(pNav.Position - 1) - Volume.ValueEx(pNav.Position);
                    }
                    else
                    {
                        Value = Field1.ValueEx(pNav.Position - 1);
                    }

                    Field1.Value(pNav.Position, Value);
                    pNav.MoveNext();
                }//Record

                pNav.MoveFirst();
                Results.AddField(Field1);

                return Results;
            }

            ///<summary>
            /// Mass Index
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pOHLCV">OHLCV Recordset</param>
            ///<param name="periods">Periods</param>
            ///<returns>Recordset</returns>
            public Recordset MassIndex(Navigator pNav, Recordset pOHLCV, int periods)
            { return MassIndex(pNav, pOHLCV, periods, "Mass Index"); }
            ///<summary>
            /// Mass Index
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pOHLCV">OHLCV Recordset</param>
            ///<param name="periods">Periods</param>
            ///<param name="Alias">Alias</param>
            ///<returns>Recordset</returns>
            public Recordset MassIndex(Navigator pNav, Recordset pOHLCV, int periods, string Alias)
            {
                Recordset Results = new Recordset();
                General GE = new General();
                MovingAverage MA = new MovingAverage();
                int Record;

                int RecordCount = pNav.RecordCount;
                if (periods < 1 || periods > RecordCount)
                    return null;

                Field Field1 = new Field(RecordCount, Alias);

                Recordset HML = GE.HighMinusLow(pNav, pOHLCV, "HML");
                Field Temp = HML.GetField("HML");
                Recordset EMA1 = MA.ExponentialMovingAverage(pNav, Temp, 9, "EMA");
                Temp = EMA1.GetField("EMA");
                Recordset EMA2 = MA.ExponentialMovingAverage(pNav, Temp, 9, "EMA");

                int Start = (periods * 2) + 1;
                pNav.Position = Start;
                for (Record = Start; Record < RecordCount + 2; Record++)
                {
                    double? Sum = 0;

                    pNav.Position = Record - periods;
                    int Period;
                    for (Period = 1; Period < periods + 1; Period++)
                    {
                        Sum = Sum + (EMA1.Value("EMA", pNav.Position) / EMA2.Value("EMA", pNav.Position));
                        pNav.MoveNext();
                    }//Period
                    pNav.MovePrevious();
                    Field1.Value(pNav.Position, Sum);

                    pNav.MoveNext();
                }//Record

                pNav.MoveFirst();
                Results.AddField(Field1);

                return Results;
            }

            ///<summary>
            /// Chaikin Money Flow
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pOHLCV">OHLCV Recordset</param>
            ///<param name="periods">Periods</param>
            ///<returns>Recordset</returns>
            public Recordset ChaikinMoneyFlow(Navigator pNav, Recordset pOHLCV, int periods)
            { return ChaikinMoneyFlow(pNav, pOHLCV, periods, "Chaikin Money Flow"); }
            ///<summary>
            /// Chaikin Money Flow
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pOHLCV">OHLCV Recordset</param>
            ///<param name="periods">Periods</param>
            ///<param name="Alias">Alias</param>
            ///<returns>Recordset</returns>
            public Recordset ChaikinMoneyFlow(Navigator pNav, Recordset pOHLCV, int periods, string Alias)
            {
                Recordset Results = new Recordset();
                int Record;

                int RecordCount = pNav.RecordCount;
                Field Field1 = new Field(RecordCount, Alias);
                for (Record = periods + 1; Record < RecordCount + 1; Record++)
                {
                    double? Sum = 0;
                    double? SumV = 0;

                    for (int n = 0; n != periods; ++n)
                    {
                        double? a = ((pOHLCV.Value("Close", Record - n) - pOHLCV.Value("Low", Record - n)) -
                                    (pOHLCV.Value("High", Record - n) - pOHLCV.Value("Close", Record - n)));
                        double? b = (pOHLCV.Value("High", Record - n) - pOHLCV.Value("Low", Record - n)) *
                                   pOHLCV.Value("Volume", Record - n);
                        if (a != 0 && b != 0)
                        {
                            Sum += a / b;
                        }

                        SumV += pOHLCV.Value("Volume", Record - n);
                    }

                    double? Value = (Sum / SumV); // * Math.Pow((double)SumV, 2);
                    Field1.Value(Record, Value);
                } //Record

                pNav.MoveFirst();
                Results.AddField(Field1);

                return Results;
            }

            ///<summary>
            /// Commodity Channel Index
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pOHLCV">OHLCV Recordset</param>
            ///<param name="periods">Periods</param>
            ///<returns>Recordset</returns>
            public Recordset CommodityChannelIndex(Navigator pNav, Recordset pOHLCV, int periods)
            { return CommodityChannelIndex(pNav, pOHLCV, periods, "CCI"); }
            ///<summary>
            /// Commodity Channel Index
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pOHLCV">OHLCV Recordset</param>
            ///<param name="periods">Periods</param>
            ///<param name="Alias">Alias</param>
            ///<returns>Recordset</returns>
            public Recordset CommodityChannelIndex(Navigator pNav, Recordset pOHLCV, int periods, string Alias)
            {
                General GN = new General();
                MovingAverage MA = new MovingAverage();
                Recordset Results = new Recordset();
                int Record;

                int RecordCount = pNav.RecordCount;
                Field Field1 = new Field(RecordCount, Alias);

                Recordset TPrs = GN.TypicalPrice(pNav, pOHLCV, "TP");
                Recordset MArs = MA.SimpleMovingAverage(pNav, TPrs.GetField("TP"), periods, "TPMA");
                for (Record = 1; Record != (2 * periods) + 1; ++Record)
                {
                    Field1.Value(Record, 0);
                }

                for (Record = (2 * periods); Record != RecordCount + 1; ++Record)
                {
                    double dMeanDeviation = 0;
                    double dTmp;
                    int Count;
                    for (Count = (Record - periods); Count != Record + 1; ++Count)
                    {
                        double? d1 = TPrs.GetField("TP").Value(Count);
                        double? d2 = MArs.GetField("TPMA").Value(Count);
                        d1 = d1.HasValue ? d1 : 0.0;
                        d2 = d2.HasValue ? d2 : 0.0;
                        dTmp = Math.Abs(d1.Value - d2.Value);
                        dMeanDeviation = dMeanDeviation + dTmp;
                    } //Count
                    dMeanDeviation = dMeanDeviation / periods;
                    dTmp = (double)((TPrs.GetField("TP").Value(Record) - MArs.GetField("TPMA").Value(Record)) / (dMeanDeviation * 0.015));
                    Field1.Value(Record, dTmp);
                } //Record

                pNav.MoveFirst();
                Results.AddField(Field1);

                return Results;
            }

            ///<summary>
            /// Stochastic Momentum Index
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pOHLCV">OHLCV Recordser</param>
            ///<param name="KPeriods">K Periods</param>
            ///<param name="KSmooth">K Smooth</param>
            ///<param name="KDoubleSmooth">K Double Smooth</param>
            ///<param name="DPeriods">D Periods</param>
            ///<param name="MAType">Moving Average Type</param>
            ///<param name="PctD_MAType">%D Movering Average Type</param>
            ///<returns>Recordset</returns>
            public Recordset StochasticMomentumIndex(Navigator pNav, Recordset pOHLCV, int KPeriods, int KSmooth, int KDoubleSmooth,
              int DPeriods, IndicatorType MAType, IndicatorType PctD_MAType)
            {
                MovingAverage MA = new MovingAverage();
                Recordset Results = new Recordset();
                General GN = new General();
                int Record;
                double? Value = 0;

                KSmooth += 1;
                int RecordCount = pNav.RecordCount;

                Field Field1 = new Field(RecordCount, "%K");


                Recordset Temp = GN.HHV(pNav, pOHLCV.GetField("High"), KPeriods, "HHV");
                Field HHV = new Field(RecordCount, "HHV");
                Temp.CopyField(HHV, "HHV");

                Temp = GN.LLV(pNav, pOHLCV.GetField("Low"), KPeriods, "LLV");
                Field LLV = new Field(RecordCount, "LLV");
                Temp.CopyField(LLV, "LLV");


                Field HHLL = new Field(RecordCount, "HHLL");
                for (Record = 1; Record != RecordCount + 1; ++Record)
                {
                    Value = HHV.Value(Record) - LLV.Value(Record);
                    HHLL.Value(Record, Value);
                }


                Field CHHLL = new Field(RecordCount, "CHHLL");
                for (Record = 1; Record != RecordCount + 1; ++Record)
                {
                    Value = pOHLCV.Value("Close", Record) - (0.5f * (HHV.Value(Record) + LLV.Value(Record)));
                    CHHLL.Value(Record, Value);
                }

                if (KSmooth > 1)
                {
                    switch (MAType)
                    {
                        case IndicatorType.SimpleMovingAverage:
                            Temp = MA.SimpleMovingAverage(pNav, CHHLL, KSmooth, "CHHLL");
                            break;
                        case IndicatorType.ExponentialMovingAverage:
                            Temp = MA.ExponentialMovingAverage(pNav, CHHLL, KSmooth, "CHHLL");
                            break;
                        case IndicatorType.TimeSeriesMovingAverage:
                            Temp = MA.TimeSeriesMovingAverage(pNav, CHHLL, KSmooth, "CHHLL");
                            break;
                        case IndicatorType.TriangularMovingAverage:
                            Temp = MA.TriangularMovingAverage(pNav, CHHLL, KSmooth, "CHHLL");
                            break;
                        case IndicatorType.VariableMovingAverage:
                            Temp = MA.VariableMovingAverage(pNav, CHHLL, KSmooth, "CHHLL");
                            break;
                        case IndicatorType.WeightedMovingAverage:
                            Temp = MA.WeightedMovingAverage(pNav, CHHLL, KSmooth, "CHHLL");
                            break;
                        case IndicatorType.VIDYA:
                            Temp = MA.VIDYA(pNav, CHHLL, KSmooth, 0.65, "CHHLL");
                            break;
                    }

                    Temp.CopyField(CHHLL, "CHHLL");
                }


                if (KDoubleSmooth > 1)
                {
                    switch (MAType)
                    {
                        case IndicatorType.SimpleMovingAverage:
                            Temp = MA.SimpleMovingAverage(pNav, CHHLL, KDoubleSmooth, "CHHLL");
                            break;
                        case IndicatorType.ExponentialMovingAverage:
                            Temp = MA.ExponentialMovingAverage(pNav, CHHLL, KDoubleSmooth, "CHHLL");
                            break;
                        case IndicatorType.TimeSeriesMovingAverage:
                            Temp = MA.TimeSeriesMovingAverage(pNav, CHHLL, KDoubleSmooth, "CHHLL");
                            break;
                        case IndicatorType.TriangularMovingAverage:
                            Temp = MA.TriangularMovingAverage(pNav, CHHLL, KDoubleSmooth, "CHHLL");
                            break;
                        case IndicatorType.VariableMovingAverage:
                            Temp = MA.VariableMovingAverage(pNav, CHHLL, KDoubleSmooth, "CHHLL");
                            break;
                        case IndicatorType.WeightedMovingAverage:
                            Temp = MA.WeightedMovingAverage(pNav, CHHLL, KDoubleSmooth, "CHHLL");
                            break;
                        case IndicatorType.VIDYA:
                            Temp = MA.VIDYA(pNav, CHHLL, KDoubleSmooth, 0.65, "CHHLL");
                            break;
                    }

                    Temp.CopyField(CHHLL, "CHHLL");

                }

                if (KSmooth > 1)
                {
                    switch (MAType)
                    {
                        case IndicatorType.SimpleMovingAverage:
                            Temp = MA.SimpleMovingAverage(pNav, HHLL, KSmooth, "HHLL");
                            break;
                        case IndicatorType.ExponentialMovingAverage:
                            Temp = MA.ExponentialMovingAverage(pNav, HHLL, KSmooth, "HHLL");
                            break;
                        case IndicatorType.TimeSeriesMovingAverage:
                            Temp = MA.TimeSeriesMovingAverage(pNav, HHLL, KSmooth, "HHLL");
                            break;
                        case IndicatorType.TriangularMovingAverage:
                            Temp = MA.TriangularMovingAverage(pNav, HHLL, KSmooth, "HHLL");
                            break;
                        case IndicatorType.VariableMovingAverage:
                            Temp = MA.VariableMovingAverage(pNav, HHLL, KSmooth, "HHLL");
                            break;
                        case IndicatorType.WeightedMovingAverage:
                            Temp = MA.WeightedMovingAverage(pNav, HHLL, KSmooth, "HHLL");
                            break;
                        case IndicatorType.VIDYA:
                            Temp = MA.VIDYA(pNav, HHLL, KSmooth, 0.65, "HHLL");
                            break;
                    }

                    Temp.CopyField(HHLL, "HHLL");
                }


                if (KDoubleSmooth > 1)
                {
                    switch (MAType)
                    {
                        case IndicatorType.SimpleMovingAverage:
                            Temp = MA.SimpleMovingAverage(pNav, HHLL, KDoubleSmooth, "HHLL");
                            break;
                        case IndicatorType.ExponentialMovingAverage:
                            Temp = MA.ExponentialMovingAverage(pNav, HHLL, KDoubleSmooth, "HHLL");
                            break;
                        case IndicatorType.TimeSeriesMovingAverage:
                            Temp = MA.TimeSeriesMovingAverage(pNav, HHLL, KDoubleSmooth, "HHLL");
                            break;
                        case IndicatorType.TriangularMovingAverage:
                            Temp = MA.TriangularMovingAverage(pNav, HHLL, KDoubleSmooth, "HHLL");
                            break;
                        case IndicatorType.VariableMovingAverage:
                            Temp = MA.VariableMovingAverage(pNav, HHLL, KDoubleSmooth, "HHLL");
                            break;
                        case IndicatorType.WeightedMovingAverage:
                            Temp = MA.WeightedMovingAverage(pNav, HHLL, KDoubleSmooth, "HHLL");
                            break;
                        case IndicatorType.VIDYA:
                            Temp = MA.VIDYA(pNav, HHLL, KDoubleSmooth, 0.65, "HHLL");
                            break;
                    }

                    Temp.CopyField(HHLL, "HHLL");
                }


                for (Record = KPeriods + 1; Record != RecordCount + 1; ++Record)
                {
                    double? a = CHHLL.Value(Record);
                    double? b = (0.5f * HHLL.Value(Record));
                    if (a != b && b != 0) Value = 100.0f * (a / b);
                    Field1.Value(Record, Value);
                }


                if (DPeriods > 1)
                {
                    switch (PctD_MAType)
                    {
                        case IndicatorType.SimpleMovingAverage:
                            Temp = MA.SimpleMovingAverage(pNav, Field1, DPeriods, "%D");
                            break;
                        case IndicatorType.ExponentialMovingAverage:
                            Temp = MA.ExponentialMovingAverage(pNav, Field1, DPeriods, "%D");
                            break;
                        case IndicatorType.TimeSeriesMovingAverage:
                            Temp = MA.TimeSeriesMovingAverage(pNav, Field1, DPeriods, "%D");
                            break;
                        case IndicatorType.TriangularMovingAverage:
                            Temp = MA.TriangularMovingAverage(pNav, Field1, DPeriods, "%D");
                            break;
                        case IndicatorType.VariableMovingAverage:
                            Temp = MA.VariableMovingAverage(pNav, Field1, DPeriods, "%D");
                            break;
                        case IndicatorType.WeightedMovingAverage:
                            Temp = MA.WeightedMovingAverage(pNav, Field1, DPeriods, "%D");
                            break;
                        case IndicatorType.VIDYA:
                            Temp = MA.VIDYA(pNav, Field1, DPeriods, 0.65, "%D");
                            break;
                    }

                    Field Field2 = new Field(RecordCount, "%D");
                    Temp.CopyField(Field2, "%D");
                    Results.AddField(Field2);
                }
                Results.AddField(Field1);

                return Results;
            }

            ///<summary>
            /// Historical Volatility
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource">Field Source</param>
            ///<returns>Recordset</returns>
            public Recordset HistoricalVolatility(Navigator pNav, Field pSource)
            { return HistoricalVolatility(pNav, pSource, 30, 365, 2, "Historical Volatility"); }
            ///<summary>
            /// Historical Volatility
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource">Field Source</param>
            ///<param name="periods">Periods</param>
            ///<param name="BarHistory">Bar History</param>
            ///<param name="StandardDeviations">Standard Deviations</param>
            ///<param name="Alias">Alias</param>
            ///<returns>Recordset</returns>
            public Recordset HistoricalVolatility(Navigator pNav, Field pSource, int periods, int BarHistory,
              int StandardDeviations, string Alias)
            {
                Recordset Results = new Recordset();
                General Stdv = new General();
                int Record;
                double? Value;

                int RecordCount = pNav.RecordCount;
                Field Field1 = new Field(RecordCount, "TEMP");
                const int Start = 2;
                for (Record = Start; Record < RecordCount + 1; Record++)
                {
                    Value = Math.Log10(pSource.Value(Record).Value / pSource.Value(Record - 1).Value);
                    Field1.Value(Record, Value);
                }

                Recordset Field2 = Stdv.StandardDeviation(pNav, Field1, periods, StandardDeviations, IndicatorType.SimpleMovingAverage, "STDV");

                for (Record = Start; Record < RecordCount + 1; Record++)
                {
                    Value = Field2.Value("STDV", Record) * Math.Sqrt(BarHistory);
                    Field1.Value(Record, Value);
                }

                Field1.Name = Alias;
                Results.AddField(Field1);

                return Results;
            }

            public Recordset ElderForceIndex(Recordset pOHLCV, string Alias)
            {
                Recordset Results = new Recordset();
                Field pClose = pOHLCV.GetField("Close");
                Field pVolume = pOHLCV.GetField("Volume");
                int RecordCount = pClose.RecordCount;
                Field field1 = new Field();
                field1.Initialize(RecordCount, Alias);

                for (int record = 2; record < RecordCount + 1; record++)
                    field1.SetValue(record, (pClose.Value(record - 1) - pClose.Value(record)) * pVolume.Value(record));

                Results.AddField(field1);
                return Results;
            }

            public Recordset ElderThermometer(Recordset pOHLCV, string Alias)
            {
                Recordset Results = new Recordset();
                Field pHigh = pOHLCV.GetField("High");
                Field pLow = pOHLCV.GetField("Low");
                int RecordCount = pLow.RecordCount;
                Field field1 = new Field();
                field1.Initialize(RecordCount, Alias);

                for (int record = 2; record < RecordCount + 1; record++)
                {
                    double hmh = Math.Abs(pHigh.ValueEx(record) - pHigh.ValueEx(record - 1));
                    double lml = Math.Abs(pLow.ValueEx(record - 1) - pLow.ValueEx(record));
                    double value = Math.Max(hmh, lml);
                    field1.SetValue(record, value);
                }

                Results.AddField(field1);
                return Results;
            }

            public Recordset MarketFacilitationIndex(Recordset pOHLCV, string Alias)
            {
                Recordset Results = new Recordset();
                Field pHigh = pOHLCV.GetField("High");
                Field pLow = pOHLCV.GetField("Low");
                Field pVolume = pOHLCV.GetField("Volume");
                int RecordCount = pVolume.RecordCount;
                Field field1 = new Field();
                field1.Initialize(RecordCount, Alias);

                for (int record = 2; record < RecordCount + 1; record++)
                    field1.SetValue(record, (pHigh.Value(record) - pLow.Value(record)) / (pVolume.Value(record) / 100000000));

                Results.AddField(field1);
                return Results;
            }

            public Recordset QStick(Navigator pNav, Recordset pOHLCV, int Periods, IndicatorType MAType, string Alias)
            {
                Recordset Results = new Recordset();

                Field pHigh = pOHLCV.GetField("High");
                Field pLow = pOHLCV.GetField("Low");

                int RecordCount = pLow.RecordCount;

                Field field1 = new Field();
                field1.Initialize(RecordCount, Alias);

                for (int record = 1; record < RecordCount + 1; record++)
                    field1.SetValue(record, (pHigh.Value(record) - pLow.Value(record)));

                MovingAverage ma = new MovingAverage();
                return ma.MovingAverageSwitch(pNav, field1, Periods, MAType, Alias);
            }

            public Recordset GopalakrishnanRangeIndex(Navigator pNav, Recordset pOHLCV, int Periods, string Alias)
            {
                Recordset Results = new Recordset();
                int recordCount = pOHLCV.GetField("Low").RecordCount;
                Field field1 = new Field();
                field1.Initialize(recordCount, Alias);

                General g = new General();
                Field hhv = g.HHV(pNav, pOHLCV.GetField("High"), Periods, "x").GetField("x");
                Field llv = g.LLV(pNav, pOHLCV.GetField("Low"), Periods, "x").GetField("x");

                for (int record = 1; record < recordCount + 1; record++)
                    field1.SetValue(record, Math.Log(hhv.ValueEx(record) - llv.ValueEx(record)) / Math.Log(Periods));

                for (int record = 1; record < Periods + 1; record++)
                    field1.SetValue(record, 0);

                Results.AddField(field1);
                return Results;
            }

            public Recordset IntradayMomentumIndex(Recordset pOHLCV, string Alias)
            {
                Recordset Results = new Recordset();
                Field pOpen = pOHLCV.GetField("Open");
                Field pClose = pOHLCV.GetField("Close");
                int recordCount = pClose.RecordCount;
                Field field1 = new Field();
                field1.Initialize(recordCount, Alias);

                double upSum = 0, dnSum = 0, prevUpSum = 0, prevDnSum = 0;

                for (int record = 2; record < recordCount + 1; record++)
                {
                    if (pClose.Value(record) > pOpen.Value(record))
                        upSum = prevUpSum + (pClose.ValueEx(record) - pOpen.ValueEx(record));
                    else
                        dnSum = prevDnSum + (pOpen.ValueEx(record) - pClose.ValueEx(record));

                    field1.SetValue(record, 100 * (upSum / (upSum + dnSum)));

                    prevUpSum = upSum;
                    prevDnSum = dnSum;
                }

                Results.AddField(field1);
                return Results;
            }

            public Recordset RAVI(Navigator pNav, Field pSource, int ShortCycle, int LongCycle, string Alias)
            {
                Recordset Results = new Recordset();
                int recordCount = pSource.RecordCount;
                Field field1 = new Field();
                field1.Initialize(recordCount, Alias);

                MovingAverage ma = new MovingAverage();
                Field sc = ma.VIDYA(pNav, pSource, ShortCycle, 0.65, "x").GetField("x");
                Field lc = ma.VIDYA(pNav, pSource, LongCycle, 0.65, "x").GetField("x");

                for (int record = 1; record < recordCount + 1; record++)
                    field1.SetValue(record, 100 * (Math.Abs(sc.ValueEx(record) - lc.ValueEx(record)) / lc.Value(record)));

                for (int record = 1; record < LongCycle + 1; record++)
                    field1.SetValue(record, 0);

                Results.AddField(field1);
                return Results;
            }

            public Recordset RandomWalkIndex(Navigator pNav, Recordset pOHLCV, int Periods, string Alias)
            {
                Recordset Results = new Recordset();
                int recordCount = pOHLCV.GetField("Low").RecordCount;
                Field hirwi = new Field();
                hirwi.Initialize(recordCount, Alias + " High");
                Field lowrwi = new Field();
                lowrwi.Initialize(recordCount, Alias + " Low");
                Field pHigh = pOHLCV.GetField("High");
                Field pLow = pOHLCV.GetField("Low");
                MovingAverage ma = new MovingAverage();
                Oscillator o = new Oscillator();
                Field atr = ma.SimpleMovingAverage(pNav, o.TrueRange(pNav, pOHLCV, "x").GetField("x"), Periods, "x").GetField("x");

                for (int record = Periods; record < recordCount + 1; record++)
                {
                    for (int n = record - 1; n > record - Periods; --n)
                    {
                        hirwi.SetValue(record, (pHigh.Value(record) - pLow.Value(n)) / (atr.Value(n) * Math.Sqrt(n)));
                        lowrwi.SetValue(record, (pHigh.Value(n) - pLow.Value(record)) / (atr.Value(n) * Math.Sqrt(n)));
                    }
                }

                for (int record = 1; record < Periods * 2; record++)
                {
                    hirwi.SetValue(record, 0);
                    lowrwi.SetValue(record, 0);
                }

                Results.AddField(hirwi);
                Results.AddField(lowrwi);
                return Results;
            }

            public Recordset TwiggsMoneyFlow(Navigator pNav, Recordset pOHLCV, int Periods, string Alias)
            {
                Recordset Results = new Recordset();

                Field pHigh = pOHLCV.GetField("High");
                Field pLow = pOHLCV.GetField("Low");
                Field pClose = pOHLCV.GetField("Close");
                Field pVolume = pOHLCV.GetField("Volume");

                int recordCount = pClose.RecordCount;
                Field field1 = new Field();
                field1.Initialize(recordCount, Alias);
                MovingAverage ma = new MovingAverage();
                Field ema = ma.ExponentialMovingAverage(pNav, pOHLCV.GetField("Volume"), Periods, "x").GetField("x");

                Field th = new Field();
                Field tl = new Field();
                tl.Initialize(recordCount, "x");
                th.Initialize(recordCount, "x");

                for (int record = 2; record < recordCount + 1; record++)
                {
                    th.SetValue(record, Math.Max(pHigh.ValueEx(record), pClose.ValueEx(record - 1)));
                    tl.SetValue(record, Math.Min(pLow.ValueEx(record), pClose.ValueEx(record - 1)));
                }

                for (int record = 2; record < recordCount + 1; record++)
                    field1.SetValue(record, ((pClose.Value(record) - tl.Value(record)) - (th.Value(record) - pClose.Value(record))) / (th.Value(record) - tl.Value(record)) * pVolume.Value(record));

                field1 = ma.ExponentialMovingAverage(pNav, field1, Periods, Alias).GetField(Alias);

                for (int record = 2; record < recordCount + 1; record++)
                    field1.SetValue(record, field1.Value(record) / ema.Value(record));

                for (int record = 1; record < Periods + 1; record++)
                    field1.SetValue(record, 0);

                Results.AddField(field1);
                return Results;
            }
        }
    }
}
