using System;
using System.Configuration;
using System.IO;
using System.Globalization;
using System.Data;
using System.Web.Mail;
using System.Text;
using System.Web;

namespace EasyTools
{
	/// <summary>
	/// Summary description for SendMail.
	/// </summary>
	public class SendMail
	{
		public SendMail()
		{
		}

		public static bool SendNotify(string ToEmail, string Subject, string Body) 
		{
			try 
			{
				SmtpMail.SmtpServer = ConfigurationSettings.AppSettings["SmtpServer"];
				MailMessage mm = new MailMessage();
				mm.BodyEncoding = Encoding.GetEncoding("iso-8859-1");
				mm.BodyFormat = MailFormat.Html;
				mm.From = ConfigurationSettings.AppSettings["FromName"];
				mm.To = ToEmail;
				if (mm.To!="") 
				{
					mm.Subject = Subject; 
					mm.Body = Body.Replace("\r\n","<br>");
					SmtpMail.Send(mm);
				}
				return true;
			} 
			catch (Exception e) 
			{
				Tools.Log("SendNotify:"+e.ToString());
			}
			return false;
		}

		public static bool SendNotify(string Content, DataRow dr ,string ToEmail, string Params)
		{
			try
			{
				string[] cc = Content.Split('\r');
				string s1 = cc[0].ToString().Trim();
				string s2 = string.Join("\r",cc,1,cc.Length-1).Trim();

				string s3,s4;
				string[] ss = Params.Split(',');
				if (ss!=null && HttpContext.Current!=null)
					foreach(string s in ss) 
					{
						s3 ="%"+s+"%";
						s4 = HttpContext.Current.Request.Params[s];
						if (s4!=null)
							s2 = s2.Replace(s3,s4);
					}

				if (dr!=null)
					for(int i =0; i<dr.Table.Columns.Count; i++) 
					{
						s3 = "%" + dr.Table.Columns[i].ColumnName+"%";
						if (dr[i]!=DBNull.Value)
						{
							s4 = dr[i].ToString();
							if (s4!=null) 
							{
								s2 = s2.Replace(s3,s4);
								s2 = s2.Replace(s3,s4);
							}
						}
					}

				return SendNotify(ToEmail,s1,s2);
			}
			catch(Exception e) 
			{
				Tools.Log("Send Notify Txt:"+e.ToString());
			}
			return false;
		}

		public static bool SendNotify(Stream stream, DataRow dr ,string ToEmail, string Params)
		{
			StreamReader sr = new StreamReader(stream);
			return SendNotify(sr.ReadToEnd(),dr,ToEmail,Params);
		}

		public static bool SendNotifyFromFile(string FileName, DataRow dr ,string ToEmail, string Params)
		{
			return SendNotify(File.OpenRead(HttpContext.Current.Request.MapPath(FileName)),dr,ToEmail,Params);
		}
	}
}
