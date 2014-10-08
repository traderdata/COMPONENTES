
namespace ModulusFE
{
    /// <summary>
    /// Line pattern used to paint lines in the chart
    /// </summary>
    public enum LinePattern
    {
        ///<summary>
        /// Solid
        ///</summary>
        Solid = 1,
        /// <summary>
        /// Dash
        /// </summary>
        Dash = 2,
        /// <summary>
        /// Dots
        /// </summary>
        Dot = 3,
        /// <summary>
        /// Dash with dots
        /// </summary>
        DashDot = 4,
        /// <summary>
        /// None, line is hidden
        /// </summary>
        None = 5
    }

    /// <summary>
    /// Info panel type of positioning
    /// </summary>
    public enum InfoPanelPositionEnum
    {
        /// <summary>
        /// info panel will not be shown
        /// </summary>
        Hidden,

        /// <summary>
        /// info panel will have a fixed position
        /// </summary>
        FixedPosition,

        /// <summary>
        /// info panel will follow the mouse when mouse is held down
        /// </summary>
        FollowMouse,
    }

    /// <summary>
    /// Compression type for tick type of the chart
    /// </summary>
    public enum TickCompressionEnum
    {
        /// <summary>
        /// the ticks will be compressed based on time differences
        /// </summary>
        Time,
        /// <summary>
        /// the ticks will be compressed based on number of ticks
        /// </summary>
        Ticks
    }

    /// <summary>
    /// Type of the chart
    /// </summary>
    public enum ChartTypeEnum
    {
        /// <summary>
        /// Tick values
        /// </summary>
        Tick,
        /// <summary>
        /// OHLC
        /// </summary>
        OHLC
    }

    /// <summary>
    /// Object from cursor. Used with function GetObjectFromCursor
    /// </summary>
    public enum ObjectFromCursor
    {
        /// <summary>
        /// Left Y axis
        /// </summary>
        PanelLeftYAxis,
        /// <summary>
        /// Right Y axis
        /// </summary>
        PanelRightYAxis,
        /// <summary>
        /// Left non paintable area
        /// </summary>
        PanelLeftNonPaintableArea,
        /// <summary>
        /// Right non paitable area
        /// </summary>
        PanelRightNonPaintableArea,
        /// <summary>
        /// Panel's paintable area
        /// </summary>
        PanelPaintableArea,
        /// <summary>
        /// Panel's title bar
        /// </summary>
        PanelTitleBar,
        /// <summary>
        /// Calendar
        /// </summary>
        Calendar,
        /// <summary>
        /// Minimized panel's bar
        /// </summary>
        PanelsBar,
        /// <summary>
        /// No object
        /// </summary>
        NoObject
    }

    ///<summary>
    /// Symbol type
    ///</summary>
    public enum SymbolType
    {
        /// <summary>
        /// Buy
        /// </summary>
        Buy = 0,
        /// <summary>
        /// Sell
        /// </summary>
        Sell = 1,
        /// <summary>
        /// Exit Short
        /// </summary>
        ExitShort = 2,
        /// <summary>
        /// Exit long
        /// </summary>
        ExitLong = 3,
        /// <summary>
        /// Signal
        /// </summary>
        Signal = 4
    }

    /// <summary>
    /// Chart Data Type
    /// </summary>
    public enum ChartDataType
    {
        /// <summary>
        /// Points
        /// </summary>
        Points = 1,
        ///<summary>
        /// Percent
        ///</summary>
        Percent = 2
    }

