using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TMap = System.Object;
using TProperties = System.Object;
using MapInfo = Geex.Edit.Common.Project.MapInfo;
namespace Geex.Edit.Common.Tools.Controls
{

    /// <summary>
    /// Tree displaying the maps
    /// </summary>
    public class MapTreeView : TreeView
    {
        /* -------------------------------------------------------------------------
         * Events
         * ------------------------------------------------------------------------*/
        #region Events
        /// <summary>
        /// Delegate for the MapSelected event
        /// </summary>
        public delegate void MapSelectedDelegate(MapInfo mapInfo);
        /// <summary>
        /// This event is fired when a map is selected.
        /// </summary>
        public event MapSelectedDelegate MapSelected;

        #region Specialization delegates
        /// <summary>
        /// Delegate used for creating empty maps.
        /// Returns a new map.
        /// </summary>
        public delegate TMap CreateMapDelegate();
        /// <summary>
        /// Delegate used for creating empty maps.
        /// Returns a new map.
        /// </summary>
        public CreateMapDelegate OnCreateMap;
        /// <summary>
        /// Delegate used for resizing maps.
        /// Resizes the given map, to the given size.
        /// </summary>
        public delegate void ResizeMapDelegate(TMap map, int width, int height);
        /// <summary>
        /// Delegate used for resizing maps.
        /// Resizes the given map, to the given size.
        /// </summary>
        public ResizeMapDelegate OnResizeMap;
        /// <summary>
        /// Delegate called when the properties button of the contextual menu
        /// is clicked.
        /// The node clicked is given as argument.
        /// </summary>
        /// <param name="map"></param>
        public delegate void PropertiesDelegate(MapTreeNode node);
        /// <summary>
        /// Delegate called when the properties button of the contextual menu
        /// is clicked.
        /// The node clicked is given as argument.
        /// </summary>
        public PropertiesDelegate OnProperties;
        /// <summary>
        /// Delegate called to save the given map.
        /// </summary>
        public delegate void SaveMapDelegate(TMap map, string filename);
        /// <summary>
        /// Delegate called to save the given map.
        /// </summary>
        public SaveMapDelegate OnSaveMap;
        /// <summary>
        /// Delegate type of OnAddMapToProject.
        /// </summary>
        public delegate void AddMapToProjectDelegate(string filename, MapInfo info);
        /// <summary>
        /// Called when a map is added to the project.
        /// The filename is given as argument.
        /// </summary>
        public AddMapToProjectDelegate OnAddMapToProject;
        /// <summary>
        /// Delegate type of OnRemoveMapFromProject
        /// </summary>
        /// <param name="filename"></param>
        public delegate void RemoveMapFromProjectDelegate(string filename, MapInfo info);
        /// <summary>
        /// Called when a map is removed from the project.
        /// </summary>
        public RemoveMapFromProjectDelegate OnRemoveMapFromProject;
        #endregion
        #endregion
        /* -------------------------------------------------------------------------
         * Variables
         * ------------------------------------------------------------------------*/
        #region Variables
        ImageList m_imageList = new ImageList();
        ImageList m_toolStripImageList = new ImageList();

        MapTreeViewClipboard m_clipboard = new MapTreeViewClipboard();
        #endregion
        /* -------------------------------------------------------------------------
         * Properties
         * ------------------------------------------------------------------------*/
        #region Properties
        /// <summary>
        /// Shortcut to the map infos.
        /// Override it in the subclass.
        /// </summary>
        public virtual List<Project.MapInfo> MapInfos
        {
            get { throw new NotImplementedException("This property must be overriden"); }
        }
        #endregion
        /* -------------------------------------------------------------------------
         * Methods
         * ------------------------------------------------------------------------*/
        #region Methods
        /// <summary>
        /// Constructor
        /// </summary>
        public MapTreeView()
            : base()
        {
            SetupImageList();
            SetupContextMenuImageList();
            SetupTree();
            this.NodeMouseClick += new TreeNodeMouseClickEventHandler(OnNodeMouseClick);
            this.MouseDown += new MouseEventHandler(OnMouseDown);
            
            /*this.ItemDrag += new ItemDragEventHandler(MapTreeView_ItemDrag);
            this.DragDrop += new DragEventHandler(MapTreeView_DragDrop);
            this.DragOver += new DragEventHandler(MapTreeView_DragOver);
            this.DragEnter += new DragEventHandler(MapTreeView_DragEnter);*/
            
        }

