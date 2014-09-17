using System;
using System.Security.Principal;
using System.Runtime.InteropServices;
using EasyTools;

namespace WebDemos
{
	/// <summary>
	/// Summary description for Impersonate.
	/// </summary>
	public class Impersonate
	{
		public Impersonate()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		private const int LOGON32_LOGON_INTERACTIVE = 2;
		private const int LOGON32_PROVIDER_DEFAULT = 0;
		[DllImport("advapi32.dll", CharSet=CharSet.Auto)]
		private static extern int LogonUser(String lpszUserName, 
			String lpszDomain,
			String lpszPassword,
			int dwLogonType, 
			int dwLogonProvider,
			ref IntPtr phToken);
		[DllImport("advapi32.dll", CharSet=System.Runtime.InteropServices.CharSet.Auto,
			 SetLastError=true)]
		private extern static int DuplicateToken(IntPtr hToken, 
			int impersonationLevel,  
			ref IntPtr hNewToken);
		
		private static WindowsImpersonationContext impersonationContext; 
		public static bool ChangeUser(string userName, string domain, string password)
		{
			WindowsIdentity tempWindowsIdentity;
			IntPtr token = IntPtr.Zero;
			IntPtr tokenDuplicate = IntPtr.Zero;

			if(LogonUser(userName, domain, password, LOGON32_LOGON_INTERACTIVE, 
				LOGON32_PROVIDER_DEFAULT, ref token) != 0)
			{
				if(DuplicateToken(token, 2, ref tokenDuplicate) != 0) 
				{
					tempWindowsIdentity = new WindowsIdentity(tokenDuplicate);
					impersonationContext = tempWindowsIdentity.Impersonate();
					if (impersonationContext != null)
						return true;
					else
						return false; 
				}
				else
					return false;
			} 
			else
				return false;
		}

		public static bool ChangeToAdmin()
		{
			if (Config.EnableChangeToAdmin)
			{
					if (Config.AdminUserName!=null && Config.AdminPassword!=null &&
						Config.AdminUserName!="" && Config.AdminPassword!="") 
					{
						Tools.Log("Before Chagne User "+Config.AdminUserName+"/"+Config.AdminPassword);
						return ChangeUser(Config.AdminUserName,"",Config.AdminPassword);
					}
			}
			return false;
		}
		
		public static void Undo()
		{
			impersonationContext.Undo();
		} 
	}
}
