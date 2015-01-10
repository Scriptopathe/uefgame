using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;
using FarseerPhysics.Common;
using UeFGame.GameComponents;
namespace UeFGame.Scenes
{
    public class SceneDebug : IScene
    {
        /* --------------------------------------------------------------------------------
        * Variables
        * -------------------------------------------------------------------------------*/
        #region Variables
        /// <summary>
        /// The map of this scene
        /// </summary>
        Tileset m_tileset;
        int[,] ids;
        Texture2D m_rectTexture;
        SpriteFont m_spriteFont;
        public static SceneDebug Instance;
        public List<Tileset.Edge> Edges { get; set; }
        public List<Vertices> MyVertices { get; set; }
        List<int> ptDisplayed = new List<int>();
        #endregion

        /* --------------------------------------------------------------------------------
        * Basics / Scene Implementation
        * -------------------------------------------------------------------------------*/
        #region Basics / Scene Implementation
        void DrawRect(ref bool[,] mask, int x, int y, int w, int h, bool val)
        {
            for (int i = x; i < x + w; i++)
            {
                for (int j = y; j < y + h; j++)
                {
                    mask[i, j] = val;
                }
            }
        }
        /// <summary>
        /// Initializes the Scene.
        /// </summary>
        public SceneDebug()
        {
            Instance = this;

            m_rectTexture = TextureRessourceCache.Cached("rect");
            m_spriteFont = RessourceCache<SpriteFont>.Cached("EditorAssets\\Fonts\\Arial");
        }
        /// <summary>
        /// Updates the scene.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {

        }
        /// <summary>
        /// Draws the scene.
        /// The sprite batch Begin() and End() functions must be called in this method.
        /// </summary>
        /// <param name="batch"></param>
        public void Draw(GameTime time, SpriteBatch batch)
        {
            batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            batch.Draw(m_rectTexture, new Rectangle(0, 0, Globals.ScreenWidth, Globals.ScreenHeight), Color.Black);

            // Draws lines
            for (int x = 0; x < Globals.ScreenWidth / (GameConstants.Tilesize + 10); x++)
            {
                int x_line = x * (GameConstants.Tilesize + 10);
                batch.Draw(m_rectTexture, new Rectangle(x_line, 0, 2, Globals.ScreenHeight), Color.Gray);
            }
            for (int y = 0; y < Globals.ScreenHeight / (GameConstants.Tilesize + 10); y++)
            {
                int y_line = y * (GameConstants.Tilesize + 10);
                batch.Draw(m_rectTexture, new Rectangle(0, y_line, Globals.ScreenWidth, 2), Color.Gray);
            }

            Random rand = Globals.Rand;
            Color col;
            col = new Color(rand.Next(255), rand.Next(255), rand.Next(255));
            int i = 0;
            foreach (Vertices vertices in MyVertices)
            {
                ptDisplayed[i] += 1;
                ptDisplayed[i] %= vertices.Count;
                Vector2 v = vertices[ptDisplayed[i]];
                batch.DrawString(m_spriteFont, "o", new Vector2((int)v.X, (int)v.Y), col);
                i++;
            }
            // Draw vertices
            batch.End();
        }
        /// <summary>
        /// Draws the given edge
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="edge"></param>
        void DrawEdge(SpriteBatch batch, Tileset.Edge edge, Color col)
        {
            int x1 = (int)ConvertUnits.ToDisplayUnits(edge.P1.X)+36;
            int y1 = (int)ConvertUnits.ToDisplayUnits(edge.P1.Y)+36;
            int x2 = (int)ConvertUnits.ToDisplayUnits(edge.P2.X)+36;
            int y2 = (int)ConvertUnits.ToDisplayUnits(edge.P2.Y)+36;
            for (int i = 0; i < 5; i++)
            {
                DrawPoint(batch, (int)(x1 + (x2-x1) * i / 5.0), (int)(y1 + (y2-y1) * i / 5.0), col);
            }
        }
        /// <summary>
        /// Draws the given tile.
        /// </summary>
        void DrawTile(SpriteBatch batch, int x, int y, TileMaskOld mask)
        {
            Color col;
            switch (y)
            {
                case 0:
                    col = Color.Red;
                    break;
                case 1:
                    col = Color.Orange;
                    break;
                case 2:
                    col = Color.Yellow;
                    break;
                case 3:
                    col = Color.YellowGreen;
                    break;
                case 4:
                    col = Color.Green;
                    break;
                case 5:
                    col = Color.Blue;
                    break;
                case 6:
                    col = Color.BlueViolet;
                    break;
                case 7:
                    col = Color.Violet;
                    break;
                default:
                    col = Color.White;
                    break;
            }
            int sx = (x + 10) * (GameConstants.Tilesize+10) + 6;
            int sy = (y + 1) *  (GameConstants.Tilesize+10) + 6;
            int verticeId = 0;
            int max = mask.GetVerticesCount();
            for (int i = 0; i < max; i++)
            {
                Vector2 pt = ConvertUnits.ToDisplayUnits(mask.GetVerticeById(verticeId));
                pt.X += sx;
                pt.Y += sy;
                DrawPoint(batch, (int)pt.X, (int)pt.Y, col);
                verticeId = mask.GetNextId(verticeId);
            }
        }
        /// <summary>
        /// Draws a point at the given coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        void DrawPoint(SpriteBatch batch, int x, int y, Color col)
        {
            batch.Draw(m_rectTexture, new Rectangle(x - 2, y - 2, 4, 4), col);
        }
        /// <summary>
        /// Disposes the scene
        /// </summary>
        public void Dispose()
        {

        }
        #endregion
    }
}