    /// <summary>
    /// Type of parameter used for indicators
    /// </summary>
    public enum ParameterType
    {
        /// <summary>
        /// Moving Average Type
        /// </summary>
        ptMAType,
        /// <summary>
        /// %D Moving Average Type
        /// </summary>
        ptPctDMAType,
        ///<summary>
        /// Symbol
        ///</summary>
        ptSymbol,
        /// <summary>
        /// Source - symbol
        /// </summary>
        ptSource,
        /// <summary>
        /// Source1 - symbol
        /// </summary>
        ptSource1,
        /// <summary>
        /// Source 2 - symbol
        /// </summary>
        ptSource2,
        /// <summary>
        /// Source 3 - symbol
        /// </summary>
        ptSource3,
        /// <summary>
        /// Volume
        /// </summary>
        ptVolume,
        /// <summary>
        /// Points or Percent
        /// </summary>
        ptPointsOrPercent,
        /// <summary>
        /// Periods
        /// </summary>
        ptPeriods,
        /// <summary>
        /// Periods 1
        /// </summary>
        ptPeriods1,
        /// <summary>
        /// Periods 2
        /// </summary>
        ptPeriods2,
        /// <summary>
        /// Periods 3
        /// </summary>
        ptPeriods3,
        /// <summary>
        /// Cycle 1
        /// </summary>
        ptCycle1,
        /// <summary>
        /// Cycle 2
        /// </summary>
        ptCycle2,
        /// <summary>
        /// Cycle 3
        /// </summary>
        ptCycle3,
        /// <summary>
        /// Short Term
        /// </summary>
        ptShortTerm,
        /// <summary>
        /// Long Term
        /// </summary>
        ptLongTerm,
        /// <summary>
        /// Rate of Change
        /// </summary>
        ptRateOfChange,
        /// <summary>
        /// %K Periods
        /// </summary>
        ptPctKPeriods,
        /// <summary>
        /// %K Slowing
        /// </summary>
        ptPctKSlowing,
        /// <summary>
        /// %D Smooth
        /// </summary>
        ptPctDSmooth,
        /// <summary>
        /// %K Smooth
        /// </summary>
        ptPctKSmooth,
        /// <summary>
        /// %D Double Smooth
        /// </summary>
        ptPctDDblSmooth,
        /// <summary>
        /// %D Periods
        /// </summary>
        ptPctDPeriods,
        /// <summary>
        /// %K Double Smooth
        /// </summary>
        ptPctKDblSmooth,
        /// <summary>
        /// Short Cycle
        /// </summary>
        ptShortCycle,
        /// <summary>
        /// Long Cycle
        /// </summary>
        ptLongCycle,
        /// <summary>
        /// Standard Deviations
        /// </summary>
        ptStandardDeviations,
        /// <summary>
        /// R2 Scale
        /// </summary>
        ptR2Scale,
        /// <summary>
        /// Minimum AF
        /// </summary>
        ptMinAF,
        /// <summary>
        /// Maximum AF
        /// </summary>
        ptMaxAF,
        /// <summary>
        /// Shift
        /// </summary>
        ptShift,
        /// <summary>
        /// Factor
        /// </summary>
        ptFactor,
        /// <summary>
        /// Signal Periods
        /// </summary>
        ptSignalPeriods,
        /// <summary>
        /// Limit Move Value
        /// </summary>
        ptLimitMoveValue,
        /// <summary>
        /// Minimum Tick Value
        /// </summary>
        ptMinTickVal,
        /// <summary>
        /// Lvels
        /// </summary>
        ptLevels,
        /// <summary>
        /// Bar History
        /// </summary>
        ptBarHistory
    }

    /// <summary>
    /// Supported indicators
    /// </summary>
    public enum IndicatorType
    {        
        /// <summary>
        /// Simple Moving Average
        /// </summary>
        SimpleMovingAverage,

        /// <summary>
        /// Exponential Moving Average
        /// </summary>
        ExponentialMovingAverage,

        /// <summary>
        /// Time Series Moving Average
        /// </summary>
        TimeSeriesMovingAverage,

        /// <summary>
        /// Triangular Moving Average
        /// </summary>
        TriangularMovingAverage,

        /// <summary>
        /// Variable Moving Average
        /// </summary>
        VariableMovingAverage,

        /// <summary>
        /// VIDYA Moving Average
        /// </summary>
        VIDYA,

        /// <summary>
        /// Welles Wilder Smoothing
        /// </summary>
        WellesWilderSmoothing,

        /// <summary>
        /// Weighted Moving Average
        /// </summary>
        WeightedMovingAverage,

        /// <summary>
        /// Williams R
        /// </summary>
        WilliamsPctR,

        /// <summary>
        /// Williams Accumulation Dist
        /// </summary>
        WilliamsAccumulationDistribution,

        /// <summary>
        /// Volume Oscillator
        /// </summary>
        VolumeOscillator,

        /// <summary>
        /// Vertical Horizontal Filter
        /// </summary>
        VerticalHorizontalFilter,

        /// <summary>
        /// Ultimate Oscillator
        /// </summary>
        UltimateOscillator,

