using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TMap = UeFGame.GameComponents.MapInitializingData;
using TProperties = System.Object;
using RpgMapWork = Geex.Edit.UeF.Project.UeFMapWork;
namespace Geex.Edit.UeF.MainForm
{
    /// <summary>
    /// UeF implementation of the map tree view.
    /// TODO : implement the events from the MapTreeView class.
    /// </summary>
    public class MapTreeView : Common.Tools.Controls.MapTreeView
    {
        #region Properties override
        /// <summary>
        /// Gets the full list of map infos of the project.
        /// </summary>
        public override List<Common.Project.MapInfo> MapInfos
        {
            get
            {
                Project.UeFProjectDataBase db = ((Project.UeFGeexProject)Common.Globals.Project).Database;
                return db.MapInfos;
            }
        }
        #endregion

        /// <summary>
        /// Constructor of the map tree view.
        /// </summary>
        public MapTreeView()
            : base()
        {
            this.OnCreateMap = new CreateMapDelegate(OnCreateUeFMap);
            this.OnResizeMap = new ResizeMapDelegate(OnResizeUeFMap);
            this.OnProperties = new PropertiesDelegate(OnUeFMapProperties);
            this.OnSaveMap = new SaveMapDelegate(OnSaveUeFMap);
            this.OnAddMapToProject = new AddMapToProjectDelegate(UeFOnAddMapToProject);
            this.OnRemoveMapFromProject = new RemoveMapFromProjectDelegate(UeFOnRemoveMapFromProject);
        }
        /// <summary>
        /// Called when a map is added to the project.
        /// Adds it into the Content project.
        /// </summary>
        /// <param name="filename"></param>
        protected void UeFOnAddMapToProject(string path, Common.Project.MapInfo info)
        {
            string dir = UeFGlobals.Project.MapDirectoryName.Replace(UeFGlobals.Project.ContentDirectory + "\\", "");
            string file = System.IO.Path.GetFileName(path);
            // AddElement("RunTimeAssets\\Data\\Maps\\Map" + mapId.ToString().PadLeft(4, '0') + ".uefmap", "UeFMapImporter", "UeFMapProcessor");
            UeFGlobals.ContentWork.AddElement(dir + "\\" + file, "UeFMapImporter", "UeFMapProcessor", true, false);
        }

        /// <summary>
        /// Called when a map is removed from the project.
        /// </summary>
        /// <param name="path"></param>
        protected void UeFOnRemoveMapFromProject(string path, Common.Project.MapInfo info)
        {
            string dir = UeFGlobals.Project.MapDirectoryName.Replace(UeFGlobals.Project.ContentDirectory + "\\", "");
            string file = System.IO.Path.GetFileName(path);
            UeFGlobals.Project.RemoveUnsavedMap(info.Id);
            try
            {
                UeFGlobals.ContentWork.DeleteElement(dir + "\\" + file);
            }
            catch (InvalidOperationException) { } // element does not exist
        }
        /// <summary>
        /// Saves the map.
        /// </summary>
        protected void OnSaveUeFMap(object mapObj, string filename)
        {
            TMap map = (TMap)mapObj;
            RpgMapWork.Save(map, filename);
        }
        /// <summary>
        /// Creates and returns a new RPG map.
        /// </summary>
        /// <returns></returns>
        protected TMap OnCreateUeFMap()
        {
            return RpgMapWork.CreateNewMap();
        }
        /// <summary>
        /// Resizes the given rpg map.
        /// </summary>
        protected void OnResizeUeFMap(object map, int width, int height)
        {
            RpgMapWork.ResizeMap((TMap)map, (short)width, (short)height);
        }
        /// <summary>
        /// Displays the properties form for the given map.
        /// </summary>
        /// <param name="map"></param>
        protected void OnUeFMapProperties(Common.Tools.Controls.MapTreeNode node)
        {   
            TMap map;
            try
            {
                map = (TMap)Common.Globals.Project.GetMapObject(node.MapInfo.Id);
            }
            catch { return; }

            Views.MapPropertiesForm form = new Views.MapPropertiesForm();
            form.Map = map;
            form.MapInfo = node.MapInfo;
            form.StartPosition = FormStartPosition.Manual;
            form.Location = new System.Drawing.Point(Common.Globals.MainForm.Location.X + 20, Common.Globals.MainForm.Location.Y + 150);
            form.Initialize();
            if (form.ShowDialog() == DialogResult.OK)
            {
                node.MapInfo.Name = form.NewName;
                if ((int)map.SizeInTiles.X != form.NewWidth || (int)map.SizeInTiles.Y != form.NewHeight)
                {
                    UeF.Project.UeFMapWork.ResizeMap(map, form.NewWidth, form.NewHeight);
                    if (map == UeFGlobals.MapView.CurrentMap)
                        UeFGlobals.MapView.RefreshMap();
                    UeFGlobals.MapView.IsDirty = true;
                    
                }
                if (map.TilesetId != form.NewTilesetId)
                {
                    map.TilesetId = form.NewTilesetId;
                    UeFGlobals.MapView.RefreshMap();
                    UeFGlobals.TilePicker.OnTilesetChanged();
                }
                node.Text = form.NewName;
            }
            // Catches the result of 
            /*
            Views.PropertiesForm f = new Views.PropertiesForm(map, node.MapInfo);
            f.StartPosition = FormStartPosition.Manual;
            f.Location = new System.Drawing.Point(Common.Globals.MainForm.Location.X+20, Common.Globals.MainForm.Location.Y+150);
            DialogResult res = f.ShowDialog();
            if (res == DialogResult.OK)
            {
                node.MapInfo.Name = f.NewMapName;
                node.Text = f.NewMapName;
                RpgMapWork.CopyMetadataFrom(map, f.NewMap);
                RpgMapWork.ResizeMap(map, f.NewMap.Width, f.NewMap.Height);
                if(map == RpgGlobals.RpgMapView.CurrentMap)
                    RpgGlobals.RpgMapView.RefreshMap();
                RpgGlobals.RpgMapView.IsDirty = true;
            }*/
        }
    }
}
