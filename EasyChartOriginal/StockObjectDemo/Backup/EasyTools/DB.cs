#define VS2003
using System;
using System.Configuration;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Reflection;

namespace EasyTools
{

	public class DbParam
	{
		public string Name;
		public object Value;
		public DbType Type;
		public int Size;

		public DbParam(string AName,DbType AType,int ASize,object AValue) 
		{
			Name = AName;
			Value = AValue;
			Type = AType;
			Size = ASize;
		}

		public DbParam(string AName,DbType AType,object AValue) 
		{
			Name = AName;
			Value = AValue;
			Type = AType;
			Size = 0;
		}

		public static OleDbType GetOleDbType(DbType dt) 
		{
			switch (dt) 
			{
				case DbType.String :
					return OleDbType.VarChar;
				case DbType.Int32 :
					return OleDbType.Integer;
				case DbType.DateTime:
					return OleDbType.Date; // DBTimeStamp;
				case DbType.Int16:
					return OleDbType.SmallInt;
				case DbType.Double:
					return OleDbType.Double;
				case DbType.Boolean:
					return OleDbType.Boolean;
				case DbType.Binary:
					return OleDbType.VarBinary;
				default :
					return OleDbType.VarChar;
			}
		}

		public static SqlDbType GetSqlType(DbType dt)
		{
			switch (dt) 
			{
				case DbType.String :
					return SqlDbType.VarChar;
				case DbType.Int32 :
					return SqlDbType.Int;
				case DbType.DateTime:
					return SqlDbType.DateTime;
				case DbType.Int16:
					return SqlDbType.SmallInt;
				case DbType.Double:
					return SqlDbType.Float;
				case DbType.Boolean:
					return SqlDbType.Variant;
				case DbType.Binary:
					return SqlDbType.VarBinary;
				default :
					return SqlDbType.VarChar;
			}
		}

#if (VS2003)
		public static OdbcType GetOdbcType(DbType dt)
		{
			switch (dt) 
			{
				case DbType.String :
					return OdbcType.VarChar;
				case DbType.Int32 :
					return OdbcType.Int;
				case DbType.DateTime:
					return OdbcType.DateTime;
				case DbType.Int16:
					return OdbcType.SmallInt;
				case DbType.Double:
					return OdbcType.Double;
				case DbType.Boolean:
					return OdbcType.SmallInt;
				case DbType.Binary:
					return OdbcType.VarBinary;
				default :
					return OdbcType.VarChar;
			}
		}
#endif

	}

	public abstract class BaseDb 
	{
		public bool IsMySql;
		public DataTable GetCacheDataSet(string CacheName,string sql)
		{
			DataTable dt = (DataTable)HttpRuntime.Cache[CacheName];
			if (dt==null) 
			{
				dt = GetDataSet(sql).Tables[0];
				HttpRuntime.Cache[CacheName] = dt;
			};
			return dt;
		}

		public string ReplaceParam(string sql,DbParam[] dp) 
		{
			StringBuilder sb = new StringBuilder(sql);
			int i = 0;
			for (int j=0; j<sb.Length; j++)
				if (sb[j]=='?') 
				{
					while (dp[i]==null) i++;
					sb.Replace("?",dp[i++].Name,j,1);
				}
			return sb.ToString();
		}

		public static BaseDb GetBaseDb(string ConnStr) 
		{
			ConnStr= ConnStr.Replace("@",HttpRuntime.AppDomainAppPath);
			if (ConnStr.StartsWith("Provider"))
				return new OleDb(ConnStr);
			else if (ConnStr.StartsWith("Init"))
				return new SQL(ConnStr);
#if (VS2003)
			else 
			{
				if (ConnStr.StartsWith("#"))
				{
					int i = ConnStr.IndexOf("|");
					if (i>0)
					{
						string s = ConnStr.Substring(1,i-1).Trim();
						ConnStr = ConnStr.Substring(i+1).Trim();

						Type t = Type.GetType(s);
						if (t.BaseType==typeof(BaseDb)) 
							return Activator.CreateInstance(t,new object[]{ConnStr}) as BaseDb;
					}
				} 
				return new ODBC(ConnStr);
			}
#else
			else 
				return null;
#endif
		}

