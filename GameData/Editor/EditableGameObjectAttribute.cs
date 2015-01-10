using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UeFGame.Editor
{
    /// <summary>
    /// Attributes of the Editable game objects.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class EditableGameObjectAttribute : Attribute
    {
        public Dictionary<string, Type> Modules;
        /// <summary>
        /// Creates a new instance of the GameObjectAttribute.
        /// </summary>
        /// <param name="initializingDataType">initializing type of the game object.</param>
        public EditableGameObjectAttribute(string name, Type initializingDataType)
            : base()
        {
            Modules = new Dictionary<string, Type>();
            Modules.Add(name, initializingDataType);
        }
        /// <summary>
        /// Creates a new instance of the GameObjectAttribute.
        /// </summary>
        /// <param name="initializingDataType">initializing type of the game object.</param>
        public EditableGameObjectAttribute(string[] names, Type[] initializingDataTypes)
            : base()
        {
            Modules = new Dictionary<string, Type>();
            for (int i = 0; i < names.Count(); i++)
            {
                Modules.Add(names[i], initializingDataTypes[i]);
            }
        }
    }
}
