using System;
using System.Collections.Generic;
using System.Text;

namespace KepServer.CidLib.Tags
{
    public class TagsCollection
    {

        public Dictionary<string, DeviceDefinition> Devices { get; private set; } = new Dictionary<string, DeviceDefinition>();

        public DeviceDefinition AddDevice(string name, string id)
        {
            DeviceDefinition device = new DeviceDefinition(name, id);
            Devices.Add(name, device);
            return device;
        }
    }
}
