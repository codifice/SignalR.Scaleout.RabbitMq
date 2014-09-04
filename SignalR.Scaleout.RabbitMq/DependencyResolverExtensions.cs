using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Messaging;


namespace SignalR.Scaleout.RabbitMq
{
    /// <summary>
    /// Provides UseRabbitMQ scaleout extension
    /// </summary>
    [CLSCompliant(false)]
    public static class DependencyResolverExtensions
    {
        /// <summary>
        /// Configure RabbitMQ for Scaleout
        /// </summary>
        /// <param name="resolver"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IDependencyResolver UseRabbitMq(this IDependencyResolver resolver, RabbitMqScaleoutConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            var bus = new Lazy<RabbitMqMessageBus>(() => new RabbitMqMessageBus(resolver, configuration));
            resolver.Register(typeof(IMessageBus), () => bus.Value);
            return resolver;
        }
                
    }
}
