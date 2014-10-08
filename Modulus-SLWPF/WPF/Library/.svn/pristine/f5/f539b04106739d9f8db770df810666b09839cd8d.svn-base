using System;

namespace ModulusFE
{
    namespace Tasdk
    {
        internal class Ichimoku
        {
            public Recordset Calc(Navigator nav, Field high, Field low, int p1, int p2, int p3)
            {
                Recordset res = new Recordset();
                int recordCount = nav.RecordCount;

                Field convLine = new Field(recordCount, "Conversion Line");
                Field baseLine = new Field(recordCount, "Base Line");
                Field leadingSpanA = new Field(recordCount + p2, "Leading Span A");
                Field leadingSpanB = new Field(recordCount + p2, "Leading Span B");
                Field lagginSpan = new Field(recordCount + p2, "Lagging Span");

                int minIndex = Math.Min(p1, Math.Min(p2, p3));

                double highestHigh = double.MinValue;
                double lowestLow = double.MaxValue;

                for (int i = minIndex + 1; i <= recordCount; i++)
                {
                    if (i >= p1)
                    {
                        for (int period = i - p1; period <= i; period++)
                        {
                            highestHigh = Math.Max(highestHigh, high.ValueEx(period));
                            lowestLow = Math.Min(lowestLow, low.ValueEx(period));
                        }

                        convLine.Value(i, (highestHigh + lowestLow) / 2);
                    }

                    if (i >= p2)
                    {
                        for (int period = i - p2; period <= i; period++)
                        {
                            highestHigh = Math.Max(highestHigh, high.ValueEx(period));
                            lowestLow = Math.Min(lowestLow, low.ValueEx(period));
                        }

                        baseLine.Value(i, (highestHigh + lowestLow) / 2);
                    }

                    if (i >= p1 && i >= p2)
                    {
                        leadingSpanA.Value(i + p2, (convLine.ValueEx(i) + baseLine.ValueEx(i)) / 2);
                    }

                    if (i >= p3)
                    {
                        for (int period = i - p3; period <= i; period++)
                        {
                            highestHigh = Math.Max(highestHigh, high.ValueEx(period));
                            lowestLow = Math.Min(lowestLow, low.ValueEx(period));
                        }

                        leadingSpanB.Value(i + p2, (highestHigh + lowestLow) / 2);
                    }
                }

                res.AddField(convLine);
                res.AddField(baseLine);
                res.AddField(leadingSpanA);
                res.AddField(leadingSpanB);
                res.AddField(lagginSpan);

                return res;
            }
        }
    }
}
