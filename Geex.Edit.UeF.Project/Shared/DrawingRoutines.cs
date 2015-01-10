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
        public static void InitTextures(ContentManager mgr)
        {
            m_lineTex = mgr.Load<Texture2D>("Editor\\lineTex");
            m_squareTex = mgr.Load<Texture2D>("Editor\\squareTex");
            m_circleTex = mgr.Load<Texture2D>("Editor\\circleTex");
        }
        public static void DrawLine(Vector2 p1, Vector2 p2)
        {

        }
        public static void DrawRectangle(Rectangle r)
        {

        }
        public static void DrawCircle(Rectangle r)
        {

        }
    }
}
