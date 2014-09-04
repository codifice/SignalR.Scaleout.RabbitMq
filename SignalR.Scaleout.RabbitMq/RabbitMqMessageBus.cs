using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Messaging;
using Newtonsoft.Json;
using System.Linq;

using EasyNetQ;
using System.Configuration;

using EasyNetQ.Topology;
using EasyNetQ.ConnectionString;

using System.Diagnostics;


namespace SignalR.Scaleout.RabbitMq
{
    /// <summary>
    /// Simple Scaleout MessageBus adapter for SignalR
    /// </summary>
    [CLSCompliant(false)]
    public class RabbitMqMessageBus : ScaleoutMessageBus
    {
        private long _idx;
        private IAdvancedBus _bus;
        private IExchange _exchange;
        private RabbitMqScaleoutConfiguration _config;

        /// <summary>
        /// Construct new Message Bus
        /// </summary>
        /// <param name="resolver"></param>
        /// <param name="configuration"></param>
        public RabbitMqMessageBus(IDependencyResolver resolver, RabbitMqScaleoutConfiguration configuration)
            : base(resolver, configuration)
        {
            _config = configuration;

            StartBus();
        }

        private void StartBus()
        {
            try
            {
                if (_bus != null)
                {
                    Close(0);
                    _bus.Dispose();
                }

                _bus = RabbitHutch.CreateBus(_config.ConnectionString).Advanced;
                // if (!System.Diagnostics.Debugger.IsAttached) System.Diagnostics.Debugger.Launch();
                var exchange = _config.ExchangeName ?? "signalr";
                var queueName = string.Join(".", "signalr"
                    , AlphanumericOnly(Environment.MachineName)
                    , AlphanumericOnly(Process.GetCurrentProcess().ProcessName)
                    , Process.GetCurrentProcess().Id).ToLowerInvariant();
                _exchange = _bus.ExchangeDeclare(exchange, ExchangeType.Fanout);
                var q = _bus.QueueDeclare(queueName, false, false, false, false
                    , (int)TimeSpan.FromMinutes(1).TotalMilliseconds
                    , (int)TimeSpan.FromMinutes(30).TotalMilliseconds);
                _bus.Bind(_exchange, q, "");
                _bus.Consume(q, (msgb, msgp, msgi) =>
                {
                    return Task.Factory.StartNew(() =>
                    {

                        var msg = JsonConvert.DeserializeObject<RmqMessage>(Encoding.UTF8.GetString(msgb));
                        Open(msg.StreamIndex);
                        base.OnReceived(msg.StreamIndex
                            , (ulong)Interlocked.Increment(ref _idx)
                            , ScaleoutMessage.FromBytes(msg.Body));

                    });

                });
                Open(0);
            }
            catch (Exception)
            {
                Close(0);
                throw;
            }
        }

        private static string AlphanumericOnly(string original)
        {
            if (original == null) throw new ArgumentNullException("original");
            return new string(original.Where(c => char.IsLetterOrDigit(c)).ToArray());
        }


        /// <summary>
        /// Send new message to Bus
        /// </summary>
        /// <param name="streamIndex"></param>
        /// <param name="messages"></param>
        /// <returns></returns>
        protected override Task Send(int streamIndex, IList<Microsoft.AspNet.SignalR.Messaging.Message> messages)
        {
            return Task.Factory.StartNew(() =>
             {
                 Open(streamIndex);
                 var scaleoutMsg = new ScaleoutMessage(messages);

                 var msg = new RmqMessage
                 {
                     StreamIndex = streamIndex,
                     Body = scaleoutMsg.ToBytes()
                 };

                 var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(msg));
                 _bus.Publish(_exchange, "", false, false, new MessageProperties(), body);
             });

        }

        /// <summary>
        /// Clear down instance
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {

            base.Dispose(disposing);

            if (disposing)
            {
                _bus.Dispose();

            }
        }
    }
}