        #region Drad and Drop (marche po)

        void MapTreeView_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy | DragDropEffects.Move;
        }

        void MapTreeView_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        void MapTreeView_DragDrop(object sender, DragEventArgs e)
        {
            MapTreeNode node = (MapTreeNode)this.GetNodeAt(e.X, e.Y);
            PasteMap(node);
            
        }

        void MapTreeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            MapTreeNode node = (MapTreeNode)e.Item;
            CopyMap(node, false);
            DoDragDrop(node, DragDropEffects.Copy | DragDropEffects.Move);
            
        }
        #endregion

        /// <summary>
        /// Used to display a context menu when the user right clicks a node, before the release of the
        /// mouse.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                TreeNode node1 = this.GetNodeAt(new System.Drawing.Point(e.X, e.Y));
                MapTreeNode node = ((MapTreeNode)node1);
                if (node == null)
                    return;
                this.SelectedNode = node1;
                if (node.MapInfo.Id != -1)
                    MapSelected(node.MapInfo);              
                OnNodeRightClick(node, e.X, e.Y);
            }
            
        }
        /// <summary>
        /// Called when the mouse is clicked on a node.
        /// Used to process right clicks for menus, and opening of maps.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnNodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                MapInfo mapInfo = ((MapTreeNode)e.Node).MapInfo;
                // If the project root is clicked
                if (IsRoot((MapTreeNode)e.Node))
                    return;
                if (FileExists(mapInfo.Id))
                    if (MapSelected != null)
                        MapSelected(mapInfo);
            }
        }
        /// <summary>
        /// Called when a node is right-clicked.
        /// Displays a context menu.
        /// </summary>
        /// <param name="node"></param>
        void OnNodeRightClick(MapTreeNode node, int x, int y)
        {
            if (node == null) return;
            // Menu strip
            ContextMenuStrip strip = new ContextMenuStrip();
            strip.ImageList = m_toolStripImageList;
            // --- Items
            // Properties map
            ToolStripButton properties = new ToolStripButton(Lang.I["MapTreeView_ContextMenu_MapProperties"]);
            properties.ImageIndex = 4;
            strip.Items.Add(properties);
            // Separator
            strip.Items.Add(new ToolStripSeparator());
            // New map
            ToolStripButton newMap = new ToolStripButton(Lang.I["MapTreeView_ContextMenu_NewMap"]);
            newMap.ImageIndex = 0;
            strip.Items.Add(newMap);
            // Delete map
            ToolStripButton delMap = new ToolStripButton(Lang.I["MapTreeView_ContextMenu_DeleteMap"]);
            delMap.ImageIndex = 1;
            strip.Items.Add(delMap);

            // Copy map
            ToolStripButton copyMap = new ToolStripButton(Lang.I["MapTreeView_ContextMenu_CopyMap"]);
            copyMap.ImageIndex = 2;
            strip.Items.Add(copyMap);
            // Cut map
            ToolStripButton cutMap = new ToolStripButton(Lang.I["MapTreeView_ContextMenu_CutMap"]);
            cutMap.ImageIndex = 3;
            strip.Items.Add(cutMap);
            // Paste map
            ToolStripButton pasteMap = new ToolStripButton(Lang.I["MapTreeView_ContextMenu_PasteMap"]);
            pasteMap.ImageIndex = 5;
            strip.Items.Add(pasteMap);

            // Disables some options
            if (IsRoot(node)) // root
            {
                properties.Enabled = false;
                copyMap.Enabled = false;
                cutMap.Enabled = false;
                delMap.Enabled = false;
            }
            else if (FileExists(node.MapInfo.Id) == false)
            {
                copyMap.Enabled = false;
                cutMap.Enabled = false;
            }
            if (!CanPasteTo(node))
                pasteMap.Enabled = false;
            newMap.Click += delegate(object sender, EventArgs e)
            {
                AddNewMap(node);
            };

            delMap.Click += delegate(object sender, EventArgs e)
            {
                DeleteMapAndChildren(node);
            };

            copyMap.Click += delegate(object sender, EventArgs e)
            {
                CopyMap(node, false);
            };

            cutMap.Click += delegate(object sender, EventArgs e)
            {
                CopyMap(node, true);
            };

            pasteMap.Click += delegate(object sender, EventArgs e)
            {
                PasteMap(node);
            };

            properties.Click += delegate(object sender, EventArgs e)
            {
                MapProperties(node);
            };
            AddMenus(strip, node, x, y);
            strip.Show(this, new System.Drawing.Point(x, y));
        }
        /// <summary>
        /// Override it to add some menus.
        /// </summary>
        protected virtual void AddMenus(ContextMenuStrip strip, MapTreeNode node, int x, int y)
        {

        }
        /// <summary>
        /// Sets up the image list
        /// </summary>
        void SetupImageList()
        {
            m_imageList.Images.Add(AppRessources.RessourceSystemBitmap("map-node.png"));
            m_imageList.Images.Add(AppRessources.RessourceSystemBitmap("map-node-folder.png"));
            m_imageList.Images.Add(AppRessources.RessourceSystemBitmap("map-node-warning.png"));
            m_imageList.Images.Add(AppRessources.RessourceSystemBitmap("map-node-main.png"));
            this.ImageList = m_imageList;
        }
        /// <summary>
        /// Sets up the image list of the context menu
        /// </summary>
        void SetupContextMenuImageList()
        {
            m_toolStripImageList = new ImageList();
            m_toolStripImageList.Images.Add(AppRessources.RessourceSystemBitmap("new.png")); // 0
            m_toolStripImageList.Images.Add(AppRessources.RessourceSystemBitmap("delete.png"));
            m_toolStripImageList.Images.Add(AppRessources.RessourceSystemBitmap("copy.png"));
            m_toolStripImageList.Images.Add(AppRessources.RessourceSystemBitmap("cut.png"));
            m_toolStripImageList.Images.Add(AppRessources.RessourceSystemBitmap("properties.gif"));
            m_toolStripImageList.Images.Add(AppRessources.RessourceSystemBitmap("paste.png")); // 5
        }
        /// <summary>
        /// Sets up the map tree.
        /// </summary>
        public void SetupTree()
        {
            this.BeginUpdate();

            this.Nodes.Clear();
            
            // Adds the main parent node
            MapTreeNode node = new MapTreeNode(Lang.I["MapTreeView_ProjectRootName"]);
            node.MapInfo = new MapInfo();
            node.MapInfo.Id = -1;
            node.ImageIndex = 3;
            node.SelectedImageIndex = 3;
            this.Nodes.Add(node);
            AddChildrenFromId(-1, node.Nodes);
            RestoreExpandNode(node.Nodes);
            node.Expand();
            this.EndUpdate();
        }
        /// <summary>
        /// Adds the children of the given map info id to the tree
        /// </summary>
        /// <returns></returns>
        public void AddChildrenFromId(int id, TreeNodeCollection ParentNodes)
        {
            // Creating sorted entries
            SortedDictionary<int, MapInfo> sortedInfos = new SortedDictionary<int, MapInfo>();
            foreach(MapInfo mapInfo in MapInfos)
            {
                if (mapInfo.ParentId == id)
                {
                    // Finding the good order (i.e avoid doubles)
                    int order = mapInfo.Order;
                    if(sortedInfos.ContainsKey(order))
                        while (sortedInfos.ContainsKey(order))
                            order++;
                    sortedInfos[order] = mapInfo;
                }
            }
            foreach (KeyValuePair<int, MapInfo> kvp in sortedInfos)
            {
                MapTreeNode node = new MapTreeNode(kvp.Value.Name);
                node.MapInfo = kvp.Value;
                ParentNodes.Add(node);
                AddChildrenFromId(kvp.Value.Id, node.Nodes);
                RestoreExpandNode(node.Nodes);
                if (FileExists(kvp.Value.Id))
                    if (node.Nodes.Count != 0)
                    {
                        node.ImageIndex = 1; // folder
                        node.SelectedImageIndex = 1;
                    }
                    else
                    {
                        node.ImageIndex = 0; // map - no folder
                        node.SelectedImageIndex = 0;
                    }
                else
                {
                    node.ImageIndex = 2; // warning : map does not exist
                    node.SelectedImageIndex = 2;
                }
            }
        }
        /// <summary>
        /// Restore the "expand" values of the nodes.
        /// </summary>
        public void RestoreExpandNode(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                MapTreeNode mapNode = ((MapTreeNode)node);
                if (mapNode.MapInfo.Expanded)
                    mapNode.Expand();
            }
        }
        #endregion
        /* -------------------------------------------------------------------------
         * Operations on the maps
         * ------------------------------------------------------------------------*/
        #region Operations
        /// <summary>
        /// Adds a new map, child of the given parentNode.
        /// </summary>
        void AddNewMap(MapTreeNode parentNode)
        {
            TMap map = OnCreateMap();
            AddMap(map, parentNode);
        }
        /// <summary>
        /// Adds a given map, child of the given parentNode.
        /// </summary>
        void AddMap(TMap map, MapTreeNode parentNode)
        {
            // Set up of the map info
            MapInfo mapInfo = new MapInfo();
            mapInfo.Id = FirstAvailableId();
            mapInfo.Order = LastOrder(parentNode);
            mapInfo.Name = "New map";
            mapInfo.ParentId = parentNode.MapInfo.Id;
            // Updating the information
            MapInfos.Add(mapInfo);
            // Saving the map
            OnSaveMap(map, Common.Globals.Project.MapFileName(mapInfo.Id));
            OnAddMapToProject(Common.Globals.Project.MapFileName(mapInfo.Id), mapInfo);
            // Adding the node
            MapTreeNode node = new MapTreeNode(mapInfo.Name);
            node.MapInfo = mapInfo;
            node.ImageIndex = 0;
            node.SelectedImageIndex = 0;
            parentNode.Nodes.Add(node);
        }
        /// <summary>
        /// Deletes a map and its children : in map infos, tree, and the files themselves.
        /// </summary>
        void DeleteMapAndChildren(MapTreeNode deletedNode)
        {
            // First Removes the child nodes
            DeleteChildren(deletedNode);

            // Then removes the parent node
            MapTreeNode parentNode = ((MapTreeNode)deletedNode.Parent);
            parentNode.Nodes.Remove(deletedNode);
            MapInfos.Remove(deletedNode.MapInfo);

            // Calls the specialization delegate
            OnRemoveMapFromProject(Common.Globals.Project.MapFileName(deletedNode.MapInfo.Id), deletedNode.MapInfo);
            // Removes the file
            if (System.IO.File.Exists(Common.Globals.Project.MapFileName(deletedNode.MapInfo.Id)))
                try { System.IO.File.Delete(Common.Globals.Project.MapFileName(deletedNode.MapInfo.Id)); }
                catch { }
                
        }
        /// <summary>
        /// Delete the children of a map, calling DeleteMapAndChildren for each of them.
        /// </summary>
        /// <param name="node"></param>
        void DeleteChildren(MapTreeNode node)
        {
            // We register the count because it will be changed each iteration.
            int count = node.Nodes.Count;
            for (int i = 0; i < count; i++)
            {
                MapTreeNode mapNode = (MapTreeNode)node.Nodes[0];
                if (mapNode != null)
                    DeleteMapAndChildren(mapNode);
            }
        }
        /// <summary>
        /// Copies the given map into the clipboard
        /// If cut is true, the map will be cut.
        /// </summary>
        void CopyMap(MapTreeNode node, bool cut)
        {
            // Gets the unsaved version of the map if it exists in memory
            TMap map = (TMap)Globals.Project.GetMapObject(node.MapInfo.Id);
            if (map == null)
                return;
            
            m_clipboard.Map = map;
            m_clipboard.MapInfo = node.MapInfo;
            m_clipboard.IsCut = cut;
            m_clipboard.SrcNode = node;
        }
        /// <summary>
        /// Pastes the copied/cut map into the destNode.
        /// </summary>
        void PasteMap(MapTreeNode destNode)
        {
            if (!CanPasteTo(destNode)) throw new ArgumentException("Can't paste to this node");

            if (!m_clipboard.IsCut)
            {
                // Simple map copy
                AddMap(m_clipboard.Map, destNode);
            }
            else
            {
                // We are going to move the entire portion of tree (children etc...)

                // Changing the parent id of the cut map :
                m_clipboard.SrcNode.MapInfo.ParentId = destNode.MapInfo.Id; ;
                // Updating the tree
                m_clipboard.SrcNode.Parent.Nodes.Remove(m_clipboard.SrcNode);
                destNode.Nodes.Add(m_clipboard.SrcNode);
                m_clipboard.SrcNode.MapInfo.Order = LastOrder(destNode);
                // Disabling the cutting, and nulls the clipboard
                m_clipboard.Reset();
            }
        }
        /// <summary>
        /// Opens the map's properties
        /// </summary>
        void MapProperties(MapTreeNode node)
        {
            // Calls the specialized delegate
            OnProperties(node);
        }
        /// <summary>
        /// Returns true if we are able to paste to the given node.
        /// </summary>
        bool CanPasteTo(MapTreeNode destNode)
        {
            if (m_clipboard.Map == null || m_clipboard.SrcNode == null)
                return false;
            // If we are cutting, we must check that we are not pasting in a child
            if (m_clipboard.IsCut)
            {
                MapTreeNode node = destNode;
                while (node.Parent != null)
                {
                    if (node.MapInfo.Id == m_clipboard.SrcNode.MapInfo.Id)
                        return false;
                    node = (MapTreeNode)node.Parent;
                }
            }
            return true;
        }
        #endregion
        /* -------------------------------------------------------------------------
         * Entries Management
         * ------------------------------------------------------------------------*/
        #region Entries Management
        /// <summary>
        /// Returns true if the file for the given map id exists.
        /// </summary>
        /// <param name="id"></param>
        public bool FileExists(int id)
        {
            return System.IO.File.Exists(Common.Globals.Project.MapFileName(id));
        }
        /// <summary>
        /// Returns the first available id
        /// </summary>
        int FirstAvailableId()
        {
            return MapInfos.Count;
        }
        /// <summary>
        /// Returns the last order within the parent node
        /// </summary>
        int LastOrder(MapTreeNode parentNode)
        {
            int order = 0;
            foreach (MapTreeNode inf in parentNode.Nodes)
            {
                order = Math.Max(inf.MapInfo.Order, order);
            }
            return order;
        }
        /// <summary>
        /// Returns true if the given node is the root.
        /// </summary>
        bool IsRoot(MapTreeNode node)
        {
            return node.MapInfo.Id == -1;
        }
        #endregion
    }
    /* -------------------------------------------------------------------------
     * OTHER CLASSES
     * ------------------------------------------------------------------------*/
    #region Other classes
    #region MapTreeViewClipboard
    class MapTreeViewClipboard
    {
        #region Variables / Properties
        public bool IsCut { get; set; }
        public MapTreeNode SrcNode { get; set; }
        public MapInfo MapInfo { get; set; }
        public TMap Map { get; set; }
        #endregion
        /// <summary>
        /// Constructor. Does nothing.
        /// </summary>
        public MapTreeViewClipboard() { }
        /// <summary>
        /// Resets the clipboard
        /// </summary>
        public void Reset()
        {
            IsCut = false;
            SrcNode = null;
            MapInfo = null;
            Map = null;
        }
    }
    #endregion

    #region MapTreeNode
    /// <summary>
    /// Map Tree Node class
    /// </summary>
    public class MapTreeNode : TreeNode
    {
        public MapInfo MapInfo
        {
            get;
            set;
        }
        public MapTreeNode(string filename)
            : base(filename)
        {

        }
    }
    #endregion
    #endregion
}