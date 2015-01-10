using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
namespace Geex.Edit.Common.Project
{
    /// <summary>
    /// Class which contains information about the project type.
    /// </summary>
    public class ProjectIdentifier
    {
        /// <summary>
        /// Gets or sets the full name of the type which represents the project. (ex : Geex.Edit.Rpg.Project.RpgProject)
        /// </summary>
        [XmlElement("TypeFullName")]
        public string TypeFullName { get; set; }
        /// <summary>
        /// Gets or sets the name of the assembly which contains the project's Type. (ex : Geex.Edit.Rpg)
        /// </summary>
        [XmlElement("AssemblyName")]
        public string AssemblyName { get; set; }
        /// <summary>
        /// Gets or sets the project version.
        /// </summary>
        [XmlElement("Version")]
        public Version Version { get; set; }
        /// <summary>
        /// Gets or sets the name of this kind of project. (ex : Rpg)
        /// </summary>
        [XmlElement("KindName")]
        public string KindName { get; set; }
        /// <summary>
        /// Creates a new instance of a project identifier;
        /// </summary>
        public ProjectIdentifier()
        {
            TypeFullName = "";
            AssemblyName = "";
            Version = new Version();
            KindName = "";
        }
    }
}
