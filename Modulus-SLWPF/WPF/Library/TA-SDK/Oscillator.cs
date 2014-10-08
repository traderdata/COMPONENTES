using System;

namespace ModulusFE
{
    namespace Tasdk
    {
        ///<summary>
        /// Oscillator type of calcualtions
        ///</summary>
        internal class Oscillator
        {

            ///<summary>
            /// Chande Momentum Oscillator
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource">Field Source</param>
            ///<param name="periods">Periods</param>
            ///<returns>Recordset</returns>
            public Recordset ChandeMomentumOscillator(Navigator pNav, Field pSource, int periods)
            { return ChandeMomentumOscillator(pNav, pSource, periods, "CMO"); }
            ///<summary>
            /// Chande Momentum Oscillator
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource">Field Source</param>
            ///<param name="periods">Periods</param>
            ///<param name="Alias">Alias</param>
            ///<returns>Recordset</returns>
            public Recordset ChandeMomentumOscillator(Navigator pNav, Field pSource, int periods, string Alias)
            {
                Recordset Results = new Recordset();
                int iRecord;

                int iRecordCount = pNav.RecordCount;

                Field Field1 = new Field(iRecordCount, Alias);

                pNav.MoveFirst();

                for (iRecord = periods + 2; iRecord < iRecordCount + 2; iRecord++)
                {

                    //Move back n periods
                    pNav.Position = iRecord - periods;
                    double? dUpSum = 0;
                    double? dDownSum = 0;

                    int iPeriod;
                    for (iPeriod = 1; iPeriod < periods + 1; iPeriod++)
                    {

                        pNav.MovePrevious();
                        double? dYesterday = pSource.Value(pNav.Position);
                        pNav.MoveNext();
                        double? dToday = pSource.Value(pNav.Position);

                        if (dToday > dYesterday)
                        {
                            dUpSum += (dToday - dYesterday);
                        }
                        else if (dToday < dYesterday)
                        {
                            dDownSum += (dYesterday - dToday);
                        }

                        pNav.MoveNext();

                    }//Period

                    pNav.MovePrevious();
                    double? dValue;
                    if (dUpSum + dDownSum != 0)
                    {
                        dValue = 100 * (dUpSum - dDownSum) / (dUpSum + dDownSum);
                    }
                    else
                    {
                        dValue = null;
                    }
                    Field1.Value(pNav.Position, dValue);

                    pNav.MoveNext();

                }//Record

                pNav.MoveFirst();

                Results.AddField(Field1);

                return Results;
            }

            ///<summary>
            /// Momentum
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource">Field Source</param>
            ///<param name="periods">Periods</param>
            ///<returns>Recordset</returns>
            public Recordset Momentum(Navigator pNav, Field pSource, int periods)
            { return Momentum(pNav, pSource, periods, "Momentum"); }
            ///<summary>
            /// Momentum
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource">Field Source</param>
            ///<param name="periods">Periods</param>
            ///<param name="Alias">Alias</param>
            ///<returns>Recordset</returns>
            public Recordset Momentum(Navigator pNav, Field pSource, int periods, string Alias)
            {
                Recordset Results = new Recordset();
                int iRecord;

                int iRecordCount = pNav.RecordCount;

                Field Field1 = new Field(iRecordCount, Alias);

                int iStart = periods + 2;
                pNav.Position = iStart;

                for (iRecord = iStart; iRecord < iRecordCount + 1; iRecord++)
                {

                    pNav.Position = pNav.Position - periods;
                    double? dValue = pSource.Value(pNav.Position);
                    pNav.Position = pNav.Position + periods;
                    dValue = 100 + ((pSource.Value(pNav.Position) - dValue) / dValue) * 100;
                    Field1.Value(pNav.Position, dValue);
                    pNav.MoveNext();

                }//Record

                pNav.MoveFirst();
                Results.AddField(Field1);
                return Results;
            }

            ///<summary>
            /// TRIX
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource">Field Source</param>
            ///<param name="periods">Periods</param>
            ///<returns>Recordset</returns>
            public Recordset TRIX(Navigator pNav, Field pSource, int periods)
            { return TRIX(pNav, pSource, periods, "TRIX"); }
            ///<summary>
            /// TRIX
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource">Field Source</param>
            ///<param name="periods">Periods</param>
            ///<param name="Alias">Alias</param>
            ///<returns>Recordset</returns>
            public Recordset TRIX(Navigator pNav, Field pSource, int periods, string Alias)
            {
                MovingAverage MA = new MovingAverage();
                Recordset Results = new Recordset();
                int iRecord;

                Recordset RS = MA.ExponentialMovingAverage(pNav, pSource, periods, "EMA1");
                Field EMA = RS.GetField("EMA1");
                RS.RemoveField("EMA1");

                RS = MA.ExponentialMovingAverage(pNav, EMA, periods, "EMA2");
                EMA = RS.GetField("EMA2");
                RS.RemoveField("EMA2");

                RS = MA.ExponentialMovingAverage(pNav, EMA, periods, "EMA3");
                EMA = RS.GetField("EMA3");
                RS.RemoveField("EMA3");

                int iRecordCount = pNav.RecordCount;

                Field Field1 = new Field(iRecordCount, Alias);

                const int iStart = 2;
                pNav.Position = iStart;

                for (iRecord = iStart; iRecord < iRecordCount + 1; iRecord++)
                {

                    pNav.MovePrevious();
                    double? dValue = EMA.Value(pNav.Position);
                    pNav.MoveNext();
                    if (dValue != 0)
                    {
                        dValue = ((EMA.Value(pNav.Position) - dValue) / dValue) * 100;
                        Field1.Value(pNav.Position, dValue);
                    }
                    pNav.MoveNext();

                }//Record

                pNav.MoveFirst();
                Results.AddField(Field1);

                return Results;
            }

