using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UeFGame.Editor
{
    public enum EditionSettings
    {
        Default,
        Browse
    }
    /// <summary>
    /// Contains information about the way the property/field should
    /// be edited (ex : browsing file for a string etc...)
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class PropertyEditionAttribute : Attribute
    {
        public EditionSettings Settings { get; private set;}
        public string GroupName { get; private set; }
        public PropertyEditionAttribute(EditionSettings settings, string groupName)
            : base()
        {
            Settings = settings;
            GroupName = groupName;
        }
        public PropertyEditionAttribute(string group)
            : base()
        {
            Settings = EditionSettings.Default;
            GroupName = group;
        }
    }
}
