using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using ModulusFE.Indicators;
using ModulusFE.LineStudies;

namespace ModulusFE
{
    public partial class StockChartX
    {
        internal const int Version = 100;

        /// <summary>
        /// Serialization type
        /// </summary>
        public enum SerializationTypeEnum
        {
            /// <summary>
            /// Entire chart is saved
            /// </summary>
            All,
            /// <summary>
            /// Chart's styles and indicators are saved
            /// </summary>
            General,
            /// <summary>
            /// All lines studies are saved
            /// </summary>
            Objects,
            /// <summary>
            /// Save only indicators
            /// </summary>
            Indicators,
        }

        /// <summary>
        /// Save's the chart into a given file name. Saves entire chart
        /// </summary>
        /// <param name="filename">File name where to save</param>
        /// <returns>true - if file was succesfully saved. false  - otherwise</returns>
        public bool SaveFile(string filename)
        {
            return SaveFile(filename, SerializationTypeEnum.All);
        }

        ///<summary>
        /// Save's the chart into a given file name. 
        ///</summary>
        ///<param name="filename">File name where to save</param>
        ///<param name="serializationType">Type of serialization to use.</param>
        /// <returns>true - if file was succesfully saved. false  - otherwise</returns>
        public bool SaveFile(string filename, SerializationTypeEnum serializationType)
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();

                XmlElement xRoot = xmlDocument.CreateElement("StockChartX");
                xmlDocument.AppendChild(xRoot);

                ConvertToXmlEx("Version", xRoot, xmlDocument, Version);
                XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("SaveType");
                xmlAttribute.Value = serializationType.ToString();
                xRoot.Attributes.Append(xmlAttribute);

                switch (serializationType)
                {
                    case SerializationTypeEnum.All:
                        SaveWorkSpace(xRoot, xmlDocument);
                        SaveChartPanels(xRoot, xmlDocument);
                        SaveSeries(xRoot, xmlDocument);
                        SaveSeriesData(xRoot, xmlDocument);
                        SaveIndicators(xRoot, xmlDocument);
                        SaveLineStudies(xRoot, xmlDocument);
                        break;
                    case SerializationTypeEnum.General:
                        SaveWorkSpace(xRoot, xmlDocument);
                        SaveIndicators(xRoot, xmlDocument);
                        SaveLineStudies(xRoot, xmlDocument);
                        break;
                    case SerializationTypeEnum.Objects:
                        SaveLineStudies(xRoot, xmlDocument);
                        SaveIndicators(xRoot, xmlDocument);
                        break;
                    case SerializationTypeEnum.Indicators:
                        SaveIndicators(xRoot, xmlDocument);
                        break;
                }