		public static BaseDb FromConfig(string ConnStr) 
		{
			return GetBaseDb(ConfigurationSettings.AppSettings[ConnStr]);
		}

		public static BaseDb Current 
		{
			get 
			{
				BaseDb bd = null;
				if (HttpContext.Current!=null)
					bd= (BaseDb)HttpContext.Current.Items["BaseDb"];
				if (bd==null) 
				{
					bd= FromConfig("ConnStr");// GetBaseDb(ConfigurationSettings.AppSettings["ConnStr"]);
					if (HttpContext.Current!=null)
						HttpContext.Current.Items["BaseDb"] = bd;
				} 
				return bd; 
			}
		}

		public int DoCommand(string sql)
		{
			return DoCommand(sql,null);
		}

		public DataSet GetDataSet(string sql)
		{
			return GetDataSet(sql,null);
		}

		public bool HasRecord(string sql)
		{
			return HasRecord(sql,null);
		}

		public bool HasRecord(string sql,DbParam[] dp)
		{
			object o = GetValue(sql,dp);
			return o!=null;
		}

		public object GetValue(string sql)
		{
			return GetValue(sql,null);
		}

		public string GetTopN(string sql,int Count) 
		{
			if (IsMySql)
				return sql+" limit "+Count;
			else return sql.Substring(0,6)+" top "+Count+" "+sql.Substring(7);
		}

		public DataTable GetDataTable(string sql,DbParam[] dp,int Count)
		{
			return GetDataTable(GetTopN(sql,Count),dp);
		}

		public DataTable GetDataTable(string sql,DbParam[] dp,int startRecord,int maxRecords) 
		{
			if (IsMySql)
				return GetDataSet(sql+" limit "+startRecord+","+maxRecords,dp,0,maxRecords).Tables[0];
			else return GetDataSet(sql,dp,startRecord,maxRecords).Tables[0];
		}

		public DataTable GetDataTable(string sql,DbParam[] dp) 
		{
			return GetDataSet(sql,dp).Tables[0];
		}

		public DataTable GetDataTable(string sql) 
		{
			return GetDataTable(sql,null);
		}

		public DataRow GetFirstRow(string sql,DbParam[] dp,int Count) 
		{
			return GetFirstRow(GetTopN(sql,Count),dp);
		}

		public DataRow GetFirstRow(string sql,DbParam[] dp) 
		{
			DataTable dt = GetDataTable(sql,dp);
			if (dt.Rows.Count>0)
				return dt.Rows[0];
			else return null;
		}

		public DataRow GetFirstRow(string sql) 
		{
			return GetFirstRow(sql,null);
		}

		public DataSet GetDataSet(string sql,DbParam[] dp) 
		{
			return GetDataSet(sql,dp,0,0);
		}

		public string GetCommaValues(string sql,DbParam[] dp,string Separator) 
		{
			DataTable dt = GetDataTable(sql,dp);
			StringBuilder sb = new StringBuilder();
			for(int i=0; i<dt.Rows.Count; i++) 
			{
				if (i!=0)
					sb.Append(',');
				sb.Append(Separator);
				sb.Append(dt.Rows[i][0]);
				sb.Append(Separator);
			}
			return sb.ToString();
		}

		public string GetCommaValues(string sql,string Separtor) 
		{
			return GetCommaValues(sql,null,Separtor);
		}

		public abstract DataSet GetDataSet(string sql,DbParam[] dp,int startRecord,int maxRecords);
		public abstract object GetValue(string sql,DbParam[] dp);
		public abstract int DoCommand(string sql,DbParam[] dp);
		public abstract BaseDb Open(bool UseTransaction);
		public abstract void Close();
		public abstract void RollBack();
	}

	public class OleDb:BaseDb
	{
		private OleDbConnection oc;
		private OleDbTransaction ot = null;

		public OleDb(string ConnStr) 
		{
			oc = new OleDbConnection(ConnStr);
		}
		
		public OleDbDataReader GetDataReader(string sql) 
		{
			return GetDataReader(sql,CommandBehavior.Default);
		}