            ///<summary>
            /// Ultimate Oscilator
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pOHLCV">OHLCV Recordset</param>
            ///<param name="Cycle1">Cycle 1</param>
            ///<param name="Cycle2">Cycle 2</param>
            ///<param name="Cycle3">Cycle 3</param>
            ///<returns>Recordset</returns>
            public Recordset UltimateOscillator(Navigator pNav, Recordset pOHLCV,
                  int Cycle1, int Cycle2, int Cycle3)
            { return UltimateOscillator(pNav, pOHLCV, Cycle1, Cycle2, Cycle3, "Ultimate Oscillator"); }
            ///<summary>
            /// Ultimate Oscilator
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pOHLCV">OHLCV Recordset</param>
            ///<param name="Cycle1">Cycle 1</param>
            ///<param name="Cycle2">Cycle 2</param>
            ///<param name="Cycle3">Cycle 3</param>
            ///<param name="Alias">Alias</param>
            ///<returns>Recordset</returns>
            public Recordset UltimateOscillator(Navigator pNav, Recordset pOHLCV,
                  int Cycle1, int Cycle2, int Cycle3, string Alias)
            {
                Recordset Results = new Recordset();
                int iRecord;

                int iRecordCount = pNav.RecordCount;

                Field Field1 = new Field(iRecordCount, Alias);

                int iPeriods = Cycle1;
                if (Cycle2 > iPeriods)
                    iPeriods = Cycle2;
                if (Cycle3 > iPeriods)
                    iPeriods = Cycle3;

                int iStart = iPeriods + 2;
                pNav.Position = iStart;

                for (iRecord = iStart; iRecord < iRecordCount + 2; iRecord++)
                {

                    double? BPSum1 = 0;
                    double? BPSum2 = 0;
                    double? BPSum3 = 0;
                    double? TRSum1 = 0;
                    double? TRSum2 = 0;
                    double? TRSum3 = 0;

                    pNav.Position = iRecord - Cycle1;
                    int iPeriod;
                    double? TL;
                    double? BP;
                    double? TR;
                    for (iPeriod = 1; iPeriod < Cycle1 + 1; iPeriod++)
                    {
                        TL = pOHLCV.GetField("Low").Value(pNav.Position) <
                             pOHLCV.GetField("Close").Value(pNav.Position - 1)
                               ? pOHLCV.GetField("Low").Value(pNav.Position)
                               : pOHLCV.GetField("Close").Value(pNav.Position - 1);
                        BP = pOHLCV.GetField("Close").Value(pNav.Position) - TL;
                        TR = pOHLCV.GetField("High").Value(pNav.Position) -
                                pOHLCV.GetField("Low").Value(pNav.Position);
                        if (TR < pOHLCV.GetField("High").Value(pNav.Position) -
                                pOHLCV.GetField("Close").Value(pNav.Position - 1))
                        {
                            TR = pOHLCV.GetField("High").Value(pNav.Position) -
                                    pOHLCV.GetField("Close").Value(pNav.Position - 1);
                        }
                        if (TR < pOHLCV.GetField("Close").Value(pNav.Position - 1) -
                                pOHLCV.GetField("Low").Value(pNav.Position))
                        {
                            TR = pOHLCV.GetField("Close").Value(pNav.Position - 1) -
                                    pOHLCV.GetField("Low").Value(pNav.Position);
                        }
                        BPSum1 += BP;
                        TRSum1 += TR;
                        pNav.MoveNext();
                    }//Period

                    pNav.Position = pNav.Position - Cycle2;
                    for (iPeriod = 1; iPeriod < Cycle2 + 1; iPeriod++)
                    {
                        TL = pOHLCV.GetField("Low").Value(pNav.Position) <
                             pOHLCV.GetField("Close").Value(pNav.Position - 1)
                               ? pOHLCV.GetField("Low").Value(pNav.Position)
                               : pOHLCV.GetField("Close").Value(pNav.Position - 1);
                        BP = pOHLCV.GetField("Close").Value(pNav.Position) - TL;
                        TR = pOHLCV.GetField("High").Value(pNav.Position) -
                                pOHLCV.GetField("Low").Value(pNav.Position);
                        if (TR < pOHLCV.GetField("High").Value(pNav.Position) -
                                pOHLCV.GetField("Close").Value(pNav.Position - 1))
                        {
                            TR = pOHLCV.GetField("High").Value(pNav.Position) -
                                    pOHLCV.GetField("Close").Value(pNav.Position - 1);
                        }
                        if (TR < pOHLCV.GetField("Close").Value(pNav.Position - 1) -
                                pOHLCV.GetField("Low").Value(pNav.Position))
                        {
                            TR = pOHLCV.GetField("Close").Value(pNav.Position - 1) -
                                    pOHLCV.GetField("Low").Value(pNav.Position);
                        }
                        BPSum2 += BP;
                        TRSum2 += TR;
                        pNav.MoveNext();
                    } //Period

                    pNav.Position = pNav.Position - Cycle3;
                    for (iPeriod = 1; iPeriod < Cycle3 + 1; iPeriod++)
                    {
                        TL = pOHLCV.GetField("Low").Value(pNav.Position) <
                             pOHLCV.GetField("Close").Value(pNav.Position - 1)
                               ? pOHLCV.GetField("Low").Value(pNav.Position)
                               : pOHLCV.GetField("Close").Value(pNav.Position - 1);
                        BP = pOHLCV.GetField("Close").Value(pNav.Position) - TL;
                        TR = pOHLCV.GetField("High").Value(pNav.Position) -
                                pOHLCV.GetField("Low").Value(pNav.Position);
                        if (TR < pOHLCV.GetField("High").Value(pNav.Position) -
                                pOHLCV.GetField("Close").Value(pNav.Position - 1))
                        {
                            TR = pOHLCV.GetField("High").Value(pNav.Position) -
                                    pOHLCV.GetField("Close").Value(pNav.Position - 1);
                        }
                        if (TR < pOHLCV.GetField("Close").Value(pNav.Position - 1) -
                                pOHLCV.GetField("Low").Value(pNav.Position))
                        {
                            TR = pOHLCV.GetField("Close").Value(pNav.Position - 1) -
                                    pOHLCV.GetField("Low").Value(pNav.Position);
                        }
                        BPSum3 += BP;
                        TRSum3 += TR;
                        pNav.MoveNext();
                    }//Period

                    pNav.MovePrevious();
                    double? dValue = (4 * (BPSum1 / TRSum1) + 2 * (BPSum2 / TRSum2) + (BPSum3 / TRSum3)) / (4 + 2 + 1) * 100;
                    Field1.Value(pNav.Position, dValue);
                    pNav.MoveNext();

                }//Record

                pNav.MoveFirst();
                Results.AddField(Field1);
                return Results;
            }

            ///<summary>
            /// Vertical Horizontal Filter
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource">Field Source</param>
            ///<param name="periods">Periods</param>
            ///<returns>Recordset</returns>
            public Recordset VerticalHorizontalFilter(Navigator pNav, Field pSource, int periods)
            { return VerticalHorizontalFilter(pNav, pSource, periods, "VHF"); }
            ///<summary>
            /// Vertical Horizontal Filter
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource">Field Source</param>
            ///<param name="periods">Periods</param>
            ///<param name="Alias">Alias</param>
            ///<returns>Recordset</returns>
            public Recordset VerticalHorizontalFilter(Navigator pNav, Field pSource, int periods, string Alias)
            {
                Recordset Results = new Recordset();
                int iRecord;

                int iRecordCount = pNav.RecordCount;

                Field Field1 = new Field(iRecordCount, Alias);

                int iStart = periods + 2;
                pNav.Position = iStart;

                for (iRecord = iStart; iRecord < iRecordCount + 2; iRecord++)
                {

                    double? HCP = 0;
                    double? LCP = pSource.Value(pNav.Position);

                    pNav.Position = iRecord - periods;

                    int iPeriod;
                    for (iPeriod = 1; iPeriod < periods + 1; iPeriod++)
                    {
                        if (pSource.Value(pNav.Position) < LCP)
                        {
                            LCP = pSource.Value(pNav.Position);
                        }
                        else if (pSource.Value(pNav.Position) > HCP)
                        {
                            HCP = pSource.Value(pNav.Position);
                        }
                        pNav.MoveNext();
                    }//Period

                    double? Sum = 0;
                    pNav.Position = iRecord - periods;
                    double? Abs;
                    for (iPeriod = 1; iPeriod < periods + 1; iPeriod++)
                    {
                        Abs = (pSource.Value(pNav.Position) - pSource.Value(pNav.Position - 1));
                        if (Abs < 0)
                            Abs = -1 * Abs;
                        Sum += Abs;
                        pNav.MoveNext();
                    }//Period

                    pNav.MovePrevious();
                    Abs = (HCP - LCP) / Sum;
                    if (Abs < 0)
                        Abs = -1 * Abs;
                    Field1.Value(pNav.Position, Abs);
                    pNav.MoveNext();

                }//Record

                pNav.MoveFirst();
                Results.AddField(Field1);
                return Results;
            }

            ///<summary>
            /// William %R
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pOHLCV">OHLCV Recordset</param>
            ///<param name="periods">Periods</param>
            ///<returns>Recordset</returns>
            public Recordset WilliamsPctR(Navigator pNav, Recordset pOHLCV, int periods)
            { return WilliamsPctR(pNav, pOHLCV, periods, "Williams' %R"); }
            ///<summary>
            /// William %R
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pOHLCV">OHLCV Recordset</param>
            ///<param name="periods">Periods</param>
            ///<param name="Alias">Alias</param>
            ///<returns>Recordset</returns>
            public Recordset WilliamsPctR(Navigator pNav, Recordset pOHLCV, int periods, string Alias)
            {
                Recordset Results = new Recordset();
                int Record;

                int RecordCount = pNav.RecordCount;

                Field Field1 = new Field(RecordCount, Alias);

                int Start = periods + 2;
                pNav.Position = Start;

                for (Record = Start; Record < RecordCount + 2; Record++)
                {

                    double? HH = 0;
                    double? LL = pOHLCV.GetField("Low").Value(pNav.Position);

                    pNav.Position = Record - periods;

                    int Period;
                    for (Period = 1; Period < periods + 1; Period++)
                    {
                        if (pOHLCV.GetField("High").Value(pNav.Position) > HH)
                        {
                            HH = pOHLCV.GetField("High").Value(pNav.Position);
                        }
                        if (pOHLCV.GetField("Low").Value(pNav.Position) < LL)
                        {
                            LL = pOHLCV.GetField("Low").Value(pNav.Position);
                        }
                        pNav.MoveNext();
                    }//Period

                    pNav.MovePrevious();
                    Field1.Value(pNav.Position, ((HH - pOHLCV.GetField("Close").Value(pNav.Position)) / (HH - LL)) * -100);
                    pNav.MoveNext();

                }//Record

                pNav.MoveFirst();
                Results.AddField(Field1);
                return Results;
            }

            ///<summary>
            /// Williams Acumulation Distribution
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pOHLCV">OHLCV Recordset</param>
            ///<returns>Recordset</returns>
            public Recordset WilliamsAccumulationDistribution(Navigator pNav, Recordset pOHLCV)
            { return WilliamsAccumulationDistribution(pNav, pOHLCV, "Williams' Accumulation Distribution"); }
            ///<summary>
            /// Williams Acumulation Distribution
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pOHLCV">OHLCV Recordset</param>
            ///<param name="Alias">Alias</param>
            ///<returns>Recordset</returns>
            public Recordset WilliamsAccumulationDistribution(Navigator pNav, Recordset pOHLCV, string Alias)
            {
                Recordset Results = new Recordset();
                int Record;

                int RecordCount = pNav.RecordCount;

                Field Field1 = new Field(RecordCount, Alias);

                const int Start = 2;
                pNav.Position = Start;

                for (Record = 1; Record < RecordCount; Record++)
                {

                    double TRH = pOHLCV.GetField("Close").ValueEx(pNav.Position - 1);
                    if (pOHLCV.GetField("High").ValueEx(pNav.Position) > TRH)
                    {
                        TRH = pOHLCV.GetField("High").ValueEx(pNav.Position);
                    }

                    double TRL = pOHLCV.GetField("Close").ValueEx(pNav.Position - 1);
                    if (pOHLCV.GetField("Low").ValueEx(pNav.Position) < TRL)
                    {
                        TRL = pOHLCV.GetField("Low").ValueEx(pNav.Position);
                    }

                    double Value;
                    if (pOHLCV.GetField("Close").ValueEx(pNav.Position) > pOHLCV.GetField("Close").ValueEx(pNav.Position - 1))
                    {
                        Value = pOHLCV.GetField("Close").ValueEx(pNav.Position) - TRL;
                    }
                    else if (pOHLCV.GetField("Close").ValueEx(pNav.Position) < pOHLCV.GetField("Close").ValueEx(pNav.Position - 1))
                    {
                        Value = pOHLCV.GetField("Close").ValueEx(pNav.Position) - TRH;
                    }
                    else
                    {
                        Value = 0;
                    }

                    Field1.Value(pNav.Position, Value + Field1.ValueEx(pNav.Position - 1));

                    pNav.MoveNext();

                }//Record

                pNav.MoveFirst();
                Results.AddField(Field1);
                return Results;
            }

