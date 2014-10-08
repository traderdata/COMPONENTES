using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ModulusFE.ChartElementProperties;
using ModulusFE.PaintObjects;
using ModulusFE.Tasdk;

#if SILVERLIGHT
using ModulusFE.SL.Utils;
#endif

namespace ModulusFE.Indicators
{
    /// <summary>
    /// Exception type used when a error comes in indicator calculation
    /// </summary>
    public class IndicatorException : Exception
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="indicator">Reference to indicator with error</param>
        public IndicatorException(string message, Indicator indicator)
            : base(message)
        {
            Indicator = indicator;
        }

        /// <summary>
        /// Reference to indicator that had an error
        /// </summary>
        public Indicator Indicator { get; private set; }
    }

    /// <summary>
    /// Base class for all indicators
    /// </summary>
    [CLSCompliant(true)]
    public partial class Indicator : Series, IChartElementPropertyAble
    {
        internal bool _toBeAdded; //indicates that the indicator is going to be added
        internal bool _calculateResult;
        internal bool _calculated;
        internal bool _dialogShown;
        internal bool _isTwin;
        internal bool _dialogNeeded = true; //used for custom indicators
        internal PaintObjectsManager<Line> _lines = new PaintObjectsManager<Line>();

        internal bool _ignoreErrors;
        internal bool _inputError;
        internal bool _calculating;
        internal bool _showDialog;
        internal IndicatorType _indicatorType;
        internal IndicatorDialog _dialog;
        internal List<object> _params = new List<object>();
        internal List<StockChartX_IndicatorsParameters.IndicatorParameter> _parameters;

        private readonly SortedList<string, IndicatorType> _MATypes =
          new SortedList<string, IndicatorType>
        {
          {"Simple", IndicatorType.SimpleMovingAverage},
          {"Exponential", IndicatorType.ExponentialMovingAverage},
          {"Time Series", IndicatorType.TimeSeriesMovingAverage},
          {"Triangular", IndicatorType.TriangularMovingAverage},
          {"Variable", IndicatorType.VariableMovingAverage},
          {"VIDYA", IndicatorType.VIDYA},
          {"Weighted", IndicatorType.WeightedMovingAverage},
        };

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public Indicator(string name, ChartPanel chartPanel)
            : base(name, SeriesTypeEnum.stIndicator, SeriesTypeOHLC.Unknown, chartPanel)
        {
            _chartPanel._chartX._dataManager.AddSeries(name, SeriesTypeOHLC.Unknown);
            _chartPanel._chartX._dataManager.BindSeries(this);
        }

        /// <summary>
        /// This property is set when adding the indicator. If is true then user will be asked via a dialog for parameters.
        /// if false, then parameters must be set via code.
        /// </summary>
        public bool UserParams { get; set; }

        /// <summary>
        /// Force the series to be painted as an oscilator (histogram)
        /// </summary>
        public bool ForceOscilatorPaint { get; set; }

        ///<summary>
        /// Get supported indicator parameters
        ///</summary>
        public IList<StockChartX_IndicatorsParameters.IndicatorParameter> IndicatorParams
        { get { return _parameters; } }



        ///<summary>
        /// Get or sets the series to be painted as a linear chart. 
        /// This property is ignored when <see cref="ForceOscilatorPaint"/> = true;
        /// If set to TRUE the negatuve values won't be considered for chart to be painted as a histogram.
        /// If set to FALSE any negative value present in series will make current series to be painted as a historamm.
        ///</summary>
        public bool ForceLinearChart { get; set; }

        /// <summary>
        /// Returns the value of a parameter that indicator uses internal
        /// </summary>
        /// <param name="parameterIndex">Parameter index</param>
        /// <returns>Parameter Value</returns>
        public object GetParameterValue(int parameterIndex)
        {
            Debug.Assert(_parameters != null);
            if (parameterIndex < 0 || parameterIndex >= _parameters.Count)
                throw new ArgumentOutOfRangeException("parameterIndex");
            object r = _params[parameterIndex];
            if (r.GetType() == typeof(IndicatorType))
                r = (int)(IndicatorType)r;
            return r;
        }

        /// <summary>
        /// Gets whether current indicator is just a twin for another main one.
        /// </summary>
        public bool IsTwin
        {
            get { return _isTwin; }
        }

        /// <summary>
        /// Get's the Twin's parentIndicator (if it has one)
        /// </summary>
        public Indicator TwinsParentIndicator
        {
            get
            {
                if (_isTwin)
                {
                    TwinIndicator twin = this as TwinIndicator;
                    if (twin != null)
                        return twin._indicatorParent;
                }
                return null;
            }
        }


        ///<summary>
        /// When adding indicator by programm use this function to set indicators' parameters
        ///</summary>
        ///<param name="parameterIndex">Parameter Index</param>
        ///<param name="value">Value</param>
        ///<exception cref="ArgumentOutOfRangeException"></exception>
        public void SetParameterValue(int parameterIndex, object value)
        {
            if (parameterIndex < 0 || parameterIndex >= _parameters.Count)
            {
                throw new ArgumentOutOfRangeException("parameterIndex");
            }

            if (value.GetType() == typeof(IndicatorType))
            {
                value = (IndicatorType)value;
            }

            _params[parameterIndex] = value;
            // mark indicator as non calculated
            _calculated = false;
            // also mark ChrtPanel as dirty
            _chartPanel._recalc = true;
        }

        /// <summary>
        /// Forces the dialog with indicators' properties to be shown
        /// </summary>
        public void ShowParametersDialog()
        {
            if (this.IsTwin)
                TwinsParentIndicator.ShowParametersDialog(this);
            else
                ShowParametersDialog(this);
        }

        internal void ShowParametersDialog(Indicator styleIndicator)
        {
            _showDialog = true;
            _dialogNeeded = true; //cause of custom indicators, force dialog to be shown
#if SILVERLIGHT
            _dialog = new IndicatorDialog
            {
                Indicator = this,
                StyleIndicator = styleIndicator
            };
#endif
            GetUserInput(styleIndicator, new Func<bool>[] { TrueAction, FalseAction });
        }

        internal override void Init()
        {
            base.Init();

            _dialogShown = false;
            _inputError = false;
            _calculating = false;
            _calculated = false;
            _shareScale = true;

            if (_indicatorType != IndicatorType.CustomIndicator)
            {
                _parameters = StockChartX_IndicatorsParameters.GetIndicatorParameters(_indicatorType);
                _params = new List<object>(new object[_parameters.Count]);
            }
            else
            {
                _parameters = new List<StockChartX_IndicatorsParameters.IndicatorParameter>();
                _params = new List<object>();
            }
        }

        /// <summary>
        /// Indicator error types
        /// </summary>
        [Flags]
        protected enum IndicatorErrorType : short
        {
            /// <summary>
            /// Indicator has circular reference
            /// </summary>
            CircularReference = 0x01,
            /// <summary>
            /// Indicator must be removed
            /// </summary>
            RemoveIndicator = 0x02,
            /// <summary>
            /// Throw an exception
            /// </summary>
            ThrowError = 0x04,
            /// <summary>
            /// Show an error message
            /// </summary>
            ShowErrorMessage = 0x08
        }

        /// <summary>
        /// Functions that executes when indicator is canceled
        /// </summary>
        /// <returns></returns>
        protected bool FalseAction()
        {
            return _calculateResult = false;
        }

        /// <summary>
        /// Method that executes after calculation on indicator is done.
        /// </summary>
        /// <returns></returns>
        protected bool PostCalculate()
        {
            _calculated = true;
            _chartPanel._chartX._updatingIndicator = false;

            return true;
        }

        /// <summary>
        /// An overradable method used by children classes.
        /// </summary>
        /// <returns></returns>
        protected virtual bool TrueAction() { throw new NotImplementedException(); }

        internal Action _postCalculateAction = () => { };

        /// <summary>
        /// A custom callback method that would let use another panel where indicator will be placed.
        /// Usefull in case when we want to place indicator in same panel as it source indicator
        /// </summary>
        public Func<Indicator, IEnumerable<StockChartX_IndicatorsParameters.IndicatorParameter>, ChartPanel> GetParentPanel;

        internal bool Calculate()
        {
            if (_calculated)
            {
                _postCalculateAction();
                return _calculateResult = true;
            }
            /*
              1. Validate the indicator parameters (if any)
              2. Validate available inputs
              3. Gather the inputs into a TA-SDK recordset
              4. Calculate the indicator
              5. If there is only one output, store the data
                 in the data_master array of this series. 
                 If there are two or more outputs, create new 
                 CSeriesStandard for each additional output
            */

            // Get input from user
            GetUserInput(this, new Func<bool>[]
										{
											() =>
												{
													if (GetParentPanel != null)
													{
														ChartPanel newOwner = GetParentPanel(this, _parameters);
														if (newOwner != null)
														{
															if (newOwner != _chartPanel)
															{
																MoveToPanel(newOwner);
																_chartPanel._chartX.Update();
															}
														}
													}

													return TrueAction();
												}, 
											FalseAction
										});

            return true;
        }

        /// <summary>
        /// Mark as true the recycled flag for linked series for current indicator
        /// </summary>
        protected void RecycleLinkedSeries()
        {
            foreach (ChartPanel chartPanel in _chartPanel._panelsContainer.Panels)
            {
                foreach (Series series in chartPanel.AllSeriesCollection)
                {
                    foreach (Series series1 in series._linkedSeries)
                    {
                        series1._recycleFlag = true;
                    }
                }
            }
        }

        /// <summary>
        /// returns an integer value for indicator parameter
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int ParamInt(int index)
        {
            return Convert.ToInt32(_params[index]);
        }

        /// <summary>
        /// returns an double value for indicator parameter
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public double ParamDbl(int index)
        {
            return Convert.ToDouble(_params[index]);
        }

        /// <summary>
        /// returns an string value for indicator parameter
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string ParamStr(int index)
        {
            return Convert.ToString(_params[index]);
        }

        /// <summary>
        /// Functions that takes care of user input dialog
        /// </summary>
        /// <param name="actions"></param>
        /// <returns></returns>
        protected bool GetUserInput(Indicator styleIndicator, params Func<bool>[] actions)
        {
            _chartPanel._chartX._updatingIndicator = true;

            Func<bool> trueResultAction = actions.Length > 0 ? actions[0] : () => true;
            Func<bool> falseResultAction = actions.Length > 1 ? actions[1] : () => false;

            // If this dialog was originally made programmatically,
            // but the client program is asking to show the dialog,
            // then convert this indicator into a user-param indicator.
            if (_showDialog)
            {
                UserParams = true;
            }

            if (_dialogShown || this is TwinIndicator)
            {
                if (HasCircularReference(this) && !_inputError)
                {
                    ProcessError(FullName + " has a circular reference to another indicator.", IndicatorErrorType.CircularReference);
                    falseResultAction();
                    return false;
                }
                if (!_inputError)
                    trueResultAction();
                return true;
            }

            /*
        First check to see if we have inputs already.
        If we do (paramStr[0] != ""), then return true.
      
        If we don't have inputs or the dialog is requested
        for some reason anyway, then show the dialog and 
        return false. The dialog will call Calculate()
        again when inputs have been collected.

        If the Calculate() function throws an error,
        ProcessError will call this function.
        */
            if (_parameters.Count == 0)
                throw new ArgumentException("Indicator " + FullName + " must have at least one parameter.");

            bool firstTime = (string.IsNullOrEmpty(ParamStr(0)));
            if (_indicatorType == IndicatorType.CustomIndicator)
            {
                if (!_dialogNeeded || !UserParams)
                {
                    trueResultAction();
#if SILVERLIGHT
                    if (!_dialogShown)
                        _postCalculateAction();
#endif
                    return true;
                }
                _dialogNeeded = false;
            }
            else
            {
                if (ParamStr(0).Length > 0 && !_inputError && !_showDialog)
                {
                    trueResultAction();
#if SILVERLIGHT
                    if (!_dialogShown)
                        _postCalculateAction();
#endif
                    return true; //No need to get user input
                }
            }

            _chartPanel._chartX._locked = true;

#if WPF
      _dialog = new IndicatorDialog
                  {
                    Owner =_chartPanel._chartX.OwnerWindow,
                    Indicator = this,
                    StyleIndicator = styleIndicator,
                    stackPanelBackground = { Background = _chartPanel._chartX.IndicatorDialogBackground },
                    Tag = actions,
                    LabelForeground = _chartPanel._chartX.IndicatorDialogLabelForeground,
                    LabelFontSize = _chartPanel._chartX.IndicatorDialogLabelFontSize,
                  };
      _dialog.NeedDescription += DialogOnNeedDescription;
#endif
#if SILVERLIGHT
            //in SL the dialog is created in ChartPanel_CalcIndicators where we simulate modal behavior
            //in a non-modal enviroment
            _dialog.Tag = actions;
#endif

            #region Set Controls Values

            ComboBox cmb = null;

            int n;
            for (n = 0; n < _params.Count; n++)
            {
                TextBox tb;
                switch (_parameters[n].ParameterType)
                {
                    case ParameterType.ptSymbol:
                        cmb = _dialog.GetComboBox(n);
                        EnumSymbols(cmb);
                        SetComboDefault(cmb, ParamStr(n));
                        _dialog.ShowHidePanel(n, false, false);
                        break;
                    case ParameterType.ptSource:
                    case ParameterType.ptSource1:
                    case ParameterType.ptSource2:
                    case ParameterType.ptSource3:
                    case ParameterType.ptVolume:
                        cmb = _dialog.GetComboBox(n);
                        EnumSeries(cmb);
                        SetComboDefault(cmb, ParamStr(n));
                        _dialog.ShowHidePanel(n, false, false);
                        break;
                    case ParameterType.ptPointsOrPercent:
                        cmb = _dialog.GetComboBox(n);
                        cmb.Items.Clear();
                        cmb.Items.Add("Points");
                        cmb.Items.Add("Percent");
                        cmb.SelectedIndex = 0;
                        SetComboDefault(cmb, ParamInt(n) == 1 ? "Points" : "Percent");
                        _dialog.ShowHidePanel(n, false, false);
                        break;
                    case ParameterType.ptMAType:
                    case ParameterType.ptPctDMAType:
                        cmb = _dialog.GetComboBox(n);
                        EnumMATypes(cmb);
                        SetComboDefault(cmb, MATypeToStr((IndicatorType)ParamInt(n)));
                        SetMAComboSel(cmb, (IndicatorType)_parameters[n].DefaultValue);
                        _dialog.ShowHidePanel(n, false, false);
                        break;
                    case ParameterType.ptBarHistory:
                    case ParameterType.ptPeriods:
                    case ParameterType.ptLevels:
                    case ParameterType.ptCycle1:
                    case ParameterType.ptCycle2:
                    case ParameterType.ptCycle3:
                    case ParameterType.ptShortTerm:
                    case ParameterType.ptLongTerm:
                    case ParameterType.ptPctKPeriods:
                    case ParameterType.ptPctDPeriods:
                    case ParameterType.ptPctKSlowing:
                    case ParameterType.ptPctKSmooth:
                    case ParameterType.ptPctDSmooth:
                    case ParameterType.ptPctDDblSmooth:
                    case ParameterType.ptPctKDblSmooth:
                    case ParameterType.ptShortCycle:
                    case ParameterType.ptLongCycle:
                    case ParameterType.ptStandardDeviations:
                    case ParameterType.ptRateOfChange:
                    case ParameterType.ptSignalPeriods:
                        tb = _dialog.GetTextBox(n);
                        tb.Text = ParamInt(n) == 0 ? Convert.ToString(_parameters[n].DefaultValue) : ParamInt(n).ToString();
                        _dialog.ShowHidePanel(n, false, true);
                        break;
                    case ParameterType.ptMinTickVal:
                    case ParameterType.ptR2Scale:
                    case ParameterType.ptMinAF:
                    case ParameterType.ptMaxAF:
                    case ParameterType.ptShift:
                    case ParameterType.ptFactor:
                    case ParameterType.ptLimitMoveValue:
                        tb = _dialog.GetTextBox(n);
                        tb.Text = ParamDbl(n) == 0.0 ? Convert.ToString(_parameters[n].DefaultValue) : ParamDbl(n).ToString();
                        _dialog.ShowHidePanel(n, false, true);
                        break;
                }

                if (firstTime)
                {
                    if (_parameters[n].ParameterType == ParameterType.ptVolume && cmb != null)
                    {
                        foreach (var item in cmb.Items)
                        {
                            string s = item.ToString();
                            if (s.IndexOf("vol", StringComparison.CurrentCultureIgnoreCase) == -1) continue;

                            cmb.SelectedItem = item;
                            break;
                        }
                    }
                }

                //Description
                TextBlock tbl = _dialog.GetTextBlock(n);
                tbl.Text = _parameters[n].Name;
            }
            //_dialog.Height = n * 25 + 85;

            //hide other panels
            for (; n < Constants.MaxIndicatorParamCount; n++)
                _dialog.ShowHidePanel(n, true, true);

            _inputError = false;
            _dialog.Title = FullName;

            _dialog.OnOk += Dialog_OnOk_GetUserInput;
            _dialog.OnCancel += Dialog_OnCancel_GetUserInput;
#if SILVERLIGHT
            _dialog.OnDelete += Dialog_OnDelete_GetUserInput;
#endif
            #endregion

            return _dialog.ShowDialog() == true;
        }

        private void DialogOnNeedDescription(IndicatorDialog sender, int index, out string description)
        {
            description = GetParamDescription(index);
        }

        /// <summary>
        /// Function that process the error
        /// </summary>
        /// <param name="Description"></param>
        /// <param name="errorType"></param>
        protected void ProcessError(string Description, IndicatorErrorType errorType)
        {
            if (_ignoreErrors)
                return;

            if (_chartPanel._chartX._dialogErrorShown) return;
            _chartPanel._chartX._dialogErrorShown = true;
            _chartPanel._chartX.FireOnDialogShown();

            string error = Description + Environment.NewLine +
                           "Click OK to fix the problem or click" + Environment.NewLine +
                           "Cancel to remove " + _name;

            if ((errorType & IndicatorErrorType.CircularReference) == IndicatorErrorType.CircularReference)
                if (_dialog != null)
                    _dialog.CanClose = false;

            if ((errorType & IndicatorErrorType.RemoveIndicator) == IndicatorErrorType.RemoveIndicator)
            {
                _recycleFlag = true;
                RecycleLinkedSeries();
                _chartPanel._chartX._dialogErrorShown = false;
                return;
            }

            if (UserParams)
            {
                MessageBoxResult mr =
                  _dialog == null
                    ? MessageBoxResult.Cancel
                    : MessageBox.Show(
#if WPF
						_chartPanel._chartX.OwnerWindow,
#endif
error,
                                      "Error:",
                                      MessageBoxButton.OKCancel
#if WPF
                              , MessageBoxImage.Warning
#endif
);

                _inputError = true;
                if (mr == MessageBoxResult.OK)
                {
                    GetUserInput(this);
                }
                else
                {

                    if (_dialog != null)
                    {
                        _dialog._userCanceled = true;
                    }
                    _recycleFlag = true;
                    RecycleLinkedSeries();
                }
            }
            else
            {
                throw new IndicatorException(Description, this);
            }

            _chartPanel._chartX._dialogErrorShown = false;
        }

        internal event EventHandler DialogClosed = delegate { };
        private void Dialog_OnCancel_GetUserInput(object sender, EventArgs e)
        {
            _showDialog = false;

            Func<bool>[] actions = (Func<bool>[])((IndicatorDialog)sender).Tag;
            if (actions.Length > 1)
                actions[1]();

            DialogClosed(this, EventArgs.Empty);
        }

        private void Dialog_OnOk_GetUserInput(object sender, EventArgs e)
        {
            if (_showDialog)
            {
                _chartPanel._recalc = true;
                _calculated = false;
                _dialogShown = true;
                Calculate();
                _dialogShown = false;
                Paint();
                foreach (Series linkedSeries in _linkedSeries)
                {
                    linkedSeries.Paint();
                }
                HideSelection();
            }
            _showDialog = false;

            Func<bool>[] actions = (Func<bool>[])((IndicatorDialog)sender).Tag;
            if (actions.Length > 0)
                actions[0]();

            DialogClosed(this, EventArgs.Empty);
        }

        private void Dialog_OnDelete_GetUserInput(object sender, EventArgs e)
        {
            _chartPanel._chartX._locked = false;
            _chartPanel._chartX.Update();
        }

        internal bool HasCircularReference(Indicator indicator)
        {
            int p1;

            for (p1 = 0; p1 < indicator._params.Count; p1++)
            {
                if (indicator._indicatorType == IndicatorType.Unknown) continue; //usually TwinIndicator, ignore

                if (indicator._params[p1].GetType() != Constants.TypeString) continue;
                string value1 = indicator._params[p1].ToString();
                if (value1.Length == 0) continue;
                Series nextIndicator = _chartPanel._chartX.GetSeriesByName(value1);
                if (nextIndicator == null || nextIndicator == this) continue;
                if (_linkedSeries.Contains(nextIndicator))
                {
                    _params[p1] = "";
                    return true;
                }
                int p2;
                Indicator ind = nextIndicator as Indicator;
                if (ind == null) continue;
                for (p2 = 0; p2 < ind._params.Count; p2++)
                {
                    if (ind._params[p2].GetType() != Constants.TypeString) continue;
                    string value2 = ind._params[p2].ToString();
                    if (!Utils.StrICmp(FullName, value2)) continue;
                    _params[p1] = "";
                    return true;
                }
                return HasCircularReference(ind);
            }
            return false;
        }

        internal bool EnsureField(Field field, string name)
        {
            if (field == null)
            {
                ProcessError("Missing source field " + name + " for indicator " + FullName, IndicatorErrorType.ShowErrorMessage);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Update linked indicators
        /// </summary>
        /// <param name="indicator"></param>
        //internal void UpdateIndicator(Indicator indicator)
        //{
        //    foreach (Indicator link in
        //      _chartPanel._panelsContainer.Panels
        //      .SelectMany(chartPanel => (from link in chartPanel.IndicatorsCollection
        //                                 from param in link._params
        //                                 .Where(param => param.GetType() == Constants.TypeString)
        //                                 .Where(param => param.ToString() == _name)
        //                                 select link)))
        //    {
        //        link.Calculate();
        //        link.UpdateIndicator(link);
        //    }
        //}

        ///<summary>
        /// Sets the selection in a given ComboBox for a given IndicatorType
        ///</summary>
        ///<param name="comboBox"></param>
        ///<param name="paramDef"></param>
        public void SetMAComboSel(ComboBox comboBox, IndicatorType paramDef)
        {
            int index = _MATypes.IndexOfValue(paramDef);
            if (index < comboBox.Items.Count)
                comboBox.SelectedIndex = index;
        }

        ///<summary>
        /// Fills a ComboBox with availe Moving Average types
        ///</summary>
        ///<param name="comboBox"></param>
        public void EnumMATypes(ComboBox comboBox)
        {
            comboBox.Items.Clear();
            foreach (string key in _MATypes.Keys)
            {
                comboBox.Items.Add(key);
            }
            comboBox.SelectedIndex = 0;
        }

        ///<summary>
        /// Gets the selected Moving Average type
        ///</summary>
        ///<param name="comboBox"></param>
        ///<returns></returns>
        public IndicatorType GetMAType(ComboBox comboBox)
        {
            Debug.Assert(comboBox.Items.Count > 0);
            return _MATypes.Values[comboBox.SelectedIndex];
        }

        ///<summary>
        /// Converts Moving Average type to a readable text
        ///</summary>
        ///<param name="maType"></param>
        ///<returns></returns>
        public string MATypeToStr(IndicatorType maType)
        {
            int index = _MATypes.IndexOfValue(maType);
            Debug.Assert(index < _MATypes.Count);
            return _MATypes.Keys[index];
        }

        ///<summary>
        /// Sets the select item in a ComboBox given a value
        ///</summary>
        ///<param name="comboBox"></param>
        ///<param name="item"></param>
        public static void SetComboDefault(ComboBox comboBox, string item)
        {
            int index = -1;
            int i = 0;
            foreach (var o in comboBox.Items)
            {
                if (o.ToString() == item)
                {
                    index = i;
                    break;
                }
                i++;
            }
            if (index != -1)
                comboBox.SelectedIndex = index;
        }

        ///<summary>
        /// Fill ComboBox with all the series that are currently in chart
        ///</summary>
        ///<param name="comboBox"></param>
        public void EnumSeries(ComboBox comboBox)
        {
            comboBox.Items.Clear();
            foreach (Series series in
              _chartPanel._panelsContainer.Panels
              .SelectMany(chartPanel => chartPanel.AllSeriesCollection
                .Where(series => !Utils.StrICmp(FullName, series.FullName) && series.RecordCount > 0)))
            {
                comboBox.Items.Add(series.FullName.ToUpper());
            }
            if (comboBox.Items.Count > 0)
                comboBox.SelectedIndex = 0;
        }

        internal Indicator EnsureSeries(string name)
        {
            Indicator series = (Indicator)_chartPanel._chartX.GetSeriesByName(name);

            if (series == null)
            {
                series = new TwinIndicator(name, _chartPanel)
                           {
                               _seriesType = _seriesType,
                               _indicatorParent = this,
                               _selectable = _selectable,
                               _strokeColor = _strokeColor,
                           };
                _chartPanel.AddSeries(series);
                _linkedSeries.Add(series);
            }
            series.Clear();
            return series;
        }

        ///<summary>
        /// Fill a ComboBox with all available symbols in the chart
        ///</summary>
        ///<param name="comboBox"></param>
        public void EnumSymbols(ComboBox comboBox)
        {
            comboBox.Items.Clear();
            Dictionary<string, bool> symbols = new Dictionary<string, bool>();
            foreach (Series series in
              _chartPanel._panelsContainer.Panels.SelectMany(chartPanel => chartPanel.SeriesCollection.Where(series => !symbols.ContainsKey(series.Name))))
            {
                comboBox.Items.Add(series.Name.ToUpper());
                symbols[series.Name] = true;
            }
            if (comboBox.Items.Count > 0)
                comboBox.SelectedIndex = 0;
        }

        ///<summary>
        /// Returns the description of a given parameter type
        ///</summary>
        ///<param name="parameterType"></param>
        ///<returns></returns>
        public static string GetParamDescription(ParameterType parameterType)
        {
            switch (parameterType)
            {
                case ParameterType.ptSymbol:
                    return "A Symbol is a group of high, low and close series that are displayed as a candle or bar chart";
                case ParameterType.ptSource:
                    return "Calculations are based upon the source field. A source field can be the open, high, low, close, volume or any other available series";
                case ParameterType.ptVolume:
                    return "This indicator requires a volume field for calculation";
                case ParameterType.ptSource1:
                    return "Calculations are based upon the source field. A source field can be the open, high, low, close, volume or any other available series";
                case ParameterType.ptSource2:
                    return "The second source field. A source field can be the open, high, low, close, volume or any other available series";
                case ParameterType.ptSource3:
                    return "The third source field. A source field can be the open, high, low, close, volume or any other available series";
                case ParameterType.ptCycle1:
                    return "The first cycle for the multi-step indicator calculations";
                case ParameterType.ptCycle2:
                    return "The second cycle for the multi-step indicator calculations";
                case ParameterType.ptCycle3:
                    return "The third cycle for the multi-step indicator calculations";
                case ParameterType.ptLongTerm:
                    return "The long term smoothing parameter";
                case ParameterType.ptShortTerm:
                    return "The short term smoothing parameter";
                case ParameterType.ptLongCycle:
                    return "The long cycle smoothing parameter";
                case ParameterType.ptShortCycle:
                    return "The short cycle smoothing parameter";
                case ParameterType.ptLevels:
                    return "The level of smoothing periods to use in this calculation";
                case ParameterType.ptPeriods:
                    return "The number of bars to use for calculating the indicator";
                case ParameterType.ptRateOfChange:
                    return "Rate of change is expressed as momentum / close(t-n) * 100";
                case ParameterType.ptPctKSlowing:
                    return "Controls smoothing of %K, where 1 is a fast stochastic and 3 is a slow stochastic";
                case ParameterType.ptPctKPeriods:
                    return "Number of bars used in the stochastic calculation";
                case ParameterType.ptPctKSmooth:
                    return "Number of bars used in the stochastic smoothing";
                case ParameterType.ptPctDSmooth:
                    return "Number of bars used in the stochastic double smoothing";
                case ParameterType.ptPctDDblSmooth:
                    return "Controls the smoothing of %D";
                case ParameterType.ptPctKDblSmooth:
                    return "Controls the smoothing of %K";
                case ParameterType.ptPctDPeriods:
                    return "Number of bars used for calculating the average of %D";
                case ParameterType.ptStandardDeviations:
                    return "A statistic used as a measure of the dispersion or variation in a distribution";
                case ParameterType.ptMinTickVal:
                    return "The dollar value of the move of the smallest tick";
                case ParameterType.ptMinAF:
                    return "Minimum acceleration factor";
                case ParameterType.ptMaxAF:
                    return "Maximum acceleration factor";
                case ParameterType.ptShift:
                    return "The percent of shift to move a series above or below another indicator";
                case ParameterType.ptPointsOrPercent:
                    return "Determines the indicator output scale in points or percent";
                case ParameterType.ptMAType:
                    return "The moving average type used for smoothing the indicator";
                case ParameterType.ptPctDMAType:
                    return "The %D moving average type used for smoothing the indicator";
                case ParameterType.ptR2Scale:
                    return "The r-squared (coefficient of determination) scale";
                case ParameterType.ptSignalPeriods:
                    return "The number of bars used for the MACD signal series";
                case ParameterType.ptLimitMoveValue:
                    return "The point value of a limit move (futures only)";
                case ParameterType.ptBarHistory:
                    return "The number of bars to use in the historical calculation (e.g. 365)";
                case ParameterType.ptFactor:
                    return "Keltner Factor";
                default:
                    return "";
            }
        }

        internal string GetParamDescription(int index)
        {
            return GetParamDescription(_parameters[index].ParameterType);
        }

        /// <summary>
        /// Returns parameter name by parameter type
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Parameter Name</returns>
        public static string GetParamName(ParameterType type)
        {
            switch (type)
            {
                case ParameterType.ptMAType:
                    return "MA Type";
                case ParameterType.ptPctDMAType:
                    return "%D MA Type";
                case ParameterType.ptSymbol:
                    return "Symbol";
                case ParameterType.ptVolume:
                    return "Volume";
                case ParameterType.ptSource:
                    return "Source";
                case ParameterType.ptSource1:
                    return "Source 1";
                case ParameterType.ptSource2:
                    return "Source 2";
                case ParameterType.ptSource3:
                    return "Source 3";
                case ParameterType.ptPointsOrPercent:
                    return "Points or Percent";
                case ParameterType.ptLevels:
                    return "Levels";
                case ParameterType.ptPeriods:
                    return "Periods";
                case ParameterType.ptPeriods1:
                    return "Period 1";
                case ParameterType.ptPeriods2:
                    return "Period 2";
                case ParameterType.ptPeriods3:
                    return "Period 3";
                case ParameterType.ptCycle1:
                    return "Cycle 1";
                case ParameterType.ptCycle2:
                    return "Cycle 2";
                case ParameterType.ptCycle3:
                    return "Cycle 3";
                case ParameterType.ptShortTerm:
                    return "Short Term";
                case ParameterType.ptLongTerm:
                    return "Long Term";
                case ParameterType.ptRateOfChange:
                    return "Rate of Chg";
                case ParameterType.ptPctKPeriods:
                    return "%K Periods";
                case ParameterType.ptPctKSlowing:
                    return "%K Slowing";
                case ParameterType.ptPctKSmooth:
                    return "%K Smooth";
                case ParameterType.ptPctDSmooth:
                    return "%D Smooth";
                case ParameterType.ptPctDDblSmooth:
                    return "%D Dbl Smooth";
                case ParameterType.ptPctKDblSmooth:
                    return "%K Dbl Smooth";
                case ParameterType.ptPctDPeriods:
                    return "%D Periods";
                case ParameterType.ptShortCycle:
                    return "Short Cycle";
                case ParameterType.ptLongCycle:
                    return "Long Cycle";
                case ParameterType.ptStandardDeviations:
                    return "Standard Dev";
                case ParameterType.ptR2Scale:
                    return "R2 Scale";
                case ParameterType.ptMinTickVal:
                    return "Minimum Tick Value";
                case ParameterType.ptMinAF:
                    return "Min AF";
                case ParameterType.ptMaxAF:
                    return "Max AF";
                case ParameterType.ptShift:
                    return "Shift";
                case ParameterType.ptFactor:
                    return "Factor";
                case ParameterType.ptSignalPeriods:
                    return "Signal Periods";
                case ParameterType.ptLimitMoveValue:
                    return "Limit Move Value";
                case ParameterType.ptBarHistory:
                    return "Bar History";
                default:
                    return "";
            }
        }

        /// <summary>
        /// Gets the indicator type
        /// </summary>
        public IndicatorType IndicatorType
        {
            get { return _indicatorType; }
        }

        internal Field SeriesToField(string name, string seriesName, int length)
        {
            Series series = _chartPanel._chartX.GetSeriesByName(seriesName);

            if (series == null)
            {
                if (!UserParams)
                    throw new IndicatorException("Invalid source field " + name + " for indicator " + _name, this);
                return null;
            }

            // Ensure the indicator has been calculated
            Indicator indicator = series as Indicator;
            if (indicator != null && !_calculating)
            {
                _calculating = true;
                if (!indicator._calculated)
                    indicator.Calculate();
                _calculating = false;
            }

            double firstVal = 0; // Get first value that isn't a null

            for (int i = 0; i < series.RecordCount; i++)
                if (series[i].Value.HasValue && series[i].Value.Value != 0)
                {
                    firstVal = series[i].Value.Value;
                    break;
                }

            Field ret = new Field(length, name);

            for (int i = 0; i < length; i++)
            {
                ret.Value(i + 1, !series[i].Value.HasValue ? firstVal : series[i].Value);
            }

            return ret;
        }

        internal void SetUserInput()
        {
            //assume we have no input error
            _inputError = false;

            for (int i = 0; i < _parameters.Count; i++)
            {
                ComboBox cmb;
                TextBox txt;
                switch (_parameters[i].ParameterType)
                {
                    case ParameterType.ptSymbol:
                    case ParameterType.ptSource:
                    case ParameterType.ptSource1:
                    case ParameterType.ptSource2:
                    case ParameterType.ptSource3:
                    case ParameterType.ptVolume:
                        cmb = _dialog.GetComboBox(i);
                        Debug.Assert(cmb != null);
                        Debug.Assert(cmb.SelectedItem != null);
                        _params[i] = cmb.SelectedItem.ToString();
                        break;
                    case ParameterType.ptPointsOrPercent:
                        cmb = _dialog.GetComboBox(i);
                        Debug.Assert(cmb != null);
                        Debug.Assert(cmb.SelectedItem != null);
                        _params[i] = Utils.StrICmp(cmb.SelectedItem.ToString(), "Points") ? 1 : 2;
                        break;
                    case ParameterType.ptMAType:
                    case ParameterType.ptPctDMAType:
                        cmb = _dialog.GetComboBox(i);
                        Debug.Assert(cmb != null);
                        Debug.Assert(cmb.SelectedItem != null);
                        _params[i] = GetMAType(cmb);
                        break;
                    case ParameterType.ptBarHistory:
                    case ParameterType.ptPeriods:
                    case ParameterType.ptLevels:
                    case ParameterType.ptCycle1:
                    case ParameterType.ptCycle2:
                    case ParameterType.ptCycle3:
                    case ParameterType.ptShortTerm:
                    case ParameterType.ptLongTerm:
                    case ParameterType.ptPctKPeriods:
                    case ParameterType.ptPctDPeriods:
                    case ParameterType.ptPctKSlowing:
                    case ParameterType.ptPctKSmooth:
                    case ParameterType.ptPctDSmooth:
                    case ParameterType.ptPctDDblSmooth:
                    case ParameterType.ptPctKDblSmooth:
                    case ParameterType.ptShortCycle:
                    case ParameterType.ptLongCycle:
                    case ParameterType.ptStandardDeviations:
                    case ParameterType.ptRateOfChange:
                    case ParameterType.ptSignalPeriods:
                        txt = _dialog.GetTextBox(i);
                        Debug.Assert(txt != null);
                        _params[i] = Convert.ToInt32(txt.Text);
                        break;
                    case ParameterType.ptMinTickVal:
                    case ParameterType.ptR2Scale:
                    case ParameterType.ptMinAF:
                    case ParameterType.ptMaxAF:
                    case ParameterType.ptShift:
                    case ParameterType.ptFactor:
                    case ParameterType.ptLimitMoveValue:
                        txt = _dialog.GetTextBox(i);
                        Debug.Assert(txt != null);
                        _params[i] = Convert.ToDouble(txt.Text);
                        break;
                }
            }
            _chartPanel._chartX.InvalidateIndicators();

            _dialogShown = true;
            _chartPanel._chartX._locked = true;

            //Calculate();
            TrueAction();

            if (!_inputError)
            {
                _chartPanel._chartX._locked = false;
                _chartPanel._chartX.InvalidateIndicators();
                _calculated = true;
            }

            //_inputError = false;
            _dialogShown = false;

        }

        internal void OnCancelDialog()
        {
            if (!_calculated && RecordCount == 0)
            {
                ProcessError("", IndicatorErrorType.RemoveIndicator);
            }
        }

        internal override void SetStrokeThickness()
        {
            _lines.Do(line => line.StrokeThickness = _strokeThickness);
            Paths.ForEach(_ => _.StrokeThickness = _strokeThickness);
        }

        internal override void SetStrokeColor()
        {
            SolidColorBrush brush = new SolidColorBrush(_strokeColor);
            _lines.Do(line => line.Stroke = brush);
            Paths.ForEach(_ => _.Stroke = brush);
        }

        internal override void SetStrokeType()
        {
            _lines.Do(line => Types.SetShapePattern(line._line, _strokePattern));
            Paths.ForEach(_ => Types.SetShapePattern(_, _strokePattern));
        }

        internal override void SetOpacity()
        {
            _lines.Do(line => line._line.Opacity = _opacity);
            Paths.ForEach(_ => _.Opacity = _opacity);
        }

        //    private void DrawLine(double x1, double y1, double x2, double y2, Brush strokeBrush)
        //    {
        //      if (x1 != x2 && Math.Abs(x2 - x1) < 1) return;
        //
        //      PaintObjects.Line linePO = _lines.GetPaintObject();
        //      Line line = linePO._line;
        //
        //      line.X1 = x1;
        //      line.X2 = x2;
        //      line.Y1 = y1;
        //      line.Y2 = y2;
        //      line.Stroke = strokeBrush;
        //      line.StrokeThickness = _strokeThickness;
        //      if (_indicatorType == IndicatorType.ParabolicSAR)
        //      {
        //        line.StrokeStartLineCap = PenLineCap.Round;
        //        line.StrokeEndLineCap = PenLineCap.Round;
        //        line.StrokeDashCap = PenLineCap.Round;
        //      }
        //      Types.SetLinePattern(line, _strokePattern);
        //    }
        //
        internal override void MoveToPanel(ChartPanel chartPanel)
        {
            if (_pathUp != null)
            {
                var c = _chartPanel._rootCanvas;
                c.Children.Remove(_pathUp);
                c.Children.Remove(_pathDown);
                c.Children.Remove(_pathNormal);
                _pathUp = null;
                _pathDown = null;
                _pathNormal = null;
            }

            _lines.RemoveAll();
            _chartPanel.DeleteSeries(this);
            _chartPanel = chartPanel;
            _chartPanel.AddSeries(this);

            base.MoveToPanel(chartPanel);
        }

        internal override void ShowHide()
        {
            Visibility visibility = _visible ? Visibility.Visible : Visibility.Collapsed;
            _lines.Do(line => line._line.Visibility = visibility);
            Paths.ForEach(_ => _.Visibility = visibility);
        }

        internal void Clear()
        {
            DM.ClearValues(SeriesIndex);
        }

        internal void AppendValue(DateTime timeStamp, double? value)
        {
            DM.AppendValue(SeriesIndex, timeStamp, value);
        }

        #region Implementation of IChartElementPropertyAble


        private ChartElementColorProperty propertyStroke;
        private ChartElementStrokeThicknessProperty propertyStrokeThickness;
        private ChartElementStrokeTypeProperty propertyStrokeType;
        private ChartElementOpacityProperty propertyOpacity;

        ///<summary>
        ///</summary>
        public IEnumerable<IChartElementProperty> Properties
        {
            get
            {
                propertyStroke = new ChartElementColorProperty("Stroke Color");
                propertyStroke.ValuePresenter.Value = new SolidColorBrush(_strokeColor);
                propertyStroke.SetChartElementPropertyValue
                  += presenter =>
                       {
                           StrokeColor = ((SolidColorBrush)presenter.Value).Color;
                       };
                yield return propertyStroke;

                propertyStrokeThickness = new ChartElementStrokeThicknessProperty("Stroke Thickness");
                propertyStrokeThickness.ValuePresenter.Value = StrokeThickness;
                propertyStrokeThickness.SetChartElementPropertyValue
                  += presenter =>
                       {
                           StrokeThickness = Convert.ToDouble(presenter.Value);
                       };
                yield return propertyStrokeThickness;

                propertyStrokeType = new ChartElementStrokeTypeProperty("Stroke Type");
                propertyStrokeType.ValuePresenter.Value = _strokePattern.ToString();
                propertyStrokeType.SetChartElementPropertyValue
                  += presenter =>
                       {
                           _strokePattern = (LinePattern)System.Enum.Parse(typeof(LinePattern), presenter.Value.ToString()
#if SILVERLIGHT
, true
#endif
);
                           SetStrokeType();
                       };
                yield return propertyStrokeType;

                propertyOpacity = new ChartElementOpacityProperty("Opacity");
                propertyOpacity.ValuePresenter.Value = _opacity;
                propertyOpacity.SetChartElementPropertyValue
                  += presenter =>
                       {
                           _opacity = Convert.ToDouble(presenter.Value);
                           SetOpacity();
                       };
                yield return propertyOpacity;
            }
        }

        #endregion

    }
}