		public OleDbDataReader GetDataReader(string sql, CommandBehavior cb) 
		{
			OleDbCommand od = new OleDbCommand(sql,oc);
			return od.ExecuteReader(cb);
		}

		public override DataSet GetDataSet(string sql,DbParam[] dp,int startRecord,int maxRecords)
		{
			OleDbCommand od = new OleDbCommand(sql,oc);
			od.Transaction = ot;

			if (dp!=null)
				for (int i=0; i<dp.Length; i++) 
					if (dp[i]!=null)
						od.Parameters.Add(dp[i].Name,DbParam.GetOleDbType(dp[i].Type),dp[i].Size).Value =dp[i].Value;

			OleDbDataAdapter da = new OleDbDataAdapter(od);
			DataSet ds = new DataSet();
			da.Fill(ds,startRecord,maxRecords,"Table0");
			return ds;
		}


		public override object GetValue(string sql,DbParam[] dp)
		{
			OleDbCommand od = new OleDbCommand(sql,oc);
			od.Transaction = ot;

			if (dp!=null)
				for (int i=0; i<dp.Length; i++) 
					if (dp[i]!=null)
						od.Parameters.Add(dp[i].Name,DbParam.GetOleDbType(dp[i].Type),dp[i].Size).Value =dp[i].Value;
			object rst = null;

			if (od.Connection.State == ConnectionState.Open)
				rst = od.ExecuteScalar();
			else 
			{
				od.Connection.Open();
				try 
				{
					rst = od.ExecuteScalar();
				} 
				finally
				{
					od.Connection.Close();
				}
			}
			return rst;
		}

		public override int DoCommand(string sql,DbParam[] dp)
		{
			OleDbCommand od = new OleDbCommand(sql,oc);
			od.Transaction = ot;
			if (dp!=null)
				for (int i=0; i<dp.Length; i++) 
					if (dp[i]!=null)
						od.Parameters.Add(dp[i].Name,DbParam.GetOleDbType(dp[i].Type),dp[i].Size).Value =dp[i].Value;
			int rst = -1;
			if (od.Connection.State == ConnectionState.Open)
				rst = od.ExecuteNonQuery();
			else 
			{
				od.Connection.Open();
				try 
				{
					rst = od.ExecuteNonQuery();
				} 
				finally
				{
					od.Connection.Close();
				}
			}
			return rst;
		}

		public override BaseDb Open(bool UseTransaction)
		{
			oc.Open();
			if (UseTransaction) 
				ot = oc.BeginTransaction();
			return this;
		}

		public override void Close()
		{
			if (ot!=null) 
			{
				ot.Commit();
				ot = null;
			}
			oc.Close();
		}

		public override void RollBack() 
		{
			ot.Rollback();
			ot = null;
		}
	}

	public class SQL:BaseDb
	{
		private SqlConnection sc;
		private SqlTransaction st;

		public SQL(string ConnStr) 
		{
			sc = new SqlConnection(ConnStr);
		}

		public SqlDataReader GetDataReader(string sql)
		{
			return GetDataReader(sql,CommandBehavior.Default);
		}

		public SqlDataReader GetDataReader(string sql, CommandBehavior cb) 
		{
			SqlCommand od = new SqlCommand(sql,sc);
			return od.ExecuteReader(cb);
		}

		public override DataSet GetDataSet(string sql,DbParam[] dp,int startRecord,int maxRecords)
		{
			SqlCommand od = new SqlCommand(ReplaceParam(sql,dp),sc);
			od.CommandTimeout = 300;
			od.Transaction = st;

			if (dp!=null)
				for (int i=0; i<dp.Length; i++) 
					if (dp[i]!=null)
						od.Parameters.Add(dp[i].Name,DbParam.GetSqlType(dp[i].Type),dp[i].Size).Value =dp[i].Value;
			SqlDataAdapter da = new SqlDataAdapter(od);
			DataSet ds = new DataSet();
			da.Fill(ds,startRecord,maxRecords,"Table0");
			return ds;
		}

