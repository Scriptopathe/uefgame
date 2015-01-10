using System;
using System.Collections;
using System.Collections.Generic;
namespace Geex.Edit.Common.Project
{
    ///<summary>Data class for map information</summary>
    public sealed class MapInfo : IComparable
    {
        int IComparable.CompareTo(object obj)
        {
            return this.Order.CompareTo(((MapInfo)obj).Order);
        }
        ///<summary>The map Name.</summary>
        public string Name;
        ///<summary>The parent map ID</summary>
        public int ParentId;
        /// <summary>Id of the map</summary>
        public int Id;
        ///<summary>The map tree display order, used internal</summary>
        public int Order;
        ///<summary>The map tree expansion flag, used internally</summary>
        public bool Expanded;
        ///<summary>Data class for map information</summary>
        public MapInfo()
        {
            Name = string.Empty;
            ParentId = 0;
            Id = 0;
            Order = 0;
            Expanded = false;
        }
    }
}