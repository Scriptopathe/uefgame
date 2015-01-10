using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace UeFGame.GameObjects.Shoots
{
    public class GatlingShootInit
    {
        /// <summary>
        /// Body Category of the shoot.
        /// </summary>
        public BodyCategories BodyCategory;
        /// <summary>
        /// Start position in sim units.
        /// </summary>
        public Vector2 SimStart;
    }
    /// <summary>
    /// Tir de mitrailleuse.
    /// </summary>
    public class GatlingShoot : Shoot, IPoolable<GatlingShootInit>
    {
        #region Static
        public static GameObjectPool<GatlingShoot, GatlingShootInit> Pool;
        static GameObjectInit baseModule = SetupBaseModule();
        static GameObjectInit SetupBaseModule()
        {
            GameObjectInit init = new GameObjectInit();
            init.Type = "UeFGame.GameObjects.Shoots.GatlingShoot";
            ModuleSet set = new ModuleSet();
            PhysicalObjectModule physics = new PhysicalObjectModule();
            physics.BodyCategory = BodyCategories.Neutral;
            physics.BodyType = FarseerPhysics.Dynamics.BodyType.Dynamic;
            physics.ShapeType = ShapeType.Rectangle;
            physics.ShapeSizeSim = new Microsoft.Xna.Framework.Vector2(ConvertUnits.ToSimUnits(20), ConvertUnits.ToSimUnits(5));
            physics.IsFixedRotation = true;
            set["physical_object"] = physics;
            init.ModuleSet = set;

            s_texture = TextureRessourceCache.Cached("RunTimeAssets\\Graphics\\Shoots\\bullet");
            return init;
        }
        /// <summary>
        /// Texture de la Gatling;
        /// </summary>
        static Texture2D s_texture;
        #endregion
        #region Variables
        

        #endregion

        #region Properties

        #endregion

        #region Methods
        /// <summary>
        /// Gatling shoot.
        /// </summary>
        public GatlingShoot() : base()
        {

        }
        /// <summary>
        /// Dessine le tir.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="batch"></param>
        /// <param name="scroll"></param>
        public override void Draw(GameTime time, Microsoft.Xna.Framework.Graphics.SpriteBatch batch, Vector2 scroll)
        {
            base.Draw(time, batch, scroll);
            if (!IsActive)
                return;
            batch.Draw(s_texture, disp(m_body.Position)-scroll, Color.White);
        }
        /// <summary>
        /// Mets à jour le tir.
        /// </summary>
        /// <param name="time"></param>
        public override void Update(GameTime time)
        {
            base.Update(time);
            if (!IsActive)
                return;
            m_body.LinearVelocity = new Vector2(10.0f, 0.0f);
        }

        /// <summary>
        /// On Collision.
        /// </summary>
        /// <param name="fixtureA"></param>
        /// <param name="fixtureB"></param>
        /// <param name="contact"></param>
        /// <returns></returns>
        protected override bool OnCollision(FarseerPhysics.Dynamics.Fixture fixtureA, FarseerPhysics.Dynamics.Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            if (!IsActive)
                return false;

            bool collides = base.OnCollision(fixtureA, fixtureB, contact);
            if (!collides)
                return false;

            if(fixtureB.Body.UserData is BodyCategories)
            {
                // wall
                Pool.Deactivate(this);
            }

            if (fixtureB.Body.UserData is PhysicalObject)
            {
                PhysicalObject obj = (PhysicalObject)(fixtureB.Body.UserData);

                if(obj.BodyCategory == this.BodyCategory)
                    return false;
            }


            return false;
        }
        /// <summary>
        /// Initialise le tir depuis la pool.
        /// </summary>
        /// <param name="init"></param>
        public void Initialize(GatlingShootInit init)
        {
            PhysicalObjectModule physics = (PhysicalObjectModule)baseModule.ModuleSet["physical_object"];
            physics.BodyCategory = init.BodyCategory;
            baseModule.ModuleSet.Base.SimPosition = init.SimStart;
            base.InitializingData = baseModule;
            base.Initialize();
        }
        #endregion
    }
}