                using (FileStream fileStream = File.Create(filename))
                {
                    using (GZipStream zipStream = new GZipStream(fileStream, CompressionMode.Compress))
                    {
                        using (XmlTextWriter xmlWriter = new XmlTextWriter(zipStream, Encoding.UTF8) { Formatting = Formatting.Indented })
                        {
                            xmlDocument.Save(xmlWriter);
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 
        ///</summary>
        ///<param name="serializationType"></param>
        ///<returns></returns>
        public byte[] Save(SerializationTypeEnum serializationType)
        {
            byte[] ret = null;
            try
            {
                XmlDocument xmlDocument = new XmlDocument();

                XmlElement xRoot = xmlDocument.CreateElement("StockChartX");
                xmlDocument.AppendChild(xRoot);

                ConvertToXmlEx("Version", xRoot, xmlDocument, Version);
                XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("SaveType");
                xmlAttribute.Value = serializationType.ToString();
                xRoot.Attributes.Append(xmlAttribute);

                switch (serializationType)
                {
                    case SerializationTypeEnum.All:
                        SaveWorkSpace(xRoot, xmlDocument);
                        SaveChartPanels(xRoot, xmlDocument);
                        SaveSeries(xRoot, xmlDocument);
                        SaveSeriesData(xRoot, xmlDocument);
                        SaveIndicators(xRoot, xmlDocument);
                        SaveLineStudies(xRoot, xmlDocument);
                        break;
                    case SerializationTypeEnum.General:
                        SaveWorkSpace(xRoot, xmlDocument);
                        SaveIndicators(xRoot, xmlDocument);
                        SaveLineStudies(xRoot, xmlDocument);
                        break;
                    case SerializationTypeEnum.Objects:
                        SaveLineStudies(xRoot, xmlDocument);
                        SaveIndicators(xRoot, xmlDocument);
                        break;
                    case SerializationTypeEnum.Indicators:
                        SaveIndicators(xRoot, xmlDocument);
                        break;
                }

                using (MemoryStream stream = new MemoryStream())
                {
                    using (XmlTextWriter xmlWriter = new XmlTextWriter(stream, Encoding.UTF8) { Formatting = Formatting.Indented })
                    {
                        xmlDocument.Save(xmlWriter);
                        ret = stream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return ret;
        }

        ///<summary>
        ///</summary>
        ///<param name="buffer"></param>
        ///<param name="serializationType"></param>
        ///<returns></returns>
        public int Load(byte[] buffer, SerializationTypeEnum serializationType)
        {
            try
            {
                Status = ChartStatus.Building;
                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (XmlTextReader xmlTextReader = new XmlTextReader(memoryStream))
                    {
                        XmlDocument xmlDocument = new XmlDocument();
                        xmlDocument.Load(xmlTextReader);

                        XmlNode xRoot = xmlDocument.DocumentElement;
                        if (xRoot == null) return -1;
                        int version = ConvertFromXmlEx("Version", xRoot, 0);

                        SerializationTypeEnum serializationTypeEnum = (SerializationTypeEnum)Enum.Parse(typeof(SerializationTypeEnum), xRoot.Attributes["SaveType"].Value);
                        if (serializationTypeEnum != serializationType) return -2;

                        if (version == 100)
                        {
                            switch (serializationType)
                            {
                                case SerializationTypeEnum.All:
                                    ReadWorkSpace(xRoot);
                                    ReadChartPanels(xRoot);
                                    ReadSeries(xRoot);
                                    ReadSeriesData(xRoot);
                                    ReadIndicators(xRoot);
                                    ReadLineStudies(xRoot);
                                    break;
                                case SerializationTypeEnum.General:
                                    ReadWorkSpace(xRoot);
                                    ReadIndicators(xRoot);
                                    ReadLineStudies(xRoot);
                                    break;
                                case SerializationTypeEnum.Objects:
                                    // ReadChartPanels(xRoot);
                                    ReadLineStudies(xRoot);
                                    ReadIndicators(xRoot);
                                    break;
                                case SerializationTypeEnum.Indicators:
                                    ReadIndicators(xRoot);
                                    break;
                                default:
                                    return -3;
                            }
                        }
                        else
                        {
                            return -4;
                        }
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return -5;
            }
            finally
            {
                Status = ChartStatus.Ready;
            }
        }

        /// <summary>
        /// open's a chart file
        /// </summary>
        /// <param name="filename">Filename</param>
        /// <returns>
        /// 0 - no errors
        /// -1 - root element is null
        /// -2 - wrong serialization type
        /// -3 - un-suported serialization type
        /// -4 - un-suported version of file
        /// -5 - unknown exception
        /// </returns>
        public int LoadFile(string filename)
        {
            return LoadFile(filename, SerializationTypeEnum.All);
        }

        /// <summary>
        /// open's a chart file
        /// </summary>
        /// <param name="filename">File name</param>
        /// <param name="serializationType">Serialization type</param>
        /// <returns>
        /// 0 - no errors
        /// -1 - root element is null
        /// -2 - wrong serialization type
        /// -3 - un-suported serialization type
        /// -4 - un-suported version of file
        /// -5 - unknown exception
        /// </returns>
        public int LoadFile(string filename, SerializationTypeEnum serializationType)
        {
            try
            {
                Status = ChartStatus.Building;
                using (FileStream fileStream = File.OpenRead(filename))
                {
                    using (GZipStream zipStream = new GZipStream(fileStream, CompressionMode.Decompress))
                    {
                        using (XmlTextReader xmlTextReader = new XmlTextReader(zipStream))
                        {
                            XmlDocument xmlDocument = new XmlDocument();
                            xmlDocument.Load(xmlTextReader);

                            XmlNode xRoot = xmlDocument.DocumentElement;
                            if (xRoot == null) return -1;
                            int version = ConvertFromXmlEx("Version", xRoot, 0);

                            SerializationTypeEnum serializationTypeEnum = (SerializationTypeEnum)Enum.Parse(typeof(SerializationTypeEnum), xRoot.Attributes["SaveType"].Value);
                            if (serializationTypeEnum != serializationType) return -2;

                            if (version == 100)
                            {
                                switch (serializationType)
                                {
                                    case SerializationTypeEnum.All:
                                        ReadWorkSpace(xRoot);
                                        ReadChartPanels(xRoot);
                                        ReadSeries(xRoot);
                                        ReadSeriesData(xRoot);
                                        ReadIndicators(xRoot);
                                        ReadLineStudies(xRoot);
                                        break;
                                    case SerializationTypeEnum.General:
                                        ReadWorkSpace(xRoot);
                                        ReadIndicators(xRoot);
                                        ReadLineStudies(xRoot);
                                        break;
                                    case SerializationTypeEnum.Objects:
                                        // ReadChartPanels(xRoot);
                                        ReadLineStudies(xRoot);
                                        ReadIndicators(xRoot);
                                        break;
                                    case SerializationTypeEnum.Indicators:
                                        ReadIndicators(xRoot);
                                        break;
                                    default:
                                        return -3;
                                }
                            }
                            else
                            {
                                return -4;
                            }
                        }
                    }
                }

                Status = ChartStatus.Ready;
                _panelsContainer.ResizePanels(PanelsContainer.ResizeType.Proportional);
                return 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return -5;
            }
            finally
            {
                Status = ChartStatus.Ready;
            }
        }

        private void SaveWorkSpace(XmlNode xRoot, XmlDocument xmlDocument)
        {
            XmlElement xmlWorkspace = xmlDocument.CreateElement("WorkSpace");
            xRoot.AppendChild(xmlWorkspace);

            ConvertToXmlEx("ShowAnimations", xmlWorkspace, xmlDocument, ShowAnimations);
            ConvertToXmlEx("YAxes", xmlWorkspace, xmlDocument, ScaleAlignment);
            ConvertToXmlEx("ScalingType", xmlWorkspace, xmlDocument, ScalingType);
            ConvertToXmlEx("ShowYGrid", xmlWorkspace, xmlDocument, YGrid);
            ConvertToXmlEx("ShowXGrid", xmlWorkspace, xmlDocument, XGrid);
            ConvertToXml("GridStroke", xmlWorkspace, xmlDocument, GridStroke);
            ConvertToXmlEx("ThreeDStyle", xmlWorkspace, xmlDocument, ThreeDStyle);
            ConvertToXmlEx("UpColor", xmlWorkspace, xmlDocument, UpColor);
            ConvertToXmlEx("DownColor", xmlWorkspace, xmlDocument, DownColor);
            ConvertToXmlEx("CrossHairs", xmlWorkspace, xmlDocument, CrossHairs);
            ConvertToXmlEx("LeftChartSpace", xmlWorkspace, xmlDocument, LeftChartSpace);
            ConvertToXmlEx("RightChartSpace", xmlWorkspace, xmlDocument, RightChartSpace);
            ConvertToXmlEx("RealTimeData", xmlWorkspace, xmlDocument, RealTimeXLabels);
            ConvertToXmlEx("FontFace", xmlWorkspace, xmlDocument, FontFace);
            ConvertToXmlEx("FontSize", xmlWorkspace, xmlDocument, FontSize);
            ConvertToXml("FontForeground", xmlWorkspace, xmlDocument, FontForeground);
            ConvertToXmlEx("PriceStyle", xmlWorkspace, xmlDocument, PriceStyle);
            ConvertToXmlEx("DisplayTitles", xmlWorkspace, xmlDocument, DisplayTitles);
            ConvertToXmlEx("DarvasBoxes", xmlWorkspace, xmlDocument, DarvasBoxes);
            ConvertToXmlEx("ChartType", xmlWorkspace, xmlDocument, ChartType);
            ConvertToXmlEx("TickPeriodicity", xmlWorkspace, xmlDocument, TickPeriodicity);
            ConvertToXmlEx("TickCompressionType", xmlWorkspace, xmlDocument, TickCompressionType);
            ConvertToXmlEx("InfoPanelFontSize", xmlWorkspace, xmlDocument, InfoPanelFontSize);
            ConvertToXmlEx("InfoPanelFontFamily", xmlWorkspace, xmlDocument, InfoPanelFontFamily);
            ConvertToXml("InfoPanelLabelsForeground", xmlWorkspace, xmlDocument, InfoPanelLabelsForeground);
            ConvertToXml("InfoPanelLabelsBackground", xmlWorkspace, xmlDocument, InfoPanelLabelsBackground);
            ConvertToXml("InfoPanelValuesForeground", xmlWorkspace, xmlDocument, InfoPanelValuesForeground);
            ConvertToXml("InfoPanelValuesBackground", xmlWorkspace, xmlDocument, InfoPanelValuesBackground);
            ConvertToXmlEx("InfoPanelPosition", xmlWorkspace, xmlDocument, InfoPanelPosition);
            ConvertToXml("IndicatorDialogBackground", xmlWorkspace, xmlDocument, IndicatorDialogBackground);
            ConvertToXmlEx("InfoPanelPositionCoords", xmlWorkspace, xmlDocument, _panelsContainer._infoPanel != null ? _panelsContainer._infoPanel.Position : new Point(0, 0));
            ConvertToXmlEx("ScalePrecision", xmlWorkspace, xmlDocument, ScalePrecision);

            for (int i = 0; i < _priceStyleParams.Length; i++)
            {
                ConvertToXmlEx("PriceStyleParam_" + i, xmlWorkspace, xmlDocument, _priceStyleParams[i]);
            }

            return;
        }

        private void ReadWorkSpace(XmlNode xRoot)
        {
            XmlNode xmlWorkspace = xRoot.SelectSingleNode("WorkSpace");
            if (xmlWorkspace == null) return;

            try
            {
                ShowAnimations = ConvertFromXmlEx("ShowAnimations", xmlWorkspace, false);
                ScaleAlignment = (ScaleAlignmentTypeEnum)Enum.Parse(typeof(ScaleAlignmentTypeEnum), ConvertFromXmlEx("YAxes", xmlWorkspace, String.Empty));
                ScalingType = (ScalingTypeEnum)Enum.Parse(typeof(ScalingTypeEnum), ConvertFromXmlEx("ScalingType", xmlWorkspace, String.Empty));
                YGrid = ConvertFromXmlEx("ShowYGrid", xmlWorkspace, true);
                XGrid = ConvertFromXmlEx("ShowXGrid", xmlWorkspace, true);
                GridStroke = (Brush)ConvertFromXml("GridStroke", xmlWorkspace);
                ThreeDStyle = ConvertFromXmlEx("ThreeDStyle", xmlWorkspace, true);
                UpColor = Utils.StringToColor(ConvertFromXmlEx("UpColor", xmlWorkspace, String.Empty));
                DownColor = Utils.StringToColor(ConvertFromXmlEx("DownColor", xmlWorkspace, String.Empty));
                CrossHairs = ConvertFromXmlEx("CrossHairs", xmlWorkspace, false);
                LeftChartSpace = ConvertFromXmlEx("LeftChartSpace", xmlWorkspace, 5);
                RightChartSpace = ConvertFromXmlEx("RightChartSpace", xmlWorkspace, 10);
                RealTimeXLabels = ConvertFromXmlEx("RealTimeData", xmlWorkspace, true);
                FontFace = ConvertFromXmlEx("FontFace", xmlWorkspace, String.Empty);
                FontForeground = (Brush)ConvertFromXml("FontForeground", xmlWorkspace);
                PriceStyle = (PriceStyleEnum)Enum.Parse(typeof(PriceStyleEnum), ConvertFromXmlEx("PriceStyle", xmlWorkspace, String.Empty));
                DisplayTitles = ConvertFromXmlEx("DisplayTitles", xmlWorkspace, true);
                DarvasBoxes = ConvertFromXmlEx("DarvasBoxes", xmlWorkspace, false);
                ChartType = (ChartTypeEnum)Enum.Parse(typeof(ChartTypeEnum), ConvertFromXmlEx("ChartType", xmlWorkspace, String.Empty));
                TickPeriodicity = ConvertFromXmlEx("TickPeriodicity", xmlWorkspace, 5);
                TickCompressionType = (TickCompressionEnum)Enum.Parse(typeof(TickCompressionEnum), ConvertFromXmlEx("TickCompressionType", xmlWorkspace, String.Empty));
                InfoPanelFontSize = ConvertFromXmlEx("InfoPanelFontSize", xmlWorkspace, 10);
                InfoPanelFontFamily = new FontFamily(ConvertFromXmlEx("InfoPanelFontFamily", xmlWorkspace, "Arial"));
                InfoPanelLabelsForeground = (Brush)ConvertFromXml("InfoPanelLabelsForeground", xmlWorkspace);
                InfoPanelLabelsBackground = (Brush)ConvertFromXml("InfoPanelLabelsBackground", xmlWorkspace);
                InfoPanelValuesForeground = (Brush)ConvertFromXml("InfoPanelValuesForeground", xmlWorkspace);
                InfoPanelValuesBackground = (Brush)ConvertFromXml("InfoPanelValuesBackground", xmlWorkspace);
                InfoPanelPosition = (InfoPanelPositionEnum)Enum.Parse(typeof(InfoPanelPositionEnum), ConvertFromXmlEx("InfoPanelPosition", xmlWorkspace, String.Empty));
                IndicatorDialogBackground = (Brush)ConvertFromXml("IndicatorDialogBackground", xmlWorkspace);
                if (_panelsContainer._infoPanel != null)
                    _panelsContainer._infoPanel.Position = Point.Parse(ConvertFromXmlEx("InfoPanelPositionCoords", xmlWorkspace, "0,0"));
                ScalePrecision = ConvertFromXmlEx("ScalePrecision", xmlWorkspace, 2);

                for (int i = 0; i < _priceStyleParams.Length; i++)
                    _priceStyleParams[i] = ConvertFromXmlEx("PriceStyleParam_" + i, xmlWorkspace, 0.0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void SaveChartPanels(XmlNode xRoot, XmlDocument xmlDocument)
        {
            XmlNode xmlChartPanels = xmlDocument.CreateElement("ChartPanels");
            xRoot.AppendChild(xmlChartPanels);
            foreach (ChartPanel chartPanel in _panelsContainer.Panels)
            {
                XmlNode xmlChartPanel = xmlDocument.CreateElement("ChartPanel");
                xmlChartPanels.AppendChild(xmlChartPanel);

                ConvertToXmlEx("Position", xmlChartPanel, xmlDocument, chartPanel._position);
                ConvertToXml("Background", xmlChartPanel, xmlDocument, chartPanel.Background);
                ConvertToXmlEx("IsHeatMap", xmlChartPanel, xmlDocument, chartPanel.IsHeatMap);
                ConvertToXmlEx("Visible", xmlChartPanel, xmlDocument, chartPanel.Visible);
                ConvertToXmlEx("Height", xmlChartPanel, xmlDocument, chartPanel.Height);
                ConvertToXmlEx("Index", xmlChartPanel, xmlDocument, chartPanel.Index);
            }
        }

        private void ReadChartPanels(XmlNode xRoot)
        {
            XmlNode xmlChartPanels = xRoot.SelectSingleNode("ChartPanels");
            if (xmlChartPanels == null) return;

            foreach (XmlNode node in xmlChartPanels.ChildNodes)
            {
                if (node.Name != "ChartPanel") continue;

                ChartPanel.PositionType positionType =
                  (ChartPanel.PositionType)Enum.Parse(typeof(ChartPanel.PositionType), ConvertFromXmlEx("Position", node, string.Empty));
                bool isHeatMap = ConvertFromXmlEx("IsHeatMap", node, false);
                ChartPanel chartPanel;
                if (isHeatMap)
                {
                    chartPanel = AddHeatMapPanel();
                }
                else
                {
                    chartPanel = AddChartPanel(positionType);
                    chartPanel.Background = (Brush)ConvertFromXml("Background", node);
                    chartPanel.Height = ConvertFromXmlEx("Height", node, 0.0);
                }

                chartPanel.Visible = ConvertFromXmlEx("Visible", node, true);
            }
        }

        private void SaveSeries(XmlNode xRoot, XmlDocument xmlDocument)
        {
            XmlNode xmlSeries = xmlDocument.CreateElement("SeriesCollection");
            xRoot.AppendChild(xmlSeries);
            foreach (ChartPanel chartPanel in _panelsContainer.Panels)
            {
                foreach (Series series in chartPanel.SeriesCollection)
                {
                    XmlNode xmlS = xmlDocument.CreateElement("Series");
                    xmlSeries.AppendChild(xmlS);

                    ConvertToXmlEx("Name", xmlS, xmlDocument, series.Name);
                    ConvertToXmlEx("OHLCType", xmlS, xmlDocument, series.OHLCType);
                    ConvertToXmlEx("PanelIndex", xmlS, xmlDocument, series._chartPanel.Index);
                    ConvertToXmlEx("UpColor", xmlS, xmlDocument, series.UpColor.HasValue ? series.UpColor.ToString() : string.Empty);
                    ConvertToXmlEx("DownColor", xmlS, xmlDocument, series.DownColor.HasValue ? series.DownColor.ToString() : string.Empty);
                    ConvertToXmlEx("StrokeThicknes", xmlS, xmlDocument, series.StrokeThickness);
                    ConvertToXmlEx("StrokeColor", xmlS, xmlDocument, series.StrokeColor);
                    ConvertToXmlEx("Selectable", xmlS, xmlDocument, series.Selectable);
                    ConvertToXmlEx("Visible", xmlS, xmlDocument, series.Visible);
                    ConvertToXmlEx("StrokePattern", xmlS, xmlDocument, series.StrokePattern);
                    ConvertToXmlEx("SeriesType", xmlS, xmlDocument, series.SeriesType);
                    ConvertToXmlEx("SeriesIndex", xmlS, xmlDocument, series.SeriesIndex);
                    ConvertToXmlEx("SeriesTypeSeries", xmlS, xmlDocument, ChartPanel.GetSeriesTypeBySeries(series));
                }
            }
        }

        private void ReadSeries(XmlNode xRoot)
        {
            XmlNode xmlSeries = xRoot.SelectSingleNode("SeriesCollection");
            if (xmlSeries == null) return;
            int useablePanelsCount = UseablePanelsCount;

            foreach (XmlNode node in xmlSeries.ChildNodes)
            {
                if (node.Name != "Series") continue;

                int panelIndex = ConvertFromXmlEx("PanelIndex", node, -1);
                if (panelIndex == -1) continue;
                if (panelIndex >= useablePanelsCount) panelIndex = 0;

                ChartPanel chartPanel = GetPanelByIndex(panelIndex);
                string seriesName = ConvertFromXmlEx("Name", node, string.Empty);
                if (seriesName.Length == 0) continue;
                SeriesTypeEnum seriesTypeSeries = (SeriesTypeEnum)Enum.Parse(typeof(SeriesTypeEnum), ConvertFromXmlEx("SeriesTypeSeries", node, SeriesTypeEnum.stUnknown.ToString()));
                SeriesTypeOHLC ohlc = (SeriesTypeOHLC)Enum.Parse(typeof(SeriesTypeOHLC), ConvertFromXmlEx("OHLCType", node, string.Empty));
                SeriesTypeEnum seriesType = (SeriesTypeEnum)Enum.Parse(typeof(SeriesTypeEnum), ConvertFromXmlEx("SeriesType", node, string.Empty));
                Series series = chartPanel.CreateSeries(seriesName, ohlc, seriesTypeSeries != SeriesTypeEnum.stUnknown ? seriesTypeSeries : seriesType);

                if (series == null)
                {
                    continue;
                }

                series.SeriesType = seriesType;
                string upColor = ConvertFromXmlEx("UpColor", node, string.Empty);
                string downColor = ConvertFromXmlEx("DownColor", node, string.Empty);
                series.StrokeThickness = ConvertFromXmlEx("StrokeThicknes", node, 1.0);
                series.StrokeColor = Utils.StringToColor(ConvertFromXmlEx("StrokeColor", node, string.Empty));
                series._selectable = ConvertFromXmlEx("Selectable", node, false);
                series.Visible = ConvertFromXmlEx("Visible", node, true);
                series.StrokePattern = (LinePattern)Enum.Parse(typeof(LinePattern), ConvertFromXmlEx("StrokePattern", node, "Solid"));
                series.UpColor = upColor.Length > 0 ? Utils.StringToColor(upColor) : (Color?)null;
                series.DownColor = downColor.Length > 0 ? Utils.StringToColor(downColor) : (Color?)null;
                series.SeriesIndex = ConvertFromXmlEx("SeriesIndex", node, 0);

                _dataManager.AddSeries(series.Name, series.OHLCType);
                _dataManager.BindSeries(series);
            }
        }

        private void SaveSeriesData(XmlNode xRoot, XmlDocument xmlDocument)
        {
            _dataManager.SaveToXml(xRoot, xmlDocument);
        }

        private void ReadSeriesData(XmlNode xRoot)
        {
            _dataManager.LoadFromXml(xRoot);
        }

        private void SaveIndicators(XmlNode xRoot, XmlDocument xmlDocument)
        {
            XmlNode xmlIndicators = xmlDocument.CreateElement("Indicators");
            xRoot.AppendChild(xmlIndicators);
            foreach (ChartPanel chartPanel in _panelsContainer.Panels)
            {
                foreach (Indicator indicator in chartPanel.IndicatorsCollection)
                {
                    if (indicator.IsTwin) continue;

                    XmlNode xmlIndicator = xmlDocument.CreateElement("Indicator");
                    xmlIndicators.AppendChild(xmlIndicator);

                    ConvertToXmlEx("Name", xmlIndicator, xmlDocument, indicator.Name);
                    ConvertToXmlEx("IndicatorType", xmlIndicator, xmlDocument, indicator.IndicatorType);
                    ConvertToXmlEx("PanelIndex", xmlIndicator, xmlDocument, indicator._chartPanel.Index);
                    ConvertToXmlEx("TotalPanelCount", xmlIndicator, xmlDocument, indicator._chartPanel._chartX.PanelsCount);
                    ConvertToXmlEx("TotalPanelsInChart", xmlIndicator, xmlDocument, _panelsContainer.Panels.Count);
                    ConvertToXmlEx("UpColor", xmlIndicator, xmlDocument, indicator.UpColor.HasValue ? indicator.UpColor.ToString() : string.Empty);
                    ConvertToXmlEx("DownColor", xmlIndicator, xmlDocument, indicator.DownColor.HasValue ? indicator.DownColor.ToString() : string.Empty);
                    ConvertToXmlEx("StrokeThicknes", xmlIndicator, xmlDocument, indicator.StrokeThickness);
                    ConvertToXmlEx("StrokeColor", xmlIndicator, xmlDocument, indicator.StrokeColor);
                    ConvertToXmlEx("Selectable", xmlIndicator, xmlDocument, indicator.Selectable);
                    ConvertToXmlEx("Visible", xmlIndicator, xmlDocument, indicator.Visible);
                    ConvertToXmlEx("StrokePattern", xmlIndicator, xmlDocument, indicator.StrokePattern);
                    ConvertToXmlEx("UserParams", xmlIndicator, xmlDocument, indicator.UserParams);
                    List<StockChartX_IndicatorsParameters.IndicatorParameter> indicatorParams =
                      StockChartX_IndicatorsParameters.GetIndicatorParameters(indicator.IndicatorType);
                    for (int i = 0; i < indicatorParams.Count; i++)
                    {
                        ConvertToXmlEx("Parameter_" + i, xmlIndicator, xmlDocument, indicator.GetParameterValue(i));
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static readonly ParameterType[] ParamsWithSeriesName
          = new[]
          {
            ParameterType.ptSource, ParameterType.ptSource1,
            ParameterType.ptSource2, ParameterType.ptSource3,
            ParameterType.ptVolume
          };

        private void ReadIndicators(XmlNode xRoot)
        {
            XmlNode xmlIndicators = xRoot.SelectSingleNode("Indicators");
            if (xmlIndicators == null) return;

            int useablePanelsCount = UseablePanelsCount;
            foreach (XmlNode node in xmlIndicators.ChildNodes)
            {
                if (node.Name != "Indicator") continue;

                int panelIndex = ConvertFromXmlEx("PanelIndex", node, -1);
                int totalPanelsInChart = ConvertFromXmlEx("TotalPanelsInChart", node, -1);
                if (panelIndex == -1)
                    continue;

                ChartPanel chartPanel;
                if (totalPanelsInChart == -1)
                {
                    if (panelIndex >= useablePanelsCount)
                        panelIndex = 0;

                    int totalPanelCount = -1;
                    if (node.ChildNodes.OfType<XmlNode>().Any(_ => _.Name == "TotalPanelCount"))
                    {
                        totalPanelCount = ConvertFromXmlEx("TotalPanelCount", node, -1);
                    }

                    if (totalPanelCount != -1)
                    {
                        int diff = totalPanelCount - panelIndex;
                        int currDiff = useablePanelsCount - panelIndex;
                        if (currDiff < diff)
                        {
                            panelIndex = -1;
                        }
                    }

                    chartPanel = panelIndex == -1 ? AddChartPanel() : GetPanelByIndex(panelIndex);
                }
                else
                {
                    if (panelIndex >= useablePanelsCount)
                        panelIndex = 0;

                    if (_panelsContainer.Panels.Count < totalPanelsInChart)
                        AddChartPanel();

                    chartPanel = GetPanelByIndex(panelIndex);
                }

                string indicatorName = ConvertFromXmlEx("Name", node, string.Empty);
                if (string.IsNullOrEmpty(indicatorName))
                {
                    continue;
                }

                IndicatorType indicatorType = (IndicatorType)Enum.Parse(typeof(IndicatorType), ConvertFromXmlEx("IndicatorType", node, string.Empty));
                int indicatorCount = GetIndicatorCountByType(indicatorType);
                if (indicatorCount > 0)
                {
                    indicatorName += indicatorCount;
                }

                Indicator indicator = AddIndicator(indicatorType, indicatorName, chartPanel, true);
                string upColor = ConvertFromXmlEx("UpColor", node, string.Empty);
                string downColor = ConvertFromXmlEx("DownColor", node, string.Empty);
                if (upColor.Length > 0)
                {
                    indicator.UpColor = Utils.StringToColor(upColor);
                }

                if (downColor.Length > 0)
                {
                    indicator.DownColor = Utils.StringToColor(downColor);
                }

                indicator.StrokeThickness = ConvertFromXmlEx("StrokeThicknes", node, 1.0);
                indicator.StrokeColor = Utils.StringToColor(ConvertFromXmlEx("StrokeColor", node, string.Empty));
                indicator.Selectable = ConvertFromXmlEx("Selectable", node, true);
                indicator.Visible = ConvertFromXmlEx("Visible", node, true);
                indicator.StrokePattern = (LinePattern)Enum.Parse(typeof(LinePattern), ConvertFromXmlEx("StrokePattern", node, "Solid"));
                indicator.UserParams = ConvertFromXmlEx("UserParams", node, false);
                List<StockChartX_IndicatorsParameters.IndicatorParameter> indicatorParams =
                  StockChartX_IndicatorsParameters.GetIndicatorParameters(indicator.IndicatorType);

                for (int i = 0; i < indicatorParams.Count; i++)
                {
                    object oValue = ConvertFromXmlEx("Parameter_" + i, node, (object)null);

                    if (!ParamsWithSeriesName.Contains(indicatorParams[i].ParameterType))
                    {
                        var type = indicatorParams[i].ValueType;
                        if (type.IsEnum)
                            oValue = System.Enum.Parse(type, oValue.ToString(), true);
                        else
                            oValue = oValue == null ? 0 : Convert.ChangeType(oValue, indicatorParams[i].ValueType, CultureInfo.InvariantCulture);

                        indicator.SetParameterValue(i, oValue);
                    }
                    else
                    {
                        //is a property that has series name 
                        Debug.Assert(indicatorParams[i].ValueType == typeof(string));
                        string sValue = oValue.ToString();
                        //try to to check for series typr
                        //string[] values = sValue.Split('.');
                        int idx = sValue.IndexOf(".open", StringComparison.InvariantCultureIgnoreCase);
                        if (idx == -1) idx = sValue.IndexOf(".high", StringComparison.InvariantCultureIgnoreCase);
                        if (idx == -1) idx = sValue.IndexOf(".low", StringComparison.InvariantCultureIgnoreCase);
                        if (idx == -1) idx = sValue.IndexOf(".close", StringComparison.InvariantCultureIgnoreCase);
                        if (idx == -1) idx = sValue.IndexOf(".volume", StringComparison.InvariantCultureIgnoreCase);
                        if (idx == -1) idx = sValue.IndexOf(".", StringComparison.InvariantCultureIgnoreCase);
                        if (idx != -1) //pointing to a non-existing series
                        {
                            if (string.IsNullOrEmpty(Symbol))
                            {
                                Symbol = sValue.Substring(idx);
                            }

                            string source = Symbol + "." + sValue.Substring(idx + 1);
                            Series s = GetSeriesByName(source);
                            if (s == null)
                            {
                                source = Symbol + ".close";
                                s = GetSeriesByName(source);
                                if (s == null && PanelsCount > 0)
                                {
                                    if (_panelsContainer.Panels[0].SeriesCount > 0)
                                    {
                                        s = _panelsContainer.Panels[0].SeriesCollection.First();
                                    }
                                }
                            }

                            if (s != null)
                            {
                                source = s.FullName;
                            }

                            indicator.SetParameterValue(i, source);

                        }
                        else
                        {
                            indicator.SetParameterValue(i, sValue);
                        }
                    }
                }
            }

            _recalc = true;
        }

        private void SaveLineStudies(XmlNode xRoot, XmlDocument xmlDocument)
        {
            XmlNode xmlLineStudies = xmlDocument.CreateElement("LineStudies");
            xRoot.AppendChild(xmlLineStudies);

            foreach (ChartPanel chartPanel in _panelsContainer.Panels)
            {
                foreach (LineStudy study in chartPanel._lineStudies)
                {
                    XmlNode xmlLineStudy = xmlDocument.CreateElement("LineStudy");
                    xmlLineStudies.AppendChild(xmlLineStudy);

                    ConvertToXmlEx("Key", xmlLineStudy, xmlDocument, study.Key);
                    ConvertToXmlEx("StudyType", xmlLineStudy, xmlDocument, study.StudyType);
                    ConvertToXmlEx("PanelIndex", xmlLineStudy, xmlDocument, study._chartPanel.Index);
                    ConvertToXmlEx("X1Value", xmlLineStudy, xmlDocument, study.X1Value);
                    ConvertToXmlEx("X2Value", xmlLineStudy, xmlDocument, study.X2Value);
                    ConvertToXmlEx("Y1Value", xmlLineStudy, xmlDocument, study.Y1Value);
                    ConvertToXmlEx("Y2Value", xmlLineStudy, xmlDocument, study.Y2Value);
                    ConvertToXmlEx("Selectable", xmlLineStudy, xmlDocument, study.Selectable);
                    ConvertToXml("StrokeColor", xmlLineStudy, xmlDocument, study.Stroke);
                    ConvertToXmlEx("StrokeThickness", xmlLineStudy, xmlDocument, study.StrokeThickness);
                    ConvertToXmlEx("StrokeType", xmlLineStudy, xmlDocument, study.StrokeType);
                    ConvertToXmlEx("ExtraArgs", xmlLineStudy, xmlDocument, ConvertToBinary(study.ExtraArgs));
                    ConvertToXmlEx("Opacity", xmlLineStudy, xmlDocument, study.Opacity);

                    if (study is IShapeAble)
                    {
                        IShapeAble shapeAble = study as IShapeAble;
                        ConvertToXml("Fill", xmlLineStudy, xmlDocument, shapeAble.Fill);
                    }
                }
            }
        }

        private void ReadLineStudies(XmlNode xRoot)
        {
            XmlNode xmlLineStudies = xRoot.SelectSingleNode("LineStudies");
            if (xmlLineStudies == null) return;
            int useablePanelsCount = UseablePanelsCount;

            foreach (XmlNode node in xmlLineStudies.ChildNodes)
            {
                if (node.Name != "LineStudy") return;

                int panelIndex = ConvertFromXmlEx("PanelIndex", node, -1);
                if (panelIndex == -1) continue;
                if (panelIndex >= useablePanelsCount) panelIndex = 0;

                string key = ConvertFromXmlEx("Key", node, String.Empty);
                string studyTypeS = ConvertFromXmlEx("StudyType", node, String.Empty);
                LineStudy.StudyTypeEnum studyType =
                  (LineStudy.StudyTypeEnum)Enum.Parse(typeof(LineStudy.StudyTypeEnum), studyTypeS);
                Brush stroke = (Brush)ConvertFromXml("StrokeColor", node);
                object[] extrArgs = ConvertFromBinary(ConvertFromXmlEx("ExtraArgs", node, string.Empty));

                LineStudy lineStudy = CreateLineStudy(studyType, key, stroke, panelIndex);
                lineStudy.SetArgs(extrArgs);

                double x1Value = ConvertFromXmlEx("X1Value", node, 0.0);
                double x2Value = ConvertFromXmlEx("X2Value", node, 0.0);
                double y1Value = ConvertFromXmlEx("Y1Value", node, 0.0);
                double y2Value = ConvertFromXmlEx("Y2Value", node, 0.0);
                lineStudy.SetXYValues(x1Value, y1Value, x2Value, y2Value);
                lineStudy._selectable = ConvertFromXmlEx("Selectable", node, false);
                lineStudy.StrokeThickness = ConvertFromXmlEx("StrokeThickness", node, 1.0);
                lineStudy.StrokeType =
                  (LinePattern)Enum.Parse(typeof(LinePattern), ConvertFromXmlEx("StrokeType", node, LinePattern.Solid.ToString()));
                lineStudy.Opacity = ConvertFromXmlEx("Opacity", node, 1.0);

                if (lineStudy is IShapeAble)
                {
                    IShapeAble shapeAble = (IShapeAble)lineStudy;
                    shapeAble.Fill = (Brush)ConvertFromXml("Fill", node);
                }
            }
        }

        private static void ConvertToXml<T>(string name, XmlNode xRoot, XmlDocument xmlDocument, T value)
        {
            XmlElement xmlElement = xmlDocument.CreateElement(name);
            xRoot.AppendChild(xmlElement);
            xmlElement.InnerText = XamlWriter.Save(value);
            return;
        }

        private static void ConvertToXmlEx<T>(string name, XmlNode xRoot, XmlDocument xmlDocument, T value)
        {
            XmlElement xmlElement = xmlDocument.CreateElement(name);
            xRoot.AppendChild(xmlElement);
            xmlElement.InnerText = Convert.ToString(value, CultureInfo.InvariantCulture);
            return;
        }

        private static T ConvertFromXmlEx<T>(string name, XmlNode xRoot, T defValue) /*where T: IConvertible*/
        {
            XmlNode xmlElement = xRoot.SelectSingleNode(name);
            if (xmlElement == null) return defValue;

            try
            {
                return (T)Convert.ChangeType(xmlElement.InnerText, typeof(T), CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                return defValue;
            }
        }

        private static object ConvertFromXml(string name, XmlNode xRoot)
        {
            XmlNode xmlElement = xRoot.SelectSingleNode(name);
            if (xmlElement == null) return null;

            try
            {
                StringReader stringReader = new StringReader(xmlElement.InnerText);
                XmlReader xmlReader = XmlReader.Create(stringReader);
                return XamlReader.Load(xmlReader);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw;
            }
        }

        private static string ConvertToBinary(object obj)
        {
            if (obj == null)
                return string.Empty;
            IFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, obj);
            return Convert.ToBase64String(stream.GetBuffer(), Base64FormattingOptions.InsertLineBreaks);
        }

        private static object[] ConvertFromBinary(string base64Text)
        {
            if (string.IsNullOrEmpty(base64Text))
                return null;
            IFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            byte[] data = Convert.FromBase64String(base64Text);
            stream.Write(data, 0, data.Length);
            stream.Seek(0, SeekOrigin.Begin);
            var res = formatter.Deserialize(stream);
            return (object[])res;
        }

        /// <summary>
        /// Saves the chart as an image. The type of image is taken from filename extension.
        /// The types of format supported are
        /// 1. png
        /// 2. jpg, jpeg
        /// 3. gif
        /// 4. bmp
        /// </summary>
        /// <param name="filename">Image file name</param>
        public void SaveAsImage(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException("filename");

            int widthLocal = (int)ActualWidth;
            int heightLocal = (int)ActualHeight;
            RenderTargetBitmap bmp = new RenderTargetBitmap(widthLocal, heightLocal, 96, 96, PixelFormats.Pbgra32);
            foreach (var panel in _panelsContainer.Panels)
            {
                bmp.Render(panel);
            }
            bmp.Render(_calendar);
            //bmp.Render(_chartScroller);
            bmp.Render(_scroller);

            string extension = Path.GetExtension(filename);
            if (string.IsNullOrEmpty(extension))
            {
                throw new ArgumentException("File name is not valid. Extension not supplied.");
            }

            BitmapEncoder encoder;
            switch (extension.ToLower())
            {
                case ".gif":
                    encoder = new GifBitmapEncoder();
                    break;
                case ".png":
                    encoder = new PngBitmapEncoder();
                    break;
                case ".jpg":
                case ".jpeg":
                    encoder = new JpegBitmapEncoder();
                    break;
                case ".bmp":
                    encoder = new BmpBitmapEncoder();
                    break;
                default:
                    throw new ArgumentException("filename");
            }

            encoder.Frames.Add(BitmapFrame.Create(bmp));

            using (Stream stream = File.Create(filename))
            {
                encoder.Save(stream);
            }
        }
    }
}
