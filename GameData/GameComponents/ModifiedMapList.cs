using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UeFGame.GameComponents
{
    public class ModifiedMapList
    {
        public List<int> m_maps;

        public List<int> GetMaps()
        {
            return m_maps;
        }

        public void Remove(int value)
        {
            m_maps.Remove(value);
        }

        public void Add(int value)
        {
            if (!m_maps.Contains(value))
                m_maps.Add(value);
        }

        public ModifiedMapList(List<int> maps)
        {
            m_maps = maps;
        }

        public void Clear()
        {
            m_maps.Clear();
        }

        public void Save(string output)
        {
            UeFGame.Tools.Serializer.Serialize<List<int>>(m_maps, output);
        }
        public static ModifiedMapList Load(string filename)
        {
            return new ModifiedMapList(UeFGame.Tools.Serializer.Deserialize<List<int>>(filename, true));
        }
    }
}
