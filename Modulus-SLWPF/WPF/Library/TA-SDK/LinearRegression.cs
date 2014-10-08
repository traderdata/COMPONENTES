using System.Collections.Generic;

namespace ModulusFE
{
    namespace Tasdk
    {
        ///<summary>
        /// Linear Regression type of calculations
        ///</summary>
        internal class LinearRegression
        {
            ///<summary>
            /// Regression
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource">Field Source</param>
            ///<param name="periods">Periods</param>
            ///<returns>Recordset</returns>
            public Recordset Regression(Navigator pNav, Field pSource, int periods)
            {
                List<double?> Y = new List<double?>(); //Value
                double? dRSquared = 0;

                Recordset Results = new Recordset();
                int iRecord;

                int iRecordCount = pNav.RecordCount;

                Field Field1 = new Field(iRecordCount, "Slope");
                Field Field2 = new Field(iRecordCount, "Intercept");
                Field Field3 = new Field(iRecordCount, "Forecast");
                Field Field4 = new Field(iRecordCount, "RSquared");

                pNav.MoveFirst();

                for (iRecord = periods; iRecord < iRecordCount + 1; iRecord++)
                {

                    int X = periods; //Period
                    Y.AddRange(new double?[X + 1]); //Y.resize(X + 1);

                    //Move back n periods
                    int iPosition = iRecord;
                    pNav.Position = iRecord - periods + 1;

                    int iPeriod;
                    for (iPeriod = 1; iPeriod < periods + 1; iPeriod++)
                    {
                        double? dValue = pSource.Value(pNav.Position);
                        Y[iPeriod] = dValue;
                        pNav.MoveNext();
                    } //Period

                    //Return to original position and reset
                    pNav.Position = iPosition;
                    double? XSum = 0;
                    double? YSum = 0;
                    double? XSquaredSum = 0;
                    double? YSquaredSum = 0;
                    double? XYSum = 0;

                    //Square
                    int N;
                    for (N = 1; N < X + 1; N++)
                    {
                        XSum += N;
                        YSum += Y[N];
                        XSquaredSum += (N * N);
                        YSquaredSum += (Y[N] * Y[N]);
                        XYSum += (Y[N] * N);
                    }//N

                    N = X; //Number of periods in calculation
                    double? q1 = (XYSum - ((XSum * YSum) / N));
                    double? q2 = (XSquaredSum - ((XSum * XSum) / N));
                    double? q3 = (YSquaredSum - ((YSum * YSum) / N));

                    double? dSlope = (q1 / q2);
                    double? dIntercept = (((1 / (double)N) * YSum) - (((int)((double)N / 2)) * dSlope));
                    double? dForecast = ((N * dSlope) + dIntercept);


                    if ((q1 * q1) != 0 && (q2 * q3) != 0)
                    {
                        dRSquared = (q1 * q1) / (q2 * q3); //Coefficient of determination (R-Squared)
                    }

                    if (iRecord > periods)
                    {
                        Field1.Value(iRecord, dSlope);
                        Field2.Value(iRecord, dIntercept);
                        Field3.Value(iRecord, dForecast);
                        Field4.Value(iRecord, dRSquared);
                    }

                    pNav.MoveNext();

                }//Record

                //Append fields to CNavigator
                Results.AddField(Field1);
                Results.AddField(Field2);
                Results.AddField(Field3);
                Results.AddField(Field4);

                pNav.MoveFirst();
                return Results;
            }

            ///<summary>
            /// Time Series Forecast
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource">Field Source</param>
            ///<param name="periods">Periods</param>
            ///<returns>Recordset</returns>
            public Recordset TimeSeriesForecast(Navigator pNav, Field pSource, int periods)
            { return TimeSeriesForecast(pNav, pSource, periods, "Time Series Forecast"); }
            ///<summary>
            /// Time Series Forecast
            ///</summary>
            ///<param name="pNav">Navigator</param>
            ///<param name="pSource">Field Source</param>
            ///<param name="periods">Periods</param>
            ///<param name="Alias">Alias</param>
            ///<returns>Recordset</returns>
            public Recordset TimeSeriesForecast(Navigator pNav, Field pSource, int periods, string Alias)
            {
                Recordset Results = Regression(pNav, pSource, periods);
                Results.RenameField("Forecast", Alias);
                Results.RemoveField("Slope");
                Results.RemoveField("Intercept");
                Results.RemoveField("RSquared");

                pNav.MoveFirst();
                return Results;
            }
        }
    }
}