        /// <summary>
        /// True Range
        /// </summary>
        TrueRange,

        /// <summary>
        /// TRIX
        /// </summary>
        TRIX,

        /// <summary>
        /// Rainbow Oscillator
        /// </summary>
        RainbowOscillator,

        /// <summary>
        /// Price Oscillator
        /// </summary>
        PriceOscillator,

        /// <summary>
        /// Parabolic SAR
        /// </summary>
        ParabolicSAR,

        /// <summary>
        /// Momentum Oscillator
        /// </summary>
        MomentumOscillator,

        /// <summary>
        /// MACD
        /// </summary>
        MACD,

        /// <summary>
        /// Ease Of Movement
        /// </summary>
        EaseOfMovement,

        /// <summary>
        /// Directional Movement System
        /// </summary>
        DirectionalMovementSystem,

        /// <summary>
        /// Detrended Price Oscillator
        /// </summary>
        DetrendedPriceOscillator,

        /// <summary>
        /// Chande Momentum Oscillator
        /// </summary>
        ChandeMomentumOscillator,

        /// <summary>
        /// Chaikin Volatility
        /// </summary>
        ChaikinVolatility,

        /// <summary>
        /// AroonOscillator
        /// </summary>
        Aroon,

        /// <summary>
        /// AroonOscillator Oscillator
        /// </summary>
        AroonOscillator,

        /// <summary>
        /// Linear Regression R-Squared
        /// </summary>
        LinearRegressionRSquared,

        /// <summary>
        /// Linear Regression Forecast
        /// </summary>
        LinearRegressionForecast,

        /// <summary>
        /// Linear Regression Slope
        /// </summary>
        LinearRegressionSlope,

        /// <summary>
        /// Linear Regression Intercept
        /// </summary>
        LinearRegressionIntercept,

        /// <summary>
        /// Price Volume Trend
        /// </summary>
        PriceVolumeTrend,

        /// <summary>
        /// Performance Index
        /// </summary>
        PerformanceIndex,

        /// <summary>
        /// Commodity Channel Index
        /// </summary>
        CommodityChannelIndex,

        /// <summary>
        /// Chaikin Money Flow
        /// </summary>
        ChaikinMoneyFlow,

        /// <summary>
        /// Weighted Close
        /// </summary>
        WeightedClose,

        /// <summary>
        /// Volume ROC
        /// </summary>
        VolumeROC,

        /// <summary>
        /// Typical Price
        /// </summary>
        TypicalPrice,

        /// <summary>
        /// Standard Deviation
        /// </summary>
        StandardDeviation,

        /// <summary>
        /// Price ROC
        /// </summary>
        PriceROC,

        /// <summary>
        /// Median Price
        /// </summary>
        Median,

        /// <summary>
        /// High Minus Low
        /// </summary>
        HighMinusLow,

        /// <summary>
        /// Bollinger Bands
        /// </summary>
        BollingerBands,

        /// <summary>
        /// Fractal Chaos Bands
        /// </summary>
        FractalChaosBands,

        /// <summary>
        /// High/Low Bands
        /// </summary>
        HighLowBands,

        /// <summary>
        /// Moving Average Envelope
        /// </summary>
        MovingAverageEnvelope,

        /// <summary>
        /// Swing Index
        /// </summary>
        SwingIndex,

        /// <summary>
        /// Accumulative Swing Index
        /// </summary>
        AccumulativeSwingIndex,

        /// <summary>
        /// Comparative RSI
        /// </summary>
        ComparativeRelativeStrength,

        /// <summary>
        /// Mass Index
        /// </summary>
        MassIndex,

        /// <summary>
        /// Money Flow Index
        /// </summary>
        MoneyFlowIndex,

        /// <summary>
        /// Negative Volume Index
        /// </summary>
        NegativeVolumeIndex,

        /// <summary>
        /// On Balance Volume
        /// </summary>
        OnBalanceVolume,

        /// <summary>
        /// Positive Volume Index
        /// </summary>
        PositiveVolumeIndex,

        /// <summary>
        /// Relative Strength Index
        /// </summary>
        RelativeStrengthIndex,

        /// <summary>
        /// Trade Volume Index
        /// </summary>
        TradeVolumeIndex,

