using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
namespace EventCompiler
{
    public class Compiler
    {
        public Stream m_stream;
        public StreamReader m_reader;
        public TextWriter m_writer;
        public int m_ErrorsCount;
        public Params m_parameters;

        public Compiler(Params parameters)
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            m_parameters = parameters;
            CompilerParameters cparams = new CompilerParameters();
            cparams.GenerateExecutable = false;
            cparams.ReferencedAssemblies.Add(m_parameters.GeexRunDll);
            cparams.ReferencedAssemblies.Add(m_parameters.ScriptingDll);
            cparams.ReferencedAssemblies.Add("Microsoft.Xna.Framework.dll");
            cparams.ReferencedAssemblies.Add("Microsoft.Xna.Framework.Game.dll");
            if (!m_parameters.IsCheckOnly)
                ProcessCompilation(cparams);
            else
                ProcessCheck(cparams);

            if (m_parameters.IsQuiet)
                return;

            Console.Read();
        }

        /// <summary>
        /// Effectue une vérification des erreurs.
        /// </summary>
        /// <param name="cparams"></param>
        public void ProcessCheck(CompilerParameters cparams)
        {
            m_stream = (Stream)File.Open(m_parameters.SingleFilename, FileMode.Open);
            m_reader = new StreamReader(m_stream);
            string klass = m_reader.ReadLine();
            string behaviorStr = m_reader.ReadLine();
            string source = Check_BuildString(m_reader.ReadToEnd(), klass, behaviorStr);
            cparams.GenerateInMemory = true;
            CompilerResults compilerResults = new CSharpCodeProvider().CreateCompiler().CompileAssemblyFromSource(cparams, source);
            try
            {
                if (!compilerResults.Errors.HasErrors)
                    return;
                StringBuilder stringBuilder = new StringBuilder("Compiler Errors :\r\n");
                foreach (CompilerError compilerError in (CollectionBase)compilerResults.Errors)
                    stringBuilder.AppendFormat("Line {0},{1}\t: {2}\n", (object)GetScriptLine(compilerError.Line), (object)compilerError.Column, (object)compilerError.ErrorText);
                stringBuilder.AppendFormat("Code :\n{0}\n\n", (object)source);
                throw new Exception(((object)stringBuilder).ToString());
            }
            catch (Exception ex)
            {
                HandleException(ex, m_parameters.SingleFilename);
            }
        }

        public int GetScriptLine(int line)
        {
            int num = Enumerable.Count<string>((IEnumerable<string>)Processor.GetUsingStatements().Split(new string[1]
          {
            "\n"
          }, StringSplitOptions.None));
            return line - 5 - num;
        }
        /// <summary>
        /// Construit le code final.
        /// </summary>
        /// <param name="script"></param>
        /// <param name="klass"></param>
        /// <param name="behaviorStr"></param>
        /// <returns></returns>
        public string Check_BuildString(string script, string klass, string behaviorStr)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(Processor.GetUsingStatements());
            string args = Processor.GetArgs(Program.GetBehaviorFromString(behaviorStr), klass);
            stringBuilder.Append("namespace UsineEnFolie\r\n{\r\n    public  class Test\r\n    {\r\n        public  void Behavior" + args + "\r\n        {\r\n            " + script + "\r\n        }\r\n    }\r\n}");
            return ((object)stringBuilder).ToString();
        }

        /// <summary>
        /// Procécède à la compilation.
        /// </summary>
        /// <param name="cparams"></param>
        public void ProcessCompilation(CompilerParameters cparams)
        {
            Console.WriteLine("Debut de la compilation...");
            if (Program.parameters.IsSingle)
                Program.CompileSingle(Program.parameters.SingleFilename, cparams);
            else
                Program.CompileAll(cparams);
            Console.WriteLine("Termine.\nErreurs : " + Program.ErrorsCount.ToString());
        }
        /// <summary>
        /// Compile un fichier.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="cparams"></param>
        public void CompileSingle(string filename, CompilerParameters cparams)
        {
            try
            {
                Processor.Compile(Importer.Import(filename), cparams);
                Console.WriteLine("Compilation : " + filename);
            }
            catch (Exception ex)
            {
                Program.HandleException(ex, filename);
            }
        }
        /// <summary>
        /// Compiles tous les fichiers.
        /// </summary>
        /// <param name="cparams"></param>
        public void CompileAll(CompilerParameters cparams)
        {
            foreach (string filename in Directory.EnumerateFiles(m_parameters.MapsFolder, "*.umap", SearchOption.AllDirectories))
                Program.CompileSingle(filename, cparams);
        }
        /// <summary>
        /// Gère une erreur de compilation et affiche un message.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="filename"></param>
        public void HandleException(Exception e, string filename)
        {
            Console.WriteLine("Une erreur est survenue, dans le fichier :" + filename);
            ++m_ErrorsCount;
            string str = filename.Replace("\\", "_").Replace("/", "_");
            if (str.Length > 40)
                str = str.Substring(str.Length - 30, 30);
            m_stream = (Stream)File.Open("Log/Erreur -" + str + ".txt", FileMode.Create);
            m_writer = (TextWriter)new StreamWriter(Program.stream);
            m_writer.Write(e.Message);
            m_writer.ToString();
            m_writer.Close();
            m_stream.Close();
        }

    }
}
