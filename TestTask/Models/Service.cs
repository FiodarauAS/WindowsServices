using System.ServiceProcess;

namespace TaskWpf
{
    public class Service
    {
        #region Definitions

        public string ServiceName { get; private set; }
        public string DisplayName { get; private set; }
        public ServiceControllerStatus Status { get; private set; }
        public string AccountName { get; private set; }
        public Service(string serviceName, string displayName, ServiceControllerStatus status, string accountName)
        {
            ServiceName = serviceName;
            DisplayName = displayName;
            Status = status;
            AccountName = accountName;
        }

        #endregion
    }
}
