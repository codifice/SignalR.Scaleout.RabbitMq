using System;
using Microsoft.AspNet.SignalR.Messaging;

namespace SignalR.Scaleout.RabbitMq
{
    /// <summary>
    /// Encapsulates default settings for RabbitMQ Scaleout
    /// </summary>
    [CLSCompliant(false)]
    public class RabbitMqScaleoutConfiguration : ScaleoutConfiguration
    {
        /// <summary>
        /// Rabbit MQ Connection String
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Exchange Name
        /// </summary>
        public string ExchangeName { get; set; }

      
    }
}