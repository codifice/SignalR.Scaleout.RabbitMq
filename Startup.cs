using Microsoft.AspNet.SignalR;
using Owin;
using System;
using System.Configuration;

namespace SignalR.Scaleout.RabbitMq
{
    /// <summary>
    /// Hookup Signalr Scalout as OWIN Module
    /// </summary>
    public class Startup
    {

        /// <summary>
        /// Hookup Signalr Scaleout
        /// </summary>
        public static void Hookup()
        {
            Hookup("rabbitmq:ConnectionString", "signalr");


        }
        /// <summary>
        /// Hooks up Signalr Scaleout with set connectionstring
        /// </summary>
        /// <param name="connectionStringName"></param>
        /// /// <param name="exchangeName"></param>
        public static void Hookup(string connectionStringName, string exchangeName)
        {
            var config = new RabbitMqScaleoutConfiguration
            {
                ConnectionString = ConfigurationManager.AppSettings[connectionStringName ?? "rabbitmq:ConnectionString"],
                ExchangeName = exchangeName ?? "signalr"


            };
            GlobalHost.DependencyResolver.UseRabbitMq(config);
        }

        /// <summary>
        /// Trigger Configuration when called by OWIN Host
        /// </summary>
        /// <param name="app"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public void Configuration(IAppBuilder app)
        {

            if (app == null) throw new ArgumentNullException("app");
            Hookup();
            app.MapSignalR();




        }
    }
}