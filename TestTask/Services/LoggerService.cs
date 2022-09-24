using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace TaskWpf
{
    public class LoggerService : INotifyPropertyChanged, ILogger
    {
        #region Definitions

        private string _logMessage;
        private StringBuilder _sbText = new StringBuilder();

        public event PropertyChangedEventHandler PropertyChanged;
        public string LogMessage
        {
            get => _logMessage;
            set
            {
                _logMessage = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public void Log(string message)
        {
            try
            {
                _sbText.Append(message);
                _sbText.AppendLine();
                LogMessage = _sbText.ToString();
            }
            catch (Exception ex)
            {
                Log($"[{DateTime.Now} Ошибка] " + ex.Message);
            }
        }
    }
}
