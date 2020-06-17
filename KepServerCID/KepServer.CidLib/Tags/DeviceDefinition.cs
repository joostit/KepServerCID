using KepServer.CidLib.Internals;
using KepServer.CidLib.Tags.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace KepServer.CidLib.Tags
{
    public class DeviceDefinition
    {

        public Dictionary<string, TagBase> Tags { get; private set; } = new Dictionary<string, TagBase>();

        public string Name { get; private set; }

        public string Id { get; set; }

        public DeviceDefinition(string name, string id)
        {
            this.Name = name;
            this.Id = id;
        }


        public DeviceDefinition AddTag(TagBase tag)
        {
            Tags.Add(tag.Name, tag);
            return this;
        }

        public TagBase GetTag(string name)
        {
            return Tags[name];
        }

    }
}
