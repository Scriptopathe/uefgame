using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Geex.Edit.UeF.Views
{
    public class GraphicsParserOptionsRpg : Common.Tools.Controls.GraphicsParserOptions
    {
        /// <summary>
        /// Presets for the GPRpgOptions
        /// </summary>
        [Flags()]
        public enum Preset
        {
            NoOptions = 0x00,
            Saturation = 0x01,
        }
        Preset m_preset;
        /// <summary>
        /// Creates a new GraphicsParserRpgOptions instance, whose basic parameters are determined by
        /// the given preset.
        /// </summary>
        /// <param name="preset"></param>
        public GraphicsParserOptionsRpg(Preset preset)
        {
            m_preset = preset;
            Values = new Dictionary<string, object>();
            if ((preset | Preset.Saturation) == preset)
            {
                Values.Add("saturation", 100);
            }
        }
        // Frames cut
        #region Frames Cut
        Size m_framesCut = new Size(1, 1);
        /// <summary>
        /// See <see cref="Common.Tools.Controls.GraphicsParserOptions"/>
        /// </summary>
        public void SetFramesCut(Size sz)
        {
            m_framesCut = sz;
        }
        /// <summary>
        /// See <see cref="Common.Tools.Controls.GraphicsParserOptions"/>
        /// </summary>
        public override Size GetFramesCut(Bitmap bmp)
        {
            return m_framesCut;
        }
        #endregion
        
        #region SpecialProperties
        /// <summary>
        /// See <see cref="Common.Tools.Controls.GraphicsParserOptions"/>
        /// </summary>
        public override Dictionary<string, object> Values
        {
            get;
            set;
        }
        #endregion
    }
    /// <summary>
    /// Graphics Parser specific to the RPG Projects.
    /// </summary>
    public partial class GraphicsParser : Common.Tools.Controls.GraphicsParser
    {
        #region Properties
        ValidityPredicateDelegate m_validityPredicate;
        /// <summary>
        /// Content work used to display the items
        /// This property must be overriden to return the
        /// good content work.
        /// </summary>
        public override Common.Project.ContentWork ContentWork
        {
            get { return UeFGlobals.ContentWork; }
        }
        /// <summary>
        /// Delegate used to get the validity of a file.
        /// This property must be overriden to return a correct
        /// delegate.
        /// </summary>
        public override ValidityPredicateDelegate ValidityPredicate
        {
            get { return m_validityPredicate; }
            set { m_validityPredicate = value;}
        }
        #endregion
        /* --------------------------------------------------------------
         * Methods
         * ------------------------------------------------------------*/
        #region Methods
        /// <summary>
        /// Creates a new instance of GraphicsParser.
        /// </summary>
        public GraphicsParser(string directory)
            : base(directory)
        {
            m_validityPredicate = new ValidityPredicateDelegate(Project.GameRessources.GetGraphicsFileValidity);
            UpdateFilesList();
        }
        #endregion
    }
}