using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceProcess;
using System.Windows;

namespace TaskWpf
{
    partial class ApplicationViewModel : INotifyPropertyChanged
    {
        #region Definitions

        private ApplicationServicesService _applicationServicesService;
        private RefreshServicesService _refreshServicesService;
        private ServiceViewModel _service;
        private RelayCommands _stopService;
        private RelayCommands _startService;
        private bool _commandEvent = false;
        private ServiceComparer _serviceComparer;
        public LoggerService Logger { get; set; } = new LoggerService();
        public ObservableCollection<ServiceViewModel> Services { get; set; } = new ObservableCollection<ServiceViewModel>();
        public event PropertyChangedEventHandler PropertyChanged;
        public ServiceViewModel SelectedService
        {
            get { return _service; }
            set
            {
                _service = value;
                OnPropertyChanged("SelectedService");
            }
        }
        public RelayCommands StartService
        {
            get
            {
                return _startService ?? (_startService = new RelayCommands(obj =>
                {
                    if (SelectedService != null)
                    {
                        if (SelectedService.Status != ServiceControllerStatus.Running)
                        {
                            ServiceController service = new ServiceController(SelectedService.ServiceName);
                            _commandEvent = true;
                            GetStatus(service);
                            SelectedService.Status = ServiceControllerStatus.StartPending;
                        }
                        else
                        {
                            Logger.Log($"[{DateTime.Now}] Невозможно запустить службу. Служба уже запущена.");
                        }
                    }
                }));
            }
        }
        public RelayCommands StopService
        {
            get
            {
                return _stopService ?? (_stopService = new RelayCommands(obj =>
                {
                    if (SelectedService != null)
                    {
                        if (SelectedService.Status != ServiceControllerStatus.Stopped)
                        {
                            ServiceController service = new ServiceController(SelectedService.ServiceName);
                            _commandEvent = true;
                            GetStatus(service);
                            SelectedService.Status = ServiceControllerStatus.StopPending;
                        }
                        else
                        {
                            Logger.Log($"[{DateTime.Now}] Невозможно остановить службу. Служба уже остановлена.");
                        }
                    }
                }));
            }
        }

        #endregion

        #region Ctor and Misc-Methods

        public ApplicationViewModel()
        {
            _applicationServicesService = new ApplicationServicesService(Logger);

            Initialization();

            _serviceComparer = new ServiceComparer(Logger);
            _refreshServicesService = new RefreshServicesService(_applicationServicesService, Logger);
            _refreshServicesService.RefreshCollection += RefreshCollection;
        }
        private void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        private void RefreshCollection(ObservableCollection<ServiceViewModel> services)
        {
            try
            {
                if (services != null)
                {
                    foreach (ServiceViewModel service in services)
                    {
                        ServiceViewModel item = Services.FirstOrDefault(s => s.ServiceName == service.ServiceName);

                        Application.Current.Dispatcher.BeginInvoke((Action)(() => 
                        {
                            if (item != null)
                            {
                                int comparerResult = _serviceComparer.Compare(item, service);

                                if (comparerResult != 0)
                                {
                                    int index = Services.IndexOf(item);

                                    if (index < Services.Count)
                                    {
                                        if (service.Status.Equals(ServiceControllerStatus.Stopped) && !_commandEvent)
                                        {
                                            Services[index].Status = service.Status;

                                            Logger.Log($"[{DateTime.Now}] Служба \"{Services[index].ServiceName}\" остановлена извне.");
                                        }
                                        else if (service.Status.Equals(ServiceControllerStatus.Running) && !_commandEvent)
                                        {
                                            Services[index].Status = service.Status;

                                            Logger.Log($"[{DateTime.Now}] Служба \"{Services[index].ServiceName}\" запущена извне.");
                                        }

                                        _commandEvent = false;
                                    }
                                }
                            }
                            else
                                Services.Add(service);
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"[{DateTime.Now} Ошибка] " + ex.Message);
            }
        }
        private async void Initialization()
        {
            var result = await _applicationServicesService.GetServices();

            try
            {
                if (result != null)
                {
                    foreach (Service service in result)
                    {
                        Services.Add(new ServiceViewModel()
                        {
                            ServiceName = service.ServiceName,
                            DisplayName = service.DisplayName,
                            Status = service.Status,
                            AccountName = service.AccountName
                        });
                    }

                    Logger.Log("Службы успешно загружены.");
                }
            }
            catch (Exception)
            {
                Logger.Log("Ошибка при загрузке служб.");
            }
        }
        private async void GetStatus(ServiceController service)
        {
            SelectedService.Status = await _applicationServicesService.GetStatus(service);
        }

        #endregion
    }
}