            ///<summary>
            /// Volume Oscilator
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="Volume">Field Volume</param>
            ///<param name="ShortTerm">Shor tTerm</param>
            ///<param name="LongTerm">Long Term</param>
            ///<param name="PointsOrPercent">Point or Percent, 0 - Point, 1 - Percent</param>
            ///<returns>Recordset</returns>
            public Recordset VolumeOscillator(Navigator pNav, Field Volume, int ShortTerm, int LongTerm,
                  int PointsOrPercent)
            { return VolumeOscillator(pNav, Volume, ShortTerm, LongTerm, PointsOrPercent, "Volume Oscillator"); }
            ///<summary>
            /// Volume Oscilator
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="Volume">Field Volume</param>
            ///<param name="ShortTerm">Shor tTerm</param>
            ///<param name="LongTerm">Long Term</param>
            ///<param name="PointsOrPercent">Point or Percent, 0 - Point, 1 - Percent</param>
            ///<param name="Alias">Alias</param>
            ///<returns>Recordset</returns>
            public Recordset VolumeOscillator(Navigator pNav, Field Volume, int ShortTerm, int LongTerm,
                  int PointsOrPercent, string Alias)
            {
                MovingAverage MA = new MovingAverage();
                Recordset Results = new Recordset();
                int Record;
                double? Value = 0;

                int RecordCount = pNav.RecordCount;

                Field Field1 = new Field(RecordCount, Alias);

                Recordset MA1 = MA.SimpleMovingAverage(pNav, Volume, ShortTerm, "MA1");
                Recordset MA2 = MA.SimpleMovingAverage(pNav, Volume, LongTerm, "MA2");

                for (Record = 1; Record < RecordCount + 1; Record++)
                {

                    if (PointsOrPercent == 1)
                    {
                        Value = MA1.GetField("MA1").Value(pNav.Position) -
                                MA2.GetField("MA2").Value(pNav.Position);
                    }
                    else if (PointsOrPercent == 2)
                    {
                        if (MA2.GetField("MA2").Value(pNav.Position) > 0)
                        {
                            Value = ((MA1.GetField("MA1").Value(pNav.Position) -
                                    MA2.GetField("MA2").Value(pNav.Position)) /
                                    MA2.GetField("MA2").Value(pNav.Position)) * 100;
                        }
                    }

                    Field1.Value(Record, Value);
                    pNav.MoveNext();

                }//Record

                pNav.MoveFirst();
                Results.AddField(Field1);

                return Results;
            }

            ///<summary>
            /// Chaikin Volatility
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pOHLCV">OHLCV Recordset</param>
            ///<param name="periods">Periods</param>
            ///<param name="ROC">ROC</param>
            ///<param name="MAType">Moving Averager Type</param>
            ///<returns>Recordset</returns>
            public Recordset ChaikinVolatility(Navigator pNav, Recordset pOHLCV, int periods, int ROC, IndicatorType MAType)
            { return ChaikinVolatility(pNav, pOHLCV, periods, ROC, MAType, "Chaikin Volatility"); }
            ///<summary>
            /// Chaikin Volatility
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pOHLCV">OHLCV Recordset</param>
            ///<param name="periods">Periods</param>
            ///<param name="ROC">ROC</param>
            ///<param name="MAType">Moving Averager Type</param>
            ///<param name="Alias">Alias</param>
            ///<returns>Recordset</returns>
            ///<exception cref="ArgumentNullException"></exception>
            public Recordset ChaikinVolatility(Navigator pNav, Recordset pOHLCV, int periods, int ROC, IndicatorType MAType, string Alias)
            {
                MovingAverage MA = new MovingAverage();
                Recordset Results = new Recordset();
                Recordset HLMA = null;
                int Record;
                double? Value = 0;

                int RecordCount = pNav.RecordCount;

                Field Field1 = new Field(RecordCount, Alias);

                Field HL = new Field(RecordCount, "HL");

                pNav.MoveFirst();
                for (Record = 1; Record < RecordCount + 1; Record++)
                {
                    HL.Value(pNav.Position, pOHLCV.GetField("High").Value(pNav.Position) -
                    pOHLCV.GetField("Low").Value(pNav.Position));
                    pNav.MoveNext();
                }

                switch (MAType)
                {
                    case IndicatorType.SimpleMovingAverage:
                        HLMA = MA.SimpleMovingAverage(pNav, HL, periods, "HLMA");
                        break;
                    case IndicatorType.ExponentialMovingAverage:
                        HLMA = MA.ExponentialMovingAverage(pNav, HL, periods, "HLMA");
                        break;
                    case IndicatorType.TimeSeriesMovingAverage:
                        HLMA = MA.TimeSeriesMovingAverage(pNav, HL, periods, "HLMA");
                        break;
                    case IndicatorType.TriangularMovingAverage:
                        HLMA = MA.TriangularMovingAverage(pNav, HL, periods, "HLMA");
                        break;
                    case IndicatorType.VariableMovingAverage:
                        HLMA = MA.VariableMovingAverage(pNav, HL, periods, "HLMA");
                        break;
                    case IndicatorType.WeightedMovingAverage:
                        HLMA = MA.WeightedMovingAverage(pNav, HL, periods, "HLMA");
                        break;
                    case IndicatorType.VIDYA:
                        HLMA = MA.VIDYA(pNav, HL, periods, 0.65, "HLMA");
                        break;
                }
                if (HLMA == null)
                    throw new ArgumentNullException();

                int Start = ROC + 1;
                pNav.Position = Start;
                for (Record = Start; Record < RecordCount + 1; Record++)
                {
                    double? MA1 = HLMA.GetField("HLMA").Value(pNav.Position - ROC);
                    double? MA2 = HLMA.GetField("HLMA").Value(pNav.Position);
                    if (MA1 != 0 && MA2 != 0) { Value = ((MA1 - MA2) / MA1) * -100; }
                    Field1.Value(Record, Value);
                    pNav.MoveNext();
                }//Record

                pNav.MoveFirst();

                Results.AddField(Field1);
                return Results;
            }

