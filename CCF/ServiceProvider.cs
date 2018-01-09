﻿using CCF.Messages;
using CCF.Transport;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CCF
{
    internal class ServiceProvider<T>
    {
        private readonly Func<T> instanceCreater;
        private readonly string password;
        private readonly Func<string, ITransporter> transporterCreator;
        private readonly ILogger<ServiceProvider<T>> logger;
        private ITransporter transporter;

        public ServiceProvider(Func<T> serviceInvoker, string password, Func<string, ITransporter> transporterCreator, ILogger<ServiceProvider<T>> logger)
        {
            instanceCreater = serviceInvoker ?? throw new ArgumentNullException(nameof(serviceInvoker));
            this.password = password;
            this.transporterCreator = transporterCreator ?? throw new ArgumentNullException(nameof(transporterCreator));
            this.logger = logger;
            UpdateConnection();
        }

        private void UpdateConnection()
        {
            try
            {
                if (transporter != null)
                    transporter.Dispose();

                logger.LogInformation("try get new transporter");
                transporter = transporterCreator(password);
                transporter.OnNeedNewInstance += NeedNewInstance;
                transporter.OnConnectionLost += () =>
                {
                    Console.WriteLine("CONNECTION LOST :( try to reconnect");
                    UpdateConnection();
                };
            }
            catch (SocketException sockEx)
            {
                logger.LogWarning(sockEx, "Error while connecting to remote host");
                UpdateConnection();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error while creating ITransporter");
                throw;
            }
        }

        private Task NeedNewInstance(string password)
        {
            var instanceTransporter = transporterCreator(password);
            var instance = instanceCreater();
            var codeInvoker = new CodeInvoker(instance, typeof(T));
            InstanceWrapper rootWrapper = new InstanceWrapper(codeInvoker, instanceTransporter);
            return Task.CompletedTask;
        }
    }
}
