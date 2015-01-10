using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using System.Reflection;
namespace UeFGame.GameObjects
{
    #region Serializable classes
    public class ModuleSerializationIdentifier
    {
        [XmlAttribute("Name")]
        public string Name;
        [XmlAttribute("Type")]
        public string TypeName;
        [XmlElement("SerializedModule")]
        public string SerializedModule;
        public ModuleSerializationIdentifier() { }
        public ModuleSerializationIdentifier(string name, Type type, string serializedModule)
        {
            Name = name; TypeName = type.FullName; SerializedModule = serializedModule;
        }
    }
    public class SerializableModule
    {
        [XmlElement("Modules")]
        public List<ModuleSerializationIdentifier> Modules;

        public SerializableModule() { }
        /// <summary>
        /// Converts a dictionnary of modules to a list of ModuleSerializationIdentifiers.
        /// </summary>
        /// <param name="modules"></param>
        public SerializableModule(Dictionary<string, Module> modules)
        {
            List<ModuleSerializationIdentifier> kvps = new List<ModuleSerializationIdentifier>();
            foreach (KeyValuePair<string, Module> kvp in modules)
            {
                kvps.Add(new ModuleSerializationIdentifier(kvp.Key, kvp.Value.GetType(), kvp.Value.SerializeString()));
            }
            Modules = kvps;
        }
        /// <summary>
        /// Converts this instance to a dictionnary.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, Module> ToDictionary()
        {
            Dictionary<string, Module> modules;
            modules = new Dictionary<string, Module>();
            foreach (var kvp in this.Modules)
            {
                Type t = Assembly.GetExecutingAssembly().GetType(kvp.TypeName, true);//kvp.TypeName;
                Module instance = (Module)Activator.CreateInstance(t);
                Module m = (Module)instance.DeserializeString(kvp.SerializedModule);
                modules.Add(kvp.Name, m);
            }
            return modules;
        }
    }
    #endregion
    /// <summary>
    /// Collection of modules used to initialize GameObjects.
    /// </summary>
    public class ModuleSet
    {
        /// <summary>
        /// Creates and return a Deep Copy of this instance of ModuleSet.
        /// </summary>
        /// <returns></returns>
        public ModuleSet DeepCopy()
        {
            ModuleSet set = new ModuleSet();
            foreach (var kvp in m_modules)
            {
                if (!set.m_modules.ContainsKey(kvp.Key))
                    set.m_modules.Add(kvp.Key, kvp.Value.DeepCopy());
                else
                    set.m_modules[kvp.Key] = kvp.Value.DeepCopy();
            }
            return set;
        }
        /// <summary>
        /// Dictionnary containing the modules.
        /// </summary>
        [XmlIgnore()]
        Dictionary<string, Module> m_modules;
        
        [XmlElement("ModulesSerializable")]
        public SerializableModule ModulesSerializable
        {
            get
            {
                return new SerializableModule(m_modules);
            }
            set
            {
                m_modules = value.ToDictionary();
            }
        }

        /// <summary>
        /// Returns the module with the given name.
        /// </summary>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        [XmlIgnore()]
        public Module this[string moduleName]
        {
            get { return m_modules[moduleName]; }
            set { m_modules[moduleName] = value; }
        }

        /// <summary>
        /// Returns the modules of this module set.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, Module> GetModules()
        {
            return m_modules;
        }

        /// <summary>
        /// Creates a new module set.
        /// </summary>
        public ModuleSet()
        {
            m_modules = new Dictionary<string, Module>();
            m_modules["base"] = new BaseModule();
        }
        /// <summary>
        /// Gets the base module of this module set.
        /// </summary>
        [XmlIgnore()]
        public BaseModule Base
        {
            get { return (BaseModule)m_modules["base"]; }
        }
    }
}