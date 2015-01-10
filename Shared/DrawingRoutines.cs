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
namespace UeFGame
{
    /// <summary>
    /// Drawing routines for debug etc...
    /// </summary>
    public static class DrawingRoutines
    {
        static Texture2D m_lineTex;
        static Texture2D m_squareTex;
        static Texture2D m_circleTex;
        static Texture2D m_pointTex;
        static Texture2D m_lineTexR;
        static Texture2D m_lineTexL;
        static Texture2D m_pixelTex;
        static Color m_trueWhite = Color.White;//new Color(200, 200, 200, 200);
        const float LayerDepth = 0.1f;
        public static void InitTextures()
        {
            m_pixelTex = TextureRessourceCache.Cached("EditorAssets\\pixelTex");
            m_squareTex = TextureRessourceCache.Cached("EditorAssets\\squareTex128");
            m_circleTex = TextureRessourceCache.Cached("EditorAssets\\circleTex");
            m_lineTex = TextureRessourceCache.Cached("EditorAssets\\lineTex");
            m_pointTex = TextureRessourceCache.Cached("EditorAssets\\pointTex");
            m_lineTexL = TextureRessourceCache.Cached("EditorAssets\\lineTexL");
            m_lineTexR = TextureRessourceCache.Cached("EditorAssets\\lineTexR");
        }
        public static void DisposeTextures()
        {
            m_lineTex.Dispose();
            m_squareTex.Dispose();
            m_circleTex.Dispose();
        }
        #region DrawLine
        public static void DrawLine(SpriteBatch batch, Vector2 p1, Vector2 p2, float layerDepth, Color color)
        {
            Vector2 line = p2 - p1;
            Vector2 normal = new Vector2(line.Y, line.X);
            normal.Normalize();
            if (normal.X > 0.5)
                normal.X = 1;
            else if (normal.X < -0.5)
                normal.X = -1;
            if (normal.Y > 0.5)
                normal.Y= 1;
            else if (normal.Y < -0.5)
                normal.Y = -1;

            Color c2 = new Color(Math.Max(color.R-80, 0), 
                Math.Max(color.G-80, 0),
                Math.Max(color.B-80, 0), 125);
            DrawRawLine(batch, p1, p2, layerDepth, color, 2);
            DrawRawLine(batch, p1+normal, p2+normal, layerDepth, color, 1);
            DrawRawLine(batch, p1-normal, p2-normal, layerDepth, color, 1);
        }
        static void DrawRawLine(SpriteBatch batch, Vector2 p1, Vector2 p2, float layerDepth, Color color, float width)
        {
            float angle = (float)Math.Atan2(p2.Y - p1.Y, p2.X - p1.X);
            float length = Vector2.Distance(p1, p2);

            batch.Draw(m_pixelTex, p1, null, color,
                       angle, Vector2.Zero, new Vector2(length, width),
                       SpriteEffects.None, layerDepth);
        }
        public static void DrawLine2(SpriteBatch batch, Vector2 p1, Vector2 p2, float layerDepth, Color color)
        {
            if (p1.X == p2.X && p1.Y == p2.Y)
                return;
            float dst = (float)Math.Sqrt((p2.X - p1.X) * (p2.X - p1.X) + (p2.Y - p1.Y) * (p2.Y - p1.Y));
            float angle;
            if (p1.X == p2.X)
                if (p1.Y < p2.Y)
                    angle = (float)Math.PI / 2.0f;
                else
                    angle = -(float)Math.PI / 2.0f;
            else if (p1.Y == p2.Y)
                if (p1.X < p2.X)
                    angle = 0.0f;
                else
                    angle = (float)Math.PI;
            else
            {
                
                // Angle between p1 and p2, rapported to (Ox) -> (p1Ox; Oxp2)
                angle = (float)Math.Acos((p2.X - p1.X) / dst);
                // This angle is between -PI/2 and PI/2
                if (p2.Y - p1.Y < 0)
                    angle = -angle; // now it's corrected.*/
            }
            if (color == Color.White)
                color = m_trueWhite;

            int dec = m_lineTexL.Width;
            batch.Draw(m_lineTex,
                       new Rectangle((int)p1.X, (int)p1.Y,
                           (int)dst, 7),
                           null,
                           color,
                           angle,
                           Vector2.Zero,
                           SpriteEffects.None,
                           layerDepth);
            /*
            batch.Draw(m_lineTex,
                       new Rectangle((int)p1.X+dec, (int)p1.Y,
                           (int)dst-dec*2, 7),
                           null,
                           color,
                           angle,
                           Vector2.Zero,
                           SpriteEffects.None,
                           layerDepth);
            
            batch.Draw(m_lineTexL,
                       new Rectangle((int)p1.X, (int)p1.Y, dec, m_lineTexL.Height),
                       null,
                       color,
                       angle,
                       Vector2.Zero,
                       SpriteEffects.None,
                       layerDepth);
            batch.Draw(m_lineTexR,
                       new Rectangle((int)p2.X-dec,
                           (int)p2.Y, dec, m_lineTexR.Height),
                       null,
                       color,
                       -angle,
                       Vector2.Zero,
                       SpriteEffects.None,
                       layerDepth);*/
        }
        #endregion

        #region Draw rectangle, point, circle
        public static void DrawRectangle(SpriteBatch batch, Rectangle r, float layerDepth, Color color)
        {
            DrawLine(batch, new Vector2(r.X, r.Y), new Vector2(r.Right, r.Y), layerDepth, color);
            DrawLine(batch, new Vector2(r.Right, r.Y), new Vector2(r.Right, r.Bottom), layerDepth, color);
            DrawLine(batch, new Vector2(r.Right, r.Bottom), new Vector2(r.X, r.Bottom), layerDepth, color);
            DrawLine(batch, new Vector2(r.X, r.Bottom), new Vector2(r.X, r.Y), layerDepth, color);
        }
        public static void DrawPoint(SpriteBatch batch, Vector2 pt, float layerDepth, Color color, int radius=3)
        {
            batch.Draw(m_pointTex,
                       new Rectangle((int)pt.X-radius, (int)pt.Y-radius, radius*2, radius*2),
                           null,
                           color,
                           0,
                           Vector2.Zero,
                           SpriteEffects.None,
                           layerDepth);
        }
        public static void DrawCircle(SpriteBatch batch, Rectangle r)
        {

        }
        #endregion
        /// <summary>
        /// Fills a rectangle
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="r"></param>
        /// <param name="layerDepth"></param>
        /// <param name="color"></param>
        public static void FillRectangle(SpriteBatch batch, Rectangle r, float layerDepth, Color color)
        {
            batch.Draw(m_squareTex,
                r,
                null,
                color,
                0.0f,
                Vector2.Zero,
                SpriteEffects.None,
                layerDepth);
        }
        public static void FillExceptHole(SpriteBatch batch, Rectangle r, Rectangle hole, float layerDepth, Color color)
        {
            Rectangle[] rects = new Rectangle[] 
            {
                new Rectangle(r.Left, r.Top, r.Width, hole.Top), // top rect
                new Rectangle(r.Left, hole.Bottom, r.Width, r.Bottom - hole.Bottom), // bottom rect
                new Rectangle(r.Left, hole.Top, hole.Left, hole.Height),
                new Rectangle(hole.Right, hole.Top, r.Right-hole.Right, hole.Height)
            };
            for (int i = 0; i < rects.Count(); i++)
            {
                FillRectangle(batch, rects[i], layerDepth, color);
            }
        }
    }
}
