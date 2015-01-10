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
    /// Class handling input
    /// </summary>
    public static class Input
    {
        #region Configuration
        public static Keys KJump = Keys.Up;
        public static Keys KRight = Keys.Right;
        public static Keys KLeft = Keys.Left;
        public static Keys KDown = Keys.Down;
        #endregion

        static KeyboardState s_lastFrameState;
        static KeyboardState s_thisState;
        /// <summary>
        /// Initialize the input.
        /// </summary>
        public static void ModuleInit()
        {
            s_lastFrameState = Keyboard.GetState();
            s_thisState = s_lastFrameState;
        }
        /// <summary>
        /// Updates the input.
        /// </summary>
        public static void Update()
        {
            s_lastFrameState = s_thisState;
            s_thisState = Keyboard.GetState();
        }
        /// <summary>
        /// Checks for a trigger.
        /// </summary>
        public static bool IsTrigger(Keys key)
        {
            return s_thisState.IsKeyDown(key) && !s_lastFrameState.IsKeyDown(key);
        }
        /// <summary>
        /// Checks if a key is pressed.
        /// </summary>
        public static bool IsPressed(Keys key)
        {
            return s_thisState.IsKeyDown(key);
        }
    }
}
