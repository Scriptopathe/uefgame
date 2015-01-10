using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UeFGame.GameComponents;
using Geex.Edit.Common.Project;
using System.Drawing;
using UeFGame.Ressources;
using System.Windows.Forms;
using FarseerPhysics.Common;
using Vector2 = Microsoft.Xna.Framework.Vector2;
namespace FileGenerator.Generators
{
    /// <summary>
    /// Permet de générer une map.
    /// </summary>
    public class MapGenerator
    {
        public List<MapInfo> MapInfos = UeFGame.Ressources.FileRessourceProvider.LoadMapInfos();
        public MapInitializingData Map;
        public string Name = "default";
        public MapGenerator()
        {
            Map = new MapInitializingData();
            
        }

        public void Generate(bool generateSnaphots)
        {
            int mapId = MapInfos.Count;
            MapInfo item = new MapInfo();
            item.Id = mapId;
            item.ParentId = -1;
            item.Name = Name;
            item.Order = 0;
            MapInfos.Add(item);
            UeFGame.Ressources.FileRessourceProvider.SaveMapInfos(MapInfos);
            string filename = UeFGame.Ressources.FileRessourceProvider.SaveMap(mapId, Map);
            Program.ContentWork.AddElement("RunTimeAssets\\Data\\Maps\\Map" + mapId.ToString().PadLeft(4, '0') + ".uefmap", "UeFMapImporter", "UeFMapProcessor");
            
            // Génère les snapshots
            if(generateSnaphots)
                GenerateSnapShot(mapId);
        }
        public void GeneratePolygonsSnapshot(int mapId, List<Vertices> polygons)
        {
            Bitmap bmp = new Bitmap((int)(Map.SizeInTiles.X * 32), (int)(Map.SizeInTiles.Y * 32));
            Bitmap texture = MapImporter.FileRessourceProvider.LoadTilesetTexture(MapImporter.FileRessourceProvider.LoadTileset(Map.TilesetId).TextureName);
            Graphics g = Graphics.FromImage(bmp);
            int z = 0;
            for (int x = 0; x < Map.TileIds[z].GetLength(0); x++)
            {
                for (int y = 0; y < Map.TileIds[z].GetLength(1); y++)
                {
                    int id = Map.TileIds[z][x, y];
                    Point srcPos = new Point((id % Tileset.TilesetWidthInTiles) * 32, (id / Tileset.TilesetWidthInTiles) * 32);
                    Point dstPos = new Point(x * 32, y * 32);
                    g.DrawImage(texture, new Rectangle(dstPos.X, dstPos.Y, 32, 32), new Rectangle(srcPos.X, srcPos.Y, 32, 32), GraphicsUnit.Pixel);

                }
            }

            // Dessin des polygones
            int Zoom = 100;
            Point center = Point.Empty;
            Pen p, p2;
            foreach (Vertices polygon in polygons)
            {
                //p = new Pen(System.Drawing.Color.FromArgb(i * 20 % 255, Math.Max(0, (255 - i * 20) % 255), 0), 3);
                p = new Pen(System.Drawing.Color.FromArgb(255, 0, 0), 1);
                p2 = new Pen(System.Drawing.Color.FromArgb(0, 255, 255), 1);
                Microsoft.Xna.Framework.Vector2 old = polygon.First();
                // Dessine lignes puis les points
                foreach (Microsoft.Xna.Framework.Vector2 vect in polygon)
                {
                    g.DrawLine(p2, new Point((int)(center.X + vect.X * Zoom), (int)(center.Y + vect.Y * Zoom)),
                                    new Point((int)(center.X + old.X * Zoom), (int)(center.Y + old.Y * Zoom)));
                    old = vect;
                }
                g.DrawLine(p2, new Point((int)(center.X + polygon.First().X * Zoom), (int)(center.Y + polygon.First().Y * Zoom)),
                    new Point((int)(center.X + old.X * Zoom), (int)(center.Y + old.Y * Zoom)));

                foreach (Microsoft.Xna.Framework.Vector2 vect in polygon)
                {
                    g.DrawEllipse(p, new Rectangle((int)(center.X + vect.X * Zoom) - 1, (int)(center.Y + vect.Y * Zoom) - 1, 2, 2));
                }
            }
            g.Flush();
            bmp.Save(FileRessourceProvider.ContentDir + "RunTimeAssets\\Data\\Maps\\Snapshot" + mapId.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
        }
        /// <summary>
        /// Génère un shapshot.
        /// </summary>
        /// <param name="mapid"></param>
        public void GenerateSnapShot(int mapid)
        {
            Bitmap bmp = new Bitmap((int)(Map.SizeInTiles.X * 32), (int)(Map.SizeInTiles.Y * 32));
            Bitmap texture = FileRessourceProvider.LoadTilesetTexture(FileRessourceProvider.LoadTileset(Map.TilesetId).TextureName);
            Graphics g = Graphics.FromImage(bmp);
            int z = 0;
            for (int x = 0; x < Map.TileIds[z].GetLength(0); x++)
            {
                for (int y = 0; y < Map.TileIds[z].GetLength(1); y++)
                {
                    int id = Map.TileIds[z][x, y];
                    Point srcPos = new Point((id % Tileset.TilesetWidthInTiles)*32, (id / Tileset.TilesetWidthInTiles)*32);
                    Point dstPos = new Point(x * 32, y * 32);
                    g.DrawImage(texture, new Rectangle(dstPos.X, dstPos.Y, 32, 32), new Rectangle(srcPos.X, srcPos.Y, 32, 32), GraphicsUnit.Pixel);
                    
                }
            }

            // Dessin des polygones
            int Zoom = 100;
            Point center = Point.Empty;
            Pen p, p2;
            foreach (Vertices polygon in Map.Polygons)
            {
                //p = new Pen(System.Drawing.Color.FromArgb(i * 20 % 255, Math.Max(0, (255 - i * 20) % 255), 0), 3);
                p = new Pen(System.Drawing.Color.FromArgb(255, 0, 0), 1);
                p2 = new Pen(System.Drawing.Color.FromArgb(0, 255, 255), 1);
                Microsoft.Xna.Framework.Vector2 old = polygon.First();
                // Dessine lignes puis les points
                foreach (Microsoft.Xna.Framework.Vector2 vect in polygon)
                {
                    g.DrawLine(p2, new Point((int)(center.X + vect.X * Zoom), (int)(center.Y + vect.Y * Zoom)),
                                    new Point((int)(center.X + old.X * Zoom), (int)(center.Y + old.Y * Zoom)));
                    old = vect;
                }
                g.DrawLine(p2, new Point((int)(center.X + polygon.First().X * Zoom), (int)(center.Y + polygon.First().Y * Zoom)),
                    new Point((int)(center.X + old.X * Zoom), (int)(center.Y + old.Y * Zoom)));

                foreach (Microsoft.Xna.Framework.Vector2 vect in polygon)
                {
                    g.DrawEllipse(p, new Rectangle((int)(center.X + vect.X * Zoom) - 1, (int)(center.Y + vect.Y * Zoom) - 1, 2, 2));
                }
            }
            g.Flush();
            bmp.Save(FileRessourceProvider.ContentDir + "RunTimeAssets\\Data\\Maps\\Snapshot" + mapid.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);

            return;
            // Snapshot du masque
            UeFGame.GameComponents.TilesetMask mask = new TilesetMask();
            mask.Raw = UeFGame.Tools.PolygonGenerator.GenerateMapMask(Map, UeFGame.Ressources.FileRessourceProvider.LoadTilesetMask(Map.TilesetId));
            bmp = new Bitmap(mask.Raw.GetLength(0), mask.Raw.GetLength(1));
            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    bmp.SetPixel(x, y, mask.Raw[x, y] ? Color.Black : Color.White);
                }
            }
            bmp.Save(FileRessourceProvider.ContentDir + "RunTimeAssets\\Data\\Maps\\Snapshot" + mapid.ToString() + "-Mask.png", System.Drawing.Imaging.ImageFormat.Png);

            // Snapshot du masque du tileset
            mask = UeFGame.Ressources.FileRessourceProvider.LoadTilesetMask(Map.TilesetId);
            bmp = new Bitmap(mask.Raw.GetLength(0), mask.Raw.GetLength(1));
            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    bmp.SetPixel(x, y, mask.Raw[x, y] ? Color.Black : Color.White);
                }
            }
            bmp.Save(FileRessourceProvider.ContentDir + "RunTimeAssets\\Data\\Maps\\Snapshot" + mapid.ToString() + "-TilesetMask.png", System.Drawing.Imaging.ImageFormat.Png);
            
        }
    }
}
