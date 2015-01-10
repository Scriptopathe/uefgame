using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using Microsoft.Xna.Framework;
using Color = System.Drawing.Color;

namespace Geex.Edit.Common.Tools.Controls
{
    public class PropertyEditionAttribute : Attribute
    {
        public string GroupName { get; set; }
        public PropertyEditionAttribute(string groupName)
        {
            GroupName = groupName;
        }
    }
    /// <summary>
    /// Control that can be used to edit the values of a dictionnary whose keys
    /// are already set up.
    /// 
    /// To initialize it properly :
    ///     - first call the parameterless constructor and then InitControl
    ///     - or call the constructor which takes the object to edit as a parameter.
    /// The given instance will be edited when the user changes the values (no copy wil
    /// be created).
    /// 
    /// To reload an object, use    ReloadObject
    /// To load a new object, use   LoadObject
    /// </summary>
    public partial class DictionnaryEditControl : UserControl
    {
        enum Tags
        {
            Property,
            Value,
            ValueCollection,
            FullObject,
        }

        #region EVents
        /// <summary>
        /// Delegate for the PropertyValueChanged event.
        /// </summary>
        public delegate void PropertyValueChangedEventHandler(object obj, string propertyName, object newValue);
        /// <summary>
        /// Fired when a property value changes.
        /// </summary>
        public event PropertyValueChangedEventHandler PropertyValueChanged;
        #endregion

        /* ---------------------------------------------------------------
         * Variables
         * -------------------------------------------------------------*/
        #region Variables
        /// <summary>
        /// Liste des propriétés.
        /// </summary>
        List<string> m_properties;
        /// <summary>
        /// Object currently being edited.
        /// </summary>
        Dictionary<string, object> m_object;
        #endregion
        /* ---------------------------------------------------------------
         * Properties
         * -------------------------------------------------------------*/
        #region Properties
        #endregion
        /* ---------------------------------------------------------------
         * Methods
         * -------------------------------------------------------------*/
        #region Methods
        /// <summary>
        /// Creates a new fully-working instance of ObjectEditControl.
        /// </summary>
        /// <param name="obj"></param>
        public DictionnaryEditControl(Dictionary<string, object> obj)
        {
            InitializeComponent();
            InitializeControl(obj);
        }
        /// <summary>
        /// Creates an instance of ObjectEditControl for the conceptor.
        /// </summary>
        public DictionnaryEditControl()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Initializes this control.
        /// </summary>
        /// <param name="obj"></param>
        public void InitializeControl(Dictionary<string, object> obj)
        {
            m_object = obj;
            InitializePropertiesList();
            InitializeListEvents();
            InitializeListViewControlers();
            RefreshList();
        }
        /// <summary>
        /// Initializes this control.
        /// </summary>
        public void InitializeControl()
        {
            InitializeListEvents();
            InitializeListViewControlers();
            // Size list
            m_listView.Columns[0].Width = this.Size.Width / 2;
            m_listView.Columns[1].Width = this.Size.Width / 2;
        }
        /// <summary>
        /// Loads an object.
        /// </summary>
        /// <param name="obj"></param>
        public void LoadObject(Dictionary<string, object> obj)
        {
            if (!(m_object == null) && m_object.GetType() == obj.GetType())
            {
                m_object = obj;
                ReloadObjectValues();
            }
            else
            {
                m_object = obj;
                ReloadObject();
            }
        }
        /// <summary>
        /// Reloads the current object.
        /// </summary>
        public void ReloadObject()
        {
            InitializePropertiesList();
            RefreshList();
        }
        /// <summary>
        /// Reloads the values of the current object.
        /// </summary>
        public void ReloadObjectValues()
        {
            if (m_object == null)
                return;
            foreach (ListViewItem item in m_listView.Items)
            {
                Tags tag = (Tags)item.SubItems[1].Tag;
                string propertyName = (string)item.SubItems[0].Tag;
                if (tag == Tags.Value)
                {
                    string value = GetValue(propertyName).ToString();
                    item.SubItems[1].Text = value;
                }
            }
        }
        /// <summary>
        /// Initialize the list's events
        /// </summary>
        void InitializeListEvents()
        {
            m_listView.SubItemClicked += new Geex.Edit.Common.Tools.Controls.SubItemEventHandler(m_listView_SubItemClicked);
            m_listView.SubItemEndEditing += new Geex.Edit.Common.Tools.Controls.SubItemEndEditingEventHandler(m_listView_SubItemEndEditing);
        }
        /// <summary>
        /// Initialize the properties list.
        /// </summary>
        void InitializePropertiesList()
        {
            m_properties = new List<string>();
            foreach (var kvp in m_object)
            {
                TreatObject(ref m_properties, "#" + kvp.Key, kvp.Value);
            }
            /*Type t = m_object.GetType();
            foreach (PropertyInfo prop in t.GetProperties())
            {
                // Check for not editable property.
                /*NotEditablePropertyAttribute[] attrs = (NotEditablePropertyAttribute[])prop.GetCustomAttributes(typeof(NotEditablePropertyAttribute), true);
                if (attrs.Count() > 0)
                    continue;
                // Check if the property is R/W.
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                // Adds the property to the list.
                TreatObject(ref m_properties, prop.Name, GetValue(prop.Name));
            }
            foreach (FieldInfo prop in t.GetFields())
            {
                // Check for not editable field.
                /*NotEditablePropertyAttribute[] attrs = (NotEditablePropertyAttribute[])prop.GetCustomAttributes(typeof(NotEditablePropertyAttribute), true);
                if (attrs.Count() > 0)
                    continue;
                 
                // Check if the field is public.
                if (!prop.IsPublic)
                    continue;
                // If we have a special object
                object value = GetValue(prop.Name);
                TreatObject(ref m_properties, prop.Name, value);
            }*/
        }