		public override object GetValue(string sql,DbParam[] dp)
		{
			SqlCommand od = new SqlCommand(ReplaceParam(sql,dp),sc);
			od.Transaction = st;
			if (dp!=null)
				for (int i=0; i<dp.Length; i++) 
					if (dp[i]!=null)
						od.Parameters.Add(dp[i].Name,DbParam.GetSqlType(dp[i].Type),dp[i].Size).Value =dp[i].Value;

			object rst = null;
			if (od.Connection.State == ConnectionState.Open)
				rst = od.ExecuteScalar();
			else 
			{
				od.Connection.Open();
				try 
				{
					rst = od.ExecuteScalar();
				} 
				finally
				{
					od.Connection.Close();
				}
			}
			return rst;
		}

		public override int DoCommand(string sql,DbParam[] dp)
		{
			SqlCommand od = new SqlCommand(ReplaceParam(sql,dp),sc);
			od.CommandTimeout = 300;
			od.Transaction = st;
			if (dp!=null)
				for (int i=0; i<dp.Length; i++)
					if (dp[i]!=null) 
						od.Parameters.Add(dp[i].Name,DbParam.GetSqlType(dp[i].Type),dp[i].Size).Value = dp[i].Value;
			int rst = -1;

			if (od.Connection.State == ConnectionState.Open)
				rst = od.ExecuteNonQuery();
			else 
			{
				od.Connection.Open();
				try 
				{
					rst = od.ExecuteNonQuery();
				} 
				finally
				{
					od.Connection.Close();
				}
			}
			return rst;
		}

		public override BaseDb Open(bool UseTransaction) 
		{
			sc.Open();
			if (UseTransaction) 
				st = sc.BeginTransaction();
			return this;
		}

		public override void Close() 
		{
			if (st!=null) 
			{
				st.Commit();
				st = null;
			}
			sc.Close();
		}

		public override void RollBack() 
		{
			st.Rollback();
			st = null;
		}
	}

#if (VS2003)
	public class ODBC:BaseDb
	{
		private OdbcConnection sc;
		private OdbcTransaction st;

		public ODBC(string ConnStr) 
		{
			sc = new OdbcConnection(ConnStr);
			IsMySql = ConnStr.ToLower().IndexOf("mysql")>0;
		}

		public override DataSet GetDataSet(string sql,DbParam[] dp,int startRecord,int maxRecords)
		{
			OdbcCommand od = new OdbcCommand(sql,sc);
			od.Transaction = st;

			if (dp!=null)
				for (int i=0; i<dp.Length; i++) 
					if (dp[i]!=null)
						od.Parameters.Add(dp[i].Name,DbParam.GetOdbcType(dp[i].Type),dp[i].Size).Value =dp[i].Value;
			OdbcDataAdapter da = new OdbcDataAdapter(od);
			DataSet ds = new DataSet();
			da.Fill(ds,startRecord,maxRecords,"Table0");
			return ds;
		}

		public override object GetValue(string sql,DbParam[] dp)
		{
			//OdbcCommand od = new OdbcCommand(ReplaceParam(sql,dp),sc);
			OdbcCommand od = new OdbcCommand(sql,sc);
			od.Transaction = st;
			if (dp!=null)
				for (int i=0; i<dp.Length; i++) 
					if (dp[i]!=null)
						od.Parameters.Add(dp[i].Name,DbParam.GetOdbcType(dp[i].Type),dp[i].Size).Value =dp[i].Value;

			object rst = null;
			if (od.Connection.State == ConnectionState.Open)
				rst = od.ExecuteScalar();
			else 
			{
				od.Connection.Open();
				try 
				{
					rst = od.ExecuteScalar();
				} 
				finally
				{
					od.Connection.Close();
				}
			}
			return rst;
		}

		public override int DoCommand(string sql,DbParam[] dp)
		{
			//OdbcCommand od = new OdbcCommand(ReplaceParam(sql,dp),sc);
			OdbcCommand od = new OdbcCommand(sql,sc);
			od.Transaction = st;
			if (dp!=null)
				for (int i=0; i<dp.Length; i++)
					if (dp[i]!=null) 
						od.Parameters.Add(dp[i].Name,DbParam.GetOdbcType(dp[i].Type),dp[i].Size).Value = dp[i].Value;
			int rst = -1;

			if (od.Connection.State == ConnectionState.Open)
				rst = od.ExecuteNonQuery();
			else 
			{
				od.Connection.Open();
				try 
				{
					rst = od.ExecuteNonQuery();
				} 
				finally
				{
					od.Connection.Close();
				}
			}
			return rst;
		}

