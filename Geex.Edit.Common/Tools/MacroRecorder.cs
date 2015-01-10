using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geex.Edit.Common.Tools
{

    public delegate void StackUpdatedDelegate();
    /// <summary>
    /// Generic Macro recorder class.
    /// How to use :
    ///     - Add records with Append(T)
    ///     - To undo a recording, retrieve the record by using Undo(), which
    ///     returns the record to be canceled
    ///     - Same thing for redo
    /// </summary>
    public class MacroRecorder<T>
    {
        List<T> m_undoStack;
        List<T> m_redoStack;
        int m_limit;
        public event StackUpdatedDelegate StackUpdated;
        /// <summary>
        /// Constructor, initializes the stacks.
        /// </summary>
        public MacroRecorder(int limit)
        {
            m_limit = limit;
            m_undoStack = new List<T>(m_limit);
            m_redoStack = new List<T>(m_limit);
        }
        /// <summary>
        /// Adds a record to the stack.
        /// </summary>
        /// <param name="unit"></param>
        public void Append(T unit)
        {
            if (m_undoStack.Count() == m_limit)
                m_undoStack.RemoveAt(0);
            m_undoStack.Add(unit);
            m_redoStack.Clear();

            if (StackUpdated != null)
                StackUpdated();
        }
        /// <summary>
        /// Undoes an operation and returns the associated data object.
        /// </summary>
        /// <returns></returns>
        public T Undo()
        {

            T obj = m_undoStack[m_undoStack.Count()-1];
            m_undoStack.RemoveAt(m_undoStack.Count()-1);
            m_redoStack.Add(obj);
            if (m_redoStack.Count() == m_limit)
                m_redoStack.RemoveAt(0);


            if (StackUpdated != null)
                StackUpdated();
            return obj;
        }
        /// <summary>
        /// Redoes an operation and returns the associated data object.
        /// </summary>
        /// <returns></returns>
        public T Redo()
        {
            T obj = m_redoStack[m_redoStack.Count()-1];
            m_redoStack.RemoveAt(m_redoStack.Count() - 1);
            m_undoStack.Add(obj);
            if (m_undoStack.Count == m_limit)
                m_undoStack.RemoveAt(0);


            if (StackUpdated != null)
                StackUpdated();
            return obj;
        }
        /// <summary>
        /// Returns true if an action can be canceled.
        /// </summary>
        /// <returns></returns>
        public bool CanUndo()
        {
            return m_undoStack.Count() != 0;
        }
        /// <summary>
        /// Returns true is an action can be redone.
        /// </summary>
        /// <returns></returns>
        public bool CanRedo()
        {
            return m_redoStack.Count() != 0;
        }
        /// <summary>
        /// Gets the count of the undo stack.
        /// </summary>
        public int GetUndoStackCount()
        {
            return m_undoStack.Count;
        }
        /// <summary>
        /// Gets the count of the redo stack.
        /// </summary>
        public int GetRedoStackCount()
        {
            return m_redoStack.Count;
        }
        /// <summary>
        /// Resets all the stacks.
        /// </summary>
        public void Reset()
        {
            m_redoStack.Clear();
            m_undoStack.Clear();

            if (StackUpdated != null)
                StackUpdated();
        }
    }
}
