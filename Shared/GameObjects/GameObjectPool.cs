using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UeFGame.GameObjects
{
    /// <summary>
    /// Représente une objet poolable.
    /// </summary>
    public interface IPoolable<TInit> : IDisposable
    {
        void Deactivate();
        void Initialize(TInit init);
        bool IsActive { get; set; }
    }
    public class GameObjectPool<T, TInit> where T : IPoolable<TInit>, new()
    {
        /* --------------------------------------------------------------------------------
        * Variables
        * -------------------------------------------------------------------------------*/
        #region Variables
        public int MAX_COUNT = 5000;
        private List<T> m_active;
        private T[] m_pool;
        private FarseerPhysics.Dynamics.World m_world;
        private List<T> m_deactivationQueue;
        #endregion
        /* --------------------------------------------------------------------------------
         * Methods
         * -------------------------------------------------------------------------------*/
        #region Methods
        /// <summary>
        /// Constructeur.
        /// </summary>
        public GameObjectPool(int maxCount = 150)
        {
            MAX_COUNT = maxCount;
            m_active = new List<T>(MAX_COUNT);
            m_deactivationQueue = new List<T>(MAX_COUNT);
            m_pool = new T[MAX_COUNT];
            for (int i = 0; i < MAX_COUNT; i++)
            {
                m_pool[i] = new T();
            }
        }
        /// <summary>
        /// Pool update.
        /// </summary>
        public void Update()
        {
            // Deactivates objects in the deactivation queue.
            while(m_deactivationQueue.Count != 0)
            {
                Free(m_deactivationQueue[0]);
                m_deactivationQueue.RemoveAt(0);
            }
        }
        
        /// <summary>
        /// Returns a List containing every active object.
        /// Use it to iterate through events.
        /// </summary>
        /// <returns></returns>
        public List<T> GetActive()
        {
            return m_active;
        }
        /// <summary>
        /// Returns one instance of the subclass of event specified by GameObject,
        /// from the pool and set it up using <paramref name="data"/>.
        /// This item becomes active and goes as the first item of the list.
        /// pool -> active.
        /// </summary>
        /// <param name="data">The data to be used to initialize the event</param>
        /// <typeparam name="GameObject">The type of the object</typeparam>
        /// <returns></returns>
        public T GenericGetFromPool(TInit data)
        {
            int FirstEmpty = -1;
            for (int i = 0; i < MAX_COUNT; i++)
            {
                // Removes a reference from the pool and adds it into the active objects.
                if (m_pool[i] != null)
                {
                    if (m_pool[i].GetType() == typeof(T))
                    {
                        T ev = (T)m_pool[i];
                        m_active.Add(m_pool[i]);
                        m_pool[i] = default(T);
                        ev.Initialize(data);
                        ev.IsActive = true;
                        return ev;
                    }
                    else
                    {
                        if (FirstEmpty == -1)
                            FirstEmpty = i;
                    }
                }
            }
            if (FirstEmpty == -1)
                throw new Exception("Not enough events in pool.");

            // The function did not return, no GameObject found
            T instance = (T)Activator.CreateInstance(typeof(T), m_world);
            m_pool[FirstEmpty].Dispose();
            m_pool[FirstEmpty] = default(T);
            m_active.Add(instance);
            instance.Initialize(data);
            return instance;
        }
        /// <summary>
        /// Removes an event from the active ones and put in the pool.
        /// active -> pool
        /// </summary>
        /// <param name="ev"></param>
        void Free(T ev)
        {
            // Removes the event from the actives ones.
            m_active.Remove(ev);
            ev.Deactivate();
            bool ok = false;
            // Adds the event in the pool.
            for (int i = 0; i < MAX_COUNT; i++)
            {
                if (m_pool[i] == null)
                {
                    m_pool[i] = ev;
                    ok = true;
                    break;
                }
            }
            if (!ok)
                throw new Exception("Problem");
        }
        /// <summary>
        /// Removes an event from the active ones and put in the pool.
        /// active -> pool
        /// </summary>
        /// <param name="ev"></param>
        public void Deactivate(T ev)
        {
            if(ev.IsActive)
                m_deactivationQueue.Add(ev);
            ev.IsActive = false;
        }

        /// <summary>
        /// Clears the pool, and marks all item as available.
        /// pool -> active.
        /// </summary>
        public void Clear()
        {
            int j = 0;
            foreach (T ev in m_active)
            {
                for (int i = j; i < MAX_COUNT; i++)
                {
                    if (m_pool[i] == null)
                    {
                        m_pool[i] = ev;
                        ev.Deactivate();
                        m_active.Remove(ev);
                        j = i;
                    }
                }
            }
        }
        /// <summary>
        /// Frees all the memory ressources held by the pool.
        /// </summary>
        public void Dispose()
        {
            foreach(T ev in m_pool)
            {
                if(ev != null)
                    ev.Dispose();
            }
            m_pool = null;

            foreach (T ev in m_active)
            {
                if(ev != null)
                    ev.Dispose();
            }
            m_active.Clear();
            m_active = null;
        }
        /// <summary>
        /// Returns true if the given object is null.
        /// </summary>
        /// <param name="obj">The object to be tested.</param>
        /// <returns>True if it is null, false otherwise.</returns>
        protected bool EventIsNull(object obj)
        {
            return (obj == null);
        }
        #endregion
    }
}
