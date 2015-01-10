using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;
using UeFGame.GameComponents;
using UeFGame.GameObjects;
namespace MapImporter
{
    public enum CompilerMode
    {
        CheckErrors,
        Compile
    }
    /// <summary>
    /// Permet la compilation des events d'une map.
    /// Deux modes sont disponibles :
    ///     - CheckError
    ///     - Compile
    /// </summary>
    public class UeFMapCompiler
    {
        #region Events / Delegates
        public delegate void HandleError(CompilerErrorCollection errors);
        public event HandleError OnError;
        #endregion

        #region Private
        MapInitializingData m_currentMap;
        string m_code;
        #endregion

        #region Properties
        /// <summary>
        /// Définit ou obtient une valeur représentant le mode de fonctionnement du compileur.
        /// </summary>
        public CompilerMode Mode { get; set; }
        /// <summary>
        /// Définit ou obtient une valeur représentant les paramètres du compileur C#.
        /// </summary>
        public CompilerParameters CompilerParams { get; set; }
        public List<string> ReferencedAssemblies { get; set; }
        #endregion
        /// <summary>
        /// Constructor.
        /// </summary>
        public UeFMapCompiler()
        {
            Mode = CompilerMode.Compile;
            ReferencedAssemblies = new List<string>();
            CompilerParams = new CompilerParameters();
        }

        /* -----------------------------------------------------------------------------------------------------------------------
        * Error checking
        * ---------------------------------------------------------------------------------------------------------------------*/
        #region Error checking
        /// <summary>
        /// Check for errors.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public CompilerErrorCollection CheckForErrors(string code)
        {
            StringBuilder codeBuilder = new StringBuilder();
            codeBuilder.AppendLine(GetUsingStatements());
            codeBuilder.AppendLine("namespace UeFGame.GameObjects.GeneratedCode {");
            codeBuilder.AppendLine("public static class Map_0 {");
            int line = codeBuilder.ToString().Split('\n').Count() - 1;
            codeBuilder.AppendLine(BuildGameObjectCode(0, code));
            codeBuilder.AppendLine("}");
            codeBuilder.AppendLine("}");
            m_code = codeBuilder.ToString();

            // Add assemblies
            var assemblies = GetBaseReferencedAssemblies();
            assemblies.AddRange(ReferencedAssemblies);
            foreach (string assembly in assemblies)
            {
                CompilerParams.ReferencedAssemblies.Add(assembly);
            }
            // Generate in memory.
            CompilerParams.GenerateExecutable = false;
            CompilerParams.GenerateInMemory = true;
            CompilerParams.OutputAssembly = "";
            CompilerResults results = new CSharpCodeProvider().CreateCompiler().CompileAssemblyFromSource(CompilerParams, m_code);

            // Mets la bonne ligne dans les erreurs
            for(int i = 0; i < results.Errors.Count; i++)
            {
                results.Errors[i].Line -= line;
            }

            return results.Errors;
        }
        #endregion
        /* -----------------------------------------------------------------------------------------------------------------------
         * Map Dll generation
         * ---------------------------------------------------------------------------------------------------------------------*/
        #region Map Dll generation
        /// <summary>
        /// Constructeur.
        /// </summary>
        /// <param name="data"></param>
        void GenerateCode(MapInitializingData data, int mapId)
        {
            m_currentMap = data;
            // Objet contenant le code final généré et compilé.
            StringBuilder codeBuilder = new StringBuilder();
            // TODO : ajouter le début du code.
            codeBuilder.AppendLine(GetUsingStatements());
            codeBuilder.AppendLine("namespace UeFGame.GameObjects.GeneratedCode {");
            codeBuilder.AppendLine("\tpublic static class Map_" + mapId.ToString() + " {");
            foreach (GameObjectInit init in data.GameObjects)
            {
                if (init.ModuleSet.GetModules().ContainsKey("game_event"))
                    codeBuilder.AppendLine(BuildGameObjectCode(init));
            }
            
            codeBuilder.AppendLine("\t}"); // end class
            codeBuilder.AppendLine("}"); // end namespace
            m_code = codeBuilder.ToString();
        }
        /// <summary>
        /// Compiles all the code.
        /// </summary>
        /// <param name="mode"></param>
        void CompileAll(string outputFilename)
        {
            CompilerParams.GenerateExecutable = false;
            CompilerParams.GenerateInMemory = false;
            CompilerParams.OutputAssembly = outputFilename;

            var assemblies = GetBaseReferencedAssemblies();
            assemblies.AddRange(ReferencedAssemblies);
            foreach (string assembly in assemblies)
            {
                CompilerParams.ReferencedAssemblies.Add(assembly);
            }
            CompileCode(m_code);
        }
        /// <summary>
        /// Compiles the given map to the given filename.
        /// </summary>
        public void CompileAll(MapInitializingData data, int mapId, string outputFilename)
        {
            GenerateCode(data, mapId);
            CompileAll(outputFilename);
        }
        /// <summary>
        /// Compile le code donné en argumment.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="cparams"></param>
        void CompileCode(string code)
        {
            CodeDomProvider prov = new CSharpCodeProvider();
            CompilerResults results = new CSharpCodeProvider().CreateCompiler().CompileAssemblyFromSource(CompilerParams, code);
            
            if (results.Errors.HasErrors)
            {
                if (OnError != null)
                {
                    OnError(results.Errors);
                }
            }
        }
        #endregion

