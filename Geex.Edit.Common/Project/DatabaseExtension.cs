using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using StringBuilder = System.Text.StringBuilder;
using Geex.Edit.Common.Extensibility;
namespace Geex.Edit.Common.Project
{
    /// <summary>
    /// A node of the ExtensionModel. Represents a variable in the database.
    /// </summary>
    [XmlRoot("variable")]
    public class Variable
    {
        /// <summary>
        /// Name of the Database Variable
        /// </summary>
        [XmlAttribute("name")]
        public string Name;
        /// <summary>
        /// Type of the Database variable.
        /// Either :
        ///     - Full Type name (if the type is builting)
        ///     - AssemblyName;FullTypeName (if the type is not builtin)
        /// The assembly name is either the name of an assembly in the "Assembly" folder of Geex.Edit,
        /// or Project Plugin Assembly, or another assembly loaded by Geex.Edit.
        /// </summary>
        [XmlAttribute("type")]
        public string Type;
        /// <summary>
        /// Default value of the DatabaseVariable
        /// </summary>
        [XmlAttribute("default")]
        public string DefaultValue;
        /// <summary>
        /// Initializes a variable.
        /// </summary>
        public Variable(string name, string type, string value)
        {
            Name = name;
            Type = type;
            DefaultValue = value;
        }
        public Variable()
        {

        }
    }
    /// <summary>
    /// Class representing a model of a database extension, used to build the extension's skeleton.
    /// (which object are stored, their type etc...)
    /// </summary>
    public class DatabaseExtensionModel
    {
        /// <summary>
        /// Variables of the model.
        /// </summary>
        public List<Variable> Variables = new List<Variable>();
        /// <summary>
        /// Name of the database extension.
        /// It will be used to choose the name of the file were to store the data.
        /// </summary>
        public string Name = "Default";

