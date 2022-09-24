using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Management;
using System.Runtime.CompilerServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TaskWpf
{
    public class ApplicationServicesService : IApplicationServicesService
    {
        #region Definitions

        private ServiceController[] scServices;
        private List<Service> Services;
        private ManagementObject wmiService;
        private readonly ILogger _logger;

        #endregion

        #region Ctor and Misc-methods

        public ApplicationServicesService(ILogger logger)
        {
            _logger = logger;
        }
        public Task<List<Service>> GetServices()
        {
            return Task.Run(() =>
            {
                try
                {
                    Services = new List<Service>();
                    scServices = ServiceController.GetServices();

                    foreach (ServiceController sc in scServices)
                    {
                        wmiService = new ManagementObject("Win32_Service.Name='" + sc.ServiceName + "'");
                        wmiService.Get();

                        var account = wmiService["StartName"];

                        if (account == null)
                            account = ServiceAccount.LocalSystem.ToString();

                        Services.Add(new Service(sc.ServiceName, sc.DisplayName, sc.Status, account.ToString()));
                    }

                    return Services;
                }
                catch (Exception ex)
                {
                    _logger.Log($"[{DateTime.Now} Ошибка] " + ex.Message);
                    return null;
                }
            });
        }
        public Task<ServiceControllerStatus> GetStatus(ServiceController service)
        {
            return Task.Run(() =>
            {
                try
                {
                    if (service.Status.Equals(ServiceControllerStatus.Stopped) ||
                        service.Status.Equals(ServiceControllerStatus.StopPending))
                    {
                        service.Start();
                        service.WaitForStatus(ServiceControllerStatus.Running);

                        _logger.Log($"[{DateTime.Now}] Служба {service.ServiceName} запущена.");
                    }
                    else
                    {
                        service.Stop();
                        service.WaitForStatus(ServiceControllerStatus.Stopped);

                        _logger.Log($"[{DateTime.Now}] Служба {service.ServiceName} остановлена.");
                    }

                    return service.Status;
                }
                catch (Exception ex)
                {
                    _logger.Log($"[{DateTime.Now} Ошибка] " + ex.Message);
                    return service.Status;
                }
            });
        }

        #endregion
    }
}