            ///<summary>
            /// Stochastic Oscilator
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pOHLCV">OHLCV Recordset</param>
            ///<param name="KPeriods">K Periods</param>
            ///<param name="KSlowingPeriods">K Slowing Periods</param>
            ///<param name="DPeriods">D Periods</param>
            ///<param name="MAType">Moving Average Type</param>
            ///<returns>Recordset</returns>
            ///<exception cref="ArgumentNullException"></exception>
            public Recordset StochasticOscillator(Navigator pNav, Recordset pOHLCV, int KPeriods, int KSlowingPeriods, int DPeriods,
              IndicatorType MAType)
            {
                MovingAverage MA = new MovingAverage();
                Recordset PctD = null;
                Recordset Results = new Recordset();
                int Record;

                int RecordCount = pNav.RecordCount;

                Field Field1 = new Field(RecordCount, "%K");

                int Start = KPeriods + 2;
                pNav.Position = Start;

                for (Record = Start; Record < RecordCount + 2; Record++)
                {

                    pNav.Position = Record - KPeriods;
                    double? HH = pOHLCV.GetField("High").Value(pNav.Position);
                    double? LL = pOHLCV.GetField("Low").Value(pNav.Position);

                    int Period;
                    for (Period = 1; Period < KPeriods + 1; Period++)
                    {

                        if (pOHLCV.GetField("High").Value(pNav.Position) > HH)
                        {
                            HH = pOHLCV.GetField("High").Value(pNav.Position);
                        }

                        if (pOHLCV.GetField("Low").Value(pNav.Position) < LL)
                        {
                            LL = pOHLCV.GetField("Low").Value(pNav.Position);
                        }

                        pNav.MoveNext();

                    } //Period

                    pNav.MovePrevious();
                    double? Value = (pOHLCV.GetField("Close").Value(pNav.Position) - LL) / (HH - LL) * 100;
                    Field1.Value(pNav.Position, Value);
                    pNav.MoveNext();

                }//Record

                if (KSlowingPeriods > 1)
                {
                    Results = null;
                    switch (MAType)
                    {
                        case IndicatorType.SimpleMovingAverage:
                            Results = MA.SimpleMovingAverage(pNav, Field1, KSlowingPeriods, "%K");
                            break;
                        case IndicatorType.ExponentialMovingAverage:
                            Results = MA.ExponentialMovingAverage(pNav, Field1, KSlowingPeriods, "%K");
                            break;
                        case IndicatorType.TimeSeriesMovingAverage:
                            Results = MA.TimeSeriesMovingAverage(pNav, Field1, KSlowingPeriods, "%K");
                            break;
                        case IndicatorType.TriangularMovingAverage:
                            Results = MA.TriangularMovingAverage(pNav, Field1, KSlowingPeriods, "%K");
                            break;
                        case IndicatorType.VariableMovingAverage:
                            Results = MA.VariableMovingAverage(pNav, Field1, KSlowingPeriods, "%K");
                            break;
                        case IndicatorType.WeightedMovingAverage:
                            Results = MA.WeightedMovingAverage(pNav, Field1, KSlowingPeriods, "%K");
                            break;
                        case IndicatorType.VIDYA:
                            Results = MA.VIDYA(pNav, Field1, KSlowingPeriods, 0.65, "%K");
                            break;
                    }
                }
                else
                {
                    Results.AddField(Field1);
                }
                if (Results == null)
                    throw new ArgumentNullException();

                Field1 = Results.GetField("%K");

                switch (MAType)
                {
                    case IndicatorType.SimpleMovingAverage:
                        PctD = MA.SimpleMovingAverage(pNav, Field1, DPeriods, "%D");
                        break;
                    case IndicatorType.ExponentialMovingAverage:
                        PctD = MA.ExponentialMovingAverage(pNav, Field1, DPeriods, "%D");
                        break;
                    case IndicatorType.TimeSeriesMovingAverage:
                        PctD = MA.TimeSeriesMovingAverage(pNav, Field1, DPeriods, "%D");
                        break;
                    case IndicatorType.TriangularMovingAverage:
                        PctD = MA.TriangularMovingAverage(pNav, Field1, DPeriods, "%D");
                        break;
                    case IndicatorType.VariableMovingAverage:
                        PctD = MA.VariableMovingAverage(pNav, Field1, DPeriods, "%D");
                        break;
                    case IndicatorType.WeightedMovingAverage:
                        PctD = MA.WeightedMovingAverage(pNav, Field1, DPeriods, "%D");
                        break;
                    case IndicatorType.VIDYA:
                        PctD = MA.VIDYA(pNav, Field1, DPeriods, 0.65, "%D");
                        break;
                }

                pNav.MoveFirst();

                if (PctD != null)
                {
                    Results.AddField(PctD.GetField("%D"));
                    PctD.RemoveField("%D");
                }
                return Results;
            }

            ///<summary>
            /// Price Oscillator
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource">Field Source</param>
            ///<param name="LongCycle">Long Cycle</param>
            ///<param name="ShortCycle">Short Cycle</param>
            ///<param name="MAType">Moving Average Type</param>
            ///<returns>Recordset</returns>
            public Recordset PriceOscillator(Navigator pNav, Field pSource, int LongCycle, int ShortCycle,
              IndicatorType MAType)
            { return PriceOscillator(pNav, pSource, LongCycle, ShortCycle, MAType, "Price Oscillator"); }
            ///<summary>
            /// Price Oscillator
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource">Field Source</param>
            ///<param name="LongCycle">Long Cycle</param>
            ///<param name="ShortCycle">Short Cycle</param>
            ///<param name="MAType">Moving Average Type</param>
            ///<param name="Alias">Alias</param>
            ///<returns>Recordset</returns>
            ///<exception cref="ArgumentNullException"></exception>
            public Recordset PriceOscillator(Navigator pNav, Field pSource, int LongCycle, int ShortCycle,
              IndicatorType MAType, string Alias)
            {
                TASDK TASDK1 = new TASDK();
                Recordset Results = new Recordset();
                MovingAverage MA = new MovingAverage();
                Recordset LongMA = null;
                Recordset ShortMA = null;
                int Record;

                if (LongCycle <= ShortCycle)
                {
                    //cout << ("ShortCycle must be less than LongCycle");
                    return Results;
                }

                int RecordCount = pNav.RecordCount;

                Field Field1 = new Field(RecordCount, Alias);

                switch (MAType)
                {
                    case IndicatorType.SimpleMovingAverage:
                        LongMA = MA.SimpleMovingAverage(pNav, pSource, LongCycle, "MA");
                        ShortMA = MA.SimpleMovingAverage(pNav, pSource, ShortCycle, "MA");
                        break;
                    case IndicatorType.ExponentialMovingAverage:
                        LongMA = MA.ExponentialMovingAverage(pNav, pSource, LongCycle, "MA");
                        ShortMA = MA.ExponentialMovingAverage(pNav, pSource, ShortCycle, "MA");
                        break;
                    case IndicatorType.TimeSeriesMovingAverage:
                        LongMA = MA.TimeSeriesMovingAverage(pNav, pSource, LongCycle, "MA");
                        ShortMA = MA.TimeSeriesMovingAverage(pNav, pSource, ShortCycle, "MA");
                        break;
                    case IndicatorType.TriangularMovingAverage:
                        LongMA = MA.TriangularMovingAverage(pNav, pSource, LongCycle, "MA");
                        ShortMA = MA.TriangularMovingAverage(pNav, pSource, ShortCycle, "MA");
                        break;
                    case IndicatorType.VariableMovingAverage:
                        LongMA = MA.VariableMovingAverage(pNav, pSource, LongCycle, "MA");
                        ShortMA = MA.VariableMovingAverage(pNav, pSource, ShortCycle, "MA");
                        break;
                    case IndicatorType.WeightedMovingAverage:
                        LongMA = MA.WeightedMovingAverage(pNav, pSource, LongCycle, "MA");
                        ShortMA = MA.WeightedMovingAverage(pNav, pSource, ShortCycle, "MA");
                        break;
                    case IndicatorType.VIDYA:
                        LongMA = MA.VIDYA(pNav, pSource, LongCycle, 0.65, "MA");
                        ShortMA = MA.VIDYA(pNav, pSource, ShortCycle, 0.65, "MA");
                        break;
                }
                if (ShortMA == null)
                    throw new ArgumentNullException();
                if (LongMA == null)
                    throw new ArgumentNullException();

                int Start = TASDK1.max(LongCycle, ShortCycle, 0) + 1;

                pNav.Position = Start;

                for (Record = Start; Record < RecordCount + 1; Record++)
                {
                    double? Value = ((ShortMA.Value("MA", pNav.Position) -
                                      LongMA.Value("MA", pNav.Position)) /
                                     LongMA.Value("MA", pNav.Position)) * 100;
                    Field1.Value(pNav.Position, Value);
                    pNav.MoveNext();

                }//Record

                pNav.MoveFirst();

                Results.AddField(Field1);
                return Results;
            }

            ///<summary>
            /// MACD
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pOHLCV">OHLCV Recordset</param>
            ///<param name="SignalPeriods">Signal Periods</param>
            ///<param name="LongCycle">Long Cycle</param>
            ///<param name="ShortCycle">Short Cycle</param>
            ///<returns>Recordset</returns>
            public Recordset MACD(Navigator pNav, Recordset pOHLCV, int SignalPeriods, int LongCycle, int ShortCycle)
            { return MACD(pNav, pOHLCV, SignalPeriods, LongCycle, ShortCycle, "MACD"); }
            ///<summary>
            /// MACD
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pOHLCV">OHLCV Recordset</param>
            ///<param name="SignalPeriods">Signal Periods</param>
            ///<param name="LongCycle">Long Cycle</param>
            ///<param name="ShortCycle">Short Cycle</param>
            ///<param name="Alias">Alias</param>
            ///<returns>Recordset</returns>
            public Recordset MACD(Navigator pNav, Recordset pOHLCV, int SignalPeriods, int LongCycle, int ShortCycle, string Alias)
            {
                return MACD(pNav, pOHLCV.GetField("Close"), SignalPeriods, LongCycle, ShortCycle, Alias);
            }

            public Recordset MACD(Navigator pNav, Field field, int SignalPeriods, int LongCycle, int ShortCycle, string Alias)
            {
                Recordset Results = new Recordset();
                MovingAverage MA = new MovingAverage();
                int Record;
                string a = Alias;

                int RecordCount = pNav.RecordCount;

                Field Field1 = new Field(RecordCount, Alias);

                Recordset EMA1 = MA.ExponentialMovingAverage(pNav, field, LongCycle, "EMA");
                Recordset EMA2 = MA.ExponentialMovingAverage(pNav, field, ShortCycle, "EMA");

                for (Record = 1; Record < RecordCount + 1; Record++)
                {
                    Field1.Value(Record, EMA2.Value("EMA", Record) - EMA1.Value("EMA", Record));
                }

                EMA1 = MA.ExponentialMovingAverage(pNav, Field1, SignalPeriods, "EMA");

                Field Field2 = EMA1.GetField("EMA");
                Field2.Name = a + "Signal";

                pNav.MoveFirst();

                Results.AddField(Field1);
                Results.AddField(Field2);

                EMA1.RemoveField(a + "Signal");
                return Results;
            }

