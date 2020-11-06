using System;
using System.Security.Principal;

namespace Identity
{
    public class WindowsUserImpersonator
    {
        public WindowsIdentity Identity { get; set; }

        public WindowsUserImpersonator(string username, string domain, string password)
        {
            if (!Login(username, domain, password))
            {
                throw new Exception("Impersonation Failed");
            }
        }

        public bool Login(string userName, string domain, string password)
        {
            IntPtr access_token = IntPtr.Zero;
            if (NativeMethods.RevertToSelf() && NativeMethods.LogonUserA(userName, domain, password, NativeMethods.LOGON32_LOGON_NEW_CREDENTIALS, NativeMethods.LOGON32_PROVIDER_DEFAULT, ref access_token) != 0)
            {
                Identity = new WindowsIdentity(access_token);
            }
            return false;
        }

        public void Dispose()
        {
            if (this.Identity == null)
                return;

            Identity.AccessToken.Close();
            Identity.Dispose();
            NativeMethods.RevertToSelf();
        }
    }
}
