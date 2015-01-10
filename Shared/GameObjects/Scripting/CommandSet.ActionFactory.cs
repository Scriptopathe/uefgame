using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
namespace UeFGame.GameObjects.Scripting
{
    public partial class CommandSet
    {
        /// <summary>
        /// Bibliothèque d'actions.
        /// </summary>
        public static class ActionFactory
        {
            /// <summary>
            /// Action test
            /// </summary>
            /// <param name="param"></param>
            /// <returns></returns>
            public static Action Test(int param)
            {
                ActionDelegate del = delegate(GameEvent evt, CommandSetContext context)
                {
                    context.TempVars.Initialize("test_i", 0);
                    if ((int)context.TempVars["test_i"] <= 5)
                        evt.Body.ApplyLinearImpulse(new Vector2(0, -0.5f));
                    else if ((int)context.TempVars["test_i"] >= 20+param)
                    {
                        context.TempVars["test_i"] = 0;
                        return false;
                    }
                
                    context.TempVars["test_i"] = (int)context.TempVars["test_i"] + 1;
                    return true;
                };
                return new Action(del);
            }

            #region Message
            /// <summary>
            /// Affiche le message passé en paramètre.
            /// </summary>
            /// <param name="text"></param>
            /// <returns></returns>
            public static Action Message(string text)
            {
                ActionDelegate del = delegate(GameEvent evt, CommandSetContext context)
                {
                    if(Globals.GameMap.HUD.MessageComponent.CurrentMessage == null)
                        Globals.GameMap.HUD.MessageComponent.CurrentMessage = new GameComponents.HUDComponents.Message(text);

                    if (Globals.GameMap.HUD.MessageComponent.CurrentMessage.IsWritingTerminated)
                    {
                        Globals.GameMap.HUD.MessageComponent.CurrentMessage = null;
                        return false;
                    }
                    else
                        return true;
                };
                return new Action(del);
            }
            /// <summary>
            /// Patiente le nombre de frames indiqué.
            /// </summary>
            /// <returns></returns>
            public static Action Wait(int frames)
            {
                return new Action(delegate(GameEvent evt, CommandSetContext context)
                    {
                        context.TempVars.Initialize("wait_i", 0);
                        if ((int)context.TempVars["wait_i"] > frames)
                        {
                            context.TempVars["wait_i"] = 0;
                            return false;
                        }
                        context.TempVars["wait_i"] = (int)context.TempVars["wait_i"] + 1;
                        return true;
                    });
            }
            
            #endregion

            #region Get Object

            #endregion
        }
    }
}