            ///<summary>
            /// MACD Histogram
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pOHLCV">OHLCV Recordset</param>
            ///<param name="SignalPeriods">Signal Periods</param>
            ///<param name="LongCycle">Long Cycle</param>
            ///<param name="ShortCycle">Short Cycle</param>
            ///<returns>Recordset</returns>
            public Recordset MACDHistogram(Navigator pNav, Recordset pOHLCV, int SignalPeriods, int LongCycle, int ShortCycle)
            { return MACDHistogram(pNav, pOHLCV, SignalPeriods, LongCycle, ShortCycle, "MACD"); }
            ///<summary>
            /// MACD Histogram
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pOHLCV">OHLCV Recordset</param>
            ///<param name="SignalPeriods">Signal Periods</param>
            ///<param name="LongCycle">Long Cycle</param>
            ///<param name="ShortCycle">Short Cycle</param>
            ///<param name="Alias">Alias</param>
            ///<returns>Recordset</returns>
            public Recordset MACDHistogram(Navigator pNav, Recordset pOHLCV, int SignalPeriods, int LongCycle, int ShortCycle, string Alias)
            {
                Recordset Results = new Recordset();
                MovingAverage MA = new MovingAverage();
                int Record;
                double? Value;
                string a = Alias;

                int RecordCount = pNav.RecordCount;

                Field Field1 = new Field(RecordCount, Alias);

                Recordset EMA1 = MA.ExponentialMovingAverage(pNav, pOHLCV.GetField("Close"), LongCycle, "EMA");
                Recordset EMA2 = MA.ExponentialMovingAverage(pNav, pOHLCV.GetField("Close"), ShortCycle, "EMA");


                for (Record = 1; Record < RecordCount + 1; Record++)
                {
                    Value = EMA2.Value("EMA", Record) - EMA1.Value("EMA", Record);
                    Field1.Value(Record, Value);
                }

                EMA1 = MA.ExponentialMovingAverage(pNav, Field1, SignalPeriods, "EMA");

                Field Field2 = EMA1.GetField("EMA");
                Field2.Name = a + "Signal";



                Field histogram = new Field(RecordCount, Alias);
                for (Record = 1; Record < RecordCount + 1; Record++)
                {
                    Value = Field1.Value(Record) - Field2.Value(Record);
                    histogram.Value(Record, Value);
                }

                pNav.MoveFirst();
                Results.AddField(histogram);
                EMA1.RemoveField(a + "Signal");

                return Results;
            }

            ///<summary>
            /// Easy Of Movement
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pOHLCV">OHLCV Recordset</param>
            ///<param name="periods">Periods</param>
            ///<param name="MAType">Moving Average Type</param>
            ///<returns>Recordset</returns>
            public Recordset EaseOfMovement(Navigator pNav, Recordset pOHLCV, int periods,
              IndicatorType MAType)
            { return EaseOfMovement(pNav, pOHLCV, periods, MAType, "Ease of Movement"); }
            ///<summary>
            /// Easy Of Movement
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pOHLCV">OHLCV Recordset</param>
            ///<param name="periods">Periods</param>
            ///<param name="MAType">Moving Average Type</param>
            ///<param name="Alias">Alias</param>
            ///<returns>Recordset</returns>
            public Recordset EaseOfMovement(Navigator pNav, Recordset pOHLCV, int periods,
              IndicatorType MAType, string Alias)
            {
                MovingAverage MA = new MovingAverage();
                Recordset Results = new Recordset();
                Recordset EMVMA = null;
                int Record;
                double? BoxRatio = 0;

                int RecordCount = pNav.RecordCount;

                Field Field1 = new Field(RecordCount, Alias);

                const int Start = 2;
                pNav.Position = Start;

                for (Record = Start; Record < RecordCount + 1; Record++)
                {

                    double? MPM = ((pOHLCV.Value("High", pNav.Position) +
                                   pOHLCV.Value("Low", pNav.Position)) / 2) -
                                 ((pOHLCV.Value("High", pNav.Position - 1) +
                                   pOHLCV.Value("Low", pNav.Position - 1)) / 2);

                    double? bd = (pOHLCV.Value("High", pNav.Position) - pOHLCV.Value("Low", pNav.Position));

                    if (bd != 0)
                    {
                        BoxRatio = pOHLCV.Value("Volume", pNav.Position) / bd;
                    }

                    double? EMV = MPM / BoxRatio;

                    Field1.Value(pNav.Position, EMV * 10000);

                    pNav.MoveNext();

                }//Record

                switch (MAType)
                {
                    case IndicatorType.SimpleMovingAverage:
                        EMVMA = MA.SimpleMovingAverage(pNav, Field1, periods, "MA");
                        break;
                    case IndicatorType.ExponentialMovingAverage:
                        EMVMA = MA.ExponentialMovingAverage(pNav, Field1, periods, "MA");
                        break;
                    case IndicatorType.TimeSeriesMovingAverage:
                        EMVMA = MA.TimeSeriesMovingAverage(pNav, Field1, periods, "MA");
                        break;
                    case IndicatorType.TriangularMovingAverage:
                        EMVMA = MA.TriangularMovingAverage(pNav, Field1, periods, "MA");
                        break;
                    case IndicatorType.VariableMovingAverage:
                        EMVMA = MA.VariableMovingAverage(pNav, Field1, periods, "MA");
                        break;
                    case IndicatorType.WeightedMovingAverage:
                        EMVMA = MA.WeightedMovingAverage(pNav, Field1, periods, "MA");
                        break;
                    case IndicatorType.VIDYA:
                        EMVMA = MA.VIDYA(pNav, Field1, periods, 0.65, "MA");
                        break;
                }

                if (EMVMA != null)
                {
                    Field1 = EMVMA.GetField("MA");
                    Field1.Name = Alias;
                    pNav.MoveFirst();
                    EMVMA.RemoveField(Alias);
                }
                Results.AddField(Field1);
                return Results;
            }

            ///<summary>
            /// Detrended Price Oscilator
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource">Field Source</param>
            ///<param name="periods">Periods</param>
            ///<param name="MAType">Moving Average Type</param>
            ///<returns>Recordset</returns>
            public Recordset DetrendedPriceOscillator(Navigator pNav, Field pSource, int periods,
              IndicatorType MAType)
            { return DetrendedPriceOscillator(pNav, pSource, periods, MAType, "DPO"); }
            ///<summary>
            /// Detrended Price Oscilator
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource">Field Source</param>
            ///<param name="periods">Periods</param>
            ///<param name="MAType">Moving Average Type</param>
            ///<param name="Alias">Alias</param>
            ///<returns>Recordset</returns>
            ///<exception cref="ArgumentNullException"></exception>
            public Recordset DetrendedPriceOscillator(Navigator pNav, Field pSource, int periods,
              IndicatorType MAType, string Alias)
            {
                MovingAverage MA = new MovingAverage();
                Recordset Results = new Recordset();
                Recordset DPOMA = null;
                int Record;

                int RecordCount = pNav.RecordCount;

                Field Field1 = new Field(RecordCount, Alias);

                switch (MAType)
                {
                    case IndicatorType.SimpleMovingAverage:
                        DPOMA = MA.SimpleMovingAverage(pNav, pSource, periods, "MA");
                        break;
                    case IndicatorType.ExponentialMovingAverage:
                        DPOMA = MA.ExponentialMovingAverage(pNav, pSource, periods, "MA");
                        break;
                    case IndicatorType.TimeSeriesMovingAverage:
                        DPOMA = MA.TimeSeriesMovingAverage(pNav, pSource, periods, "MA");
                        break;
                    case IndicatorType.TriangularMovingAverage:
                        DPOMA = MA.TriangularMovingAverage(pNav, pSource, periods, "MA");
                        break;
                    case IndicatorType.VariableMovingAverage:
                        DPOMA = MA.VariableMovingAverage(pNav, pSource, periods, "MA");
                        break;
                    case IndicatorType.WeightedMovingAverage:
                        DPOMA = MA.WeightedMovingAverage(pNav, pSource, periods, "MA");
                        break;
                    case IndicatorType.VIDYA:
                        DPOMA = MA.VIDYA(pNav, pSource, periods, 0.65, "MA");
                        break;
                }
                if (DPOMA == null)
                    throw new ArgumentNullException();

                int Start = periods + 1;
                pNav.Position = Start;

                for (Record = Start; Record < RecordCount + 1; Record++)
                {
                    Field1.Value(pNav.Position, pSource.Value(pNav.Position) -
                    DPOMA.Value("MA", pNav.Position - ((periods / 2) + 1)));
                    pNav.MoveNext();
                }//Record

                pNav.MoveFirst();

                Results.AddField(Field1);
                return Results;
            }