        /// <summary>
        /// Retourne les statements "using".
        /// </summary>
        /// <returns></returns>
        string GetUsingStatements()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("using System;");
            builder.AppendLine("using System.Collections.Generic;");
            builder.AppendLine("using Microsoft.Xna.Framework;");
            builder.AppendLine("using Microsoft.Xna.Framework.Graphics;");
            builder.AppendLine("using FarseerPhysics;");
            builder.AppendLine("using FarseerPhysics.Collision;");
            builder.AppendLine("using FarseerPhysics.Common;");
            builder.AppendLine("using FarseerPhysics.Dynamics;");
            builder.AppendLine("using UeFGame.GameObjects;");
            builder.AppendLine("using UeFGame.GameObjects.Scripting;");
            return builder.ToString();
        }
        /// <summary>
        /// Gets the referenced assemblies.
        /// </summary>
        /// <returns></returns>
        List<string> GetBaseReferencedAssemblies()
        {
            List<string> refs = new List<string>();
            refs.Add("System.dll");
            refs.Add("System.Core.dll");
            refs.Add("System.Xml.dll");
            refs.Add("C:\\Program Files (x86)\\Microsoft XNA\\XNA Game Studio\\v4.0\\References\\Windows\\x86\\Microsoft.Xna.Framework.dll");
            refs.Add("C:\\Program Files (x86)\\Microsoft XNA\\XNA Game Studio\\v4.0\\References\\Windows\\x86\\Microsoft.Xna.Framework.Graphics.dll");
            refs.Add("C:\\Program Files (x86)\\Microsoft XNA\\XNA Game Studio\\v4.0\\References\\Windows\\x86\\Microsoft.Xna.Framework.Game.dll");
            if (Geex.Edit.Common.Globals.Project!= null)
            {
                refs.Add(Geex.Edit.Common.AppRessources.BaseDir() + "\\Modules\\FarseerPhysics.dll");
                refs.Add(Geex.Edit.Common.AppRessources.BaseDir() + "\\Modules\\Shared.dll");
                refs.Add(Geex.Edit.Common.AppRessources.BaseDir() + "\\Modules\\GameData.dll");
            }
            else
            {

            }
            return refs;
        }
        /// <summary>
        /// Compile les scripts de l'objet donné en argument.
        /// </summary>
        string BuildGameObjectCode(GameObjectInit init)
        {
            string code = init.ModuleSet.Base.Script;
            int id = init.ModuleSet.Base.BehaviorID;
            if (code == null)
                return "";
            return BuildGameObjectCode(id, code);
        }
        /// <summary>
        /// Compile les scripts de l'objet donné en argument.
        /// </summary>
        string BuildGameObjectCode(int id, string code)
        {
            // #uefinitialize crée l'encapsulation permettant l'appel de la méthode décrite après par le jeu.

            return UeFMapCompilerPreprocessor.Run(code.Replace("#uefinitialize", "public static void Evt_" + id.ToString() + "_Initialize(GameObject self)"));
        }
    }
}