        /// <summary>
        /// Refreshes the list.
        /// </summary>
        void RefreshList()
        {
            m_listView.Items.Clear();
            int depth = 0;
            // List of groups by name :
            Dictionary<string, ListViewGroup> groups = new Dictionary<string, ListViewGroup>(); // groups by name

            // The default group
            ListViewGroup defaultGroup = new ListViewGroup("default", "Global");
            ListViewGroup parentGroup = defaultGroup;
            groups.Add("default", defaultGroup);
            // Color of the objects
            Color objColor = Color.FromArgb(221, 238, 255);
            
            foreach (string property in m_properties)
            {
                bool isCollection = false;
                bool isGroup = false;
                string displayName; // name displayed
                object displayValue; // object displayed
                string propertyName = property; // working property name
                PropertyEditionAttribute attr = null;
                // Checks the type of property / group.
                if (property.Contains("++")) // start group
                {
                    isGroup = true;
                    displayName = property.Replace("++", "");
                    displayValue = GetValue(displayName).ToString();
                    attr = GetPropertyAttributes(displayName);
                    propertyName = displayName;
                }
                else if (property.StartsWith("--")) // end group
                {
                    depth--;
                    continue;
                }
                else if (property.EndsWith(".Item")) // indexer
                {
                    isCollection = true;
                    displayValue = "(collection)";
                    displayName = property.Replace(".Item", ".[]");
                    attr = GetPropertyAttributes(property);
                    propertyName = "$"; // means indexer.
                }
                else if (property.StartsWith("#")) // indexed item
                {
                    displayName = "[" + property.Replace("#", "") + "]";
                    displayValue = GetValue(property).ToString();
                    attr = GetPropertyAttributes(property);
                }
                else
                {
                    displayValue = GetValue(property);
                    displayName = property;
                    attr = GetPropertyAttributes(property);
                }
                // Choose the right group
                string groupName = defaultGroup.Name;

                if (depth == 0 && attr != null)
                {
                    if (attr.GroupName != "")
                    {
                        groupName = attr.GroupName;
                        if (!groups.Keys.Contains(groupName))
                            groups.Add(groupName, new ListViewGroup(groupName));
                    }
                }

                ListViewGroup group = groups[groupName];
                if (isGroup && depth == 0)
                    // Group of the parent (in the root only)
                    parentGroup = group;
                else if (depth > 0)
                    // It's a child and it will take its parent's group's.
                    group = parentGroup;

                // Adds spacing
                if (depth > 0)
                {
                    char[] spaces = new char[depth * 3];
                    for (int i = 0; i < spaces.Count(); i++) { spaces[i] = ' '; }
                    string spacer = new string(spaces);
                    displayName = spacer + displayName;
                }

                // Adds the item.
                ListViewItem item = new ListViewItem(new string[] { displayName, displayValue.ToString() }, group);
                
                // Set the tags
                item.SubItems[0].Tag = propertyName;
                if (isCollection)
                {
                    item.SubItems[1].Tag = Tags.ValueCollection;
                }
                else if (isGroup)
                {
                    item.SubItems[1].Tag = Tags.FullObject;
                    item.BackColor = objColor;
                    depth++;
                }
                else
                {
                    item.SubItems[1].Tag = Tags.Value;
                }

                m_listView.Items.Add(item);
            }
            // Adds all the groups to the list view.
            foreach (ListViewGroup grp in groups.Values)
            {
                m_listView.Groups.Add(grp);
            }
        }

