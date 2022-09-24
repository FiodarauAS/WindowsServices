using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace TaskWpf
{
    public interface IApplicationServicesService
    {
        Task<List<Service>> GetServices();
        Task<ServiceControllerStatus> GetStatus(ServiceController service);
    }
}