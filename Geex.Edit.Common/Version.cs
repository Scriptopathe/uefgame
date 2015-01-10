using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
namespace Geex.Edit.Common
{
    [Serializable]
    public class Version
    {
        #region Properties
        /// <summary>
        /// Gets or sets the compatibility number.
        /// If a project and a plugin are of the same kind and have the same conpatibility
        /// number, then they are compatible.
        /// </summary>
        [XmlElement("compatibility")]
        public int CompatibilityNumber
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the release version number.
        /// </summary>
        /// <returns></returns>
        [XmlElement("release")]
        public int Release
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the major version number.
        /// </summary>
        [XmlElement("major")]
        public int Major
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the minor version number.
        /// </summary>
        [XmlElement("minor")]
        public int Minor
        {
            get;
            set;
        }
        #endregion

        /// <summary>
        /// Create a new instance of the Version object with default value 0.0.0.
        /// </summary>
        public Version()
        {

        }
        /// <summary>
        /// Create a new instance of the Version object with given values :
        /// </summary>
        /// <param name="compatibilityNumber">compatibility number</param>
        /// <param name="release">release version number</param>
        /// <param name="major">major version number</param>
        /// <param name="minor">minor version number</param>
        public Version(int compatibilityNumber, int release, int major, int minor)
        {
            CompatibilityNumber = compatibilityNumber;
            Release = release;
            Major = major;
            Minor = minor;
        }
        /// <summary>
        /// Returns true if this version is newer than the given version.
        /// Returns false if the two version are equal.
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public bool IsNewerThan(Version version)
        {
            return (this.CompatibilityNumber > version.CompatibilityNumber) ? true :
                ((this.Release > version.Release) ? true :
                (this.Major > version.Major ? true : 
                (this.Minor > version.Minor)));
        }
        /// <summary>
        /// Returns true if this version and the given version are equal.
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public bool IsEqualTo(Version version)
        {
            return (this.CompatibilityNumber == version.CompatibilityNumber) &&
                (this.Release == version.Release) && 
                (this.Major == version.Major) &&
                (this.Minor == version.Minor);
        }
        /// <summary>
        /// Returns a string representing the Version object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.CompatibilityNumber.ToString() + "." +
                this.Release.ToString() + "." +
                this.Major.ToString() + "." +
                this.Minor.ToString();
        }
    }
}
