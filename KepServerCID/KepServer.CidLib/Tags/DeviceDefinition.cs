using KepServer.CidLib.Internals;
using System;
using System.Collections.Generic;
using System.Text;

namespace KepServer.CidLib.Tags
{
    public class DeviceDefinition
    {

        public Dictionary<string, TagApiBase> Tags { get; private set; } = new Dictionary<string, TagApiBase>();

        public string Name { get; private set; }

        public string Id { get; set; }

        public DeviceDefinition(string name, string id)
        {
            this.Name = name;
            this.Id = id;
        }


        public DeviceDefinition AddTag(TagApiBase tag)
        {
            Tags.Add(tag.Name, tag);
            return this;
        }

        public TagApiBase GetTag(string name)
        {
            return Tags[name];
        }

    }
}
