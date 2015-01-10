using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
namespace UeFGame.GameObjects.Player.Combos
{
    /// <summary>
    /// Représente un arbre permettant de détecter l'appui de touche selon des modèles de combos en chaine.
    /// </summary>
    public class ChainCombo
    {
        #region Delegates / Events
        public delegate void OnActionDelegate(int actionId);
        public event OnActionDelegate OnAction;
        #endregion

        #region Variables

        /// <summary>
        /// Noeuds principaux du combo.
        /// </summary>
        Dictionary<Keys, ChainComboNode> Nodes;
        /// <summary>
        /// Noeud actuel du combo.
        /// </summary>
        ChainComboNode m_currentNode = null;
        /// <summary>
        /// Last key stroke.
        /// </summary>
        TimeSpan LastKeyStroke;
        #endregion

        #region Methods
        /// <summary>
        /// Constructeur.
        /// </summary>
        public ChainCombo()
        {
            Nodes = new Dictionary<Keys, ChainComboNode>();
            // Node 1
            ChainComboNode nodeA = new ChainComboNode(500, 0);
            Nodes.Add(Keys.A, nodeA);

            // Node 2
            ChainComboNode nodeB = new ChainComboNode(500, 1);
            Nodes.Add(Keys.Z, nodeB);

            // Node 2.1
            ChainComboNode nodeBA = new ChainComboNode(500, 2);
            nodeB.Children.Add(Keys.A, nodeBA);

            // Node 2.2
            ChainComboNode nodeBB = new ChainComboNode(500, 3);
            nodeB.Children.Add(Keys.Z, nodeBB);

            // Node 2.3
            ChainComboNode nodeBC = new ChainComboNode(500, 4);
            nodeB.Children.Add(Keys.E, nodeBC);

            // Node 2.3.1
            ChainComboNode nodeBCA = new ChainComboNode(500, 5);
            nodeBC.Children.Add(Keys.A, nodeBCA);

            OnAction += new OnActionDelegate(OnActionTest);
        }
        /// <summary>
        /// Mets à jour le chain combo.
        /// </summary>
        /// <param name="time"></param>
        public void Update(GameTime time)
        {
            // On vérifie si une touche du prochain noeud est appuyée.
            bool actionTriggered = false;
            if (m_currentNode != null)
            {
                foreach (var kvp in m_currentNode.Children)
                {
                    if (Input.IsTrigger(kvp.Key))
                    {
                        m_currentNode = kvp.Value;
                        LastKeyStroke = time.TotalGameTime;
                        actionTriggered = true;
                        OnAction(m_currentNode.Action);
                        break;
                    }
                }
            }
            // Si aucune action ne s'est déclenchée,
            // on regarde si le temps imparti est écoulé ou non.
            if (!actionTriggered)
            {
                if (m_currentNode != null)
                {
                    int delay = (int)(time.TotalGameTime.TotalMilliseconds - LastKeyStroke.TotalMilliseconds);
                    if (delay > m_currentNode.Delay)
                        m_currentNode = null;
                }
                // Si on a appuyé sur une touche du premier noeud, on y revient.
                foreach (var kvp in Nodes)
                {
                    if (Input.IsTrigger(kvp.Key))
                    {
                        m_currentNode = kvp.Value;
                        LastKeyStroke = time.TotalGameTime;
                        OnAction(m_currentNode.Action);
                        break;
                    }
                }
            }
        }
        #endregion

        #region Actions TEST
        /// <summary>
        /// Appelé lorsqu'une action est exécutée.
        /// </summary>
        /// <param name="actionId"></param>
        void OnActionTest(int actionId)
        {
            switch (actionId)
            {
                case 0: // A
                    Particle(100, 200);
                    break;
                case 1: // B
                    Particle(500, 200);
                    break;
                case 2: // BA
                    Particle(400, 300);
                    break;
                case 3: // BB
                    Particle(500, 300);
                    break;
                case 4: // BC
                    Particle(600, 300);
                    break;
                case 5: // BCA
                    Particle(550, 400);
                    break;
            }
        }
        void Shoot()
        {
            Shoots.GatlingShootInit init = new Shoots.GatlingShootInit();
            init.BodyCategory = BodyCategories.Friend;
            init.SimStart = new Vector2(60, 60) + new Microsoft.Xna.Framework.Vector2(1f, 0.12f);
            Shoots.GatlingShoot.Pool.GenericGetFromPool(init);
        }

        void Particle(int x, int y)
        {
            var m_partInit = new Particles.ImageParticleInit();
            m_partInit.TextureName = "RunTimeAssets\\Graphics\\Particles\\test";
            m_partInit.Lifetime = 500;
            m_partInit.FadeOutTime = 250;
            m_partInit.AngleVelocity = 1f;
            m_partInit.PxX = (int)x;
            m_partInit.PxY = (int)y;
            m_partInit.Angle = 60;
            Particles.ImageParticle.Pool.GenericGetFromPool(m_partInit);
        }

        #endregion
    }
}