        public DatabaseExtensionModel()
        {

        }
    }
    /// <summary>
    /// The data saved in the database
    /// </summary>
    [XmlInclude(typeof(CustomSerializableObject))]
    [XmlInclude(typeof(CustomSerializationAttribute))]
    public class DatabaseExtensionSavedData
    {
        /// <summary>
        /// Values of the objects saved.
        /// </summary>
        List<object> m_values;
        /// <summary>
        /// Keys of the object saved.
        /// </summary>
        List<string> m_keys;
        /// <summary>
        /// Gets or sets the keys of the objects saved.
        /// </summary>
        public List<string> Keys
        {
            get { return m_keys; }
            set { m_keys = value; }
        }
        /// <summary>
        /// Gets or sets the values of the objects saved.
        /// </summary>
        public List<object> Values
        {
            get { return m_values; }
            set { m_values = value; }
        }
        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        public DatabaseExtensionSavedData()
        {
            m_values = new List<object>();
            m_keys = new List<string>();
        }
        /// <summary>
        /// Builds the saved data from a dictionnary
        /// </summary>
        /// <param name="dict"></param>
        public void Build(Dictionary<string, object> dict)
        {
            foreach (KeyValuePair<string, object> kvp in dict)
            {
                m_keys.Add(kvp.Key);
                if (CustomSerializableObject.IsCustomSerializable(kvp.Value))
                    m_values.Add(new CustomSerializableObject(kvp.Value));
                else
                    m_values.Add(kvp.Value);
            }
        }
        /// <summary>
        /// Creates a dictionnary from the Keys and Values.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> ToDict()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            for (int i = 0; i < m_keys.Count; i++)
            {
                dict.Add(m_keys[i], GetValue(i));
            }
            return dict;
        }
        /// <summary>
        /// For each key of the dictionnary, completes with this data.
        /// </summary>
        /// <param name="Data"></param>
        public void CompleteWithData(Dictionary<string, object> data)
        {
            // Iterates through the keys of the dictionary, and find the keys
            // this class handles to set the value of the corresponding objects.
            string[] keys = data.Keys.ToArray();
            foreach (string key in keys)
            {
                if (m_keys.Contains(key))
                    if(data.ContainsKey(key))
                        data[key] = GetValue(m_keys.IndexOf(key));
                    else
                        data.Add(key, GetValue(m_keys.IndexOf(key)));
            }
        }
        /// <summary>
        /// Gets the object value of the object in the given index.
        /// As the values might be "SerializableObject", this method
        /// retrieves the true object if needed.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        object GetValue(int index)
        {
            object value = m_values[index];
            Type t = value.GetType();
            if (value.GetType() == typeof(CustomSerializableObject))
                value = ((CustomSerializableObject)value).GetObject();
            return value;
        }
    }
    /// <summary>
    /// Attributes indicating an object must be serialized using the custom serialization process.
    /// </summary>
    public class CustomSerializationAttribute : Attribute
    {
        public CustomSerializationAttribute()
            : base()
        {

        }
    }
    /// <summary>
    /// Represents a serializable object of unresolved type which cannot be serialized using XML
    /// </summary>
    public class CustomSerializableObject
    {
        /// <summary>
        /// Name of the fields of the object.
        /// </summary>
        public List<string> VariableNames;
        /// <summary>
        /// Values of the fields of the object.
        /// </summary>
        public List<object> VariableValues;
        /// <summary>
        /// Object's Type's fullname
        /// </summary>
        public string ObjType;
        /// <summary>
        /// Object's Assembly's fullname
        /// </summary>
        public string ObjAssembly;
        /// <summary>
        /// Parameterless constructor used by XmlSerializer.
        /// </summary>
        public CustomSerializableObject()
        {
            VariableNames = new List<string>();
            VariableValues = new List<object>();
        }
        /// <summary>
        /// Constructor, create a serializable object from the given object.
        /// </summary>
        public CustomSerializableObject(object obj)
        {
            // if(!Assembly.GetExecutingAssembly().GetTypes().Contains(value.GetType()))
            Type type = obj.GetType();
            ObjType = type.FullName;
            ObjAssembly = type.Assembly.GetName().Name;
            VariableNames = new List<string>();
            VariableValues = new List<object>();
            foreach (FieldInfo info in type.GetFields())
            {
                if (info.GetCustomAttributes(typeof(NonSerializedAttribute), true).Count() == 0)
                {
                    VariableNames.Add(info.Name);
                    object value = info.GetValue(obj);
                    if (IsCustomSerializable(value))
                        value = new CustomSerializableObject(value);
                    VariableValues.Add(value);
                }
            }
        }
        /// <summary>
        /// Returns the object which is represented in this class.
        /// </summary>
        public object GetObject()
        {
            Assembly a;
            try
            {
                a = AssemblyManager.GetAssembly(ObjAssembly);
            }
            catch
            {
                return null;
            }

            Type type = a.GetType(ObjType);
            object obj = Activator.CreateInstance(type);
            foreach (string varName in VariableNames)
            {
                FieldInfo field = type.GetField(varName);
                int index = VariableNames.IndexOf(varName);
                object value = VariableValues[index];
                if (value.GetType() == typeof(CustomSerializableObject))
                    value = ((CustomSerializableObject)value).GetObject();
                field.SetValue(obj, value);
            }
            return obj;
        }
        /// <summary>
        /// Returns a value indicating if the given object must be serialized using CustomSerializableObject.
        /// </summary>
        /// <returns></returns>
        public static bool IsCustomSerializable(object obj)
        {
            return Type.GetType(obj.GetType().FullName) == null;
        }
    }
    /// <summary>
    /// Extension used to append an extension to a project's database.
    /// A Database extension works for only one project.
    /// It is a xml file located in [projectfolder]\\Data\\DatabaseExtensions\\, which describes
    /// the structure of the extension. (ie : an int named "NumberOfMyNewThings", or even a custom object type)
    /// 
    /// TODO : try it in everypossible situation to ensure it does not crash Geex.Edit
    /// Create Error reports.
    /// </summary>
    public class DatabaseExtension
    {
        #region Variables
        readonly static string[] systemTypes = new string[] { 
            "string", "short", "int", "float", "double", "long", "bool",
            "System.String", "System.Int16", "System.Int32", "System.Int64",
            "System.Single", "System.Double", "System.Boolean"};
        /// <summary>
        /// Name of the extension.
        /// </summary>
        string m_name;
        /// <summary>
        /// Objects contained in the extension.
        /// </summary>
        Dictionary<string, object> m_objects;
        #endregion

        #region Properties
        /// <summary>
        /// Returns the object with the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object this[string name]
        {
            get { return m_objects[name]; }
            set { m_objects[name] = value; }
        }
        /// <summary>
        /// Gets or sets the name of the extension.
        /// </summary>
        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Creates a new instance of DataBase extension.
        /// Loads the extension from its description file.
        /// </summary>
        /// <param name="descFilename"></param>
        public DatabaseExtension(string descFilename)
        {
            /* Debug
             * DatabaseExtensionModel mod = new DatabaseExtensionModel();
            mod.Name = "Nameuh";
            mod.Nodes.Add(new Variable("Test", "TestClassLibrary;TestClassLibrary.SuperObject", "none"));
            mod.Nodes.Add(new Variable("Int", "int", "5"));
            mod.Nodes.Add(new Variable("Floateuh", "float", "5.6"));
            Tools.Serializer.Serialize<DatabaseExtensionModel>(mod, descFilename);*/
            Name = "Failed";
            DatabaseExtensionModel obj;
            try
            {
                obj = Tools.Serializer.Deserialize<DatabaseExtensionModel>(descFilename);
            }
            catch { return; }
            m_name = obj.Name;
            // Sets up the objects of the database
            m_objects = new Dictionary<string, object>();
            foreach (Variable node in obj.Variables)
            {
                Type objType;
                if (node.Type.Contains(';'))
                {
                    string[] pair = node.Type.Split(';');
                    string assemblyName = pair[0];
                    string typeName = pair[1];
                    try
                    {
                        Assembly a = AssemblyManager.GetAssembly(assemblyName);
                        objType = a.GetType(typeName, true);
                    }
                    catch
                    {
                        continue; // ignore this item if we cannot get its type.
                    }
                }
                else
                {
                    // For all system types
                    if(systemTypes.Contains(node.Type))
                    {
                        m_objects[node.Name] = GetDefaultValueOfSystemType(node.Type, node.DefaultValue);
                        continue;
                    }
                    // For other types
                    try
                    {
                        objType = Type.GetType(node.Type, true);
                    }
                    catch
                    {
                        continue; // ignore this item if we cannot get its type.
                    }
                }
                // If the type does not exist, ignore the item.
                if (objType == null)
                    continue;
                object nodeObj = Activator.CreateInstance(objType);
                m_objects[node.Name] = nodeObj;
            }
        }

        /// <summary>
        /// Loads the extension's managed data.
        /// </summary>
        /// <param name="proj"></param>
        public void Load(GeexProject proj)
        {
            string dstFilename = proj.DataDirectory + "\\" + m_name + "-extension.xml";
            if (System.IO.File.Exists(dstFilename))
            {
                try
                {
                    // Deserialize the Saved Data.
                    DatabaseExtensionSavedData data = Tools.Serializer.Deserialize<DatabaseExtensionSavedData>(dstFilename);
                    // Fills the m_objects field with the serialized data transformed into RunTime objects.
                    data.CompleteWithData(m_objects);
                }
                catch
                {

                }
            }
        }
        /// <summary>
        /// Loads the extension's managed data.
        /// </summary>
        /// <param name="proj"></param>
        public void Save(GeexProject proj)
        {
            string dstFilename = proj.DataDirectory + "\\" + m_name + "-extension.xml";
            // Creates a new extension of Database Saved data
            DatabaseExtensionSavedData data = new DatabaseExtensionSavedData();
            // Builds the data with the GameObjects.
            // It will transform them into Serializable objects, in a Serializable class.
            data.Build(m_objects);
            Tools.Serializer.Serialize<DatabaseExtensionSavedData>(data, dstFilename);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Gets the default value for a system type.
        /// </summary>
        /// <param name="systemType"></param>
        /// <returns></returns>
        object GetDefaultValueOfSystemType(string systemType, string defaultValue)
        {
            switch (systemType)
            {
                case "string":
                case "System.String":
                    return defaultValue;
                case "int":
                case "System.Int32":
                    int value = 0;
                    try { value = Int32.Parse(defaultValue); }
                    catch { }
                    return value;
                case "short":
                case "System.Int16":
                    short valueS = 0;
                    try { valueS = Int16.Parse(defaultValue); }
                    catch { }
                    return valueS;
                case "long":
                case "System.Int64":
                    long ValueL = 0;
                    try { ValueL = Int64.Parse(defaultValue); }
                    catch { }
                    return ValueL;
                case "float":
                case "System.Single":
                    float valueF = 0.0f;
                    try { valueF = float.Parse(defaultValue); }
                    catch { }
                    return valueF;
                case "double":
                case "System.Double":
                    double valueD = 0.0;
                    try { valueD = float.Parse(defaultValue); }
                    catch { }
                    return valueD;
                case "bool":
                case "System.Boolean":
                    bool valueB = false;
                    try { valueB = bool.Parse(defaultValue); }
                    catch { }
                    return valueB;
            }
            throw new Exception("Not a system type");
        }
        #endregion
    }
}