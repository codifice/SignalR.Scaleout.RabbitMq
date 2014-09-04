using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace SignalR.Scaleout.RabbitMq
{
    /// <summary>
    /// Allow hub interactions when not hosted in same process (ie in Console Application)
    /// </summary>
    [CLSCompliant(false)]
    public class DummyHubDescriptorProvider : IHubDescriptorProvider
    {

        private Dictionary<string, HubDescriptor> _descriptions = new Dictionary<string, HubDescriptor>(StringComparer.Ordinal);
        /// <summary>
        /// Get All Available Hubs
        /// </summary>
        /// <returns></returns>
        public IList<HubDescriptor> GetHubs()
        {
            return _descriptions.Values.OrderBy(hd => hd.Name, StringComparer.Ordinal).ToList();
        }

        /// <summary>
        /// Attempt to resolve a hub (generating a new dummy descriptor if not found)
        /// </summary>
        /// <param name="hubName"></param>
        /// <param name="descriptor"></param>
        /// <returns></returns>
        public bool TryGetHub(string hubName, out HubDescriptor descriptor)
        {
            if (_descriptions.ContainsKey(hubName))
            {
                descriptor = _descriptions[hubName];
            }
            else
            {
                _descriptions[hubName] = (descriptor = new HubDescriptor { HubType = typeof(Hub), Name = hubName, NameSpecified = false });
            }
            return true;
        }
    }
}
