using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace CloudInit
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
#if DEBUG
            // If the program is running in debug mode, don't start a service. Just run the CIService.Main method
            CIService.Main(null);
#else
            // Run this application as a service
            ServiceBase[] ServicesToRun = new ServiceBase[] { new CloudInitService() };
            ServiceBase.Run(ServicesToRun);
#endif
        }
    }
}
