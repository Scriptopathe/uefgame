using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UeFGame.GameComponents
{
    /// <summary>
    /// Represents a material for a tile.
    /// TODO : get by ID.
    /// </summary>
    public abstract class TileMaterial
    {
        #region Static
        /// <summary>
        /// Cache des matériaux en fonction de leur ID.
        /// </summary>
        private static Dictionary<int, TileMaterial> s_MaterialCache = new Dictionary<int, TileMaterial>();

        public static readonly TileMaterialVoid Void = new TileMaterialVoid(0);

        public static readonly TileMaterialDefault Default = new TileMaterialDefault(1);

        /// <summary>
        /// Constructeur protégé.
        /// </summary>
        protected TileMaterial(int id)
        {
            Id = id;
            s_MaterialCache.Add(Id, this);
        }
        /// <summary>
        /// Retourne un TileMaterial en fonction de son ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static TileMaterial GetMaterialById(int id)
        {
            if (s_MaterialCache.ContainsKey(id))
                return s_MaterialCache[id];
            throw new Exception("L'id de materiau spécifié n'existe pas");
        }
        #endregion

        #region Abstract
        /// <summary>
        /// Id du matériau.
        /// </summary>
        public int Id
        {
            get;
            set;
        }
        /// <summary>
        /// Density of the body composed of a group of tile with the same material.
        /// </summary>
        public abstract float Density { get; }
        /// <summary>
        /// Friction of the body composed of a group of tile with the same material.
        /// </summary>
        public abstract float Friction { get; }
        /// <summary>
        /// Hardness of the material :
        ///     - if -1 unbreakable
        ///     - else : indicates the amount of damages the tile must take
        ///     before being destroyed.
        /// </summary>
        public abstract int Hardness { get; }
        /// <summary>
        /// Indicates if the given material is Passable.
        /// Collision will be detected, but this will not
        /// prevent movement collisions.
        /// </summary>
        public abstract bool IsPassable { get; }
        #endregion
    }
    /// <summary>
    /// Default material.
    /// </summary>
    public sealed class TileMaterialDefault : TileMaterial
    {
        public TileMaterialDefault(int id) : base(id) { }
        public override float Friction
        {
            get { return 0.1f; }
        }
        public override float Density
        {
            get { return 5.0f; }
        }
        public override int Hardness
        {
            get { return -1; }
        }
        public override bool IsPassable
        {
            get { return false; }
        }
    }
    /// <summary>
    /// Default material.
    /// </summary>
    public sealed class TileMaterialVoid : TileMaterial
    {
        public TileMaterialVoid(int id) : base(id) { }
        public override float Friction
        {
            get { return 0.0f; }
        }
        public override float Density
        {
            get { return 0.0f; }
        }
        public override int Hardness
        {
            get { return -1; }
        }
        public override bool IsPassable
        {
            get { return true; }
        }
    }
}
