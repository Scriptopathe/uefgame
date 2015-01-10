using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UeFGame.GameObjects
{
    public class GameObjectPool
    {
        /* --------------------------------------------------------------------------------
        * Variables
        * -------------------------------------------------------------------------------*/
        #region Variables
        public const int MAX_COUNT = 500;
        private List<GameObject> active;
        private GameObject[] pool;
        private FarseerPhysics.Dynamics.World m_world;
        #endregion
        /* --------------------------------------------------------------------------------
         * Methods
         * -------------------------------------------------------------------------------*/
        #region Methods
        /// <summary>
        /// Constructeur.
        /// </summary>
        public GameObjectPool(FarseerPhysics.Dynamics.World world)
        {
            m_world = world;
            active = new List<GameObject>();
            pool = new GameObject[MAX_COUNT];
            for (int i = 0; i < MAX_COUNT; i++)
            {
                pool[i] = new UniqueBodyGameObject(world);
            }
        }
        /// <summary>
        /// Returns a List containing every active object.
        /// Use it to iterate through events.
        /// </summary>
        /// <returns></returns>
        public List<GameObject> GetActive()
        {
            return active;
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
        public TObj GenericGetFromPool<TInit, TObj>(TInit data) where TInit : GameObjectInitializingData
            where TObj : GameObject
        {
            int FirstEmpty = -1;
            for (int i = 0; i < MAX_COUNT; i++)
            {
                // Removes a reference from the pool and adds it into the active objects.
                if (pool[i] != null)
                {
                    if (pool[i] is TObj)
                    {
                        TObj ev = (TObj)pool[i];
                        active.Add(pool[i]);
                        pool[i] = null;
                        ev.InitializeFromData(data, m_world);
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
            TObj instance = (TObj)Activator.CreateInstance(typeof(TObj), m_world);
            pool[FirstEmpty].Dispose();
            pool[FirstEmpty] = null;
            active.Add(instance);
            instance.InitializeFromData(data, m_world);
            return instance;
        }
        /// <summary>
        /// Returns one event from the pool and set it up using <paramref name="data"/>.
        /// This item becomes active and goes as the first item of the list.
        /// pool -> active.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public GameObject GetFromPool(GameObjectInitializingData data)
        {
            // return GetFromPool<Event>(data);
            for (int i = 0; i < MAX_COUNT; i++)
            {
                // Removes a reference from the pool and adds it into the active objects.
                if (pool[i] != null)
                {
                    GameObject ev = pool[i];
                    active.Add(pool[i]);
                    pool[i] = null;
                    ev.InitializeFromData(data, m_world);
                    return ev;
                }
            }
            throw new Exception("Not enough events in pool");
        }
        /// <summary>
        /// Removes an event from the active ones and put in the pool.
        /// active -> pool
        /// </summary>
        /// <param name="ev"></param>
        public void Free(GameObject ev)
        {
            // Removes the event from the actives ones.
            active.Remove(ev);
            // Adds the event in the pool.
            for (int i = 0; i < MAX_COUNT; i++)
            {
                if (pool[i] == null)
                {
                    pool[i] = ev;
                    ev.Deactivate();
                    break;
                }
            }
        }

        /// <summary>
        /// Clears the pool, and marks all item as available.
        /// pool -> active.
        /// </summary>
        public void Clear()
        {
            int j = 0;
            foreach (GameObject ev in active)
            {
                for (int i = j; i < MAX_COUNT; i++)
                {
                    if (pool[i] == null)
                    {
                        pool[i] = ev;
                        ev.Deactivate();
                        active.Remove(ev);
                        j = i;
                    }
                }
            }
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
