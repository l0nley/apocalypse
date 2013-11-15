﻿using System.ServiceProcess;

namespace winapisvc
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        static void Main()
        {
            var servicesToRun = new ServiceBase[] 
                { 
                    new WinApiService() 
                };
            ServiceBase.Run(servicesToRun);
        }
    }
}