            ///<summary>
            /// Parabolic SAR
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="HighPrice">High Price</param>
            ///<param name="LowPrice">Low Price</param>
            ///<returns>Recordset</returns>
            public Recordset ParabolicSAR(Navigator pNav, Field HighPrice, Field LowPrice)
            { return ParabolicSAR(pNav, HighPrice, LowPrice, 0.02, 0.2, "Parabolic SAR"); }
            ///<summary>
            /// Parabolic SAR
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="HighPrice">High Price</param>
            ///<param name="LowPrice">Low Price</param>
            ///<param name="MinAF">Min AF</param>
            ///<param name="MaxAF">MAX AF</param>
            ///<param name="Alias">Alias</param>
            ///<returns>Recordset</returns>
            public Recordset ParabolicSAR(Navigator pNav, Field HighPrice, Field LowPrice, double MinAF, double MaxAF, string Alias)
            {
                Recordset Results = new Recordset();

                int Record;
                int Position;
                double? pSAR;

                int RecordCount = pNav.RecordCount;

                Field Field1 = new Field(RecordCount, Alias);

                const int Start = 2;
                pNav.Position = Start;

                double? Max = HighPrice.Value(1);
                double? Min = LowPrice.Value(1);

                if (HighPrice.Value(2) - HighPrice.Value(1) < LowPrice.Value(2) - LowPrice.Value(1))
                {
                    pSAR = Max;
                    Position = -1;
                }
                else
                {
                    pSAR = Min;
                    Position = 1;
                }

                double? pAF = MinAF;
                double? SAR = pSAR;
                double? Hi = Max;
                double? Lo = Min;
                double? pHi = Hi;
                double? pLo = Lo;
                double? AF = MinAF;

                for (Record = Start; Record < RecordCount + 1; ++Record)
                {

                    if (Position == 1)
                    {

                        if (HighPrice.Value(Record) > Hi)
                        {
                            Hi = HighPrice.Value(Record);
                            if (AF < MaxAF) AF = AF + MinAF;
                        }
                        SAR = pSAR + pAF * (pHi - pSAR);
                        if (LowPrice.Value(Record) < SAR)
                        {
                            Position = -1;
                            AF = MinAF;
                            SAR = pHi;
                            Hi = 0;
                            Lo = LowPrice.Value(Record);
                        }

                    }
                    else if (Position == -1)
                    {
                        if (LowPrice.Value(Record) < Lo)
                        {
                            Lo = LowPrice.Value(Record);
                            if (AF < MaxAF) AF = AF + MinAF;
                        }
                        SAR = pSAR + pAF * (pLo - pSAR);
                        if (HighPrice.Value(Record) > SAR)
                        {
                            Position = 1;
                            AF = MinAF;
                            SAR = pLo;
                            Lo = 0;
                            Hi = HighPrice.Value(Record);
                        }
                    }

                    pHi = Hi;
                    pLo = Lo;
                    pSAR = SAR;
                    pAF = AF;

                    Field1.Value(Record, SAR);

                    pNav.MoveNext();


                } // Record

                pNav.MoveFirst();
                Results.AddField(Field1);
                return Results;
            }

            ///<summary>
            /// Directional Movement System
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pOHLCV">OHLCV Recordset</param>
            ///<param name="periods">Periods</param>
            ///<returns>Recordset</returns>
            public Recordset DirectionalMovementSystem(Navigator pNav, Recordset pOHLCV, int periods)
            {
                MovingAverage MA = new MovingAverage();

                Recordset Results = new Recordset();
                Recordset ADX = new Recordset();

                int Record;

                int RecordCount = pNav.RecordCount;

                Recordset rsTemp = TrueRange(pNav, pOHLCV, "TR");
                Field TR = rsTemp.GetField("TR");
                rsTemp.RemoveField("TR");

                Recordset wSumTR = MA.WellesWilderSmoothing(TR, periods, "TRSum");

                Field UpDMI = new Field(RecordCount, "UpDMI");
                Field DnDMI = new Field(RecordCount, "DnDMI");
                Field DIN = new Field(RecordCount, "DI-");
                Field DIP = new Field(RecordCount, "DI+");

                for (Record = 2; Record != RecordCount + 1; ++Record)
                {
                    double? high0 = pOHLCV.Value("High", Record);
                    double? high1 = pOHLCV.Value("High", Record - 1);
                    double? low1 = pOHLCV.Value("Low", Record - 1);
                    double? low0 = pOHLCV.Value("Low", Record);
                    high0 = high0.HasValue ? high0 : 0.0;
                    high1 = high1.HasValue ? high1 : 0.0;
                    low1 = low1.HasValue ? low1 : 0.0;
                    low0 = low0.HasValue ? low0 : 0.0;

                    double HDIF = (double)(high0 - high1);// pOHLCV.Value("High", Record) - pOHLCV.Value("High", Record - 1);
                    double LDIF = (double)(low1 - low0);// pOHLCV.Value("Low", Record - 1) - pOHLCV.Value("Low", Record);

                    if ((HDIF < 0 && LDIF < 0) || (HDIF == LDIF))
                    {
                        UpDMI.Value(Record, 0);
                        DnDMI.Value(Record, 0);
                    }
                    else if (HDIF > LDIF)
                    {
                        UpDMI.Value(Record, HDIF);
                        DnDMI.Value(Record, 0);
                    }
                    else if (HDIF < LDIF)
                    {
                        UpDMI.Value(Record, 0);
                        DnDMI.Value(Record, LDIF);
                    }

                } //Record

                Recordset wSumUpDMI = MA.WellesWilderSmoothing(UpDMI, periods, "DM+Sum");
                Recordset wSumDnDMI = MA.WellesWilderSmoothing(DnDMI, periods, "DM-Sum");

                for (Record = 2; Record != RecordCount + 1; ++Record)
                {
                    DIP.Value(Record, Math.Floor(100 * wSumUpDMI.ValueEx("DM+Sum", Record) /
                                                 wSumTR.ValueEx("TRSum", Record)));
                    DIN.Value(Record, Math.Floor(100 * wSumDnDMI.ValueEx("DM-Sum", Record) /
                                                 wSumTR.ValueEx("TRSum", Record)));

                } //Record

                Field DX = new Field(RecordCount, "DX");

                for (Record = 2; Record != RecordCount + 1; ++Record)
                {
                    double a = Math.Abs(DIP.ValueEx(Record) - DIN.ValueEx(Record));
                    double b = DIP.ValueEx(Record) + DIN.ValueEx(Record);
                    if (a > 0 && b > 0)
                    {
                        DX.Value(Record, Math.Floor(100 * (a / b)));
                    }

                } //Record

                Field ADXF = new Field(RecordCount, "ADX");
                ADX.AddField(ADXF);

                for (Record = periods + 1; Record != RecordCount + 1; ++Record)
                {
                    double Value = Math.Floor(((ADX.ValueEx("ADX", Record - 1) * (periods - 1)) +
                                               DX.ValueEx(Record)) / periods + 0.5);
                    ADX.Value("ADX", Record, Value);
                } //Record

                Field ADXR = new Field(RecordCount, "ADXR");

                for (Record = periods + 1; Record != RecordCount + 1; ++Record)
                {

                    ADXR.Value(Record, Math.Floor((ADX.ValueEx("ADX", Record) +
                                                   ADX.ValueEx("ADX", Record - 1)) / 2) + 0.5);

                } //Record

                pNav.MoveFirst();

                Results.AddField(ADX.GetField("ADX"));
                Results.AddField(ADXR);
                Results.AddField(DX);
                Results.AddField(wSumTR.GetField("TRSum"));
                Results.AddField(DIN);
                Results.AddField(DIP);

                ADX.RemoveField("ADX");
                ADX.RemoveField("ADXF");
                wSumTR.RemoveField("TRSum");

                return Results;
            }

            ///<summary>
            /// True Range
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pOHLCV">OHLCV Recordset</param>
            ///<returns>Recordset</returns>
            public Recordset TrueRange(Navigator pNav, Recordset pOHLCV)
            { return TrueRange(pNav, pOHLCV, "TR"); }
            ///<summary>
            /// True Range
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pOHLCV">OHLCV Recordset</param>
            ///<param name="Alias">Alias</param>
            ///<returns>Recordset</returns>
            public Recordset TrueRange(Navigator pNav, Recordset pOHLCV, string Alias)
            {
                Recordset Results = new Recordset();

                int Record;

                int RecordCount = pNav.RecordCount;

                Field Field1 = new Field(RecordCount, Alias);

                const int Start = 2;
                pNav.Position = Start;

                for (Record = 2; Record != RecordCount + 1; ++Record)
                {

                    double? T1 = pOHLCV.Value("High", Record) - pOHLCV.Value("Low", Record);
                    double T2 = Math.Abs((int)(pOHLCV.Value("High", Record) - pOHLCV.Value("Close", Record - 1)));
                    double T3 = Math.Abs((int)(pOHLCV.Value("Close", Record - 1) - pOHLCV.Value("Low", Record)));

                    double? Value = 0;
                    if (T1 > Value)
                        Value = T1;
                    if (T2 > Value)
                        Value = T2;
                    if (T3 > Value)
                        Value = T3;

                    Field1.Value(Record, Value);

                } //Record

                Field1.Name = Alias;

                pNav.MoveFirst();

                Results.AddField(Field1);

                return Results;
            }

