using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceTools
{
    public class ServiceInstaller
    {
        private const int STANDARD_RIGHTS_REQUIRED = 983040;
        private const int SERVICE_WIN32_OWN_PROCESS = 16;

        [DllImport("advapi32.dll", EntryPoint = "OpenSCManagerA")]
        private static extern IntPtr OpenSCManager(
          string lpMachineName,
          string lpDatabaseName,
          ServiceManagerRights dwDesiredAccess);

        [DllImport("advapi32.dll", EntryPoint = "OpenServiceA", CharSet = CharSet.Ansi)]
        private static extern IntPtr OpenService(
          IntPtr hSCManager,
          string lpServiceName,
          ServiceRights dwDesiredAccess);

        [DllImport("advapi32.dll", EntryPoint = "CreateServiceA")]
        private static extern IntPtr CreateService(
          IntPtr hSCManager,
          string lpServiceName,
          string lpDisplayName,
          ServiceRights dwDesiredAccess,
          int dwServiceType,
          ServiceBootFlag dwStartType,
          ServiceError dwErrorControl,
          string lpBinaryPathName,
          string lpLoadOrderGroup,
          IntPtr lpdwTagId,
          string lpDependencies,
          string lp,
          string lpPassword);

        [DllImport("advapi32.dll")]
        private static extern int CloseServiceHandle(IntPtr hSCObject);

        [DllImport("advapi32.dll")]
        private static extern int QueryServiceStatus(
          IntPtr hService,
          SERVICE_STATUS lpServiceStatus);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern int DeleteService(IntPtr hService);

        [DllImport("advapi32.dll")]
        private static extern int ControlService(
          IntPtr hService,
          ServiceControl dwControl,
          SERVICE_STATUS lpServiceStatus);

        [DllImport("advapi32.dll", EntryPoint = "StartServiceA")]
        private static extern int StartService(
          IntPtr hService,
          int dwNumServiceArgs,
          int lpServiceArgVectors);

        public static void Uninstall(string ServiceName)
        {
            IntPtr num1 = OpenSCManager(ServiceManagerRights.Connect);
            try
            {
                IntPtr num2 = OpenService(num1, ServiceName, ServiceRights.StandardRightsRequired | ServiceRights.QueryStatus | ServiceRights.Stop);
                if (num2 == IntPtr.Zero)
                    throw new Exception("Service not installed.");
                try
                {
                    StopService(num2);
                    if (DeleteService(num2) == 0)
                        throw new Exception("Could not delete service " + (object)Marshal.GetLastWin32Error());
                }
                finally
                {
                    CloseServiceHandle(num2);
                }
            }
            finally
            {
                CloseServiceHandle(num1);
            }
        }

        public static bool ServiceIsInstalled(string ServiceName)
        {
            IntPtr num = OpenSCManager(ServiceManagerRights.Connect);
            try
            {
                IntPtr hSCObject = OpenService(num, ServiceName, ServiceRights.QueryStatus);
                if (hSCObject == IntPtr.Zero)
                    return false;
                CloseServiceHandle(hSCObject);
                return true;
            }
            finally
            {
                CloseServiceHandle(num);
            }
        }

        public static void InstallAndStart(string ServiceName, string DisplayName, string FileName)
        {
            IntPtr num1 = OpenSCManager(ServiceManagerRights.Connect | ServiceManagerRights.CreateService);
            try
            {
                IntPtr num2 = OpenService(num1, ServiceName, ServiceRights.QueryStatus | ServiceRights.Start);
                if (num2 == IntPtr.Zero)
                    num2 = CreateService(num1, ServiceName, DisplayName, ServiceRights.QueryStatus | ServiceRights.Start, 16, ServiceBootFlag.AutoStart, ServiceError.Normal, FileName, (string)null, IntPtr.Zero, (string)null, (string)null, (string)null);
                if (num2 == IntPtr.Zero)
                    throw new Exception("Failed to install service.");
                try
                {
                    StartService(num2);
                }
                finally
                {
                    CloseServiceHandle(num2);
                }
            }
            finally
            {
                CloseServiceHandle(num1);
            }
        }

        public static void StartService(string Name)
        {
            IntPtr num1 = OpenSCManager(ServiceManagerRights.Connect);
            try
            {
                IntPtr num2 = OpenService(num1, Name, ServiceRights.QueryStatus | ServiceRights.Start);
                if (num2 == IntPtr.Zero)
                    throw new Exception("Could not open service.");
                try
                {
                    StartService(num2);
                }
                finally
                {
                    CloseServiceHandle(num2);
                }
            }
            finally
            {
                CloseServiceHandle(num1);
            }
        }

        public static void StopService(string Name)
        {
            IntPtr num1 = OpenSCManager(ServiceManagerRights.Connect);
            try
            {
                IntPtr num2 = OpenService(num1, Name, ServiceRights.QueryStatus | ServiceRights.Stop);
                if (num2 == IntPtr.Zero)
                    throw new Exception("Could not open service.");
                try
                {
                    StopService(num2);
                }
                finally
                {
                    CloseServiceHandle(num2);
                }
            }
            finally
            {
                CloseServiceHandle(num1);
            }
        }

        private static void StartService(IntPtr hService)
        {
            //SERVICE_STATUS serviceStatus = new SERVICE_STATUS();
            StartService(hService, 0, 0);
            WaitForServiceStatus(hService, ServiceState.Starting, ServiceState.Run);
        }

        private static void StopService(IntPtr hService)
        {
            SERVICE_STATUS lpServiceStatus = new SERVICE_STATUS();
            ControlService(hService, ServiceControl.Stop, lpServiceStatus);
            WaitForServiceStatus(hService, ServiceState.Stopping, ServiceState.Stop);
        }

        public static ServiceState GetServiceStatus(string ServiceName)
        {
            IntPtr num = OpenSCManager(ServiceManagerRights.Connect);
            try
            {
                IntPtr hService = OpenService(num, ServiceName, ServiceRights.QueryStatus);
                if (hService == IntPtr.Zero)
                    return ServiceState.NotFound;
                try
                {
                    return GetServiceStatus(hService);
                }
                finally
                {
                    CloseServiceHandle(num);
                }
            }
            finally
            {
                CloseServiceHandle(num);
            }
        }

        private static ServiceState GetServiceStatus(IntPtr hService)
        {
            SERVICE_STATUS lpServiceStatus = new SERVICE_STATUS();
            if (QueryServiceStatus(hService, lpServiceStatus) == 0)
                throw new Exception("Failed to query service status.");
            return lpServiceStatus.dwCurrentState;
        }

        private static bool WaitForServiceStatus(
          IntPtr hService,
          ServiceState WaitStatus,
          ServiceState DesiredStatus)
        {
            SERVICE_STATUS lpServiceStatus = new SERVICE_STATUS();
            QueryServiceStatus(hService, lpServiceStatus);
            if (lpServiceStatus.dwCurrentState == DesiredStatus)
                return true;
            int tickCount = Environment.TickCount;
            int dwCheckPoint = lpServiceStatus.dwCheckPoint;
            while (lpServiceStatus.dwCurrentState == WaitStatus)
            {
                int millisecondsTimeout = lpServiceStatus.dwWaitHint / 10;
                if (millisecondsTimeout < 1000)
                    millisecondsTimeout = 1000;
                else if (millisecondsTimeout > 10000)
                    millisecondsTimeout = 10000;
                Task.Delay(millisecondsTimeout);
                if (QueryServiceStatus(hService, lpServiceStatus) != 0)
                {
                    if (lpServiceStatus.dwCheckPoint > dwCheckPoint)
                    {
                        tickCount = Environment.TickCount;
                        dwCheckPoint = lpServiceStatus.dwCheckPoint;
                    }
                    else if (Environment.TickCount - tickCount > lpServiceStatus.dwWaitHint)
                        break;
                }
                else
                    break;
            }
            return lpServiceStatus.dwCurrentState == DesiredStatus;
        }

        private static IntPtr OpenSCManager(ServiceManagerRights Rights)
        {
            IntPtr num = OpenSCManager((string)null, (string)null, Rights);
            if (!(num == IntPtr.Zero))
                return num;
            throw new Exception("Could not connect to service control manager.");
        }

        [StructLayout(LayoutKind.Sequential)]
        private class SERVICE_STATUS
        {
            public int dwServiceType;
            public ServiceState dwCurrentState;
            public int dwControlsAccepted;
            public int dwWin32ExitCode;
            public int dwServiceSpecificExitCode;
            public int dwCheckPoint;
            public int dwWaitHint;
        }
    }
}
