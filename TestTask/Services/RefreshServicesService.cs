using System;
using System.Collections.ObjectModel;
using System.Timers;

namespace TaskWpf
{
    public class RefreshServicesService : IRefreshServicesService
    {
        #region Definitions

        private Timer _resfreshTimer = new Timer();
        private ObservableCollection<ServiceViewModel> _services = new ObservableCollection<ServiceViewModel>();
        private IApplicationServicesService _applicationServicesService;
        private ILogger _logger;
        private int _interval = 2000;

        public delegate void RefreshObservableCollection(ObservableCollection<ServiceViewModel> services);
        public event RefreshObservableCollection RefreshCollection;

        #endregion

        #region Ctor and misc-methods

        public RefreshServicesService(IApplicationServicesService applicationServicesService, ILogger logger)
        {
            _applicationServicesService = applicationServicesService;
            _logger = logger;
            InitTimer();
        }
        public void InitTimer()
        {
            _resfreshTimer.Interval = _interval;
            _resfreshTimer.Elapsed += RefreshTimerElapsed;
            _resfreshTimer.AutoReset = true;
            _resfreshTimer.Enabled = true;
            _resfreshTimer.Start();
        }
        public async void RefreshTimerElapsed(Object source, ElapsedEventArgs e)
        {
            var result = await _applicationServicesService.GetServices();

            try
            {
                if (result != null)
                {
                    lock (_services)
                    {
                        _services.Clear();

                        foreach (Service service in result)
                        {
                            if (service != null)
                            {
                                _services.Add(new ServiceViewModel()
                                {
                                    ServiceName = service.ServiceName,
                                    DisplayName = service.DisplayName,
                                    Status = service.Status,
                                    AccountName = service.AccountName
                                });
                            }
                        }

                        RefreshCollection.BeginInvoke(_services, null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log($"[{DateTime.Now} Ошибка] Ошибка при обновлении служб." + ex.Message);
            }
        }

        #endregion
    }
}