            ///<summary>
            /// Arron
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pOHLCV">OHLCV Recordset</param>
            ///<param name="periods">Periods</param>
            ///<returns>Recordset</returns>
            public Recordset Aroon(Navigator pNav, Recordset pOHLCV, int periods)
            {
                Recordset Results = new Recordset();

                int Record;

                int RecordCount = pNav.RecordCount;

                Field AUp = new Field(RecordCount, "Aroon Up");
                Field ADn = new Field(RecordCount, "Aroon Down");
                Field AOs = new Field(RecordCount, "Aroon Oscillator");

                for (Record = periods + 1; Record != RecordCount + 1; ++Record)
                {

                    double? HighestHigh = pOHLCV.Value("High", Record);
                    double? LowestLow = pOHLCV.Value("Low", Record);
                    int HighPeriod = Record;
                    int LowPeriod = Record;

                    int Period;
                    for (Period = Record - periods; Period != Record; ++Period)
                    {

                        if (pOHLCV.Value("High", Period) > HighestHigh)
                        {
                            HighestHigh = pOHLCV.Value("High", Period);
                            HighPeriod = Period;
                        }

                        if (pOHLCV.Value("Low", Period) >= LowestLow) continue;
                        LowestLow = pOHLCV.Value("Low", Period);
                        LowPeriod = Period;
                    } // Period

                    AUp.Value(Record, ((double)periods - (Record - HighPeriod)) / periods * 100);
                    ADn.Value(Record, ((double)periods - (Record - LowPeriod)) / periods * 100);
                    AOs.Value(Record, (AUp.Value(Record) - ADn.Value(Record)));

                } // Record

                Results.AddField(AUp);
                Results.AddField(ADn);
                Results.AddField(AOs);

                return Results;
            }

            ///<summary>
            /// Rainbow Oscillator
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource">Field Source</param>
            ///<param name="Levels">Levels</param>
            ///<param name="MAType">Moving Average Type</param>
            ///<returns>Recordset</returns>
            public Recordset RainbowOscillator(Navigator pNav, Field pSource, int Levels,
              IndicatorType MAType)
            { return RainbowOscillator(pNav, pSource, Levels, MAType, "Rainbow Oscillator"); }
            ///<summary>
            /// Rainbow Oscillator
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource">Field Source</param>
            ///<param name="Levels">Levels</param>
            ///<param name="MAType">Moving Average Type</param>
            ///<param name="Alias">Alias</param>
            ///<returns>Recordset</returns>
            ///<exception cref="ArgumentNullException"></exception>
            public Recordset RainbowOscillator(Navigator pNav, Field pSource, int Levels,
              IndicatorType MAType, string Alias)
            {
                MovingAverage MA = new MovingAverage();
                Recordset Results = new Recordset();
                Recordset rsMA = null;

                int Record;
                int Level;

                int RecordCount = pNav.RecordCount;

                Field Field1 = new Field(RecordCount, Alias);

                for (Level = 2; Level != Levels + 1; ++Level)
                {

                    switch (MAType)
                    {
                        case IndicatorType.SimpleMovingAverage:
                            rsMA = MA.SimpleMovingAverage(pNav, pSource, Levels, "MA");
                            break;
                        case IndicatorType.ExponentialMovingAverage:
                            rsMA = MA.ExponentialMovingAverage(pNav, pSource, Levels, "MA");
                            break;
                        case IndicatorType.TimeSeriesMovingAverage:
                            rsMA = MA.TimeSeriesMovingAverage(pNav, pSource, Levels, "MA");
                            break;
                        case IndicatorType.TriangularMovingAverage:
                            rsMA = MA.TriangularMovingAverage(pNav, pSource, Levels, "MA");
                            break;
                        case IndicatorType.VariableMovingAverage:
                            rsMA = MA.VariableMovingAverage(pNav, pSource, Levels, "MA");
                            break;
                        case IndicatorType.WeightedMovingAverage:
                            rsMA = MA.WeightedMovingAverage(pNav, pSource, Levels, "MA");
                            break;
                        case IndicatorType.VIDYA:
                            rsMA = MA.VIDYA(pNav, pSource, Level, 0.65, "MA");
                            break;
                    }
                    if (rsMA == null)
                        throw new ArgumentNullException();

                    for (Record = 1; Record != RecordCount + 1; ++Record)
                    {
                        double Value = rsMA.ValueEx("MA", Record);
                        Field1.Value(Record, (pSource.ValueEx(Record) - Value) + Field1.ValueEx(Record));
                    } // Record

                } // Level

                for (Record = 1; Record != RecordCount + 1; ++Record)
                {
                    Field1.Value(Record, (Field1.Value(Record) / Levels));
                } // Record

                Results.AddField(Field1);
                return Results;
            }

            ///<summary>
            /// Fractal Chaos Oscillator
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pOHLCV">OHLCV Recordset</param>
            ///<param name="periods">Periods</param>
            ///<returns>Recordset</returns>
            public Recordset FractalChaosOscillator(Navigator pNav, Recordset pOHLCV, int periods)
            { return FractalChaosOscillator(pNav, pOHLCV, periods, "Fractal Chaos Oscillator"); }
            ///<summary>
            /// Fractal Chaos Oscillator
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pOHLCV">OHLCV Recordset</param>
            ///<param name="periods">Periods</param>
            ///<param name="Alias">Alias</param>
            ///<returns>Recordset</returns>
            public Recordset FractalChaosOscillator(Navigator pNav, Recordset pOHLCV, int periods, string Alias)
            {
                Recordset Results = new Recordset();

                int RecordCount = pNav.RecordCount;
                int Record;

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

                    fH1.Value(Record, fH.Value(Record - 4));
                    fL1.Value(Record, fL.Value(Record - 4));

                    fH2.Value(Record, fH.Value(Record - 3));
                    fL2.Value(Record, fL.Value(Record - 3));

                    fH3.Value(Record, fH.Value(Record - 2));
                    fL3.Value(Record, fL.Value(Record - 2));

                    fH4.Value(Record, fH.Value(Record - 1));
                    fL4.Value(Record, fL.Value(Record - 1));

                }

                for (Record = 2; Record < RecordCount + 1; ++Record)
                {

                    if ((fH3.Value(Record) > fH1.Value(Record)) &&
                        (fH3.Value(Record) > fH2.Value(Record)) &&
                        (fH3.Value(Record) >= fH4.Value(Record)) &&
                        (fH3.Value(Record) >= fH.Value(Record)))
                    {
                        fFR.Value(Record, 1);
                    }

                    if ((fL3.Value(Record) < fL1.Value(Record)) &&
                        (fL3.Value(Record) < fL2.Value(Record)) &&
                        (fL3.Value(Record) <= fL4.Value(Record)) &&
                        (fL3.Value(Record) <= fL.Value(Record)))
                    {
                        fFR.Value(Record, -1);
                    }

                }

                fFR.Name = Alias;
                Results.AddField(fFR);

                return Results;
            }

            ///<summary>
            /// Prime Number Oscilator
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource">Field Source</param>
            ///<returns>Recordset</returns>
            public Recordset PrimeNumberOscillator(Navigator pNav, Field pSource)
            { return PrimeNumberOscillator(pNav, pSource, "Prime Number Oscillator"); }
            ///<summary>
            /// Prime Number Oscilator
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource">Field Source</param>
            ///<param name="Alias">Alias</param>
            ///<returns>Recordset</returns>
            public Recordset PrimeNumberOscillator(Navigator pNav, Field pSource, string Alias)
            {
                Recordset Results = new Recordset();
                int RecordCount = pNav.RecordCount;
                int Record;
                Field fPrime = new Field(RecordCount, Alias);
                General GN = new General();

                long Top = 0, Bottom = 0;

                for (Record = 1; Record != RecordCount + 1; ++Record)
                {

                    long Value = (long)(pSource.Value(Record));
                    if (Value < 10) Value = Value * 10;

                    long N;
                    for (N = Value; N != 1; --N)
                    {
                        if (General.IsPrime(N))
                        {
                            Bottom = N;
                            break;
                        }
                    }

                    for (N = Value; N != Value * 2; ++N)
                    {
                        if (General.IsPrime(N))
                        {
                            Top = N;
                            break;
                        }
                    }

                    if (Math.Abs((double)(Value - Top)) < Math.Abs((double)(Value - Bottom)))
                        fPrime.Value(Record, Value - Top);
                    else
                        fPrime.Value(Record, Value - Bottom);

                } // Record
                Results.AddField(fPrime);

                return Results;
            }

