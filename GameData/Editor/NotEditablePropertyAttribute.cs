using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UeFGame.Editor
{
    /// <summary>
    /// Attribute that says to the editor that a property
    /// is not editable.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class NotEditablePropertyAttribute : Attribute
    {

    }
}