        #endregion
        /* ---------------------------------------------------------------
         * List events
         * -------------------------------------------------------------*/
        #region List events
        TextBox m_textboxControler;
        NumericUpDown m_integerControler;
        ComboBox m_enumControler;
        /// <summary>
        /// Initialize the ListView's controlers.
        /// </summary>
        void InitializeListViewControlers()
        {
            // textbox controler
            m_textboxControler = new TextBox();
            m_listView.Controls.Add(m_textboxControler);
            m_textboxControler.Visible = false;
            // Integer controler
            m_integerControler = new NumericUpDown();
            m_listView.Controls.Add(m_integerControler);
            m_integerControler.Visible = false;
            // Enum controler
            m_enumControler = new ComboBox();
            m_enumControler.Visible = false;
            m_enumControler.DropDownStyle = ComboBoxStyle.DropDown;
            m_listView.Controls.Add(m_enumControler);
            
        }
        /// <summary>
        /// Called when the edition ends.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_listView_SubItemEndEditing(object sender, Controls.SubItemEndEditingEventArgs e)
        {
            ListViewItem.ListViewSubItem subItem = e.Item.SubItems[e.SubItem];
            string propertyName = (string)e.Item.SubItems[0].Tag;
            
            object value = GetValue(propertyName);
            var attribute = GetPropertyAttributes(propertyName);
            
            Type t = value.GetType();
            // Check the type :
            try
            {
                if (GetStringEditableTypes().Contains(t))
                {
                    var o = m_object;
                    if (t == typeof(float))
                    {
                        SetValue(propertyName, float.Parse(m_textboxControler.Text));
                    }
                    else if (t == typeof(double))
                    {
                        SetValue(propertyName, double.Parse(m_textboxControler.Text));
                    }
                    else if (t == typeof(string))
                    {
                        SetValue(propertyName, m_textboxControler.Text);
                    }
                    e.DisplayText = GetValue(propertyName).ToString();
                }
                else if (GetIntegerEditableTypes().Contains(t))
                {
                    MethodInfo parse = t.GetMethod("Parse", new Type[] { typeof(string)});
                    SetValue(propertyName, parse.Invoke(null, new object[] { m_integerControler.Value.ToString() }));
                    e.DisplayText = GetValue(propertyName).ToString();
                }
                else if (t.IsEnum)
                {
                    string[] enumVals = m_enumControler.Text.Split('|');
                    dynamic enumVal = Enum.Parse(t, enumVals[0]);
                    // If there are many ones
                    if(enumVals.Count() > 1)
                        for (int i = 1; i < enumVals.Count(); i++)
                        {
                            dynamic newEnumEval = Enum.Parse(t, enumVals[i]);
                            enumVal = enumVal | newEnumEval;
                        }
                    // object enumVal = Enum.Parse(t, m_enumControler.Text);
                    SetValue(propertyName, enumVal);
                    e.DisplayText = GetValue(propertyName).ToString();
                }

            }
            catch
            {
                e.DisplayText = value.ToString();
            }
            if (PropertyValueChanged != null)
                PropertyValueChanged(m_object, propertyName, GetValue(propertyName));

            e.Cancel = false;
            
        }
        /// <summary>
        /// Called when a sub-item is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_listView_SubItemClicked(object sender, Controls.SubItemEventArgs e)
        {
            ListViewItem.ListViewSubItem subItem =  e.Item.SubItems[e.SubItem];
            // Clicked on a "PropertyName" :
            if (subItem.Tag.GetType() == typeof(string))
                return;
            string propertyName = (string)e.Item.SubItems[0].Tag;
            // Check for index
            if (propertyName == "$") // means index
                return;
            // If we got a value.
            if (((Tags)subItem.Tag & Tags.Value) == Tags.Value)
            {
                object value = GetValue(propertyName);
                if (GetStringEditableTypes().Contains(value.GetType()))
                {
                    m_listView.StartEditing(m_textboxControler, e.Item, e.SubItem);
                }
                else if(GetIntegerEditableTypes().Contains(value.GetType()))
                {
                    Type t = value.GetType();
                    var minValue = t.GetField("MinValue", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                    var maxValue = t.GetField("MaxValue", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                    dynamic min = minValue.GetValue(null);
                    dynamic max = maxValue.GetValue(null);
                    m_integerControler.Minimum = min;
                    m_integerControler.Maximum = max;
                    m_listView.StartEditing(m_integerControler, e.Item, e.SubItem);
                }
                else if (value.GetType().IsEnum)
                {
                    string[] values = value.GetType().GetEnumNames();
                    m_enumControler.Items.Clear();
                    m_enumControler.Items.AddRange(values);
                    m_listView.StartEditing(m_enumControler, e.Item, e.SubItem);
                    
                }

            }
        }
        /// <summary>
        /// Gets the types editables with a single Textbox.
        /// </summary>
        /// <returns></returns>
        Type[] GetStringEditableTypes()
        {
            return new Type[] { typeof(string), typeof(float), typeof(double) };
        }
        /// <summary>
        /// Gets the integer editable types.
        /// </summary>
        /// <returns></returns>
        Type[] GetIntegerEditableTypes()
        {
            return new Type[] { typeof(int), typeof(byte), typeof(ushort), typeof(short),
                typeof(uint), typeof(long), typeof(ulong) };
        }
        #endregion
        /* ---------------------------------------------------------------
         * Object treatment
         * -------------------------------------------------------------*/
        #region Object treatment
        /// <summary>
        /// Treats special objects.
        /// </summary>
        /// <returns></returns>
        bool TreatObject(ref List<string> properties, string currentName, object obj)
        {
            Type t = obj.GetType();
            // Check for system types
            if (t == typeof(int) || t == typeof(float) || t == typeof(double) || t == typeof(long) ||
               t == typeof(string) || t == typeof(bool) || t == typeof(string) || t == typeof(char) || t == typeof(byte) ||
                t == typeof(short) || t == typeof(ushort) || t == typeof(uint) || t == typeof(ulong))
            {
                properties.Add(currentName);
                return false;
            }
            // Checks for list
            if (obj.GetType().IsArray || obj.GetType().IsSubclassOf(typeof(System.Collections.ICollection)) ||
                obj.GetType().IsSubclassOf(typeof(System.Collections.Generic.ICollection<>)) ||
                obj.GetType().IsSubclassOf(typeof(System.Collections.Generic.IList<>)))
            {
                return false;
            }
            // Check for enum
            if (obj.GetType().IsEnum)
            {
                properties.Add(currentName);
                return false;
            }
            // Adds the property list
            properties.Add("++" + currentName);
            foreach (PropertyInfo property in obj.GetType().GetProperties())
            {
                // Prevents for infinite loops
                if (property.PropertyType == obj.GetType() || property.PropertyType.IsSubclassOf(obj.GetType()))
                    continue;
                // Check for read/write
                if (!property.CanRead || !property.CanWrite)
                    continue;
                // Is indexer
                if (property.Name == "Item")
                {
                    properties.Add(currentName + "." + property.Name);
                    continue;
                }
                // Check for not editable property.
                /*NotEditablePropertyAttribute[] attrs = (NotEditablePropertyAttribute[])property.GetCustomAttributes(typeof(NotEditablePropertyAttribute), true);
                if (attrs.Count() > 0)
                    continue;*/

                object value = GetValue(obj, property.Name);
                TreatObject(ref properties, currentName + "." + property.Name, value);
            }
            foreach (FieldInfo field in obj.GetType().GetFields())
            {
                // Prevents for infinite loops
                if (field.FieldType == obj.GetType() || field.FieldType.IsSubclassOf(obj.GetType()))
                    continue;
                // Check for read/write
                if (!field.IsPublic)
                    continue;
                // Check for not editable property.
                /*NotEditablePropertyAttribute[] attrs = (NotEditablePropertyAttribute[])field.GetCustomAttributes(typeof(NotEditablePropertyAttribute), true);
                if (attrs.Count() > 0)
                    continue;*/
                object value = GetValue(obj, field.Name);
                TreatObject(ref properties, currentName + "." + field.Name, value);
            }
            properties.Add("--");
            return true;
        }

        /// <summary>
        /// Gets the current value of the property.
        /// </summary>
        public object GetValue(string propertyName)
        {
            
            object currentObj = m_object;
            if (propertyName.StartsWith("#"))
            {
                // Here we get an indexed property.
                // So we change the current object to the item of the indexation, and
                // the property name to what follows it (maybe nothing).
                int indexOfPoint = propertyName.IndexOf(".");
                string indexName;
                string nextName = propertyName;
                dynamic dynCurrent = currentObj;
                if (indexOfPoint > 0)
                {
                    // #name.   Take 1 to 4 (len 3) -> len = indexPoint - index# - 1 = indexPoint - 2
                    // 012345 
                    indexName = propertyName.Substring(1, propertyName.IndexOf('.') - 2);
                    nextName = propertyName.Substring(propertyName.IndexOf('.'));
                }
                else // form : #name
                {
                    // We only need to return the indexed value.
                    indexName = propertyName.Replace("#", "");
                    return dynCurrent[indexName];
                }
                // We switch the propertyName to the name after the indexValue.
                // IE : #bla.truc.chose becomes truc.chose 
                propertyName = nextName;
                currentObj = dynCurrent[indexName];
            }
            string[] objs = propertyName.Split('.');
            if (propertyName.Contains('.'))
            {
                for (int i = 0; i < objs.Count() - 1; i++)
                {
                    currentObj = GetValue(currentObj, objs[i]);
                }
                return GetValue(currentObj, objs.Last());
            }
            return GetValue(m_object, propertyName);
        }
        /// <summary>
        /// Gets the value of the property/field of the given object.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public object GetValue(object obj, string propertyName)
        {
            if (obj != null && propertyName != null)
            {
                Type t = obj.GetType();
                System.Reflection.PropertyInfo prop = obj.GetType().GetProperty(propertyName);
                if (prop != null)
                {
                    return prop.GetValue(obj, null);
                }
                else
                {
                    System.Reflection.FieldInfo info = obj.GetType().GetField(propertyName);
                    if (info != null)
                        return info.GetValue(obj);
                    else
                        throw new InvalidOperationException("The property or field " + propertyName + " doesn't exist for the object " + obj.GetType().Name);
                }
            }
            else
            {
                throw new InvalidOperationException("m_object and PropertyName must not be null");
            }
        }
        /// <summary>
        /// Gets the current value of the property.
        /// </summary>
        public void SetValue(ref object obj, string propertyName, object value, string parentPropertyName)
        {
            if (obj != null && propertyName != null)
            {
                System.Reflection.PropertyInfo prop = obj.GetType().GetProperty(propertyName);
                if (prop != null)
                {
                    prop.SetValue(obj, value, null);
                    if (parentPropertyName != null)
                    {
                        SetValue(parentPropertyName, obj);
                    }
                }
                else
                {
                    System.Reflection.FieldInfo info = obj.GetType().GetField(propertyName);
                    if (info != null)
                    {
                        info.SetValue(obj, value);
                        if (parentPropertyName != null)
                        {
                            SetValue(parentPropertyName, obj);
                        }
                    }
                    else
                        throw new InvalidOperationException("The property or field " + propertyName + " doesn't exist for the object " + obj.GetType().Name);
                }
            }
            else
            {
                throw new InvalidOperationException("m_object and PropertyName must be valid");
            }
        }
        /// <summary>
        /// Sets the value of a property.
        /// </summary>
        public void SetValue(string propertyName, object value)
        {
            object currentObj = m_object;
            // Handles the index property setting.
            // It works only for the first object of the hierarchy.
            if (propertyName.StartsWith("#"))
            {
                // Here we get an indexed property.
                // So we change the current object to the item of the indexation, and
                // the property name to what follows it (maybe nothing).
                int indexOfPoint = propertyName.IndexOf(".");
                string indexName;
                string nextName = propertyName;
                dynamic dynCurrent = currentObj;
                if (indexOfPoint > 0)
                {
                    // #name.   Take 1 to 4 (len 3) -> len = indexPoint - index# - 1 = indexPoint - 2
                    // 012345 
                    indexName = propertyName.Substring(1, propertyName.IndexOf('.') - 2);
                    nextName = propertyName.Substring(propertyName.IndexOf('.'));
                }
                else // form : #name
                {
                    // We only need to set the indexed value.
                    indexName = propertyName.Replace("#", "");
                    dynCurrent[indexName] = value;
                    return;
                }
                // We switch the propertyName to the name after the indexValue.
                // IE : #bla.truc.chose becomes truc.chose 
                propertyName = nextName;
                currentObj = dynCurrent[indexName];
            }
            string[] objs = propertyName.Split('.');
            if (propertyName.Contains('.'))
            {
                string parentProperty = "";
                for (int i = 0; i < objs.Count() - 1; i++)
                {
                    currentObj = GetValue(currentObj, objs[i]);
                    parentProperty += objs[i];
                }
                
                SetValue(ref currentObj, objs.Last(), value, parentProperty);
            }
            else
            {
                currentObj = m_object;
                SetValue(ref currentObj, propertyName, value, null);
            }
        }
        /// <summary>
        /// Gets the attributes of the given property.
        /// </summary>
        /// <param name="propertyname"></param>
        public PropertyEditionAttribute GetPropertyAttributes(string propertyName)
        {
            if (propertyName.StartsWith("#"))
                return null;
            object currentObj;
            if (propertyName.Contains('.'))
            {
                string[] objs = propertyName.Split('.');
                currentObj = m_object;
                for (int i = 0; i < objs.Count() - 1; i++)
                {
                    currentObj = GetValue(currentObj, objs[i]);
                }
                propertyName = objs.Last();
            }
            else
            {
                currentObj = m_object;
            }

            if (currentObj != null && propertyName != null)
            {
                Type t = currentObj.GetType();
                System.Reflection.PropertyInfo prop = currentObj.GetType().GetProperty(propertyName);
                if (prop != null)
                {
                    PropertyEditionAttribute[] attrs = (PropertyEditionAttribute[])prop.GetCustomAttributes(
                                                                        typeof(PropertyEditionAttribute), true);
                    if (attrs.Count() == 0)
                        return null;
                    else
                        return attrs.First();
                }
                else
                {
                    System.Reflection.FieldInfo info = currentObj.GetType().GetField(propertyName);
                    if (info != null)
                    {
                        PropertyEditionAttribute[] attrs = (PropertyEditionAttribute[])info.GetCustomAttributes(
                                                                            typeof(PropertyEditionAttribute), true);
                        if (attrs.Count() == 0)
                            return null;
                        else
                            return attrs.First();
                    }
                    else
                        throw new InvalidOperationException("The property or field " + propertyName + " doesn't exist for the object " + currentObj.GetType().Name);
                }
            }
            else
            {
                throw new InvalidOperationException("m_object and PropertyName must not be null");
            }
        }
            
        
        #endregion
    }
}
