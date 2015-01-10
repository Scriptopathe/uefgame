using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UeFGame.GameObjects;
namespace Geex.Edit.UeF.MapView
{
    /// <summary>
    /// Tile Macro Unit class
    /// This is a unit of information for tile macro recording.
    /// </summary>
    public class MacroUnit
    {
        /// <summary>
        /// Constructor, initializes the macro unit.
        /// </summary>
        public MacroUnit()
        {

        }
        /// <summary>
        /// Undoes the unit
        /// </summary>
        public virtual void Undo()
        {

        }
        /// <summary>
        /// Redoes the unit.
        /// </summary>
        public virtual void Redo()
        {

        }
        #region Tools
        /// <summary>
        /// Gets the current value of the property.
        /// </summary>
        public object GetValue(object obj, string propertyName)
        {
            if (obj != null && propertyName != null)
            {
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
                throw new InvalidOperationException("obj and PropertyName must not be null");
            }
        }
        /// <summary>
        /// Gets the current value of the property.
        /// </summary>
        public void SetValue(object obj, string propertyName, object value)
        {
            if (obj != null && propertyName != null)
            {
                System.Reflection.PropertyInfo prop = obj.GetType().GetProperty(propertyName);
                if (prop != null)
                {
                    prop.SetValue(obj, value, null);
                }
                else
                {
                    System.Reflection.FieldInfo info = obj.GetType().GetField(propertyName);
                    if (info != null)
                        info.SetValue(obj, value);
                    else
                        throw new InvalidOperationException("The property or field " + propertyName + " doesn't exist for the object " + obj.GetType().Name);
                }
            }
            else
            {
                throw new InvalidOperationException("obj and PropertyName must be valid");
            }
        }
        #endregion
    }
    /// <summary>
    /// Macro unit that indicates that a game object's initializing data property
    /// has been changed.
    /// </summary>
    public class GameObjectMacroUnit : MacroUnit
    {
        /// <summary>
        /// Delegate containing a method which undoes the changes provided by this unit.
        /// </summary>
        public UeFGame.GameObjects.GameObject.EditorActionDelegate UndoAction;
          /// <summary>
        /// Delegate containing a method which redoes the changes provided by this unit.
        /// </summary>
        public UeFGame.GameObjects.GameObject.EditorActionDelegate RedoAction;
        /// <summary>
        /// Undoes this unit.
        /// </summary>
        public override void Undo()
        {
            UndoAction();
        }
        /// <summary>
        /// Redoes this unit.
        /// </summary>
        public override void Redo()
        {
            RedoAction();
        }

        public GameObjectMacroUnit(UeFGame.GameObjects.GameObject.EditorActionDelegate undoAction,
            UeFGame.GameObjects.GameObject.EditorActionDelegate redoAction)
        {
            UndoAction = undoAction;
            RedoAction = redoAction;
        }

        public GameObjectMacroUnit()
        {

        }
    }

    /// <summary>
    /// Macro unit that indicates that a game object's initializing data property
    /// has been changed.
    /// </summary>
    public class GameObjectMacroUnitOLDMOTHERFUCKER : MacroUnit
    {
        /// <summary>
        /// The target Game object.
        /// </summary>
        GameObjectInit GameObject;
        /// <summary>
        /// The property that has been modified.
        /// </summary>
        public string PropertyName;
        /// <summary>
        /// The old value of the property.
        /// </summary>
        public object OldValue;
        /// <summary>
        /// The old value of the property.
        /// </summary>
        public object NewValue;
        /// <summary>
        /// Creates a new GameObjectMacroUnit.
        /// </summary>
        public GameObjectMacroUnitOLDMOTHERFUCKER(GameObjectInit gameObj, string propertyName)
        {
            GameObject = gameObj;
            PropertyName = propertyName;
        }

        public GameObjectMacroUnitOLDMOTHERFUCKER()
        {

        }
        /// <summary>
        /// Undoes this unit.
        /// </summary>
        public override void Undo()
        {
            SetValue(GameObject, PropertyName, OldValue);
        }
        /// <summary>
        /// Redoes this unit.
        /// </summary>
        public override void Redo()
        {
            SetValue(NewValue, PropertyName, OldValue);
        }
        /// <summary>
        /// Gets the current value of the object(s) concerned in the macro.
        /// </summary>
        /// <returns></returns>
        public virtual object GetCurrentValue()
        {
            return GetValue(GameObject, PropertyName);
        }
    }
    public class MultipleGameObjectMacroUnit : GameObjectMacroUnit
    {
        List<GameObjectInit> GameObjects;
        public List<GameObject.EditorActionDelegate> UndoActions;
        public List<GameObject.EditorActionDelegate> RedoActions;
        public MultipleGameObjectMacroUnit(List<GameObject> gameObjs, List<GameObject.EditorActionDelegate> undoActions,
            List<GameObject.EditorActionDelegate> redoActions)
            : base()
        {
            GameObjects = new List<GameObjectInit>(gameObjs.Count);
            foreach(GameObject obj in gameObjs)
                GameObjects.Add(obj.InitializingData);
            UndoActions = undoActions;
            RedoActions = redoActions;
        }

        public override void  Undo()
        {
            for(int i = 0; i < GameObjects.Count; i++)
            {
                UndoActions[i]();
            }
        }

        public override void  Redo()
        {
            for (int i = 0; i < GameObjects.Count; i++)
            {
                RedoActions[i]();
            }
        }
    }
}