        /// <summary>
        /// Stochastic Oscillator
        /// </summary>
        StochasticOscillator,

        /// <summary>
        /// Stochastic Momentum Index
        /// </summary>
        StochasticMomentumIndex,

        /// <summary>
        /// Fractal Chaos Oscillator
        /// </summary>
        FractalChaosOscillator,

        /// <summary>
        /// Prime Number Oscillator
        /// </summary>
        PrimeNumberOscillator,

        /// <summary>
        /// Prime Number Bands
        /// </summary>
        PrimeNumberBands,

        /// <summary>
        /// Historical Volatility
        /// </summary>
        HistoricalVolatility,

        /// <summary>
        /// MACD Histogram
        /// </summary>
        MACDHistogram,

        /// <summary>
        /// Ichimoku Kinko Hyo
        /// </summary>
        Ichimoku,

        /// <summary>
        /// Elder Ray Bull Power
        /// </summary>
        ElderRayBullPower,

        /// <summary>
        /// Elder Ray Bear Power
        /// </summary>
        ElderRayBearPower,

        /// <summary>
        /// Ehler's Fisher Transform
        /// </summary>
        EhlerFisherTransform,

        /// <summary>
        /// Elder Force Index
        /// </summary>
        ElderForceIndex,

        /// <summary>
        /// Elder Thermometer
        /// </summary>
        ElderThermometer,

        /// <summary>
        /// Keltner Channel
        /// </summary>
        KeltnerChannel,

        /// <summary>
        /// Stoller Average Range Channels
        /// </summary>
        StollerAverageRangeChannels,

        /// <summary>
        /// Market Facilitation Index
        /// </summary>
        MarketFacilitationIndex,

        /// <summary>
        /// Schaff Trend Cycle
        /// </summary>
        SchaffTrendCycle,

        /// <summary>
        /// QStick
        /// </summary>
        QStick,

        /// <summary>
        /// Center Of Gravity
        /// </summary>
        CenterOfGravity,

        /// <summary>
        /// Coppock Curve
        /// </summary>
        CoppockCurve,

        /// <summary>
        /// Chande Forecast Oscillator
        /// </summary>
        ChandeForecastOscillator,

        /// <summary>
        /// Gopalakrishnan Range Index
        /// </summary>
        GopalakrishnanRangeIndex,

        /// <summary>
        /// Intraday Momentum Index
        /// </summary>
        IntradayMomentumIndex,

        /// <summary>
        /// Klinger Volume Oscillator
        /// </summary>
        KlingerVolumeOscillator,

        /// <summary>
        /// Pretty Good Oscillator
        /// </summary>
        PrettyGoodOscillator,

        /// <summary>
        /// RAVI
        /// </summary>
        RAVI,

        /// <summary>
        /// RandomWalkIndex
        /// </summary>
        RandomWalkIndex,

        /// <summary>
        /// Twiggs Money Flow
        /// </summary>
        TwiggsMoneyFlow,

        /// <summary>
        /// An indicator whos values are populated by the user
        /// </summary>
        CustomIndicator,

        /// <summary>
        /// Unknown
        /// </summary>
        Unknown,

        /// <summary>
        /// Indicador de High Low Activator
        /// </summary>
        HighLowActivator
    }

    /// <summary>
    /// Series Types
    /// </summary>
    public enum SeriesTypeEnum
    {
        /// <summary>
        /// Standard Line Chart
        /// </summary>
        stLineChart,

        /// <summary>
        /// Volume Bar Chart
        /// </summary>
        stVolumeChart,

        /// <summary>
        /// Bar Chart
        /// </summary>
        stStockBarChart,

        /// <summary>
        /// HLC Bar Chart
        /// </summary>
        stStockBarChartHLC,

        /// <summary>
        /// Candle Chart
        /// </summary>
        stCandleChart,

        /// <summary>
        /// Indicator - used only internally
        /// </summary>
        stIndicator,

        /// <summary>
        /// Unknown
        /// </summary>
        stUnknown
    }

