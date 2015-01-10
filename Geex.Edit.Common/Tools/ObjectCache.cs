using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geex.Edit.Common.Tools
{
    /// <summary>
    /// Generic class for object cacheing.
    /// </summary>
    public sealed class ObjectCache<TID, T> : IDisposable
    {
        public delegate T LoadObjectDelegate(TID id);
        #region Variables / Properties
        /// <summary>
        /// Objects of the cache.
        /// </summary>
        Dictionary<TID, T> m_objects;
        /// <summary>
        /// Maximum of objects contained in the cache.
        /// </summary>
        int m_maxObjects;
        /// <summary>
        /// Delegate used to load objects.
        /// </summary>
        LoadObjectDelegate m_loadObjectDelegate;
        #endregion

        #region Methods
        /// <summary>
        /// Creates a new instance of Object Cache
        /// <param name="max">Maximum number of instances stored in cache.</param>
        /// <param name="loadObjectDelegate">Delegate which will load the objects
        /// given their id.</param>
        /// </summary>
        public ObjectCache(int max, LoadObjectDelegate loadObjectDelegate)
        {
            m_maxObjects = max;
            m_objects = new Dictionary<TID, T>(max);
            m_loadObjectDelegate = loadObjectDelegate;
        }
        /// <summary>
        /// Retrives an object from the cache.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T this[TID id]
        {
            get 
            { 
                if(!m_objects.ContainsKey(id))
                    m_objects[id] = m_loadObjectDelegate(id);

                // Removes the older element
                if (m_objects.Count > m_maxObjects)
                    m_objects.Remove(m_objects.Keys.First());

                return m_objects[id];
            }
        }
        /// <summary>
        /// Disposes all the objects in the cache.
        /// </summary>
        public void Dispose()
        {
            foreach(T obj in m_objects.Values)
            {
                if (obj is IDisposable)
                    ((IDisposable)obj).Dispose();
            }
            m_objects.Clear();
        }
        #endregion
    }
}
