using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.ServiceProcess;

namespace TaskWpf
{
    public class ServiceViewModel : INotifyPropertyChanged
    {
        #region Definitions

        private string _serviceName;
        private string _displayName;
        private string _accountName;
        private ServiceControllerStatus _status;

        public event PropertyChangedEventHandler PropertyChanged;
        public string ServiceName
        {
            get { return _serviceName; }
            set
            {
                _serviceName = value;
                OnPropertyChanged("ServiceName");
            }
        }
        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                _displayName = value;
                OnPropertyChanged("DisplayName");
            }
        }
        public ServiceControllerStatus Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged("Status");
            }
        }
        public string AccountName
        {
            get { return _accountName; }
            set
            {
                _accountName = value;
                OnPropertyChanged("AccountName");
            }
        }

        #endregion

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}

