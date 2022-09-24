using System;
using System.Collections.Generic;

namespace TaskWpf
{
    public class ServiceComparer : IComparer<ServiceViewModel>
    {
        private ILogger _logger;
        public ServiceComparer(ILogger logger)
        {
            _logger = logger;
        }
        public int Compare(ServiceViewModel s1, ServiceViewModel s2)
        {
            if (s1 is null || s2 is null)
                _logger.Log($"[{DateTime.Now}] Некорректное значение параметра");

            if (s1.Status == s2.Status)
                return 0;

            return 1;
        }
    }
    
}
