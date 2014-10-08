using System;
using System.Collections.Generic;
using ModulusFE.LineStudies;

namespace ModulusFE
{
    /// <summary>
    /// The class has information parameters for line studies used in the chart
    /// </summary>
    public static partial class StockChartX_LineStudiesParams
    {
        ///<summary>
        /// Information about a line study
        ///</summary>
        public class LineStudyParams
        {
            ///<summary>
            /// Line study type
            ///</summary>
            public LineStudy.StudyTypeEnum StudyTypeEnum { get; internal set; }
            ///<summary>
            /// CLR Type of line study
            ///</summary>
            public Type CLRType { get; internal set; }
            ///<summary>
            /// Friendly name of the line study
            ///</summary>
            public string FriendlyName { get; internal set; }

            /// <summary>
            /// ToString()
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return FriendlyName;
            }
        }

        private static readonly Dictionary<LineStudy.StudyTypeEnum, LineStudyParams> _lineStudiesTypes = new Dictionary<LineStudy.StudyTypeEnum, LineStudyParams>();

        internal static void RegisterLineStudy(LineStudy.StudyTypeEnum studyTypeEnum, Type CLRType, string friendlyName)
        {
            if (_lineStudiesTypes.ContainsKey(studyTypeEnum))
                return;
            _lineStudiesTypes[studyTypeEnum] = new LineStudyParams()
                                                 {
                                                     CLRType = CLRType,
                                                     FriendlyName = friendlyName,
                                                     StudyTypeEnum = studyTypeEnum
                                                 };
        }

        internal static string GetLineStudyFriendlyName(LineStudy.StudyTypeEnum studyTypeEnum)
        {
            RegisterAll();

            return _lineStudiesTypes[studyTypeEnum].FriendlyName;
        }

        /// <summary>
        /// Gets the CLR type by internal line study type.
        /// </summary>
        /// <param name="studyTypeEnum">Internal type</param>
        /// <returns>CLR Type</returns>
        public static Type GetLineStudyCLRType(LineStudy.StudyTypeEnum studyTypeEnum)
        {
            RegisterAll();

            return _lineStudiesTypes[studyTypeEnum].CLRType;
        }

        ///<summary>
        /// Gets the registered line studies
        ///</summary>
        public static IEnumerable<LineStudyParams> LineStudiesTypes
        {
            get
            {
                RegisterAll();

                foreach (LineStudyParams study in _lineStudiesTypes.Values)
                {
                    yield return study;
                }
            }
        }

        private static bool _registered;
        private static void RegisterAll()
        {
            if (_registered) return;
            _registered = true;

            Register_Ellipse();
            Register_ErrorChannel();
            Register_FibonacciArcs();
            Register_FibonacciFan();
            Register_FibonacciRetracements();
            Register_FibonacciTimeZones();
            Register_GannFan();
            Register_HorizontalLine();
            Register_QuadrantLines();
            Register_RaffRegression();
            Register_Rectangle();
            Register_SpeedLines();
            Register_TironeLevels();
            Register_TrendLine();
            Register_VerticalLine();
            Register_ImageObject();
            Register_StaticText();
            Register_WpfFrameworkElement();
        }
    }
}