		public override BaseDb Open(bool UseTransaction) 
		{
			sc.Open();
			if (UseTransaction) 
				st = sc.BeginTransaction();
			return this;
		}

		public override void Close() 
		{
			if (st!=null) 
			{
				st.Commit();
				st = null;
			}
			sc.Close();
		}

		public override void RollBack() 
		{
			try 
			{
				st.Rollback();
			}
			finally
			{
				st = null;
			}
		}
	}
#endif

	public class DB
	{
		public static int DoCommand(string sql)
		{
			return BaseDb.Current.DoCommand(sql,null);
		}

		public static int DoCommand(string sql,DbParam[] dp)
		{
			return BaseDb.Current.DoCommand(sql,dp);
		}
		public static DataSet GetDataSet(string sql)
		{
			return BaseDb.Current.GetDataSet(sql,null);
		}

		public static DataSet GetDataSet(string sql,DbParam[] dp) 
		{
			return BaseDb.Current.GetDataSet(sql,dp);
		}

		public static DataSet GetDataSet(string sql,DbParam[] dp,int startRecord,int maxRecords) 
		{
			return BaseDb.Current.GetDataSet(sql,dp,startRecord,maxRecords);
		}

		public static bool HasRecord(string sql)
		{
			return BaseDb.Current.HasRecord(sql,null);
		}

		public static bool HasRecord(string sql,DbParam[] dp) 
		{
			return BaseDb.Current.HasRecord(sql,dp);
		}

		public static object GetValue(string sql)
		{
			return BaseDb.Current.GetValue(sql,null);
		}

		public static object GetValue(string sql,DbParam[] dp) 
		{
			return BaseDb.Current.GetValue(sql,dp);
		}

		public static DataTable GetDataTable(string sql) 
		{
			return GetDataTable(sql,null);
		}

		public static DataTable GetDataTable(string sql,DbParam[] dp) 
		{
			return GetDataSet(sql,dp).Tables[0];
		}

		public static DataTable GetDataTable(string sql,DbParam[] dp,int startRecord,int maxRecord) 
		{
			return GetDataSet(sql,dp,startRecord,maxRecord).Tables[0];
		}

		public static DataTable GetDataTable(string sql,DbParam[] dp,int Count)
		{
			return BaseDb.Current.GetDataTable(sql,dp,Count);
		}

		public static DataTable GetCacheDataSet(string CacheName,string sql) 
		{
			return BaseDb.Current.GetCacheDataSet(CacheName,sql);
		}

		public static DataTable GetCacheDataSet(string CacheName) 
		{
			return BaseDb.Current.GetCacheDataSet(CacheName,"select * from "+CacheName);
		}

		public static DataRow GetFirstRow(string sql,DbParam[] dp) 
		{
			return BaseDb.Current.GetFirstRow(sql,dp);
		}

		public static DataRow GetFirstRow(string sql) 
		{
			return BaseDb.Current.GetFirstRow(sql,null);
		}

		public static DataRow GetFirstRow(string sql,DbParam[] dp,int Count) 
		{
			return BaseDb.Current.GetFirstRow(sql,dp,Count);
		}

		public static string GetCommaValues(string sql,string Separtor) 
		{
			return BaseDb.Current.GetCommaValues(sql,Separtor);
		}

		public static string GetCommaValues(string sql,DbParam[] dp,string Separtor) 
		{
			return BaseDb.Current.GetCommaValues(sql,dp,Separtor);
		}

		public static BaseDb Open(bool UseTransaction) 
		{
			return BaseDb.Current.Open(UseTransaction);
		}

		public static void RollBack() 
		{
			BaseDb.Current.RollBack();
		}

		public static void Close() 
		{
			BaseDb.Current.Close();
		}
	}
}
