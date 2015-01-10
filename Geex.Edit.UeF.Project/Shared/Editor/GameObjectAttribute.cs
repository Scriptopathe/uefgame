using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UeFGame.Editor
{
    /// <summary>
    /// Attributes of the Editable game objects.
    /// </summary>
    class EditableGameObjectAttribute : Attribute
    {
        public Type InitializingDataType;
        /// <summary>
        /// Creates a new instance of the GameObjectAttribute.
        /// </summary>
        /// <param name="initializingDataType">initializing type of the game object.</param>
        public EditableGameObjectAttribute(Type initializingDataType)
            : base()
        {
            InitializingDataType = initializingDataType;
        }
    }
}
