using Easychart.Finance;
using Easychart.Finance.DataProvider;
using System.Reflection;
using System.Runtime.CompilerServices;
[assembly: AssemblyVersion("1.0.0.135")]
namespace FML
{
	#region Formula Group Trend Indicators
	public class BBI:FormulaBase
	{
		public double N1=0;
		public double N2=0;
		public double N3=0;
		public double N4=0;
		public BBI():base()
		{
			AddParam("N1","3","1","100","",FormulaParamType.Double);
			AddParam("N2","6","1","100","",FormulaParamType.Double);
			AddParam("N3","12","1","100","",FormulaParamType.Double);
			AddParam("N4","24","1","100","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData NONAME0=(MA(C,N1)+MA(C,N2)+MA(C,N3)+MA(C,N4))/4;
			return new FormulaPackage(new FormulaData[]{NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "Bullish-Bearish Indicator";}
		}
	
		public override string Description
		{
			get{return "algorithm\r\n\r\nBBI = a weighted sum of the other indicators that have a bullish trend.";}
		}
	} //class BBI

	public class CYS:FormulaBase
	{
		public double P1=0;
		public double P2=0;
		public CYS():base()
		{
			AddParam("P1","4","1","15","",FormulaParamType.Double);
			AddParam("P2","5","1","15","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData VAR1=IF(YEAR>=2010 & MONTH>=2,0,1); VAR1.Name="Var1";
			FormulaData VAR2=VOL*C; VAR2.Name="Var2";
			FormulaData VAR3=EMA(VAR2,13)/EMA(VOL,13); VAR3.Name="Var3";
			FormulaData CYS= (EMA(CLOSE,P1)-VAR3)/VAR3*100*VAR1; CYS.Name="CYS";
			FormulaData ML= EMA(CYS,P2)*VAR1; ML.Name="ML";
			FormulaData LO= 0; LO.Name="LO";LO.SetAttrs(" POINTDOT");
			return new FormulaPackage(new FormulaData[]{CYS,ML,LO},"");
		}
	
		public override string LongName
		{
			get{return "";}
		}
	
		public override string Description
		{
			get{return "This indicator shows the percentage of buyer which earn money.";}
		}
	} //class CYS

	public class DDI:FormulaBase
	{
		public double N=0;
		public double N1=0;
		public double M=0;
		public double M1=0;
		public DDI():base()
		{
			AddParam("N","13","1","100","",FormulaParamType.Double);
			AddParam("N1","30","1","100","",FormulaParamType.Double);
			AddParam("M","10","1","100","",FormulaParamType.Double);
			AddParam("M1","5","1","100","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData TR=MAX(ABS(H-REF(H,1)),ABS(L-REF(L,1))); TR.Name="TR";
			FormulaData DMZ=IF((H+L)<=(REF(H,1)+REF(L,1)),0,MAX(ABS(H-REF(H,1)),ABS(L-REF(L,1)))); DMZ.Name="DMZ";
			FormulaData DMF=IF((H+L)>=(REF(H,1)+REF(L,1)),0,MAX(ABS(H-REF(H,1)),ABS(L-REF(L,1)))); DMF.Name="DMF";
			FormulaData DIZ=SUM(DMZ,N)/(SUM(DMZ,N)+SUM(DMF,N)); DIZ.Name="DIZ";
			FormulaData DIF=SUM(DMF,N)/(SUM(DMF,N)+SUM(DMZ,N)); DIF.Name="DIF";
			FormulaData DDI=DIZ-DIF; DDI.Name="DDI";DDI.SetAttrs("COLORSTICK");
			FormulaData ADDI=SMA(DDI,N1,M); ADDI.Name="ADDI";
			FormulaData AD=MA(ADDI,M1); AD.Name="AD";
			
			return new FormulaPackage(new FormulaData[]{DDI,ADDI,AD},"");
		}
	
		public override string LongName
		{
			get{return "Directional Divergence Index";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class DDI

	public class DMA:FormulaBase
	{
		public double SHORT=0;
		public double LONG=0;
		public double M=0;
		public DMA():base()
		{
			AddParam("SHORT","10","2","300","",FormulaParamType.Double);
			AddParam("LONG","50","2","300","",FormulaParamType.Double);
			AddParam("M","10","1","300","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData DDD= (MA(CLOSE,SHORT)-MA(CLOSE,LONG)); DDD.Name="DDD";
			FormulaData AMA= MA(DDD,M); AMA.Name="AMA";
			return new FormulaPackage(new FormulaData[]{DDD,AMA},"");
		}
	
		public override string LongName
		{
			get{return "Daily Moving Average";}
		}
	
		public override string Description
		{
			get{return "A moving average calculated based upon daily activity. A number preceding the abbreviation indicates the daily time period used to calculate the average.";}
		}
	} //class DMA

	public class DMI:FormulaBase
	{
		public double N=0;
		public double M=0;
		public DMI():base()
		{
			AddParam("N","14","2","100","",FormulaParamType.Double);
			AddParam("M","6","1","100","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData TR= SUM(MAX(MAX(HIGH-LOW,ABS(HIGH-REF(CLOSE,1))),ABS(LOW-REF(CLOSE,1))),N); TR.Name="TR";
			FormulaData HD= HIGH-REF(HIGH,1); HD.Name="HD";
			FormulaData LD= REF(LOW,1)-LOW; LD.Name="LD";
			FormulaData DMP= SUM(IF(HD>0 & HD>LD,HD,0),N); DMP.Name="DMP";
			FormulaData DMM= SUM(IF(LD>0 & LD>HD,LD,0),N); DMM.Name="DMM";
			FormulaData PDI= DMP*100/TR; PDI.Name="PDI";
			FormulaData MDI= DMM*100/TR; MDI.Name="MDI";
			FormulaData ADX= MA(ABS(MDI-PDI)/(MDI+PDI)*100,M); ADX.Name="ADX";
			FormulaData ADXR=(ADX+REF(ADX,M))/2; ADXR.Name="ADXR";
			return new FormulaPackage(new FormulaData[]{PDI,MDI,ADX,ADXR},"");
		}
	
		public override string LongName
		{
			get{return "Directional Movement Index";}
		}
	
		public override string Description
		{
			get{return "Directional movement is a system for providing trading signals to be used for price breaks from a trading range.  The system involves 5 indicators which are the Directional Movement Index (DX), the plus Directional Indicator (+DI), the minus Directional Indicator (-DI), the average Directional Movement (ADX) and the Directional movement rating (ADXR).  The system was developed J. Welles Wilder and is explained thoroughly in his book, New Concepts in Technical Trading Systems .\n\nThe basic Directional Movement Trading system involves plotting the 14day +DI and the 14 day -DI on top of each other.  When the +DI rises above the -DI, it is a bullish signal.  A bearish signal occurs when the +DI falls below the -DI.  To avoid whipsaws, Wilder identifies a trigger point to be the extreme price on the day the lines cross.  If you have received a buy signal, you would wait for the security to rise above the extreme price (the high price on the day the lines crossed).  If you are waiting for a sell signal the extreme point is then defined as the low price on the day's the line cross.";}
		}
	} //class DMI

	public class EMA4:FormulaBase
	{
		public double P1=0;
		public double P2=0;
		public double P3=0;
		public double P4=0;
		public EMA4():base()
		{
			AddParam("P1","5","1","300","",FormulaParamType.Double);
			AddParam("P2","10","1","300","",FormulaParamType.Double);
			AddParam("P3","20","1","300","",FormulaParamType.Double);
			AddParam("P4","60","1","300","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData MA1=EMA(CLOSE,P1); MA1.Name="MA1";
			FormulaData MA2=EMA(CLOSE,P2); MA2.Name="MA2";
			FormulaData MA3=EMA(CLOSE,P3); MA3.Name="MA3";
			FormulaData MA4=EMA(CLOSE,P4); MA4.Name="MA4";
			return new FormulaPackage(new FormulaData[]{MA1,MA2,MA3,MA4},"");
		}
	
		public override string LongName
		{
			get{return "Exponential Moving Averages";}
		}
	
		public override string Description
		{
			get{return "An exponential moving average (EMA) is calculated by adding a percentage of yesterday's moving average to a percentage of today's closing value.  In this way an investor can put more emphasis on more recent data and less weight on past data in the calculation of the moving average.";}
		}
	} //class EMA4

	public class MA4:FormulaBase
	{
		public double P1=0;
		public double P2=0;
		public double P3=0;
		public double P4=0;
		public MA4():base()
		{
			AddParam("P1","5","0","300","",FormulaParamType.Double);
			AddParam("P2","10","0","300","",FormulaParamType.Double);
			AddParam("P3","20","0","300","",FormulaParamType.Double);
			AddParam("P4","30","0","300","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData MA1=MA(CLOSE,P1); MA1.Name="MA1";
			FormulaData MA2=MA(CLOSE,P2); MA2.Name="MA2";
			FormulaData MA3=MA(CLOSE,P3); MA3.Name="MA3";
			FormulaData MA4=MA(CLOSE,P4); MA4.Name="MA4";
			return new FormulaPackage(new FormulaData[]{MA1,MA2,MA3,MA4},"");
		}
	
		public override string LongName
		{
			get{return "Moving Averages";}
		}
	
		public override string Description
		{
			get{return "Moving averages are used to help identify the trend of prices.  By creating an average of prices, that \"moves\" with the addition of new data, the price action on the security being analyzed is \"smoothed\".  In other words, by calculating the average value of a underlying security or indicator, day to day fluctuations are reduced in importance and what remains is a stronger indication of the trend of prices over the period being analyzed.";}
		}
	} //class MA4

	public class MACD:FormulaBase
	{
		public double LONG=0;
		public double SHORT=0;
		public double M=0;
		public MACD():base()
		{
			AddParam("LONG","26","20","100","",FormulaParamType.Double);
			AddParam("SHORT","12","5","40","",FormulaParamType.Double);
			AddParam("M","9","2","60","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData DIFF= EMA(CLOSE,SHORT) - EMA(CLOSE,LONG); DIFF.Name="DIFF";
			FormulaData DEA= EMA(DIFF,M); DEA.Name="DEA";
			FormulaData MACD= 2*(DIFF-DEA); MACD.Name="MACD";MACD.SetAttrs(" COLORSTICK");
			
			return new FormulaPackage(new FormulaData[]{DIFF,DEA,MACD},"");
		}
	
		public override string LongName
		{
			get{return "Moving Average Convergence Divergence";}
		}
	
		public override string Description
		{
			get{return "The Moving Average Convergence/Divergence indicator (MACD) is calculated by subtracting the value of a 26-period exponential moving average from a 12-period exponential moving average (EMA). A 9-period dotted exponential moving average (the \"signal line\") of the difference between the 26 and 12 period EMA is used as the signal line.\n\nThe basic MACD trading rule is to sell when the MACD falls below its 9 day signal line and to buy when the MACD rises above the 9 day signal line.  Traders sometimes vary the calculation period of the signal line and may use different moving average lengths in calculating the MACD dependent on the security and trading strategy.";}
		}
	} //class MACD

	public class MTM:FormulaBase
	{
		public double N=0;
		public double N1=0;
		public MTM():base()
		{
			AddParam("N","6","1","100","",FormulaParamType.Double);
			AddParam("N1","6","1","100","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData MTM= CLOSE-REF(CLOSE,N); MTM.Name="MTM";
			FormulaData MTMMA= MA(MTM,N1); MTMMA.Name="MTMMA";
			return new FormulaPackage(new FormulaData[]{MTM,MTMMA},"");
		}
	
		public override string LongName
		{
			get{return "Momentum";}
		}
	
		public override string Description
		{
			get{return "The Momentum is simply the difference between the current point (price or something else) and the point N periods ago.\n\nUsage:\n\nThe rising line signals that the uptrend is getting stronger, the horizontal line at zero level means there is no trend, and falling line means the downtrend is getting stronger.\n\nThe momentum can be used for identifying trends, overbought/oversold conditions and divergences.\n";}
		}
	} //class MTM

	public class PPO:FormulaBase
	{
		public double LONG=0;
		public double SHORT=0;
		public double N=0;
		public PPO():base()
		{
			AddParam("LONG","26","5","100","",FormulaParamType.Double);
			AddParam("SHORT","12","2","40","",FormulaParamType.Double);
			AddParam("N","9","1","100","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData PPO=(EMA(CLOSE,SHORT)-EMA(CLOSE,LONG))/EMA(CLOSE,LONG)*100; PPO.Name="PPO";
			FormulaData NONAME0=EMA(PPO,N);
			return new FormulaPackage(new FormulaData[]{PPO,NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "Price Oscillator";}
		}
	
		public override string Description
		{
			get{return "An indicator which attempts to assess the momentum of price activity by the use of two or more moving averages, for a predefined time frame period.";}
		}
	} //class PPO

	public class SAR:FormulaBase
	{
		public double N=0;
		public double STEP=0;
		public double MAXP=0;
		public SAR():base()
		{
			AddParam("N","10","1","100","",FormulaParamType.Double);
			AddParam("STEP","2","1","100","",FormulaParamType.Double);
			AddParam("MAXP","20","5","100","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData NONAME0=SAR(N,STEP,MAXP);NONAME0.SetAttrs("CIRCLEDOT");
			return new FormulaPackage(new FormulaData[]{NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "Stop and Reverse";}
		}
	
		public override string Description
		{
			get{return "Parabolic Stop and Release was introduced by J Welles Wilder.  The Parabolic SAR is a trend following indicator that is designed to create a trailing stop.  This is a point that follows a prevailing trend, giving a possible value for a stop loss order that is far enough away from the original trend to avoid being stopped out with a small consolidation and retraction moves.  The trailing stop moves with the trend, accelerating closer to price action as time passes giving the path of the indicator a parabolic look.  When price penetrates the SAR a new calculation is started taking the other side of the market with an initial setting that again allows a certain amount of initial volatility if the trend is slow to get underway.";}
		}
	} //class SAR

	public class TRIX:FormulaBase
	{
		public double N=0;
		public double M=0;
		public TRIX():base()
		{
			AddParam("N","12","3","100","",FormulaParamType.Double);
			AddParam("M","20","1","100","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData TR= EMA(EMA(EMA(CLOSE,N),N),N); TR.Name="TR";
			FormulaData TRIX= (TR-REF(TR,1))/REF(TR,1)*100; TRIX.Name="TRIX";
			FormulaData TRMA=  MA(TRIX,M); TRMA.Name="TRMA";
			
			return new FormulaPackage(new FormulaData[]{TRIX,TRMA},"");
		}
	
		public override string LongName
		{
			get{return "";}
		}
	
		public override string Description
		{
			get{return "TRIX is a momentum indicator that displays the percent rate-of-change of a triple exponentially smoothed moving average of the security's closing price. It is designed to keep you in trends equal to or shorter than the number of periods you specify.\n\n";}
		}
	} //class TRIX

	public class VMACD:FormulaBase
	{
		public double SHORT=0;
		public double LONG=0;
		public double M=0;
		public VMACD():base()
		{
			AddParam("SHORT","12","1","50","",FormulaParamType.Double);
			AddParam("LONG","26","20","100","",FormulaParamType.Double);
			AddParam("M","9","30","50","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData DIFF= EMA(VOL,SHORT) - EMA(VOL,LONG); DIFF.Name="DIFF";
			FormulaData DEA= EMA(DIFF,M); DEA.Name="DEA";
			FormulaData MACD= DIFF-DEA; MACD.Name="MACD";MACD.SetAttrs(" COLORSTICK");
			return new FormulaPackage(new FormulaData[]{DIFF,DEA,MACD},"");
		}
	
		public override string LongName
		{
			get{return "Volumn Moving Average Convergence Divergence";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class VMACD

	public class ZIG:FormulaBase
	{
		public double PER=0;
		public ZIG():base()
		{
			AddParam("PER","10","1","60","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData NONAME0=ZIG(PER);NONAME0.SetAttrs("Width2");
			return new FormulaPackage(new FormulaData[]{NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "Zig Zag";}
		}
	
		public override string Description
		{
			get{return "Zig Zag is trend following indicator that helps define what the trend has been, and can be used as a significance test to help determine when changes in the current price might indicate when the trend of price might be changing.  The zig zag indicators filters out changes in a data item that are less than a specific amount that you define.  Below is a chart of National Semiconductor.  If you bought every time the zig zag moved up and sold every time the zig zag moved down, every trade would be a winner.\n\n";}
		}
	} //class ZIG

	public class ADX:FormulaBase
	{
		public double N=0;
		public ADX():base()
		{
			AddParam("N","14","1","100","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData TR= SUM(MAX(MAX(HIGH-LOW,ABS(HIGH-REF(CLOSE,1))),ABS(LOW-REF(CLOSE,1))),N); TR.Name="TR";
			FormulaData HD= HIGH-REF(HIGH,1); HD.Name="HD";
			FormulaData LD= REF(LOW,1)-LOW; LD.Name="LD";
			FormulaData DMP= SUM(IF(HD>0 & HD>LD,HD,0),N); DMP.Name="DMP";
			FormulaData DMM= SUM(IF(LD>0 & LD>HD,LD,0),N); DMM.Name="DMM";
			FormulaData PDI= DMP*100/TR; PDI.Name="PDI";
			SETNAME(PDI,"+DI");
			FormulaData MDI= DMM*100/TR; MDI.Name="MDI";
			SETNAME(MDI,"-DI");
			FormulaData ADX= MA(ABS(MDI-PDI)/(MDI+PDI)*100,N); ADX.Name="ADX";ADX.SetAttrs("Width2");
			
			return new FormulaPackage(new FormulaData[]{PDI,MDI,ADX},"");
		}
	
		public override string LongName
		{
			get{return "Average Directional Index";}
		}
	
		public override string Description
		{
			get{return "J. Welles Wilder Jr. developed the Average Directional Index (ADX) in order to evaluate the strength of the current trend, be it up or down. It's important to detemine whether the market is trending or trading (moving sideways), because certain indicators give more useful results depending on the market doing one or the other.\n\n";}
		}
	} //class ADX

	public class PERF:FormulaBase
	{
		public PERF():base()
		{
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData NONAME0=(C-C.FIRST)/C.FIRST*100;
			return new FormulaPackage(new FormulaData[]{NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "Performance";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class PERF

	#endregion

	#region Formula Group Momentum Indicators
	public class B3612:FormulaBase
	{
		public B3612():base()
		{
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData B36= MA(CLOSE,3)-MA(CLOSE,6); B36.Name="B36";
			FormulaData B612= MA(CLOSE,6)-MA(CLOSE,12); B612.Name="B612";
			return new FormulaPackage(new FormulaData[]{B36,B612},"");
		}
	
		public override string LongName
		{
			get{return "Bias3-Bias6 and Bias6-Bias12";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class B3612

	public class BIAS:FormulaBase
	{
		public double L1=0;
		public double L2=0;
		public double L3=0;
		public BIAS():base()
		{
			AddParam("L1","6","1","300","",FormulaParamType.Double);
			AddParam("L2","12","1","300","",FormulaParamType.Double);
			AddParam("L3","24","1","300","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData BIAS1= (CLOSE-MA(CLOSE,L1))/MA(CLOSE,L1)*100; BIAS1.Name="BIAS1";
			FormulaData BIAS2= (CLOSE-MA(CLOSE,L2))/MA(CLOSE,L2)*100; BIAS2.Name="BIAS2";
			FormulaData BIAS3= (CLOSE-MA(CLOSE,L3))/MA(CLOSE,L3)*100; BIAS3.Name="BIAS3";
			return new FormulaPackage(new FormulaData[]{BIAS1,BIAS2,BIAS3},"");
		}
	
		public override string LongName
		{
			get{return "BIAS";}
		}
	
		public override string Description
		{
			get{return "Show the distance of close and moving average.";}
		}
	} //class BIAS

	public class CCI:FormulaBase
	{
		public double N=0;
		public CCI():base()
		{
			AddParam("N","14","2","100","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData TYP= (HIGH + LOW + CLOSE)/3; TYP.Name="TYP";
			FormulaData NONAME0=(TYP-MA(TYP,N))/(0.015*AVEDEV(TYP,N));
			return new FormulaPackage(new FormulaData[]{NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "Commodity Channel Index";}
		}
	
		public override string Description
		{
			get{return "The Commodity Channel Index measures the position of price in relation to its moving average. This can be used to highlight when the market is overbought/oversold or to signal when a trend is weakening. The indicator is similar in concept to Bollinger Bands but is presented as an indicator line rather than as overbought/oversold levels.\n\nThe Commodity Channel Index was developed by Donald Lambert and is outlined in his book Commodities Channel Index: Tools for Trading Cyclic Trends.\n\n";}
		}
	} //class CCI

	public class DBCD:FormulaBase
	{
		public double N=0;
		public double M=0;
		public double T=0;
		public DBCD():base()
		{
			AddParam("N","5","1","100","",FormulaParamType.Double);
			AddParam("M","16","1","100","",FormulaParamType.Double);
			AddParam("T","76","1","100","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData BIAS=(C-MA(C,N))/MA(C,N); BIAS.Name="BIAS";
			FormulaData DIF=(BIAS-REF(BIAS,M)); DIF.Name="DIF";
			FormulaData DBCD=SMA(DIF,T,1); DBCD.Name="DBCD";
			FormulaData MM=MA(DBCD,5); MM.Name="MM";
			return new FormulaPackage(new FormulaData[]{DBCD,MM},"");
		}
	
		public override string LongName
		{
			get{return "Bias Convergence Divergence";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class DBCD

	public class DPO:FormulaBase
	{
		public DPO():base()
		{
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData NONAME0=C - REF(MA(CLOSE,20),11);
			return new FormulaPackage(new FormulaData[]{NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "Detrended Price Oscillator";}
		}
	
		public override string Description
		{
			get{return "Detrended Price Oscillator compares closing price to a prior moving average, eliminating cycles longer than the moving average.\n\nThe real power of the Detrended Price Oscillator is in identifying turning points in longer cycles:\n\nWhen Detrended Price Oscillator shows a higher trough - expect an upturn in the intermediate cycle;\nWhen Detrended Price Oscillator experiences a lower peak - expect a downturn.\n";}
		}
	} //class DPO

	public class FastSTO:FormulaBase
	{
		public double N=0;
		public double M=0;
		public FastSTO():base()
		{
			AddParam("N","14","1","100","",FormulaParamType.Double);
			AddParam("M","3","2","40","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData K=(CLOSE-LLV(LOW,N))/(HHV(HIGH,N)-LLV(LOW,N))*100; K.Name="K";
			FormulaData D=MA(K,M); D.Name="D";
			return new FormulaPackage(new FormulaData[]{K,D},"");
		}
	
		public override string LongName
		{
			get{return "Fast Stochastic";}
		}
	
		public override string Description
		{
			get{return "The Stochastic Oscillator is a measure of the relative momentum of current prices to previous closing prices within a given interval.  When it is plotted, it is two lines that move within a range of 0 and 100.  Values above 80 are considered to be in overbought territory giving an indication that a reversal in price is possible.  Values below 20 are considered oversold and again are an indication that a reversal of the price trend is a higher risk.  In a strong trending environment, the Stochastic Oscillator can stay in overbought or oversold territory for some time while price continues in a single direction.  In relation to a longer term price trend environment, the stochastic provides little interest.  In its construction it is meant to relate the current periods momentum to the most recent previous periods of momentum in price in an attempt to identify periods where momentum may be easing or increasing.  The easing (at a top) or increase (at a bottom) of momentum occurs at reversal points for the price trend being measured.  However changing momentum also occurs during times when there is no change in the overall trend in prices and should be understood as a period when a reversal in price trend is possible but not guaranteed.";}
		}
	} //class FastSTO

	public class LWR:FormulaBase
	{
		public double N=0;
		public double M1=0;
		public double M2=0;
		public LWR():base()
		{
			AddParam("N","9","1","100","",FormulaParamType.Double);
			AddParam("M1","3","2","40","",FormulaParamType.Double);
			AddParam("M2","3","2","40","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData RSV= (HHV(HIGH,N)-CLOSE)/(HHV(HIGH,N)-LLV(LOW,N))*100; RSV.Name="RSV";
			FormulaData LWR1=SMA(RSV,M1,1); LWR1.Name="LWR1";
			FormulaData LWR2=SMA(LWR1,M2,1); LWR2.Name="LWR2";
			return new FormulaPackage(new FormulaData[]{LWR1,LWR2},"");
		}
	
		public override string LongName
		{
			get{return "";}
		}
	
		public override string Description
		{
			get{return "100-Stochastic";}
		}
	} //class LWR

	public class ROC:FormulaBase
	{
		public double N=0;
		public double M=0;
		public ROC():base()
		{
			AddParam("N","12","1","100","",FormulaParamType.Double);
			AddParam("M","6","1","50","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData ROC=(CLOSE-REF(CLOSE,N))/REF(CLOSE,N)*100; ROC.Name="ROC";
			FormulaData ROCMA=MA(ROC,M); ROCMA.Name="ROCMA";
			return new FormulaPackage(new FormulaData[]{ROC,ROCMA},"");
		}
	
		public override string LongName
		{
			get{return "Price Rate-of-Change";}
		}
	
		public override string Description
		{
			get{return "ROC is a refinement of Momentum - readings fluctuate as percentages around the zero line. Further details are given at Construction.\n\nThe indicator is designed for use in ranging markets - to detect trend weakness and likely reversal points. However, when combined with a trend indicator, it can be used in trending markets.\n\nRanging Markets\nFirst, you will need to set overbought and oversold levels based on your observation of past ranging markets. The levels should cut across at least two-thirds of the peaks and troughs.\n\nGo long when ROC crosses to below the oversold level and then rises back above it.\nGo long on bullish divergences - where the first trough is below the oversold level.\nGo short when ROC crosses to above the overbought level and then falls back below it.\nGo short on a bearish divergence - with the first peak above the overbought level.\n";}
		}
	} //class ROC

	public class RSI:FormulaBase
	{
		public double N1=0;
		public RSI():base()
		{
			AddParam("N1","14","2","100","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData LC= REF(CLOSE,1); LC.Name="LC";
			FormulaData RSI=SMA(MAX(CLOSE-LC,0),N1,1)/SMA(ABS(CLOSE-LC),N1,1)*100; RSI.Name="RSI";
			
			return new FormulaPackage(new FormulaData[]{RSI},"");
		}
	
		public override string LongName
		{
			get{return "Relative Strength Index";}
		}
	
		public override string Description
		{
			get{return "The Relative Strength Index was introduced by Welles Wilder. It is an indicator for overbought / oversold conditions. It is going up when the market is strong, and down, when the market is weak. The oscillation range is between 0 and 100.\n\nThe indicator is non-linear, it is moving faster in the middle of its range, and slower - in the overbought / oversold territory.\n\nThe RSI should not be confused with the relative strength indicator which is used to compare stocks to each other.\n\n";}
		}
	} //class RSI

	public class SI:FormulaBase
	{
		public SI():base()
		{
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData LC=REF(C,1); LC.Name="LC";
			FormulaData AA=ABS(H-LC); AA.Name="AA";
			FormulaData BB=ABS(L-LC); BB.Name="BB";
			FormulaData CC=ABS(H-REF(L,1)); CC.Name="CC";
			FormulaData DD=ABS(LC-REF(O,1)); DD.Name="DD";
			FormulaData R=IF(AA>BB & AA>CC,AA+BB/2+DD/4,IF(BB>CC & BB>AA,BB+AA/2+DD/4,CC+DD/4)); R.Name="R";
			FormulaData X=(C-LC+(C-O)/2+LC-REF(O,1)); X.Name="X";
			FormulaData SI=16*X/R*MAX(AA,BB); SI.Name="SI";
			return new FormulaPackage(new FormulaData[]{SI},"");
		}
	
		public override string LongName
		{
			get{return "";}
		}
	
		public override string Description
		{
			get{return "Swing Index\n";}
		}
	} //class SI

	public class SlowSTO:FormulaBase
	{
		public double N=0;
		public double M1=0;
		public double M2=0;
		public SlowSTO():base()
		{
			AddParam("N","14","1","100","",FormulaParamType.Double);
			AddParam("M1","3","2","50","",FormulaParamType.Double);
			AddParam("M2","3","2","50","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData A=(CLOSE-LLV(LOW,N))/(HHV(HIGH,N)-LLV(LOW,N))*100; A.Name="A";
			FormulaData K=MA(A,M1); K.Name="K";
			FormulaData D=MA(K,M2); D.Name="D";
			return new FormulaPackage(new FormulaData[]{K,D},"");
		}
	
		public override string LongName
		{
			get{return "Slow Stochastic";}
		}
	
		public override string Description
		{
			get{return "The Slow Stochastic applies further smoothing to the Stochastic oscillator, to reduce volatility and improve signal accuracy.";}
		}
	} //class SlowSTO

	public class SRDM:FormulaBase
	{
		public double N=0;
		public SRDM():base()
		{
			AddParam("N","30","1","100","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData DMZ=IF((H+L)<=(REF(H,1)+REF(L,1)),0,MAX(ABS(H-REF(H,1)),ABS(L-REF(L,1)))); DMZ.Name="DMZ";
			FormulaData DMF=IF((H+L)>=(REF(H,1)+REF(L,1)),0,MAX(ABS(H-REF(H,1)),ABS(L-REF(L,1)))); DMF.Name="DMF";
			FormulaData ADMZ=MA(DMZ,10); ADMZ.Name="ADMZ";
			FormulaData ADMF=MA(DMF,10); ADMF.Name="ADMF";
			FormulaData SRDM=IF(ADMZ>ADMF,(ADMZ-ADMF)/ADMZ,IF(ADMZ=ADMF,0,(ADMZ-ADMF)/ADMF)); SRDM.Name="SRDM";
			FormulaData ASRDM=SMA(SRDM,N,1); ASRDM.Name="ASRDM";
			return new FormulaPackage(new FormulaData[]{SRDM,ASRDM},"");
		}
	
		public override string LongName
		{
			get{return "SRDM";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class SRDM

	public class VROC:FormulaBase
	{
		public double N=0;
		public VROC():base()
		{
			AddParam("N","12","2","300","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData NONAME0=(VOL-REF(VOL,N))/REF(VOL,N)*100;
			return new FormulaPackage(new FormulaData[]{NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "Rate of Change (Volume)";}
		}
	
		public override string Description
		{
			get{return "Rate of Change Volume (ROCV) is an oscillator applied to volume rather than price and is calculated in the same manner as the Rate of Change (Price) indicator.\n\nROCV highlights increases in volume, which normally occur at most significant market tops, bottoms and breakouts.\n\n";}
		}
	} //class VROC

	public class VRSI:FormulaBase
	{
		public double N=0;
		public VRSI():base()
		{
			AddParam("N","6","1","100","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData NONAME0=SMA(MAX(VOL-REF(VOL,1),0),N,1)/SMA(ABS(VOL-REF(VOL,1)),N,1)*100;
			return new FormulaPackage(new FormulaData[]{NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "Volumn Relative Strength Index";}
		}
	
		public override string Description
		{
			get{return "Volumn RSI";}
		}
	} //class VRSI

	public class WR:FormulaBase
	{
		public double N=0;
		public WR():base()
		{
			AddParam("N","14","2","100","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData NONAME0=-100*(HHV(HIGH,N)-CLOSE)/(HHV(HIGH,N)-LLV(LOW,N));
			return new FormulaPackage(new FormulaData[]{NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "Williams %R";}
		}
	
		public override string Description
		{
			get{return "The The Williams %R indicator was introduced by Larry Williams. It is working by identifying the overbought / oversold levels. The scale extends from 0 to -100.\n\nUsage\nThe overbought level is considered 0 to -20, and oversold -70 to -100.\n\nAs a confirmation signal, we can wait for the indicator to cross the -50 line.\n";}
		}
	} //class WR

	public class SO:FormulaBase
	{
		public double N1=0;
		public double N2=0;
		public SO():base()
		{
			AddParam("N1","3","1","10","",FormulaParamType.Double);
			AddParam("N2","3","1","10","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData K=(C-L)/(H-L); K.Name="K";
			FormulaData SK= SMA(K,N1,1); SK.Name="SK";
			FormulaData SD= SMA(SK,N2,1); SD.Name="SD";
			return new FormulaPackage(new FormulaData[]{SK,SD},"");
		}
	
		public override string LongName
		{
			get{return "Stochastic Oscillator";}
		}
	
		public override string Description
		{
			get{return "The Stochastic Oscillator was introduced by George C. Lane. The indicator provides information about the location of a current close in relation to the period's high and low. The closer the close is to the period's high, the higher is the buying pressure, and the closer the close is to the period's low, the more selling pressure is.\n\nUsage:\n\nThe indicator is considered bullish, when above 80, and bearish, when below 20. As this definition does not provide any insights on when to buy or sell, consider generating signals when the indicator moves from the overbought / oversold territory back.\n\nThe crossings between the %K and its moving average can be used for the same purpose.\n\nFinally, the divergence can be considered a very strong signal. When the divergence develops when the indicator is moving from the overbought / oversold levels, it is a sell / buy signal.\n\nAdditionally, the K39 (unsmoothed 39 period stochastic oscillator) was reported to generate good results when tested on paper. A buy signal is generated when K crosses above 50% and the close price is above the previous week's highest close. Sell signals are generated when K crosses below 50% and the close is below the previous week's lowest close.\n\nAn additional confirmation can be provided by some indicators from the different group, for example, the On Balance Volume (OBV) indicator.\n\nThe most value of a stochastics is when the strong trend is present. According to Lane, the safest way to trade is to buy when the trend is up, and to sell with the downtrend.\n";}
		}
	} //class SO

	public class MFI:FormulaBase
	{
		public double N=0;
		public MFI():base()
		{
			AddParam("N","14","1","100","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData TP=(HIGH + LOW + CLOSE) / 3; TP.Name="TP";
			FormulaData LTP= REF(TP,1); LTP.Name="LTP";
			FormulaData MF=TP*V; MF.Name="MF";
			FormulaData PMF= IF(TP>LTP,MF,0); PMF.Name="PMF";
			FormulaData NMF= IF(TP<=LTP,MF,0); NMF.Name="NMF";
			FormulaData MR=MA(PMF,N)/MA(NMF,N); MR.Name="MR";
			FormulaData MFI=100-(100/(1+MR)); MFI.Name="MFI";
			FormulaData NONAME0=20;NONAME0.SetAttrs("Color#808080,NoValueLabel");
			FormulaData NONAME1=80;NONAME1.SetAttrs("Color#808080,NoValueLabel");
			FormulaData NONAME2=FILLRGN(MFI>80,MFI,80);NONAME2.SetAttrs("Brush#20808000");
			FormulaData NONAME3=FILLRGN(MFI<20,MFI,20);NONAME3.SetAttrs("Brush#20800000");
			SETHLINE(20,50,80,100);
			SETYMINMAX(0,100);
			return new FormulaPackage(new FormulaData[]{MFI,NONAME0,NONAME1,NONAME2,NONAME3},"");
		}
	
		public override string LongName
		{
			get{return "Money Flow";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class MFI

	public class StochRSI:FormulaBase
	{
		public double N=0;
		public StochRSI():base()
		{
			AddParam("N","14","1","100","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData LC= REF(CLOSE,1); LC.Name="LC";
			FormulaData RSI=SMA(MAX(CLOSE-LC,0),N,1)/SMA(ABS(CLOSE-LC),N,1)*100; RSI.Name="RSI";
			FormulaData NONAME0=(RSI-LLV(RSI,N))/(HHV(RSI,N)-LLV(RSI,N));
			return new FormulaPackage(new FormulaData[]{NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "StochRSI";}
		}
	
		public override string Description
		{
			get{return "Developed by Tushard Chande and Stanley Kroll, StochRSI is an oscillator that measures the level of RSI relative to its range, over a set period of time. The indicator uses RSI as the foundation and applies to it the formula behind Stochastics. The result is an oscillator that fluctuates between 0 and 1.";}
		}
	} //class StochRSI

	public class EOM:FormulaBase
	{
		public double N=0;
		public EOM():base()
		{
			AddParam("N","14","1","1000","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData MP=(H+L)/2; MP.Name="MP";
			FormulaData MM=MP-REF(MP,1); MM.Name="MM";
			FormulaData BR=V/(H-L)/1000000; BR.Name="BR";
			FormulaData EOM= MM/BR; EOM.Name="EOM";
			FormulaData NONAME0=MA(EOM,N);
			return new FormulaPackage(new FormulaData[]{EOM,NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "Ease of Movement";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class EOM

	#endregion

	#region Formula Group Volatility Indicators
	public class PSY:FormulaBase
	{
		public double N=0;
		public PSY():base()
		{
			AddParam("N","12","1","100","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData NONAME0=COUNT(CLOSE>REF(CLOSE,1),N)/N*100;
			return new FormulaPackage(new FormulaData[]{NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "Psychogical line";}
		}
	
		public override string Description
		{
			get{return "people resist paying more for a stock than others unless the stock continues to move up.  People resist selling a stock for less than the price others have been getting for it unless the price continues to decline.  People who purchase the stock at the top of a trading range have a strong inclination to wait until the price comes back before they get out.\n";}
		}
	} //class PSY

	public class VR:FormulaBase
	{
		public double N=0;
		public VR():base()
		{
			AddParam("N","26","5","300","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData LC=REF(CLOSE,1); LC.Name="LC";
			FormulaData NONAME0=SUM(IF(CLOSE>LC,VOL,0),N)/
SUM(IF(CLOSE<=LC,VOL,0),N)*100;
			return new FormulaPackage(new FormulaData[]{NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "Volatility Ratio";}
		}
	
		public override string Description
		{
			get{return "This ratio is derived from the Volatility Ratio introduced by Jack Schwager in Technical Analysis to identify wide-ranging days.\n\nDesigned to highlight breakouts from a trading range, this VR compared to true range for the indicator period.";}
		}
	} //class VR

	public class ATR:FormulaBase
	{
		public double N=0;
		public ATR():base()
		{
			AddParam("N","10","1","300","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData LC=REF(CLOSE,1); LC.Name="LC";
			FormulaData TR= MAX(HIGH-LOW,ABS(LC-HIGH),ABS(LC-LOW)); TR.Name="TR";
			FormulaData ATR= SMA(TR,N,1); ATR.Name="ATR";
			return new FormulaPackage(new FormulaData[]{ATR},"");
		}
	
		public override string LongName
		{
			get{return "Average True Range";}
		}
	
		public override string Description
		{
			get{return "The Average True Range indicator was created by J. Welles Wilder. Its primary use is for determining the volatility of the security. The idea is to replace the high - low interval for the given period, as the high-low does not take into consideration gaps and limit moves.";}
		}
	} //class ATR

	public class VOLATI:FormulaBase
	{
		public double N=0;
		public VOLATI():base()
		{
			AddParam("N","10","2","100","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData EC= EMA(HIGH-LOW,N); EC.Name="EC";
			FormulaData NONAME0=(EC-REF(EC,N))/REF(EC,N)*100;
			return new FormulaPackage(new FormulaData[]{NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "Chaikin's Volatility";}
		}
	
		public override string Description
		{
			get{return "Marc Chaikin measures volatility as the trading range between high and low for each period. This does not take trading gaps into account as Average True Range does.";}
		}
	} //class VOLATI

	public class Aroon:FormulaBase
	{
		public double N=0;
		public Aroon():base()
		{
			AddParam("N","25","0","1000","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData UP=(N-HHVBARS(C,N))/N*100; UP.Name="Up";
			FormulaData DOWN=(N-LLVBARS(C,N))/N*100; DOWN.Name="Down";
			return new FormulaPackage(new FormulaData[]{UP,DOWN},"");
		}
	
		public override string LongName
		{
			get{return "Aroon";}
		}
	
		public override string Description
		{
			get{return "The Aroon indicator system consists of two lines, 'Aroon(up)' and 'Aroon(down)'. It takes a single parameter which is the number of time periods to use in the calculation. Aroon(up) is the amount of time (on a percentage basis) that has elapsed between the start of the time period and the point at which the highest price during that time period occurred. If the stock closes at a new high for the given period, Aroon(up) will be +100. For each subsequent period that passes without another new high, Aroon(up) moves down by an amount equal to (1 / # of periods) x 100.";}
		}
	} //class Aroon

	public class HV:FormulaBase
	{
		public double N=0;
		public double T=0;
		public HV():base()
		{
			AddParam("N","21","1","10000","",FormulaParamType.Double);
			AddParam("T","252","0","1000000000","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData LOGS=LOG(C/REF(C,1)); LOGS.Name="LogS";
			FormulaData TLOGS=SUM(LOGS,N); TLOGS.Name="TLogs";
			FormulaData ALOGS=TLOGS/N; ALOGS.Name="ALogs";
			FormulaData DELTA=LOGS-ALOGS; DELTA.Name="Delta";
			FormulaData SSD=SUM(DELTA*DELTA,N); SSD.Name="SSD";
			FormulaData HV= SQRT(SSD/(N-1))*SQRT(T); HV.Name="HV";
			return new FormulaPackage(new FormulaData[]{HV},"");
		}
	
		public override string LongName
		{
			get{return "Historic Volatility";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class HV

	#endregion

	#region Formula Group Price Volumn Indicators
	public class ASI:FormulaBase
	{
		public ASI():base()
		{
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData LC=REF(CLOSE,1); LC.Name="LC";
			FormulaData AA=ABS(HIGH-LC); AA.Name="AA";
			FormulaData BB=ABS(LOW-LC); BB.Name="BB";
			FormulaData CC=ABS(HIGH-REF(LOW,1)); CC.Name="CC";
			FormulaData DD=ABS(LC-REF(OPEN,1)); DD.Name="DD";
			FormulaData R=IF(AA>BB & AA>CC,AA+BB/2+DD/4,IF(BB>CC & BB>AA,BB+AA/2+DD/4,CC+DD/4)); R.Name="R";
			FormulaData X=(CLOSE-LC+(CLOSE-OPEN)/2+LC-REF(OPEN,1)); X.Name="X";
			FormulaData SI=16*X/R*MAX(AA,BB); SI.Name="SI";
			FormulaData ASI=SUM(SI,0); ASI.Name="ASI";
			return new FormulaPackage(new FormulaData[]{ASI},"");
		}
	
		public override string LongName
		{
			get{return "Accumulation Swing Index";}
		}
	
		public override string Description
		{
			get{return "The Accumulation Swing Index (ASI) is a cumulative sum of the Welles Wilder’s Swing Index indicator";}
		}
	} //class ASI

	public class OBV:FormulaBase
	{
		public double N=0;
		public OBV():base()
		{
			AddParam("N","20","1","100","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData OBV=SUM(IF(CLOSE>REF(CLOSE,1),VOL,IF(CLOSE<REF(CLOSE,1),-VOL,0)),0); OBV.Name="OBV";OBV.SetAttrs("Width2");
			FormulaData M=MA(OBV,N); M.Name="M";
			
			return new FormulaPackage(new FormulaData[]{OBV,M},"");
		}
	
		public override string LongName
		{
			get{return "On Balance Volume";}
		}
	
		public override string Description
		{
			get{return "Volume is the number of shares or contracts that change ownership over a given period of time.  It is an indication of supply and demand that is independent from price and can relate a great deal about the relative enthusiasm of buyers and sellers in the market place.  On Balance Volume is one indicator that is designed to track changes in volume over time.  It is the running total of volume calculated in such a way as to add the day's volume to a cumulative total if the day's close was higher than the previous day's close and to subtract the day's volume from the cumulative total on down days.  The assumption is that changes in volume will precede changes in price trend.  On Balance Volume was created by Joseph Granville and has a number of interpretive qualities and should be used in conjunction with other indications of price trend reversals.\n\nAnother use of On Balance Volume (OBV) is to look at the trend in the indicator.  A rising trend in the OBV gives sign of a healthy move into the security.  A doubtful trend, or sideways trend in the OBV leaves price trend suspect and a candidate for a reversal of the trend.  A falling OBV trend signals an exodus from the security despite price activity and leads to the caution that price may follow if it is not already.  As indicated on the graphs above, look for divergences between the price and the OBV indicator.  Divergences between the peaks warns of a potential fall in prices.  Divergences between the troughs warns of a potential rise in prices.";}
		}
	} //class OBV

	public class PVT:FormulaBase
	{
		public PVT():base()
		{
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData NONAME0=SUM((CLOSE-REF(CLOSE,1))/REF(CLOSE,1)*VOL,0);
			return new FormulaPackage(new FormulaData[]{NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "PRICE AND VOLUME TREND";}
		}
	
		public override string Description
		{
			get{return "Price and Volume Trend (PVT) is a variation of On Balance Volume, used to determine the strength of trends and warn of reversals.\n\nPVT = yesterday's PVT + today's Volume * (today's Close - yesterday's Close) / yesterday's Close\n";}
		}
	} //class PVT

	public class SOBV:FormulaBase
	{
		public SOBV():base()
		{
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData NONAME0=SUM(IF(ISUP,VOL,IF(ISDOWN,-VOL,0)),0);
			return new FormulaPackage(new FormulaData[]{NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "SOBV";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class SOBV

	public class WVAD:FormulaBase
	{
		public double N1=0;
		public double N2=0;
		public WVAD():base()
		{
			AddParam("N1","10","1","100","",FormulaParamType.Double);
			AddParam("N2","20","1","100","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData WVAD= (CLOSE-OPEN)/(HIGH-LOW)*VOL; WVAD.Name="WVAD";
			FormulaData MA1=MA(WVAD,N1); MA1.Name="MA1";
			FormulaData MA2=MA(WVAD,N2); MA2.Name="MA2";
			
			return new FormulaPackage(new FormulaData[]{WVAD,MA1,MA2},"");
		}
	
		public override string LongName
		{
			get{return "Williams'Variable Accumulation Distribution";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class WVAD

	public class NVI:FormulaBase
	{
		public double START=0;
		public double N1=0;
		public double N2=0;
		public NVI():base()
		{
			AddParam("Start","1","0","1000000000","",FormulaParamType.Double);
			AddParam("N1","9","0","10000","",FormulaParamType.Double);
			AddParam("N2","255","0","10000","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData A=IF(V<=REF(V,1),C/REF(C,1),1); A.Name="A";
			FormulaData NVI=START*MUL(A); NVI.Name="NVI";
			FormulaData M1=EMA(NVI,N1); M1.Name="M1";
			FormulaData M2=EMA(NVI,N2); M2.Name="M2";
			return new FormulaPackage(new FormulaData[]{NVI,M1,M2},"");
		}
	
		public override string LongName
		{
			get{return "Negative Volume Index";}
		}
	
		public override string Description
		{
			get{return "The Negative Volume Index (\"NVI\") focuses on days where the volume decreases from the previous day. The premise being that the \"smart money\" takes positions on days when volume decreases.\n\n1.Take yesterday's Negative Volume Index\n2.If today's volume is lower than yesterday, add:\n{ ( Close [today] - Close [yesterday] ) / Close [yesterday] } * NVI [yesterday]\n3.Otherwise, add zero.";}
		}
	} //class NVI

	public class PVI:FormulaBase
	{
		public double START=0;
		public double N1=0;
		public double N2=0;
		public PVI():base()
		{
			AddParam("Start","1","0","1000000000","",FormulaParamType.Double);
			AddParam("N1","9","0","10000","",FormulaParamType.Double);
			AddParam("N2","255","0","10000","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData A=IF(V>=REF(V,1),C/REF(C,1),1); A.Name="A";
			FormulaData PVI=START*MUL(A); PVI.Name="PVI";
			FormulaData M1=MA(PVI,N1); M1.Name="M1";
			FormulaData M2=MA(PVI,N2); M2.Name="M2";
			return new FormulaPackage(new FormulaData[]{PVI,M1,M2},"");
		}
	
		public override string LongName
		{
			get{return "Positive Volume Index";}
		}
	
		public override string Description
		{
			get{return "1.Take yesterday's Positive Volume Index\n2.If today's volume is greater than yesterday, add:\n{ ( Close [today] - Close [yesterday] ) / Close [yesterday] } * PVI [yesterday]\n3.Otherwise, add zero";}
		}
	} //class PVI

	#endregion

	#region Formula Group Bands Indicators
	public class BBIBOLL:FormulaBase
	{
		public double N=0;
		public double P=0;
		public BBIBOLL():base()
		{
			AddParam("N","10","1","100","",FormulaParamType.Double);
			AddParam("P","3","0.1","20","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData BBI=(MA(CLOSE,3)+MA(CLOSE,6)+MA(CLOSE,12)+MA(CLOSE,24))/4; BBI.Name="BBI";
			FormulaData UPR=BBI+P*STD(BBI,N); UPR.Name="UPR";
			FormulaData DWN=BBI-P*STD(BBI,N); DWN.Name="DWN";
			return new FormulaPackage(new FormulaData[]{BBI,UPR,DWN},"");
		}
	
		public override string LongName
		{
			get{return "BBIBOLL";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class BBIBOLL

	public class BB:FormulaBase
	{
		public double N=0;
		public double P=0;
		public BB():base()
		{
			AddParam("N","20","5","300","",FormulaParamType.Double);
			AddParam("P","2","0.1","10","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData MID=  MA(CLOSE,N); MID.Name="MID";
			FormulaData UPPER= MID + P*STD(CLOSE,N); UPPER.Name="UPPER";
			FormulaData LOWER= MID - P*STD(CLOSE,N); LOWER.Name="LOWER";
			return new FormulaPackage(new FormulaData[]{MID,UPPER,LOWER},"");
		}
	
		public override string LongName
		{
			get{return "Bollinger Bands";}
		}
	
		public override string Description
		{
			get{return "The Bollinger Bands were introduced by John Bollinger. Its primary use is for presenting the volatility of the security in an easy to view form. The indicator consists of three bands: a simple moving average (middle), SMA plus 2 standard deviations (upper), and SMA minus 2 standard deviations (lower).";}
		}
	} //class BB

	public class CDP:FormulaBase
	{
		public CDP():base()
		{
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData PT= REF(HIGH,1)-REF(LOW,1); PT.Name="PT";
			FormulaData CDP= (HIGH + LOW + CLOSE)/3; CDP.Name="CDP";
			FormulaData AH= CDP + PT; AH.Name="AH";
			FormulaData AL= CDP - PT; AL.Name="AL";
			FormulaData NH= 2*CDP-LOW; NH.Name="NH";
			FormulaData NL= 2*CDP-HIGH; NL.Name="NL";
			return new FormulaPackage(new FormulaData[]{CDP,AH,AL,NH,NL},"");
		}
	
		public override string LongName
		{
			get{return "CDP";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class CDP

	public class ENV:FormulaBase
	{
		public double N=0;
		public ENV():base()
		{
			AddParam("N","14","2","300","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData UPPER= MA(CLOSE,N)*1.06; UPPER.Name="UPPER";
			FormulaData LOWER= MA(CLOSE,N)*0.94; LOWER.Name="LOWER";
			return new FormulaPackage(new FormulaData[]{UPPER,LOWER},"");
		}
	
		public override string LongName
		{
			get{return "ENVELOPES (TRADING BANDS)";}
		}
	
		public override string Description
		{
			get{return "An envelope is comprised of two moving averages. One moving average is shifted upward and the second moving average is shifted downward.\n\n";}
		}
	} //class ENV

	public class MIKE:FormulaBase
	{
		public double N=0;
		public MIKE():base()
		{
			AddParam("N","12","1","200","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData TYP=(HIGH+LOW+CLOSE)/3; TYP.Name="TYP";
			FormulaData LL=LLV(LOW,N); LL.Name="LL";
			FormulaData HH=HHV(HIGH,N); HH.Name="HH";
			FormulaData WR=TYP+(TYP-LL); WR.Name="WR";
			FormulaData MR=TYP+(HH-LL); MR.Name="MR";
			FormulaData SR=2*HH-LL; SR.Name="SR";
			FormulaData WS=TYP-(HH-TYP); WS.Name="WS";
			FormulaData MS=TYP-(HH-LL); MS.Name="MS";
			FormulaData SS=2*LL-HH; SS.Name="SS";
			return new FormulaPackage(new FormulaData[]{WR,MR,SR,WS,MS,SS},"");
		}
	
		public override string LongName
		{
			get{return "MIKE";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class MIKE

	public class SR:FormulaBase
	{
		public SR():base()
		{
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData M=(H+L+C)/3; M.Name="M";
			FormulaData A=H-L; A.Name="A";
			FormulaData RR=M+A; RR.Name="RR";
			FormulaData SS=M-A; SS.Name="SS";
			FormulaData R=BACKSET(ISLASTBAR,5)*RR.LASTDATA; R.Name="R";R.SetAttrs("Width2,HighSpeed,ColorRed");
			FormulaData S=BACKSET(ISLASTBAR,5)*SS.LASTDATA; S.Name="S";S.SetAttrs("Width2,HighSpeed,ColorDarkGreen");
			FormulaData NONAME0=DRAWNUMBER(BARSSINCE(R)==1,R,R,"f2");NONAME0.SetAttrs("Label0,VCenter,Right,ColorRed");
			FormulaData NONAME1=DRAWNUMBER(BARSSINCE(S)==1,S,S,"f2");NONAME1.SetAttrs("Label0,VCenter,Right,ColorDarkGreen");
			
			return new FormulaPackage(new FormulaData[]{R,S,NONAME0,NONAME1},"");
		}
	
		public override string LongName
		{
			get{return "Support & Resistance";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class SR

	public class BBWidth:FormulaBase
	{
		public double N=0;
		public double P=0;
		public BBWidth():base()
		{
			AddParam("N","20","1","100","",FormulaParamType.Double);
			AddParam("P","2","0.1","100","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData NONAME0=P*STD(C,N)*2;NONAME0.SetAttrs("Width1.6,HighQuality");
			return new FormulaPackage(new FormulaData[]{NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "Bollinger Band Width";}
		}
	
		public override string Description
		{
			get{return "The Bollinger Band Width indicator charts the width of the Bollinger Bands. When the Bollinger Band Width increases in value, it indicates that the volatility of the underlying stock has also increased.\n";}
		}
	} //class BBWidth

	public class Keltner:FormulaBase
	{
		public double LENGTH=0;
		public double FACTOR=0;
		public Keltner():base()
		{
			AddParam("Length","16","1","100","",FormulaParamType.Double);
			AddParam("Factor","1.3","0","100","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData SHIFT= FACTOR * ATR(LENGTH); SHIFT.Name="Shift";
			FormulaData AVG= EMA(C, LENGTH); AVG.Name="Avg";
			FormulaData UPPER= AVG + SHIFT; UPPER.Name="Upper";
			FormulaData LOWER= AVG - SHIFT; LOWER.Name="Lower";
			return new FormulaPackage(new FormulaData[]{UPPER,LOWER},"");
		}
	
		public override string LongName
		{
			get{return "Keltner channel";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class Keltner

	#endregion

	#region Formula Group Index Indicators
	public class ABI:FormulaBase
	{
		public ABI():base()
		{
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData NONAME0=ABS(ADVANCE - DECLINE);
			return new FormulaPackage(new FormulaData[]{NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "Absolute Breadth Index";}
		}
	
		public override string Description
		{
			get{return "The Absolute Breadth Index (\"ABI\") is a market momentum indicator that was developed by Norman G. Fosback.\n\nThe ABI shows how much activity, volatility, and change is taking place on the New York Stock Exchange while ignoring the direction prices are headed.\n\nYou can think of the ABI as an \"activity index.\" High readings indicate market activity and change, while low readings indicate lack of change.";}
		}
	} //class ABI

	public class ADL:FormulaBase
	{
		public ADL():base()
		{
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData NONAME0=SUM(ADVANCE-DECLINE,0);
			return new FormulaPackage(new FormulaData[]{NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "Advance/Decline line";}
		}
	
		public override string Description
		{
			get{return "The advance and decline line is a cumulative, ongoing sum of the difference between the number of stocks closing higher minus the number of stocks closing lower each day.  It can be used as a measure of market strength as it moves higher when the there are more advancing issues than declining issues.  It moves lower when there are more declining issues than advancing issues.  Plotting the Advance/Decline line allows insight into market strength.  At times the major U.S. indices can continue higher while we are seeing a drop in the advance/decline line.  This is called a divergence and warns that we may be at the end of an upward movement and sets the stage for a possible reversal of price trend.  However such divergences can exist over a long period of time before evidence of price trend reversal occur.  It becomes a matter of sound analysis to build as wide a body of evidence as possible in forming an outlook for the future path of prices.";}
		}
	} //class ADL

	public class ADR:FormulaBase
	{
		public double N=0;
		public ADR():base()
		{
			AddParam("N","10","1","200","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData NONAME0=SUM(ADVANCE,N)/SUM(DECLINE,N);
			return new FormulaPackage(new FormulaData[]{NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "ADVANCE/DECLINE RATIO";}
		}
	
		public override string Description
		{
			get{return "The Advance/Decline Ratio (\"A/D Ratio\") shows the ratio of advancing issues to declining issues. It is calculated by dividing the number of advancing issues by the number of declining issues.\n\nInterpretation\n\nThe A/D Ratio is similar to the Advancing-Declining Issues in that it displays market breadth. But, where the Advancing-Declining Issues subtracts the advancing/declining values, the A/D Ratio divides the values. The advantage of the Ratio is that it remains constant regardless of the number of issues that are traded on the New York Stock Exchange (which has steadily increased).\n\nA moving average of the A/D Ratio is often used as an overbought/oversold indicator. The higher the value, the more \"excessive\" the rally and the more likely a correction. Likewise, low readings imply an oversold market and suggest a technical rally.\n\nKeep in mind, however, that markets that appear to be extremely overbought or oversold may stay that way for some time. When investing using overbought and oversold indicators, it is wise to wait for the prices to confirm your belief that a change is due before placing your trades.\n\nDay-to-day fluctuations of the Advance/Decline Ratio are often eliminated by smoothing the ratio with a moving average\n";}
		}
	} //class ADR

	public class BT:FormulaBase
	{
		public double N=0;
		public BT():base()
		{
			AddParam("N","10","1","300","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData NONAME0=MA(ADVANCE/(ADVANCE-DECLINE),N);
			return new FormulaPackage(new FormulaData[]{NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "Breadth Trust";}
		}
	
		public override string Description
		{
			get{return "The Breadth Thrust indicator is a market momentum indicator. It was developed by Dr. Martin Zweig. The Breadth Thrust is calculated by dividing a 10-day exponential moving average of the number of advancing issues, by the number of advancing plus declining issues.\n\nInterpretation\n\nA \"Breadth Thrust\" occurs when, during a 10-day period, the Breadth Thrust indicator rises from below 40% to above 61.5%. A \"Thrust\" indicates that the stock market has rapidly changed from an oversold condition to one of strength, but has not yet become overbought.\n\nAccording to Dr. Zweig, there have only been fourteen Breadth Thrusts since 1945. The average gain following these fourteen Thrusts was 24.6% in an average time-frame of eleven months. Dr. Zweig also points out that most bull markets begin with a Breadth Thrust.";}
		}
	} //class BT

	public class CHAIKIN:FormulaBase
	{
		public double LONG=0;
		public double SHORT=0;
		public CHAIKIN():base()
		{
			AddParam("LONG","10","5","300","",FormulaParamType.Double);
			AddParam("SHORT","3","1","300","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData ADL= SUM(ADVANCE-DECLINE,0); ADL.Name="ADL";
			FormulaData CHA=  MA(ADL,SHORT) - MA(ADL,LONG); CHA.Name="CHA";
			return new FormulaPackage(new FormulaData[]{CHA},"");
		}
	
		public override string LongName
		{
			get{return "Chaikin's Accumulation/Distribution Indicator";}
		}
	
		public override string Description
		{
			get{return "A cumulative volume indicator attempting to assess the net volume movement on a day or week.";}
		}
	} //class CHAIKIN

	public class MCO:FormulaBase
	{
		public double N1=0;
		public double N2=0;
		public MCO():base()
		{
			AddParam("N1","19","10","80","",FormulaParamType.Double);
			AddParam("N2","39","30","100","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData NONAME0=EMA(ADVANCE-DECLINE,N1)/10 - EMA(ADVANCE-DECLINE,N2)/20;
			return new FormulaPackage(new FormulaData[]{NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "McClellan Oscillator";}
		}
	
		public override string Description
		{
			get{return "The McClellan Oscillator can be used as an overbought/oversold indicator.  It also has value at predicting short term trend changes when it crosses the zero line. A rising indicator that crosses the zero line from below is a bullish sign.  A falling indicator that crosses the zero line from above is a bearish sign.\n\nThe McClellan Oscillator is calculated by subtracting a 39 day moving average of advances minus declines, from a 19 day moving average of advances minus declines.  Generally it is not considered a forward looking indicator but can tell you a lot about trend.\n";}
		}
	} //class MCO

	public class OBOS:FormulaBase
	{
		public double N=0;
		public OBOS():base()
		{
			AddParam("N","10","2","100","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData NONAME0=MA(ADVANCE-DECLINE,N);
			return new FormulaPackage(new FormulaData[]{NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "Over Buy/Over Sell";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class OBOS

	public class STIX:FormulaBase
	{
		public double N=0;
		public STIX():base()
		{
			AddParam("N","11","1","100","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData NONAME0=EMA(ADVANCE/(ADVANCE+DECLINE)*100,N);
			return new FormulaPackage(new FormulaData[]{NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "STIX";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class STIX

	#endregion

	#region Formula Group Volumn Indicators
	public class AMOUNT:FormulaBase
	{
		public double N1=0;
		public double N2=0;
		public AMOUNT():base()
		{
			AddParam("N1","5","1","100","",FormulaParamType.Double);
			AddParam("N2","20","1","100","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData NONAME0=AMOUNT;NONAME0.SetAttrs("VOLSTICK");
			FormulaData MA1=MA(AMOUNT,N1); MA1.Name="MA1";
			FormulaData MA2=MA(AMOUNT,N2); MA2.Name="MA2";
			return new FormulaPackage(new FormulaData[]{NONAME0,MA1,MA2},"");
		}
	
		public override string LongName
		{
			get{return "AMOUNT";}
		}
	
		public override string Description
		{
			get{return "AMOUNT";}
		}
	} //class AMOUNT

	public class VOLMA:FormulaBase
	{
		public double M1=0;
		public VOLMA():base()
		{
			AddParam("M1","60","1","100","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData VV=V; VV.Name="VV";VV.SetAttrs("VOLSTICK");
			SETNAME(VV,"");
			FormulaData MA1=MA(VV,M1); MA1.Name="MA1";
			SETNAME(MA1,"MA");
			
			return new FormulaPackage(new FormulaData[]{VV,MA1},"");
		}
	
		public override string LongName
		{
			get{return "Volumn";}
		}
	
		public override string Description
		{
			get{return "Volumn and moving average";}
		}
	} //class VOLMA

	public class VOSC:FormulaBase
	{
		public double SHORT=0;
		public double LONG=0;
		public VOSC():base()
		{
			AddParam("SHORT","12","2","50","",FormulaParamType.Double);
			AddParam("LONG","26","15","100","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData NONAME0=(MA(VOL,SHORT)-MA(VOL,LONG))/MA(VOL,SHORT)*100;
			return new FormulaPackage(new FormulaData[]{NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "Volume Oscillator";}
		}
	
		public override string Description
		{
			get{return "The Volume Oscillator (VO) identifies trends in volume using a two moving average system.\n\nThe Volume Oscillator measures the difference between a faster and slower moving average (MA).\n\nIf the fast MA is above the slow MA the oscillator will be positive.\nIf the fast MA is below the slow MA then the oscillator will be negative.\nThe Volume Oscillator will be zero when the two MA's cross.";}
		}
	} //class VOSC

	public class VSTD:FormulaBase
	{
		public double N=0;
		public VSTD():base()
		{
			AddParam("N","10","1","1000","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData NONAME0=STD(VOL,N);
			return new FormulaPackage(new FormulaData[]{NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "Volumn STD";}
		}
	
		public override string Description
		{
			get{return "Volumn STD";}
		}
	} //class VSTD

	public class PVO:FormulaBase
	{
		public double N1=0;
		public double N2=0;
		public double N3=0;
		public PVO():base()
		{
			AddParam("N1","12","1","100","",FormulaParamType.Double);
			AddParam("N2","26","1","100","",FormulaParamType.Double);
			AddParam("N3","9","1","100","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData E1=EMA(V,N1); E1.Name="E1";
			FormulaData PVO= (E1-EMA(V,N2))/E1*100; PVO.Name="PVO";PVO.SetAttrs("Width1.6,HighQuality");
			FormulaData M= EMA(PVO,N3); M.Name="M";
			return new FormulaPackage(new FormulaData[]{PVO,M},"");
		}
	
		public override string LongName
		{
			get{return "Percentage Volume Oscillator";}
		}
	
		public override string Description
		{
			get{return "The Percentage Volume Oscillator (PVO) is the percentage difference between two moving averages of volume.";}
		}
	} //class PVO

	public class MaxV:FormulaBase
	{
		public double N=0;
		public MaxV():base()
		{
			AddParam("N","20","0","100000","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData NONAME0=HHV(V,N);
			return new FormulaPackage(new FormulaData[]{NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "N day's maximum volume";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class MaxV

	public class COLORV:FormulaBase
	{
		public COLORV():base()
		{
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData NONAME0=STICKLINE(C>REF(C,1),V,0);NONAME0.SetAttrs("Brush#FF0000,Color#800000");
			FormulaData NONAME1=STICKLINE(C<=REF(C,1),V,0);NONAME1.SetAttrs("Brush#00FF00,Color#008000");
			return new FormulaPackage(new FormulaData[]{NONAME0,NONAME1},"");
		}
	
		public override string LongName
		{
			get{return "Volume with custom color";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class COLORV

	#endregion

	#region Formula Group Oscillator Indicators
	public class AD:FormulaBase
	{
		public double N=0;
		public AD():base()
		{
			AddParam("N","20","1","1000","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData AD=SUM(((CLOSE-LOW)-(HIGH-CLOSE))/(HIGH-LOW)*VOL,0); AD.Name="AD";
			FormulaData M=MA(AD,N); M.Name="M";
			return new FormulaPackage(new FormulaData[]{AD,M},"");
		}
	
		public override string LongName
		{
			get{return "Accumulation/Distribution";}
		}
	
		public override string Description
		{
			get{return "The Accumulation/Distribution is a momentum indicator that associates changes in price and volume. The indicator is based on the premise that the more volume that accompanies a price move, the more significant the price move.\n\n";}
		}
	} //class AD

	public class MI:FormulaBase
	{
		public double N=0;
		public MI():base()
		{
			AddParam("N","12","1","100","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData A=C-REF(C,N); A.Name="A";
			FormulaData MI=SMA(A,N,1); MI.Name="MI";
			return new FormulaPackage(new FormulaData[]{A,MI},"");
		}
	
		public override string LongName
		{
			get{return "MI";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class MI

	public class MICD:FormulaBase
	{
		public double N=0;
		public double N1=0;
		public double N2=0;
		public MICD():base()
		{
			AddParam("N","3","1","100","",FormulaParamType.Double);
			AddParam("N1","10","1","100","",FormulaParamType.Double);
			AddParam("N2","20","1","100","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData MI=C-REF(C,1); MI.Name="MI";
			FormulaData AMI=SMA(MI,N,1); AMI.Name="AMI";
			FormulaData DIF=MA(REF(AMI,1),N1)-MA(REF(AMI,1),N2); DIF.Name="DIF";
			FormulaData MICD=SMA(DIF,10,1); MICD.Name="MICD";
			return new FormulaPackage(new FormulaData[]{DIF,MICD},"");
		}
	
		public override string LongName
		{
			get{return "MICD";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class MICD

	public class RC:FormulaBase
	{
		public double N=0;
		public RC():base()
		{
			AddParam("N","50","1","100","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData RC=C/REF(C,N); RC.Name="RC";
			FormulaData ARC=SMA(REF(RC,1),N,1); ARC.Name="ARC";
			return new FormulaPackage(new FormulaData[]{ARC},"");
		}
	
		public override string LongName
		{
			get{return "Rate of Change";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class RC

	public class RCCD:FormulaBase
	{
		public double N=0;
		public double N1=0;
		public double N2=0;
		public RCCD():base()
		{
			AddParam("N","59","1","100","",FormulaParamType.Double);
			AddParam("N1","21","1","100","",FormulaParamType.Double);
			AddParam("N2","28","1","100","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData RC=C/REF(C,N); RC.Name="RC";
			FormulaData ARC=SMA(REF(RC,1),N,1); ARC.Name="ARC";
			FormulaData DIF=MA(REF(ARC,1),N1)-MA(REF(ARC,1),N2); DIF.Name="DIF";
			FormulaData RCCD=SMA(DIF,N,1); RCCD.Name="RCCD";
			return new FormulaPackage(new FormulaData[]{DIF,RCCD},"");
		}
	
		public override string LongName
		{
			get{return "Rate of Change Convergence Divergence";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class RCCD

	public class SRMI:FormulaBase
	{
		public double N=0;
		public SRMI():base()
		{
			AddParam("N","9","1","100","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData NONAME0=IF(C<REF(C,N),(C-REF(C,N))/REF(C,N),IF(C==REF(C,N),0,(C-REF(C,N))/C));
			return new FormulaPackage(new FormulaData[]{NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "SRMI";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class SRMI

	public class CMF:FormulaBase
	{
		public double N=0;
		public CMF():base()
		{
			AddParam("N","20","1","100","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData D=(HIGH-LOW); D.Name="D";
			FormulaData AD=IF(D==0,0,((CLOSE-LOW)-(HIGH-CLOSE))/D*VOL); AD.Name="AD";
			FormulaData MV=MA(VOL,N); MV.Name="MV";
			FormulaData CMF=IF(MV==0,0,MA(AD,N)/MV); CMF.Name="CMF";CMF.SetAttrs("COLORSTICK");
			return new FormulaPackage(new FormulaData[]{CMF},"");
		}
	
		public override string LongName
		{
			get{return "Chaikin Money Flow";}
		}
	
		public override string Description
		{
			get{return "Developed by Marc Chaikin, the Chaikin Money Flow oscillator is calculated from the daily readings of the Accumulation/Distribution Line. The basic premise behind the Accumulation Distribution Line is that the degree of buying or selling pressure can be determined by the location of the close relative to the high and low for the corresponding period (Closing Location Value). There is buying pressure when a stock closes in the upper half of a period's range and there is selling pressure when a stock closes in the lower half of the period's trading range. The Closing Location Value multiplied by volume forms the Accumulation/Distribution Value for each period.";}
		}
	} //class CMF

	public class ULT:FormulaBase
	{
		public double N1=0;
		public double N2=0;
		public double N3=0;
		public ULT():base()
		{
			AddParam("N1","7","1","100","",FormulaParamType.Double);
			AddParam("N2","14","1","100","",FormulaParamType.Double);
			AddParam("N3","28","1","100","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData LC=REF(C,1); LC.Name="LC";
			FormulaData TL=MIN(L,LC); TL.Name="TL";
			FormulaData BP=C-TL; BP.Name="BP";
			FormulaData TR= MAX(H-L,ABS(LC-H),ABS(LC-L)); TR.Name="TR";
			FormulaData BPSUM1= MA(BP,N1); BPSUM1.Name="BPSum1";
			FormulaData BPSUM2= MA(BP,N2); BPSUM2.Name="BPSum2";
			FormulaData BPSUM3= MA(BP,N3); BPSUM3.Name="BPSum3";
			FormulaData TRSUM1= MA(TR,N1); TRSUM1.Name="TRSum1";
			FormulaData TRSUM2= MA(TR,N2); TRSUM2.Name="TRSum2";
			FormulaData TRSUM3= MA(TR,N3); TRSUM3.Name="TRSum3";
			FormulaData RAWUO=4*(BPSUM1/TRSUM1)+2*(BPSUM2/TRSUM2)+(BPSUM3/TRSUM3); RAWUO.Name="RawUO";
			FormulaData NONAME0=(RAWUO/(4+2+1))*100;NONAME0.SetAttrs("Width1.6,HighQuality");
			
			return new FormulaPackage(new FormulaData[]{NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "Ultimate Oscillator";}
		}
	
		public override string Description
		{
			get{return "Developed by Larry Williams and first described in a 1985 article for Technical Analysis of Stocks and Commodities magazine, the \"Ultimate\" Oscillator combines a stock's price action during three different time frames into one bounded oscillator. Values range from 0 to 100 with 50 as the center line. Oversold territory exists below 30 and overbought territory extends from 70 to 100.\n\n";}
		}
	} //class ULT

	public class AroonOsc:FormulaBase
	{
		public double N=0;
		public AroonOsc():base()
		{
			AddParam("N","25","0","1000","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData NONAME0=0;
			FormulaData NONAME1=FML(DP,"Aroon(N)[UP]")-FML(DP,"Aroon(N)[DOWN]");
			return new FormulaPackage(new FormulaData[]{NONAME0,NONAME1},"");
		}
	
		public override string LongName
		{
			get{return "Aroon Oscillator";}
		}
	
		public override string Description
		{
			get{return "The Aroon Oscillator was constructed by subtracting Aroon(down) from Aroon(up). Since Aroon(up) and Aroon(down) oscillate between 0 and +100, the Aroon Oscillator oscillate between -100 and +100 with zero as the center crossover line.";}
		}
	} //class AroonOsc

	#endregion

	#region Formula Group Others
	public class MASS:FormulaBase
	{
		public double N1=0;
		public double N2=0;
		public MASS():base()
		{
			AddParam("N1","9","2","100","",FormulaParamType.Double);
			AddParam("N2","25","5","100","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData NONAME0=SUM(EMA((HIGH-LOW),N1)/EMA(EMA((HIGH-LOW),N1),N1),N2);
			return new FormulaPackage(new FormulaData[]{NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "Mass Index";}
		}
	
		public override string Description
		{
			get{return "The Mass Index was designed to identify trend reversals by measuring the narrowing and widening of the range between the high and low prices. As this range widens, the Mass Index increases; as the range narrows the Mass Index decreases.\n\nThe Mass Index was developed by Donald Dorsey.\n\n";}
		}
	} //class MASS

	public class STD:FormulaBase
	{
		public double N=0;
		public STD():base()
		{
			AddParam("N","26","2","100","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData NONAME0=STD(CLOSE,N);
			return new FormulaPackage(new FormulaData[]{NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "STD";}
		}
	
		public override string Description
		{
			get{return "STD";}
		}
	} //class STD

	public class VHF:FormulaBase
	{
		public double N=0;
		public VHF():base()
		{
			AddParam("N","28","3","100","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData NONAME0=(HHV(CLOSE,N)-LLV(CLOSE,N))/SUM(ABS(CLOSE-REF(CLOSE,1)),N);
			return new FormulaPackage(new FormulaData[]{NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "Vertical Horizontal Filter";}
		}
	
		public override string Description
		{
			get{return "Vertical Horizontal Filter (VHF) was created by Adam White to identify trending and ranging markets. VHF measures the level of trend activity, similar to ADX in the Directional Movement System. Trend indicators can then be employed in trending markets and momentum indicators in ranging markets.\n\nVary the number of periods in the Vertical Horizontal Filter to suit different time frames. White originally recommended 28 days but now prefers an 18-day window smoothed with a 6-day moving average.\n";}
		}
	} //class VHF

	public class WAD:FormulaBase
	{
		public WAD():base()
		{
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData NONAME0=SUM(IF( CLOSE>REF(CLOSE,1),CLOSE-MIN(REF(CLOSE,1),LOW),IF(CLOSE<REF(CLOSE,1),
CLOSE-MAX(REF(CLOSE,1),HIGH),0)),0);
			return new FormulaPackage(new FormulaData[]{NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "William's Accumulation/Distribution";}
		}
	
		public override string Description
		{
			get{return "A price indicator attempting to assess the accumulation or distribution of securities.";}
		}
	} //class WAD

	public class ZigLabel:FormulaBase
	{
		public double N=0;
		public ZigLabel():base()
		{
			AddParam("N","6","1","100","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData NONAME0=DRAWNUMBER(FINDPEAK(N),H,H,"f2");NONAME0.SetAttrs("Label3");
			FormulaData NONAME1=DRAWNUMBER(FINDTROUGH(N),L,L,"f2");NONAME1.SetAttrs("Label3,Valign2");
			return new FormulaPackage(new FormulaData[]{NONAME0,NONAME1},"");
		}
	
		public override string LongName
		{
			get{return "Zig Zag Label";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class ZigLabel

	public class PR:FormulaBase
	{
		public string STOCKCODE="";
		public double N=0;
		public PR():base()
		{
			AddParam("StockCode","^DJI","0","0","",FormulaParamType.String);
			AddParam("N","20","1","100","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData PR=C/FML(STOCKCODE,"C")*100; PR.Name="PR";PR.SetAttrs("HighQuality");
			FormulaData NONAME0=EMA(PR,N);
			return new FormulaPackage(new FormulaData[]{PR,NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "Price Relative";}
		}
	
		public override string Description
		{
			get{return "The Price Relative compares the performance of one security against that of another. It is often used to compare the performance of a particular stock to a market index, usually the S&P 500. Because the goal of many portfolio managers is to outperform the S&P 500, they are usually interested in the strongest stocks. The price relative offers a straightforward and accurate portrayal of a stock's performance relative to the market.";}
		}
	} //class PR

	public class Fibonnaci:FormulaBase
	{
		public double N=0;
		public Fibonnaci():base()
		{
			AddParam("N","100","1","100","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData A= HHV(H,N); A.Name="A";
			FormulaData B= LLV(L,N); B.Name="B";
			FormulaData HH=BACKSET(ISLASTBAR,N)*A.LASTDATA; HH.Name="HH";HH.SetAttrs("Width2");
			FormulaData LL=BACKSET(ISLASTBAR,N)*B.LASTDATA; LL.Name="LL";LL.SetAttrs("Width2");
			FormulaData HEIGHT= HH-LL; HEIGHT.Name="Height";
			FormulaData A1= LL+HEIGHT*0.382; A1.Name="A1";
			FormulaData A2= LL+HEIGHT*0.5; A2.Name="A2";
			FormulaData A3= LL+HEIGHT*0.618; A3.Name="A3";
			SETTEXTVISIBLE(HH,FALSE);
			SETTEXTVISIBLE(LL,FALSE);
			SETTEXTVISIBLE(A1,FALSE);
			SETTEXTVISIBLE(A2,FALSE);
			SETTEXTVISIBLE(A3,FALSE);
			return new FormulaPackage(new FormulaData[]{HH,LL,A1,A2,A3},"");
		}
	
		public override string LongName
		{
			get{return "Fibonnaci";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class Fibonnaci

	public class LinRegr:FormulaBase
	{
		public double N=0;
		public double P=0;
		public LinRegr():base()
		{
			AddParam("N","14","1","1000","",FormulaParamType.Double);
			AddParam("P","100","0","100","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData A= LR(C,N); A.Name="A";
			FormulaData DIST=C-A; DIST.Name="Dist";
			FormulaData M= MAX(MAXVALUE(DIST),ABS(MINVALUE(DIST)))*P/100; M.Name="M";
			FormulaData UPPER= A +M; UPPER.Name="Upper";
			FormulaData LOWER= A - M; LOWER.Name="Lower";
			FormulaData NONAME0=A;
			SETNAME(A,"");
			SETTEXTVISIBLE(UPPER,FALSE);
			SETTEXTVISIBLE(LOWER,FALSE);
			return new FormulaPackage(new FormulaData[]{UPPER,LOWER,NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "Linear Regression Channels";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class LinRegr

	public class RawData:FormulaBase
	{
		public string DATANAME="";
		public RawData():base()
		{
			AddParam("DataName","0","0","0","",FormulaParamType.String);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData O=ORGDATA(DATANAME); O.Name="O";
			SETNAME(O,DATANAME);
			SETTEXTVISIBLE(FALSE);
			return new FormulaPackage(new FormulaData[]{O},"");
		}
	
		public override string LongName
		{
			get{return "Provide raw data from data provider";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class RawData

	public class ZigW:FormulaBase
	{
		public double N=0;
		public ZigW():base()
		{
			AddParam("N","10","1","100","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData NONAME0=ZIG(N);NONAME0.SetAttrs("Width2");
			FormulaData A=FINDPEAK(N); A.Name="A";
			FormulaData B=FINDTROUGH(N); B.Name="B";
			FormulaData A1=TOVALUE(A,H,0.5); A1.Name="A1";
			FormulaData A2=TOVALUE(B,L,0.5); A2.Name="A2";
			FormulaData NONAME1=POLYLINE(A,H);NONAME1.SetAttrs("StyleDash");
			FormulaData NONAME2=POLYLINE(B,L);NONAME2.SetAttrs("StyleDash,SameColor");
			FormulaData A3=ZIGP(N); A3.Name="A3";
			FormulaData NONAME3=DRAWNUMBER(A1,A1,A3,"f3");NONAME3.SetAttrs("Label3,VAlign0");
			FormulaData NONAME4=DRAWNUMBER(A2,A2,A3,"f3");NONAME4.SetAttrs("Label3,VAlign0");
			return new FormulaPackage(new FormulaData[]{NONAME0,NONAME1,NONAME2,NONAME3,NONAME4},"");
		}
	
		public override string LongName
		{
			get{return "Zig /w retracements";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class ZigW

	public class ZigSR:FormulaBase
	{
		public double N=0;
		public ZigSR():base()
		{
			AddParam("N","10","1","100","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData A1=PEAK(N); A1.Name="A1";
			FormulaData A2=PEAK(N,2); A2.Name="A2";
			FormulaData B1=PEAKBARS(N); B1.Name="B1";
			FormulaData B2=PEAKBARS(N,2); B2.Name="B2";
			FormulaData NONAME0=DRAWLINE(B2.LASTDATA,A2.LASTDATA,B1.LASTDATA,A1.LASTDATA,1);
			FormulaData NONAME1=DRAWTEXT(B2.LASTDATA,A2.LASTDATA,"{HIGH} +hi[UP]");NONAME1.SetAttrs("SameColor");
			FormulaData NONAME2=DRAWTEXT(B1.LASTDATA,A1.LASTDATA,"{HIGH} +hi");NONAME2.SetAttrs("SameColor");
			FormulaData D1=TROUGH(N); D1.Name="D1";
			FormulaData D2=TROUGH(N,2); D2.Name="D2";
			FormulaData E1=TROUGHBARS(N); E1.Name="E1";
			FormulaData E2=TROUGHBARS(N,2); E2.Name="E2";
			FormulaData NONAME3=DRAWLINE(E2.LASTDATA,D2.LASTDATA,E1.LASTDATA,D1.LASTDATA,1);NONAME3.SetAttrs("SameColor");
			FormulaData NONAME4=DRAWTEXT(E2.LASTDATA,D2.LASTDATA,"{LOW} +lo[up channel]");NONAME4.SetAttrs("VAlign2,SameColor");
			FormulaData NONAME5=DRAWTEXT(E1.LASTDATA,D1.LASTDATA,"{LOW} +lo[b/o retest]");NONAME5.SetAttrs("VAlign2,SameColor");
			
			return new FormulaPackage(new FormulaData[]{NONAME0,NONAME1,NONAME2,NONAME3,NONAME4,NONAME5},"");
		}
	
		public override string LongName
		{
			get{return "Zig support and resistance";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class ZigSR

	public class ZigIcon:FormulaBase
	{
		public double N=0;
		public ZigIcon():base()
		{
			AddParam("N","6","1","100","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData NONAME0=DRAWICON(FINDPEAK(N),H,"dn.gif");NONAME0.SetAttrs("Top");
			FormulaData NONAME1=DRAWICON(FINDTROUGH(N),L,"up.gif");NONAME1.SetAttrs("Bottom");
			return new FormulaPackage(new FormulaData[]{NONAME0,NONAME1},"");
		}
	
		public override string LongName
		{
			get{return "Draw buy sell Icon according Zig";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class ZigIcon

	public class TimeLabel:FormulaBase
	{
		public double D=0;
		public double T=0;
		public string TEXT="";
		public string STYLE="";
		public TimeLabel():base()
		{
			AddParam("D","20031231","0","100000000","",FormulaParamType.Double);
			AddParam("T","091000","0","100000000","",FormulaParamType.Double);
			AddParam("Text","Rumour","0","0","",FormulaParamType.String);
			AddParam("Style","Font(Verdana, 10pt, style=Bold, Italic),ColorRed","0","0","",FormulaParamType.String);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData AA= DRAWTEXT(NEARESTTIME(D,T),C,TEXT); AA.Name="AA";
			SETATTR(AA,STYLE);
			SETTEXTVISIBLE(FALSE);
			return new FormulaPackage(new FormulaData[]{AA},"");
		}
	
		public override string LongName
		{
			get{return "Draw label at specific time";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class TimeLabel

	#endregion

	#region Formula Group Native
	public class HL:FormulaBase
	{
		public double N=0;
		public HL():base()
		{
			AddParam("N","0","1","20000","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData NONAME0=N;
			return new FormulaPackage(new FormulaData[]{NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "Horizon Line";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class HL

	public class EMA:FormulaBase
	{
		public double N=0;
		public EMA():base()
		{
			AddParam("N","12","1","1000","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData NONAME0=EMA(C,N);
			return new FormulaPackage(new FormulaData[]{NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "EMA";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class EMA

	public class MA:FormulaBase
	{
		public double N=0;
		public MA():base()
		{
			AddParam("N","12","1","1000","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData NONAME0=MA(C,N);
			return new FormulaPackage(new FormulaData[]{NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "MA";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class MA

	public class MAIN:FormulaBase
	{
		public MAIN():base()
		{
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData M=STOCK; M.Name="M";
			SETNAME("");
			SETTEXTVISIBLE(M,FALSE);
			return new FormulaPackage(new FormulaData[]{M},"");
		}
	
		public override string LongName
		{
			get{return "Stock area";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class MAIN

	public class VOL:FormulaBase
	{
		public VOL():base()
		{
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData NONAME0=VOL;NONAME0.SetAttrs("VOLSTICK");
			return new FormulaPackage(new FormulaData[]{NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "Volumn View";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class VOL

	public class DotLine:FormulaBase
	{
		public double N=0;
		public DotLine():base()
		{
			AddParam("N","0","0","20000","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData NONAME0=N;NONAME0.SetAttrs("StyleDash,ColorRed");
			return new FormulaPackage(new FormulaData[]{NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "Dot Horizon Line";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class DotLine

	public class MAIN2:FormulaBase
	{
		public MAIN2():base()
		{
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData M=STOCK; M.Name="M";
			SETNAME(M,STKLABEL);
			SETTEXTVISIBLE(FALSE);
			
			return new FormulaPackage(new FormulaData[]{M},"");
		}
	
		public override string LongName
		{
			get{return "Stock area show symbol and value";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class MAIN2

	public class OverlayV:FormulaBase
	{
		public OverlayV():base()
		{
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData A=VOL; A.Name="A";A.SetAttrs("VOLSTICK,HIGH0.2,Alpha100");
			SETNAME(A,"V");
			SETTEXTVISIBLE(FALSE);
			return new FormulaPackage(new FormulaData[]{A},"");
		}
	
		public override string LongName
		{
			get{return "Volume for overlay";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class OverlayV

	public class REF:FormulaBase
	{
		public string INDI="";
		public double N=0;
		public double M=0;
		public REF():base()
		{
			AddParam("Indi","C","","","",FormulaParamType.Indicator);
			AddParam("N","1","0","10000","",FormulaParamType.Double);
			AddParam("M","20","0","10000","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData A=REF(FML(INDI),N); A.Name="A";
			FormulaData NONAME0=BACKSET(ISLASTVALUE(A),A,M);
			return new FormulaPackage(new FormulaData[]{NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "Reference values of N days before";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class REF

	public class REFC:FormulaBase
	{
		public double N=0;
		public REFC():base()
		{
			AddParam("N","1","0","1000000","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData NONAME0=REF(C,N);
			return new FormulaPackage(new FormulaData[]{NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "Reference Close Value";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class REFC

	#endregion

	#region Formula Group Extend
	public class AreaBB:FormulaBase
	{
		public double N=0;
		public double P=0;
		public AreaBB():base()
		{
			AddParam("N","26","5","300","",FormulaParamType.Double);
			AddParam("P","2","0.1","10","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData MID=  MA(CLOSE,N); MID.Name="MID";
			FormulaData UPPER= MID + P*STD(CLOSE,N); UPPER.Name="UPPER";UPPER.SetAttrs("Color#8080c0");
			FormulaData LOWER= MID - P*STD(CLOSE,N); LOWER.Name="LOWER";LOWER.SetAttrs("Color#8080c0");
			FormulaData NONAME0=FILLRGN(1,LOWER,UPPER);NONAME0.SetAttrs("Brush#200000C0");
			
			return new FormulaPackage(new FormulaData[]{UPPER,LOWER,NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "Area Bollinger Bands";}
		}
	
		public override string Description
		{
			get{return "The Bollinger Bands were introduced by John Bollinger. Its primary use is for presenting the volatility of the security in an easy to view form. The indicator consists of three bands: a simple moving average (middle), SMA plus 2 standard deviations (upper), and SMA minus 2 standard deviations (lower).";}
		}
	} //class AreaBB

	public class AreaRSI:FormulaBase
	{
		public double N1=0;
		public AreaRSI():base()
		{
			AddParam("N1","14","2","100","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData LC= REF(CLOSE,1); LC.Name="LC";
			FormulaData RSI=SMA(MAX(CLOSE-LC,0),N1,1)/SMA(ABS(CLOSE-LC),N1,1)*100; RSI.Name="RSI";
			FormulaData NONAME0=70;NONAME0.SetAttrs("HighSpeed");
			FormulaData NONAME1=30;NONAME1.SetAttrs("HighSpeed");
			FormulaData NONAME2=FILLRGN(RSI>70,RSI,70);NONAME2.SetAttrs("Brush#20808000");
			FormulaData NONAME3=FILLRGN(RSI<30,RSI,30);NONAME3.SetAttrs("Brush#20800000");
			return new FormulaPackage(new FormulaData[]{RSI,NONAME0,NONAME1,NONAME2,NONAME3},"");
		}
	
		public override string LongName
		{
			get{return "Relative Strength Index";}
		}
	
		public override string Description
		{
			get{return "The Relative Strength Index was introduced by Welles Wilder. It is an indicator for overbought / oversold conditions. It is going up when the market is strong, and down, when the market is weak. The oscillation range is between 0 and 100.\n\nThe indicator is non-linear, it is moving faster in the middle of its range, and slower - in the overbought / oversold territory.\n\nThe RSI should not be confused with the relative strength indicator which is used to compare stocks to each other.\n\n";}
		}
	} //class AreaRSI

	public class SR2:FormulaBase
	{
		public SR2():base()
		{
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData M=(H+L+C)/3; M.Name="M";
			FormulaData S=M +((-L+(2*M)-(-H+2*M))); S.Name="S";S.SetAttrs("Color#80c080");
			FormulaData R=M-((-L+(2*M))-(-H+2*M)); R.Name="R";R.SetAttrs("Color#80c080");
			FormulaData NONAME0=FILLRGN(1,S,R);NONAME0.SetAttrs("Brush#2000C000");
			return new FormulaPackage(new FormulaData[]{S,R,NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "Support & Resistance";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class SR2

	public class Compare:FormulaBase
	{
		public string STOCKCODE="";
		public Compare():base()
		{
			AddParam("StockCode","^DJI","0","0","",FormulaParamType.Symbol);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData NONAME0=FML(STOCKCODE,"C");NONAME0.SetAttrs("FirstDataOfView,HighQuality");
			SETNAME(STOCKCODE);
			return new FormulaPackage(new FormulaData[]{NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "Compare Stocks";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class Compare

	public class CmpIndi:FormulaBase
	{
		public string STOCKCODE="";
		public string INDI="";
		public CmpIndi():base()
		{
			AddParam("StockCode","^DJI","0","0","",FormulaParamType.Symbol);
			AddParam("Indi","RSI(14)","0","0","",FormulaParamType.Indicator);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData V1=FML(INDI); V1.Name="V1";V1.SetAttrs("HighQuality");
			FormulaData V2=FML(STOCKCODE,INDI); V2.Name="V2";V2.SetAttrs("HighQuality");
			SETNAME(V1,STKLABEL);
			SETNAME(V2,STOCKCODE);
			SETNAME(INDI);
			
			return new FormulaPackage(new FormulaData[]{V1,V2},"");
		}
	
		public override string LongName
		{
			get{return "Compare two indicators";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class CmpIndi

	public class SRAxisY:FormulaBase
	{
		public SRAxisY():base()
		{
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData M=(H+L+C)/3; M.Name="M";
			FormulaData A=H-L; A.Name="A";
			FormulaData RR=M+A; RR.Name="RR";
			FormulaData SS=M-A; SS.Name="SS";
			FormulaData R= DRAWAXISY(RR,-10,12); R.Name="R";R.SetAttrs("Width2,Color#A0FF0000,AxisMargin12");
			FormulaData S= DRAWAXISY(SS,-10,12); S.Name="S";S.SetAttrs("Width2,Color#A0004000");
			FormulaData NONAME0=DRAWTEXTAXISY(RR,"R",1);NONAME0.SetAttrs("Color#FF0000,VCenter");
			FormulaData NONAME1=DRAWTEXTAXISY(SS,"S",1);NONAME1.SetAttrs("Color#004000,VCenter");
			SETNAME("SR");
			return new FormulaPackage(new FormulaData[]{R,S,NONAME0,NONAME1},"");
		}
	
		public override string LongName
		{
			get{return "Support & Resistance on AxisY";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class SRAxisY

	public class SlowSTO2:FormulaBase
	{
		public double N=0;
		public double M1=0;
		public double M2=0;
		public SlowSTO2():base()
		{
			AddParam("N","14","1","100","",FormulaParamType.Double);
			AddParam("M1","3","1","50","",FormulaParamType.Double);
			AddParam("M2","9","1","50","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData A=(C-LLV(L,N))/(HHV(H,N)-LLV(L,N))*100; A.Name="A";
			FormulaData K=MA(A,M1); K.Name="K";K.SetAttrs("ColorDarkGreen,Width2,HighQuality");
			FormulaData D=MA(K,M2); D.Name="D";
			FormulaData NONAME0=PARTLINE(K>=D,K);NONAME0.SetAttrs("ColorRed,Width2,HighQuality");
			
			return new FormulaPackage(new FormulaData[]{K,D,NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "Slow Stochastic";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class SlowSTO2

	public class RefIndi:FormulaBase
	{
		public string INDI="";
		public double N=0;
		public RefIndi():base()
		{
			AddParam("Indi","MACD[DIFF]","0","0","",FormulaParamType.String);
			AddParam("N","10","1","10000","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData NONAME0=REF(FML(INDI),N);
			SETNAME(INDI+"-"+N);
			return new FormulaPackage(new FormulaData[]{NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "Reference indicator's value";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class RefIndi

	public class MainArea:FormulaBase
	{
		public double N=0;
		public MainArea():base()
		{
			AddParam("N","100","1","100000","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData MAIN= C; MAIN.Name="MAIN";MAIN.SetAttrs("HighQuality");
			FormulaData NONAME0=FILLAREA(MAIN);NONAME0.SetAttrs("Brush#20808000");
			SETTEXTVISIBLE(MAIN,FALSE);
			SETTEXTVISIBLE(FALSE);
			return new FormulaPackage(new FormulaData[]{MAIN,NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "Main Area View";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class MainArea

	public class SR3:FormulaBase
	{
		public SR3():base()
		{
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData P= (H+L+C)/3; P.Name="P";
			FormulaData YL= REF(L,1); YL.Name="YL";
			FormulaData YH= REF(H,1); YH.Name="YH";
			FormulaData R1= 2 * P - YL; R1.Name="R1";
			FormulaData S1= 2 * P - YH; S1.Name="S1";
			FormulaData R2= (P - S1) + R1; R2.Name="R2";
			FormulaData S2= P - (R1-S1); S2.Name="S2";
			FormulaData R3= (P + R2) - S1; R3.Name="R3";
			FormulaData S3= (P - R2) + S1; S3.Name="S3";
			FormulaData R4= (P + R3) - S1; R4.Name="R4";
			FormulaData S4= (P - R3) + S1; S4.Name="S4";
			FormulaData R5= (P + R4) - S1; R5.Name="R5";
			FormulaData S5= (P - R4) + S1; S5.Name="S5";
			return new FormulaPackage(new FormulaData[]{R1,S1,R2,S2,R3,S3,R4,S4,R5,S5},"");
		}
	
		public override string LongName
		{
			get{return "Support & Resistance 2";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class SR3

	public class Compare2:FormulaBase
	{
		public string STOCKCODE="";
		public Compare2():base()
		{
			AddParam("StockCode","^DJI","0","0","",FormulaParamType.Symbol);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData NONAME0=FML(STOCKCODE,"STOCK");NONAME0.SetAttrs("FullView,HighQuality,MonoStock");
			SETNAME(STOCKCODE);
			return new FormulaPackage(new FormulaData[]{NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "Compare Stocks";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class Compare2

	public class TwoDiff:FormulaBase
	{
		public string SYMBOL2="";
		public TwoDiff():base()
		{
			AddParam("Symbol2","INTL","0","0","",FormulaParamType.String);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData C1=FML(SYMBOL2,"C")-C; C1.Name="C1";
			FormulaData H1=FML(SYMBOL2,"H")-H; H1.Name="H1";
			FormulaData L1=FML(SYMBOL2,"L")-L; L1.Name="L1";
			FormulaData O1=FML(SYMBOL2,"O")-O; O1.Name="O1";
			FormulaData NONAME0=GETSTOCK(O1,C1,MAX(C1,O1,H1,L1),MIN(C1,O1,H1,L1));
			
			return new FormulaPackage(new FormulaData[]{NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "Difference of Two Symbol";}
		}
	
		public override string Description
		{
			get{return "draw difference of two symbol in candle mode.";}
		}
	} //class TwoDiff

	public class TradingIcon:FormulaBase
	{
		public string INDI="";
		public TradingIcon():base()
		{
			AddParam("Indi","Trading.RSI","0","0","",FormulaParamType.Indicator);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData NONAME0=DRAWICON(FML(INDI+"[ExitLong]"),H,"DN.GIF");NONAME0.SetAttrs("TOP");
			FormulaData NONAME1=DRAWICON(FML(INDI+"[EnterLong]"),L,"UP.GIF");NONAME1.SetAttrs("BOTTOM");
			return new FormulaPackage(new FormulaData[]{NONAME0,NONAME1},"");
		}
	
		public override string LongName
		{
			get{return "Trading Icon";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class TradingIcon

	public class Compare3:FormulaBase
	{
		public string STOCKCODE="";
		public Compare3():base()
		{
			AddParam("StockCode","^DJI","0","0","",FormulaParamType.Symbol);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData NONAME0=FML(STOCKCODE,"C");NONAME0.SetAttrs("HighQuality");
			SETNAME(STOCKCODE);
			return new FormulaPackage(new FormulaData[]{NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "Compare Stocks";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class Compare3

	public class ColorSar:FormulaBase
	{
		public double N=0;
		public double STEP=0;
		public double MAXP=0;
		public ColorSar():base()
		{
			AddParam("N","10","1","100","",FormulaParamType.Double);
			AddParam("STEP","2","1","100","",FormulaParamType.Double);
			AddParam("MAXP","20","5","100","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData M= SAR(N,STEP,MAXP); M.Name="M";M.SetAttrs("COLORBLUE");
			FormulaData A1=STOCK; A1.Name="A1";A1.SetAttrs("MONOSTOCK,BRUSHRED,COLORRED");
			FormulaData A2=IF(M<C,STOCK,NAN); A2.Name="A2";A2.SetAttrs("MONOSTOCK,BRUSHBLUE,COLORBLUE");
			FormulaData A3=IF(M<C,M,NAN); A3.Name="A3";A3.SetAttrs("COLORRED,CIRCLEDOT");
			SETNAME(M,"SAR");
			SETTEXTVISIBLE(A1,FALSE);
			SETTEXTVISIBLE(A2,FALSE);
			SETTEXTVISIBLE(A3,FALSE);
			SETTEXTVISIBLE(FALSE);
			return new FormulaPackage(new FormulaData[]{M,A1,A2,A3},"");
		}
	
		public override string LongName
		{
			get{return "Color candle bar based on sar";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class ColorSar

	#endregion

	#region Formula Group Intraday
	public class OpeningRange:FormulaBase
	{
		public double N=0;
		public OpeningRange():base()
		{
			AddParam("N","100000","0","240000","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData H1=IF(ISLASTDAY & TIME<=N,H,NAN); H1.Name="H1";
			FormulaData L1=IF(ISLASTDAY & TIME<=N,L,NAN); L1.Name="L1";
			FormulaData HH=HHV(H1,0); HH.Name="HH";
			FormulaData LL=LLV(L1,0); LL.Name="LL";
			FormulaData NONAME0=EXTEND(IF(HH.LASTVALUE==HH,HH,NAN));NONAME0.SetAttrs("StyleDash,Width2");
			FormulaData NONAME1=EXTEND(IF(LL.LASTVALUE==LL,LL,NAN));NONAME1.SetAttrs("StyleDash,Width2");
			
			return new FormulaPackage(new FormulaData[]{NONAME0,NONAME1},"");
		}
	
		public override string LongName
		{
			get{return "Opening Range";}
		}
	
		public override string Description
		{
			get{return "a horizontal line based on the high and low of the first X minutes of the trading day.";}
		}
	} //class OpeningRange

	public class PriorClose:FormulaBase
	{
		public PriorClose():base()
		{
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData NONAME0=FML(DP,"REFC#DAY");NONAME0.SetAttrs("StyleDash,ColorRed,NoValueLabel,Horizontal,Width2");
			SETTEXTVISIBLE(FALSE);
			return new FormulaPackage(new FormulaData[]{NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "Prior day's close";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class PriorClose

	public class PriorHLC:FormulaBase
	{
		public PriorHLC():base()
		{
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData NONAME0=FML(DP,"RefIndi(H,1)#DAY");NONAME0.SetAttrs("StyleDash,NoValueLabel,Horizontal,Width2");
			FormulaData NONAME1=FML(DP,"RefC#DAY");NONAME1.SetAttrs("StyleDash,NoValueLabel,Horizontal,Width2");
			FormulaData NONAME2=FML(DP,"RefIndi(L,1)#DAY");NONAME2.SetAttrs("StyleDash,NoValueLabel,Horizontal,Width2");
			SETTEXTVISIBLE(FALSE);
			return new FormulaPackage(new FormulaData[]{NONAME0,NONAME1,NONAME2},"");
		}
	
		public override string LongName
		{
			get{return "Prior day's High,Low,Close";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class PriorHLC

	public class TDP:FormulaBase
	{
		public double N=0;
		public TDP():base()
		{
			AddParam("N","3","0","1000","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData COND= BETWEEN(DOUBLEDATE,LASTDOUBLEDATE-N,LASTDOUBLEDATE-0.00001); COND.Name="Cond";
			FormulaData HH=HHV(IF(COND,H,NAN)).LASTVALUE; HH.Name="HH";
			FormulaData LL=LLV(IF(COND,L,NAN)).LASTVALUE; LL.Name="LL";
			FormulaData CC=IF(COND,C,NAN).LASTVALUE; CC.Name="CC";
			FormulaData P=(HH+LL+CC)/3; P.Name="P";
			FormulaData M=(HH+LL)/2; M.Name="M";
			FormulaData PD=ABS(P-M); PD.Name="PD";
			FormulaData PH=IF(ISLASTDAY,P+PD,NAN); PH.Name="PH";PH.SetAttrs("StyleDash,Width2");
			FormulaData PL=IF(ISLASTDAY,P-PD,NAN); PL.Name="PL";PL.SetAttrs("StyleDash,Width2");
			
			return new FormulaPackage(new FormulaData[]{PH,PL},"");
		}
	
		public override string LongName
		{
			get{return "Three Days Pivot Range";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class TDP

	#endregion

} // namespace FML