            public Recordset ElderRay(Navigator pNav, Recordset pOHLCV, int Periods, IndicatorType MAType, string Alias)
            {
                Recordset Results = new Recordset();

                Field pClose = pOHLCV.GetField("Close");
                Field pHigh = pOHLCV.GetField("High");
                Field pLow = pOHLCV.GetField("Low");

                int RecordCount = pClose.RecordCount;
                if (Periods < 1) Periods = 13;

                Field fBullPower = new Field(), fBearPower = new Field();
                fBullPower.Initialize(RecordCount, Alias + " Bull Power");
                fBearPower.Initialize(RecordCount, Alias + " Bear Power");

                MovingAverage ma = new MovingAverage();
                Field ema = ma.MovingAverageSwitch(pNav, pClose, Periods, MAType, "ema").GetField("ema");

                for (int record = Periods + 1; record < RecordCount + 1; record++)
                {
                    fBullPower.SetValue(record, pHigh.Value(record) - ema.Value(record));
                    fBearPower.SetValue(record, ema.Value(record) - pLow.Value(record));
                }

                Results.AddField(fBullPower);
                Results.AddField(fBearPower);
                return Results;
            }

            public Recordset EhlerFisherTransform(Navigator pNav, Recordset pOHLCV, int Periods, string Alias)
            {
                Recordset Results = new Recordset();
                if (Periods < 1) Periods = 10;
                int RecordCount = pOHLCV.GetField("Close").RecordCount;
                Field fish = new Field();
                fish.Initialize(RecordCount, Alias);
                Field trigger = new Field();
                trigger.Initialize(RecordCount, Alias + " Trigger");

                General g = new General();
                Field price = g.MedianPrice(pNav, pOHLCV, "price").GetField("price");
                Field maxh = g.HHV(pNav, price, Periods, "maxh").GetField("maxh");
                Field minl = g.LLV(pNav, price, Periods, "minl").GetField("minl");

                double value1 = 0, prevValue1 = 0, prevFish = 0;
                for (int record = 1; record < RecordCount + 1; record++)
                {
                    double mh = maxh.ValueEx(record);
                    double ml = minl.ValueEx(record);
                    if (mh != ml)
                        value1 = 0.33 * 2 * ((price.ValueEx(record) - ml) / (mh - ml) - 0.5) + 0.67 * prevValue1;
                    else
                        value1 = 0;
                    if (value1 > 0.99) value1 = 0.999;
                    if (value1 < -0.99) value1 = -0.999;
                    double fishValue = 0.5 * Math.Log((1 + value1) / (1 - value1)) + 0.5 * prevFish;
                    fish.SetValue(record, fishValue);
                    trigger.SetValue(record, prevFish);
                    prevValue1 = value1;
                    prevFish = fishValue;
                }

                Results.AddField(fish);
                Results.AddField(trigger);
                return Results;
            }

            public Recordset SchaffTrendCycle(Navigator pNav, Field pSource, int Periods, int ShortCycle, int LongCycle, /*IndicatorType MAType,*/ string Alias)
            {
                Recordset results = new Recordset();
                int recordCount = pSource.RecordCount;
                double Factor = 0.5;
                Field Frac1 = new Field();
                Frac1.Initialize(recordCount, "x");
                Field Frac2 = new Field();
                Frac2.Initialize(recordCount, "x");
                Field PF = new Field();
                PF.Initialize(recordCount, "x");
                Field PFF = new Field();
                PFF.Initialize(recordCount, Alias);

                General g = new General();
                Oscillator o = new Oscillator();

                Field XMac = o.MACD(pNav, pSource, 2, LongCycle, ShortCycle, /*MAType,*/ "x").GetField("x");
                Field Value1 = g.LLV(pNav, XMac, Periods, "x").GetField("x");
                Field Value2 = g.HHV(pNav, XMac, Periods, "x").GetField("x");

                for (int record = 1; record < recordCount + 1; ++record)
                    Value2.SetValue(record, Value2.Value(record) - Value1.Value(record));

                for (int record = 2; record < recordCount + 1; ++record)
                {
                    if (Value2.Value(record) > 0)
                        Frac1.SetValue(record, ((XMac.Value(record) - Value1.Value(record)) / Value2.Value(record)) * 100);
                    else
                        Frac1.SetValue(record, Frac1.Value(record - 1));

                    PF.SetValue(record, PF.Value(record - 1) + (Factor * (Frac1.Value(record) - PF.Value(record - 1))));

                }

                Field Value3 = g.LLV(pNav, PF, Periods, "x").GetField("x");
                Field Value4 = g.HHV(pNav, PF, Periods, "x").GetField("x");

                for (int record = 1; record < recordCount + 1; ++record)
                    Value4.SetValue(record, Value4.Value(record) - Value3.Value(record));

                for (int record = 2; record < recordCount + 1; ++record)
                {
                    if (Value2.Value(record) > 0)
                        Frac2.SetValue(record, ((PF.Value(record) - Value3.Value(record)) / Value4.Value(record)) * 100);
                    else
                        Frac2.SetValue(record, Frac2.Value(record - 1));

                    PFF.SetValue(record, PFF.Value(record - 1) + (Factor * (Frac2.Value(record) - PFF.Value(record - 1))));
                }

                results.AddField(PFF);
                return results;
            }

            public Recordset CenterOfGravity(Field pSource, int Periods, string Alias)
            {
                Recordset results = new Recordset();
                int recordCount = pSource.RecordCount;
                Field field1 = new Field();
                field1.Initialize(recordCount, Alias);

                for (int record = Periods + 1; record < recordCount + 1; ++record)
                {
                    double num = 0, den = 0;
                    int count = 1;
                    for (int n = record - 1; n > record - Periods; --n)
                    {
                        num += (pSource.ValueEx(n) * (count + 1));
                        den += pSource.ValueEx(n);
                        count++;
                    }
                    field1.SetValue(record, -1 * num / den);
                }

                results.AddField(field1);
                return results;
            }

            public Recordset CoppockCurve(Navigator pNav, Field pSource, string Alias)
            {
                Recordset results = new Recordset();
                int recordCount = pSource.RecordCount;
                Field field1 = new Field();
                field1.Initialize(recordCount, Alias);

                General g = new General();
                Field roc14 = g.PriceROC(pNav, pSource, 14, "x").GetField("x");
                Field roc11 = g.PriceROC(pNav, pSource, 11, "x").GetField("x");

                for (int record = 1; record < recordCount + 1; ++record)
                    field1.SetValue(record, roc14.Value(record) + roc11.Value(record));

                MovingAverage ma = new MovingAverage();

                return ma.WeightedMovingAverage(pNav, field1, 10, Alias);
            }

            public Recordset ChandeForecastOscillator(Navigator pNav, Field pSource, int Periods, string Alias)
            {
                Recordset results = new Recordset();
                int recordCount = pSource.RecordCount;
                Field field1 = new Field();
                field1.Initialize(recordCount, Alias);

                LinearRegression r = new LinearRegression();
                Field f = r.TimeSeriesForecast(pNav, pSource, Periods, "x").GetField("x");

                for (int record = 1; record < recordCount + 1; ++record)
                    field1.SetValue(record, ((pSource.Value(record) - f.Value(record)) / pSource.Value(record)) * 100);

                for (int record = 1; record < Periods + 1; ++record)
                    field1.SetValue(record, 0);

                results.AddField(field1);
                return results;
            }

            public Recordset KlingerVolumeOscillator(Navigator pNav, Recordset pOHLCV, int SignalPeriods, int LongCycle, int ShortCycle, /*int MAType,*/ string Alias)
            {
                Recordset Results = new Recordset();
                Field pVolume = pOHLCV.GetField("Volume");

                int recordCount = pVolume.RecordCount;
                Field field1 = new Field();
                field1.Initialize(recordCount, Alias);

                General g = new General();
                Field tp = g.TypicalPrice(pNav, pOHLCV, "x").GetField("x");

                Field sv = new Field();
                sv.Initialize(recordCount, "sv");

                for (int record = 2; record < recordCount + 1; record++)
                {
                    if (tp.Value(record) >= tp.Value(record - 1))
                        sv.SetValue(record, pVolume.Value(record));
                    else
                        sv.SetValue(record, -1 * pVolume.Value(record));
                }

                Oscillator o = new Oscillator();
                return o.MACD(pNav, sv, SignalPeriods, LongCycle, ShortCycle, /*MAType,*/ Alias);
            }

            public Recordset PrettyGoodOscillator(Navigator pNav, Recordset pOHLCV, int Periods, string Alias)
            {
                Recordset Results = new Recordset();
                Field pClose = pOHLCV.GetField("Close");
                int recordCount = pClose.RecordCount;
                Field field1 = new Field();
                field1.Initialize(recordCount, Alias);
                Field sv = new Field();
                sv.Initialize(recordCount, "sv");
                MovingAverage ma = new MovingAverage();
                Field sma = ma.SimpleMovingAverage(pNav, pOHLCV.GetField("Close"), Periods, "x").GetField("x");
                Field tr = TrueRange(pNav, pOHLCV, "x").GetField("x");
                Field ema = ma.ExponentialMovingAverage(pNav, tr, Periods, "x").GetField("x");

                for (int record = 2; record < recordCount + 1; record++)
                    field1.SetValue(record, (pClose.Value(record) - sma.Value(record)) / ema.Value(record));

                for (int record = 1; record < Periods + 1; ++record)
                    field1.SetValue(record, 0);

                Results.AddField(field1);
                return Results;
            }
        }
    }
}
