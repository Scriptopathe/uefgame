using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
namespace Geex.Edit.Common.MapView
{
    public delegate void ContentLoadedEventHandler();
    /// <summary>
    /// Main interface from a map view. Every map view class must implement it.
    /// </summary>
    public interface IMapView : IDisposable
    {
        /// <summary>
        /// Event which will be fired at the LoadContent method of the MapView.
        /// IE : use it to perform initialization that needs the device to be created.
        /// </summary>
        event ContentLoadedEventHandler ContentLoaded;
        /// <summary>
        /// Indicates if the view must be refreshed.
        /// </summary>
        bool IsDirty { get; set; }
        /// <summary>
        /// Gets the map currently being edited.
        /// </summary>
        object CurrentMapObject { get; }
        /// <summary>
        /// Refreshes the drawing of the map.
        /// It has to be called when the size of the map changed, or the zoom etc...
        /// </summary>
        void RefreshMap();
        /// <summary>
        /// Gets the Xna graphics device associated to the MapView.
        /// </summary>
        GraphicsDevice GraphicsDevice { get; }
    }
}
