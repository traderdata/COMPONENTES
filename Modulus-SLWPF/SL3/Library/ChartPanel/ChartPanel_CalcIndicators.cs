using System.Collections.Generic;
using ModulusFE.Indicators;

namespace ModulusFE
{
    public partial class ChartPanel
    {
        private IEnumerator<IndicatorDialog> _currentWindow;
        private void ProcessIndicatorDialog(IEnumerable<IndicatorDialog> windows)
        {
            _currentWindow = windows.GetEnumerator();
            Step();
        }

        private void Step()
        {
            if (_currentWindow.MoveNext())
            {
                IndicatorDialog dialog = _currentWindow.Current;

                //dialog.OnClose += (sender,e) => Step();
                //dialog.OnOk += (sender,e) => Step();
                //dialog.OnCancel += (sender,e) => Step();
                dialog.Indicator.DialogClosed += (sender, e) => Step();
                dialog.Indicator._postCalculateAction = Step;

                dialog.Indicator.Calculate();//here we decide either to show dialog or not
            }
            else if (_currentWindow != null)
            {
                _currentWindow.Dispose();
            }
        }

        ///<summary>
        /// Initializes indicator calculation
        ///</summary>
        public void CalculateIndicators()
        {
            IEnumerable<IndicatorDialog> dialogs = ProcessIndicators();
            ProcessIndicatorDialog(dialogs);
        }


        private IEnumerable<IndicatorDialog> ProcessIndicators()
        {
            bool raiseIndicatorAddEvent = false;
            bool userCanceled = true;
            string indicatorName = "";

            if (!IsHeatMap && _recalc)
            {
                //lock (_series)
                {
                    List<Indicator> toBeRemoved = new List<Indicator>();
                    List<Series> copyOfOriginal = new List<Series>();
                    copyOfOriginal.AddRange(_series);
                    foreach (Series series in copyOfOriginal)
                    {
                        Indicator indicator = series as Indicator;
                        if (indicator == null || indicator is TwinIndicator) continue;
                        if (indicator._recycleFlag)
                        {
                            toBeRemoved.Add(indicator);
                            continue;
                        }
                        _chartX._updatingIndicator = true;

                        using (IndicatorDialog dialog = new IndicatorDialog())
                        {
                            //dialog.AppRoot = _chartX.AppRoot; //!!! Required
                            indicator._dialog = dialog;
                            dialog.Indicator = indicator;
                            yield return dialog;
                            if (indicator._toBeAdded)
                            {
                                raiseIndicatorAddEvent = true;
                                userCanceled = !indicator._calculateResult;
                                indicatorName = indicator.FullName;
                            }
                            indicator._toBeAdded = false;
                            if (!indicator._calculateResult)
                                toBeRemoved.Add(indicator);
                        }
                    }
                    foreach (Indicator indicator in toBeRemoved)
                    {
                        RemoveSeries(indicator, true);
                    }
                }
            }

            PostIndicatorCalculate();
            if (_afterPaintAction != null)
            {
                _afterPaintAction();
            }

            if (raiseIndicatorAddEvent)
            {
                _chartX.FireIndicatorAddCompleted(_index, indicatorName, userCanceled);
            }
        }
    }
}