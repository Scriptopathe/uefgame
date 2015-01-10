using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Options for rendering such as :
/// - Zoom
/// - Make inactive layer transparent
/// - Hiding layers
/// - Showing/Hiding panoramas
/// - Tone change
/// etc...
/// </summary>
namespace Geex.Edit.UeF.Graphics
{
    /// <summary>
    /// Manages options for the GraphicsManager
    /// </summary>
    public class RenderOptions
    {
        #region Delegates / Events
        public delegate void ZoomChangedDelegate(float newValue);
        /// <summary>
        /// Event fired when the zoom changed.
        /// </summary>
        public event ZoomChangedDelegate ZoomChanged;
        #endregion
        /// <summary>
        /// Max zoom constant
        /// </summary>
        public const int MaxZoom = 3;
        /// <summary>
        /// This value is set to true if only the active layer and lower layers
        /// must be shown.
        /// </summary>
        public bool ShowActiveAndLowerOnly = false;
        /// <summary>
        /// If this value is set to true, the inactive layer will be grayed.
        /// </summary>
        public bool GreyInactiveLayers = true;
        /// <summary>
        /// This value is set to true if the event layer must be shown
        /// </summary>
        public bool ShowEventLayer = true;
        /// <summary>
        /// This value is set to true if the locked picture layer must be shown
        /// </summary>
        public bool ShowLockedPictures = true;
        /// <summary>
        /// Shows the grid if set to true
        /// </summary>
        public bool ShowGrid = true;
        /// <summary>
        /// Shows the panorama if set to true
        /// </summary>
        public bool ShowPanorama = true;
        /// <summary>
        /// Scrolls the panorama if set to true.
        /// </summary>
        public bool ScrollPanorama = true;
        /// <summary>
        /// Indicates the active layer
        /// </summary>
        public int ActiveLayer = 0;
        /// <summary>
        /// Indicates if the cursor must be displayed.
        /// </summary>
        public bool IsCursorVisible = true;
        /// <summary>
        /// Indicates if the given layer must be drawn
        /// </summary>
        /// <param name="z"></param>
        /// <returns></returns>
        public bool MustDrawLayer(int z)
        {
            return !(ShowActiveAndLowerOnly & z > ActiveLayer);
        }
        /// <summary>
        /// Indicates if the given layer must be greyed
        /// </summary>
        /// <param name="z"></param>
        /// <returns></returns>
        public bool MustGreyLayer(int z)
        {
            return (GreyInactiveLayers & ActiveLayer != z);//(GreyInactiveLayers & z > ActiveLayer);
        }
        float m_zoom = 0.5f;
        /// <summary>
        /// Gets or sets the Zoom.
        /// </summary>
        public float Zoom
        {
            get { return m_zoom; }
            set
            {
                m_zoom = value;
                if (ZoomChanged != null)
                    ZoomChanged(m_zoom);
            }
        }
    }
}
