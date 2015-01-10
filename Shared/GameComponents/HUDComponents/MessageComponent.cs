using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace UeFGame.GameComponents.HUDComponents
{
    /// <summary>
    /// Représente un message qui sera affiché par le MessageComponent.
    /// </summary>
    public class Message
    {
        #region Structs
        /// <summary>
        /// Représente un morceau de message.
        /// </summary>
        struct MessagePart
        {
            public int Count() { return Message.Count(); }
            public string Message;
            public MessagePart(string message)
            {
                Message = message;
            }
        }
        #endregion

        #region Variables
        /// <summary>
        /// Parties du message.
        /// </summary>
        List<MessagePart> m_parts;
        /// <summary>
        /// Index du charactère devant être dessiné.
        /// </summary>
        int m_currentChar;
        /// <summary>
        /// Compteur de frames.
        /// </summary>
        int m_counter;
        #endregion

        #region Properties
        /// <summary>
        /// Obtient une valeur indiquant si l'écriture du message est terminée.
        /// </summary>
        public bool IsWritingTerminated
        {
            get
            {
                int count = 0;
                foreach (MessagePart part in m_parts)
                {
                    count += part.Count();
                }
                return m_currentChar > count;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Crée un message à partir du string donné.
        /// </summary>
        /// <param name="msg"></param>
        public Message(string msg)
        {
            m_currentChar = 0;
            m_parts = new List<MessagePart>();
            m_parts.AddRange(msg.Split('\n').SelectMany((string str) => new List<MessagePart>() { new MessagePart(str), new MessagePart("\n"), }));
        }
        /// <summary>
        /// Va à la fin du message.
        /// </summary>
        public void GoToEnd()
        {
           int count = 0;
           foreach (MessagePart part in m_parts)
           {
               count += part.Count();
           }
           m_currentChar = count;
        }
        /// <summary>
        /// Dessine le texte de ce message.
        /// </summary>
        /// <param name="batch"></param>
        public void Draw(SpriteBatch batch, Point point, SpriteFont font)
        {
            int partId = 0;
            int startPartId = 0;
            float x = point.X;
            float y = point.Y;

            int finishedPartId = 0;
            // On dessine toutes les parties finies
            int lastFinishedChar = 0;
            for (int i = 0; i < m_currentChar; i++)
            {
                if (i - startPartId >= m_parts[finishedPartId].Count())
                {
                    finishedPartId++;
                    startPartId = i;
                    lastFinishedChar = i;
                }
            }
            
            // finishedPartId est la partie qui doit être complétée.
            for (int i = 0; i < finishedPartId; i++)
            {
                string text = m_parts[i].Message;
                if (text == "\n")
                {
                    y += 25;
                    x = point.X;
                }
                else
                {
                    batch.DrawString(font, text, new Vector2(x, y), Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                    x += font.MeasureString(text).X;
                }
            }
            
            // Ici : lastFinishedCHAR
            for (int i = 0; i < m_currentChar - lastFinishedChar; i++)
            {
                string text = m_parts[finishedPartId].Message[i].ToString();
                if (text == "\n")
                {
                    y += 25;
                    x = point.X;
                }
                else
                {
                    batch.DrawString(font, text, new Vector2(x, y), Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                    x += font.MeasureString(text).X;
                }
            }

            if(!IsWritingTerminated && m_counter % 2 == 0)
                m_currentChar++;
            m_counter++;
        }
        #endregion
    }
    /// <summary>
    /// Représente un composant de message.
    /// </summary>
    public class MessageComponent : HUDComponent
    {
        #region Variables
        /// <summary>
        /// Référence vers la police utilisée pour dessiner les messages.
        /// </summary>
        SpriteFont m_font;
        #endregion

        #region Properties
        /// <summary>
        /// Représente le message actuellement affiché par le HUD.
        /// </summary>
        public Message CurrentMessage { get; set; }

        #endregion
        /// <summary>
        /// Crée une nouvelle instance de MessageComponent.
        /// </summary>
        public MessageComponent()
        {
            m_font = Globals.Content.Load<SpriteFont>("Fonts\\showcard_gothic");
        }
        /// <summary>
        /// Dessine le message.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="batch"></param>
        public override void Draw(GameTime time, SpriteBatch batch)
        {
            if(CurrentMessage != null)
                CurrentMessage.Draw(batch, new Point(0, 0), m_font);
        }
    }
}