    /// <summary>
    /// Price Style
    /// </summary>
    public enum PriceStyleEnum
    {
        /// <summary>
        /// Standard
        /// </summary>
        psStandard,
        /// <summary>
        /// Point and Figure
        /// </summary>
        psPointAndFigure,
        /// <summary>
        /// Renko
        /// </summary>
        psRenko,
        /// <summary>
        /// Kagi
        /// </summary>
        psKagi,
        /// <summary>
        /// Three Line Break
        /// </summary>
        psThreeLineBreak,
        /// <summary>
        /// Equivolume
        /// </summary>
        psEquiVolume,
        /// <summary>
        /// Equivolume Shadow
        /// </summary>
        psEquiVolumeShadow,
        /// <summary>
        /// Candle Volume
        /// </summary>
        psCandleVolume,
        /// <summary>
        /// Heikin Ashi
        /// xClose = (Open+High+Low+Close)/4 - Average price of the current bar
        /// xOpen = [xOpen(Previous Bar) + Close(Previous Bar)]/2 - Midpoint of the previous bar
        /// xHigh = Max(High, xOpen, xClose) - Highest value in the set
        /// xLow = Min(Low, xOpen, xClose) - Lowest value in the set 
        /// </summary>
        psHeikinAshi,
        ///<summary>
        /// Unknown
        ///</summary>
        psUnknown
    }

    /// <summary>
    /// Scaling type
    /// </summary>
    public enum ScalingTypeEnum
    {
        /// <summary>
        /// Linear
        /// </summary>
        Linear,
        /// <summary>
        /// Semi log
        /// </summary>
        Semilog
    }

    /// <summary>
    /// Y scale alignment
    /// </summary>
    public enum ScaleAlignmentTypeEnum
    {
        /// <summary>
        /// Left side
        /// </summary>
        Left,
        /// <summary>
        /// Right side
        /// </summary>
        Right,
        /// <summary>
        /// Both sides (not yet supported)
        /// </summary>
        Both
    }

    /// <summary>
    /// Position of the tick box on Y axis
    /// </summary>
    public enum TickBoxPosition
    {
        ///<summary>
        /// Position tick box on Left Y axis if is visible
        ///</summary>
        Left,
        ///<summary>
        /// Position tick box on Right Y axis if is visible
        ///</summary>
        Right,
        ///<summary>
        /// Tick box is invisible
        ///</summary>
        None
    }

    /// <summary>
    /// OHLC type of the series
    /// </summary>
    public enum SeriesTypeOHLC
    {
        ///<summary>
        /// Open 
        ///</summary>
        Open,
        ///<summary>
        /// High
        ///</summary>
        High,
        ///<summary>
        /// Low
        ///</summary>
        Low,
        ///<summary>
        /// Close
        ///</summary>
        Close,
        ///<summary>
        /// Volume
        ///</summary>
        Volume,
        /// <summary>
        /// Usually refers to indicators
        /// </summary>
        Unknown
    }

    /// <summary>
    /// Type of Y Scale calculation
    /// </summary>
    public enum YGridStepType
    {
        /// <summary>
        /// Will calculate the Y axis step that is a multiplier of 1, 1.25, 1.5, 2, 5 standard numbers.
        /// While the step will be correct this type of calculation might lead into havinf a big gap between max series values
        /// and max value from Y axis.
        /// </summary>
        NiceStep,

        /// <summary>
        /// Will calculate the smalles possible step in order to fit best series into Y axis
        /// </summary>
        MinimalGap,
    }

    /// <summary>
    /// There are two ways to display the calendar. Version2 is has more functionality, but may behave oddly in some
    /// circumstances
    /// </summary>
    public enum CalendarVersionType
    {
        /// <summary>
        /// The older version of the calendar layout
        /// </summary>
        Version1,
        /// <summary>
        /// The newer version of the calendar layout. It has some issues that may cause issues, but it also has a lot of 
        /// new functionality.
        /// </summary>
        Version2
    }

    /// <summary>
    /// When the Calendar V2 outputs it label value to the screen there is a choice about which value to display.
    /// Each label defines a 'block' of data. The lable can display the timestamp for beginning of the block, or the 
    /// timestamp of the first valid existing piece of data within the block.
    /// </summary>
    public enum CalendarLabelBlockOutputType
    {
        /// <summary>
        /// The start datetime of the block
        /// </summary>
        Beginning,
        /// <summary>
        /// The first valid data point in the block
        /// </summary>
        FirstValid,
        /// <summary>
        /// The end datetime of the block
        /// </summary>
        End,
    }

}

