using System;

namespace Easychart.Finance
{
	/// <summary>
	/// This class implement chinese calendar
	/// http://www.hh.gov.cn/nlyxldz.htm
	/// </summary>
	public class Chinese
	{
		static int[] LunarInfo ={
									0x04bd8,0x04ae0,0x0a570,0x054d5,0x0d260,0x0d950,0x16554,0x056a0,0x09ad0,0x055d2,
									0x04ae0,0x0a5b6,0x0a4d0,0x0d250,0x1d255,0x0b540,0x0d6a0,0x0ada2,0x095b0,0x14977,
									0x04970,0x0a4b0,0x0b4b5,0x06a50,0x06d40,0x1ab54,0x02b60,0x09570,0x052f2,0x04970,
									0x06566,0x0d4a0,0x0ea50,0x06e95,0x05ad0,0x02b60,0x186e3,0x092e0,0x1c8d7,0x0c950,
									0x0d4a0,0x1d8a6,0x0b550,0x056a0,0x1a5b4,0x025d0,0x092d0,0x0d2b2,0x0a950,0x0b557,
									0x06ca0,0x0b550,0x15355,0x04da0,0x0a5d0,0x14573,0x052d0,0x0a9a8,0x0e950,0x06aa0,
									0x0aea6,0x0ab50,0x04b60,0x0aae4,0x0a570,0x05260,0x0f263,0x0d950,0x05b57,0x056a0,
									0x096d0,0x04dd5,0x04ad0,0x0a4d0,0x0d4d4,0x0d250,0x0d558,0x0b540,0x0b5a0,0x195a6,
									0x095b0,0x049b0,0x0a974,0x0a4b0,0x0b27a,0x06a50,0x06d40,0x0af46,0x0ab60,0x09570,
									0x04af5,0x04970,0x064b0,0x074a3,0x0ea50,0x06b58,0x055c0,0x0ab60,0x096d5,0x092e0,
									0x0c960,0x0d954,0x0d4a0,0x0da50,0x07552,0x056a0,0x0abb7,0x025d0,0x092d0,0x0cab5,
									0x0a950,0x0b4a0,0x0baa4,0x0ad50,0x055d9,0x04ba0,0x0a5b0,0x15176,0x052b0,0x0a930,
									0x07954,0x06aa0,0x0ad50,0x05b52,0x04b60,0x0a6e6,0x0a4e0,0x0d260,0x0ea65,0x0d530,
									0x05aa0,0x076a3,0x096d0,0x04bd7,0x04ad0,0x0a4d0,0x1d0b6,0x0d250,0x0d520,0x0dd45,
									0x0b5a0,0x056d0,0x055b2,0x049b0,0x0a577,0x0a4b0,0x0aa50,0x1b255,0x06d20,0x0ada0
								};

		static int[] SolarMonth = {31,28,31,30,31,30,31,31,30,31,30,31};
		static string[] Gan = {"甲","乙","丙","丁","戊","己","庚","辛","壬","癸"};
		static string[] Zhi = {"子","丑","寅","卯","辰","巳","午","未","申","酉","戌","亥"};
		static string[] Animals = {"鼠","牛","虎","兔","龙","蛇","马","羊","猴","鸡","狗","猪"};
		static string[] SolarTerm = {"小寒","大寒","立春","雨水","惊蛰","春分","清明","谷雨","立夏","小满","芒种","夏至","小暑","大暑","立秋","处暑","白露","秋分","寒露","霜降","立冬","小雪","大雪","冬至"};
		static int[] TermInfo = {0,21208,42467,63836,85337,107014,128867,150921,173149,195551,218072,240693,263343,285989,308563,331033,353350,375494,397447,419210,440795,462224,483532,504758};
		static string[] Str1 = {"日","一","二","三","四","五","六","七","八","九","十"};
		static string[] Str2 = {"初","十","廿","卅","　"};
		static string[] MonthName = {"1 月","2 月","3 月","4 月","5 月","6 月","7 月","8 月","9 月","10 月","11 月","12 月"};

		public Chinese()
		{
		}

		static int LeapMonth(int y) 
		{
			return (LunarInfo[y-1900] & 0xf);
		}

		static int LeapDays(int y) 
		{
			if (LeapMonth(y)!=0)  
				return (LunarInfo[y-1900] & 0x10000)!=0? 30: 29;
			else return 0;
		}

		static int MonthDays(int y,int m) 
		{
			return (LunarInfo[y-1900] & (0x10000>>m))!=0? 30: 29;
		}

		static int lYearDays(int y) 
		{
			int Sum = 348;
			for(int i=0x8000; i>0x8; i>>=1)
				Sum += (LunarInfo[y-1900] & i)!=0?1: 0;
			return (Sum+LeapDays(y));
		}

		static public LunarInfo Lunar(DateTime d) 
		{
			LunarInfo li = new LunarInfo();

			DateTime BaseDate = new DateTime(1900,1,1);
			int Offset = (int)((TimeSpan)(d - BaseDate)).TotalDays;

			li.DayCyl = Offset + 40;
			li.MonthCyl = 14;

			int Temp = 0;
			int i;
			for(i=1900; i<2050 && Offset>0; i++) 
			{
				Temp = lYearDays(i);
				Offset -= Temp;
				li.MonthCyl += 12;
			}

			if(Offset<0) 
			{
				Offset += Temp;
				i--;
				li.MonthCyl -= 12;
			}

			li.Year = i;
			li.YearCyl = i-1864;

			int Leap = LeapMonth(i); //闰哪个月
			li.IsLeap = false;

			for(i=1; i<13 && Offset>0; i++) 
			{
				//闰月
				if (Leap>0 && i==(Leap+1) && li.IsLeap==false)
				{
					i--; 
					li.IsLeap = true; 
					Temp = LeapDays(li.Year); 
				}
				else
				{
					Temp = MonthDays(li.Year, i);
				}

				//解除闰月
				if(li.IsLeap==true && i==(Leap+1)) li.IsLeap = false;

				Offset -= Temp;
				if(li.IsLeap == false) li.MonthCyl++;
			}

			if (Offset==0 && Leap>0 && i==Leap+1)
			{
				if(li.IsLeap)
					li.IsLeap = false; 
				else
				{
					li.IsLeap = true; 
					i--; 
					li.MonthCyl--;
				}
			}

			if (Offset<0)
			{
				Offset += Temp; 
				i--; 
				li.MonthCyl--; 
			}

			li.Month = i-1;
			li.Day = Offset;
			return li;
		}
	}

	public class LunarInfo 
	{
		public int Year;
		public int Month;
		public int Day;
		public bool IsLeap;
		public int YearCyl;
		public int MonthCyl;
		public int DayCyl;
	}
}
