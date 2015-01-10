using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileGenerator.Generators;
using UeFGame.GameComponents;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
namespace FileGenerator
{
    class Program
    {
        public static Geex.Edit.Common.Project.ContentWork ContentWork;
        static void InitializeContentWork()
        {
            ContentWork = new Geex.Edit.Common.Project.ContentWork("..\\..\\..\\UsineEnFolie\\UsineEnFolie.csproj");
        }

        static void Main(string[] args)
        {
            InitializeContentWork();
            // Utilisé pour le timing
            DateTime start, end;

            Console.WriteLine("-- Test generation --");

            // ------------------------------------------
            // Génération database
            MapImporter.GameDatabase.Load();
            
            MapGenerator gen = new MapGenerator();
            string folder = "..\\..\\..\\UsineEnFolieContent\\RunTimeAssets\\Data\\Maps\\";
            gen.Map = UeFGame.Tools.Serializer.Deserialize<MapInitializingData>(folder + "Map0004.uefmap");

            // Test rectangles
            List<Rectangle> clipRects = new List<Rectangle>();
            int rects = (int)(gen.Map.SizeInTiles.X / 20);
            int sx = (int)(gen.Map.SizeInTiles.X / rects);
            int sy = (int)(gen.Map.SizeInTiles.Y / rects);
            for (int x = 0; x < rects; x++)
            {
                for (int y = 0; y < rects; y++)
                {
                    clipRects.Add(new Rectangle(x * sx, y * sy, sx, sy));
                }
            }

            gen.GeneratePolygonsSnapshot(4, UeFGame.Tools.PolygonGenerator.GeneratePolygonsFromMap(gen.Map, clipRects,
                UeFGame.Tools.OptimizationMode.Ultimate));
            /*Console.WriteLine("-- Database loaded --");
            // ------------------------------------------

            // ------------------------------------------
            // Genère un tileset de base.
            TilesetGenerator gen = new TilesetGenerator();
            gen.Tilesets.Clear();
            Tileset tileset = new Tileset();
            tileset.Name = "Test00";
            tileset.TextureName = "usine0001";
            gen.Tilesets.Add(tileset);
            Console.WriteLine("-- Tileset generation... --");
            gen.Generate();
            Console.WriteLine("---- Done.");
            // ------------------------------------------

            // ------------------------------------------
            // Génère un masque pour ce tileset
            Console.WriteLine("-- Mask generation... --"); start = DateTime.Now;
            MaskGenerator gen2 = new MaskGenerator();
            gen2.Mask = UeFGame.Tools.PolygonGenerator.GenerateMaskFromTexture(UeFGame.Ressources.FileRessourceProvider.LoadTilesetTexture(tileset.TextureName));
            gen2.TilesetId = 0;
            gen2.Generate();

            end = DateTime.Now;
            Console.WriteLine("---- Done. (" + (end-start).TotalMilliseconds.ToString() + "ms)");
            // ------------------------------------------

            // ------------------------------------------
            // Génère une map à partir de tout ce bordel.
            MapGenerator mapgen = new MapGenerator();
            mapgen.MapInfos.Clear();
            mapgen.Map = new MapInitializingData();
            mapgen.Map.TilesetId = 0;
            mapgen.Name = "testmap";
            mapgen.Map.SizeInTiles = new Microsoft.Xna.Framework.Vector2(200, 200);
            mapgen.Map.Version = 1;
            int z = 0;
            Random rand = new Random();
            // ------------------------------------------

            // ------------------------------------------
            // Génère le tiling de la map.
            for (int x = 0; x < mapgen.Map.TileIds[z].GetLength(0); x++)
            {
                for (int y = 0; y < mapgen.Map.TileIds[z].GetLength(1); y++)
                {
                    if (y < 10)
                    {
                        mapgen.Map.TileIds[z][x, y] = 0;
                    }
   
                    if (y >= 10 && y <= 30 && x <= 25)
                    {
                        mapgen.Map.TileIds[z][x, y] = 1 + (x % 3);
                    }
                    if (y >= 20 && y <= 23 && x >= 10 && x <= 60)
                    {
                        mapgen.Map.TileIds[z][x, y] = ((7 + y % 2) * 8) + 1 + x % 2;
                    }
                    if (x >= 60 && x <= 65 && y >= 5 && y <= 45)
                    {
                        mapgen.Map.TileIds[z][x, y] = ((9 + y % 3) * 8) + 1 + x % 3;
                    }
                    if (y <= 1 || x <= 1)
                    {
                        mapgen.Map.TileIds[z][x, y] = ((11) * 8) + 3 + x % 3;
                    }
                }
            }
            
            // ------------------------------------------

            // ------------------------------------------
            // Gameobjects
            mapgen.Map.GameObjects = new List<UeFGame.GameObjects.GameObjectInit>();
            // ------------------------------------------


            // Test rectangles
            List<Rectangle> clipRects = new List<Rectangle>();
            int rects = 8;
            int sx = (int)(mapgen.Map.SizeInTiles.X / rects);
            int sy = (int)(mapgen.Map.SizeInTiles.Y / rects);
            for (int x = 0; x < rects; x++)
            {
                for (int y = 0; y < rects; y++)
                {
                    clipRects.Add(new Rectangle(x * sx, y * sy, sx, sy));
                }
            }

            // 
            // ------------------------------------------
            // Polygones
            Console.WriteLine("-- Polygon generation... --"); start = DateTime.Now;
            mapgen.Map.Polygons = UeFGame.Tools.PolygonGenerator.GeneratePolygonsFromMap(mapgen.Map, clipRects);
            end = DateTime.Now;
            Console.WriteLine("---- Done. (" + (end - start).TotalMilliseconds.ToString() + "ms)");

            // ------------------------------------------
            // Enregistrement
            Console.WriteLine("-- Snapshots generation... --"); start = DateTime.Now;
            mapgen.Generate(true);
            end = DateTime.Now;
            Console.WriteLine("---- Done. (" + (end - start).TotalMilliseconds.ToString() + "ms)");

            // Fin
            Console.WriteLine("-- All done.");
            Console.Read();
            ContentWork.Save();*/
        }
    }
}
