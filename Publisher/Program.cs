using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using SignalR.Scaleout.RabbitMq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Publisher
{
    class Program
    {
        static void Main(string[] args)
        {

            var autoResetEvent = new AutoResetEvent(false);
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                // cancel the cancellation to allow the program to shutdown cleanly
                eventArgs.Cancel = true;
                autoResetEvent.Set();
            };

            PublishTime(autoResetEvent);
            Console.WriteLine("Now shutting down");
        }

        private static void PublishTime(AutoResetEvent autoResetEvent)
        {
            var config = new RabbitMqScaleoutConfiguration
            {
                ConnectionString = ConfigurationManager.AppSettings["rabbitmq:ConnectionString"],
                ExchangeName = "signalr"


            };
            GlobalHost.DependencyResolver.UseRabbitMq(config);
            GlobalHost.DependencyResolver.Register(typeof(IHubDescriptorProvider), () => new DummyHubDescriptorProvider());
            var context = GlobalHost.ConnectionManager.GetHubContext("TickerHub");




            using (var timer = new System.Threading.Timer((o) =>
            {
                context.Clients.All.currentTime(DateTime.Now);
            }))
            {

                timer.Change(0, 900);

                // main blocks here waiting for ctrl-C
                autoResetEvent.WaitOne();
            }
        }
    }
}
