using MapDataPipeline;
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
  internal class Program
  {
    public static Stream stream;
    public static StreamReader reader;
    public static TextWriter writer;
    public static int ErrorsCount;
    public static Params parameters;

    private static void Main(string[] args)
    {
      Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
      Program.CheckCommandLine();
      Program.ReadParams();
      CompilerParameters cparams = new CompilerParameters();
      cparams.GenerateExecutable = false;
      cparams.ReferencedAssemblies.Add(Program.parameters.GeexRunDll);
      cparams.ReferencedAssemblies.Add(Program.parameters.ScriptingDll);
      cparams.ReferencedAssemblies.Add("Microsoft.Xna.Framework.dll");
      cparams.ReferencedAssemblies.Add("Microsoft.Xna.Framework.Game.dll");
      if (!Program.parameters.IsCheckOnly)
        Program.ProcessCompilation(cparams);
      else
        Program.ProcessCheck(cparams);
      if (Program.parameters.IsQuiet)
        return;
      Console.Read();
    }
    
    public static void ProcessCheck(CompilerParameters cparams)
    {
      Program.stream = (Stream) File.Open(Program.parameters.SingleFilename, FileMode.Open);
      Program.reader = new StreamReader(Program.stream);
      string klass = Program.reader.ReadLine();
      string behaviorStr = Program.reader.ReadLine();
      string source = Program.Check_BuildString(Program.reader.ReadToEnd(), klass, behaviorStr);
      cparams.GenerateInMemory = true;
      CompilerResults compilerResults = new CSharpCodeProvider().CreateCompiler().CompileAssemblyFromSource(cparams, source);
      try
      {
        if (!compilerResults.Errors.HasErrors)
          return;
        StringBuilder stringBuilder = new StringBuilder("Compiler Errors :\r\n");
        foreach (CompilerError compilerError in (CollectionBase) compilerResults.Errors)
          stringBuilder.AppendFormat("Line {0},{1}\t: {2}\n", (object) Program.GetScriptLine(compilerError.Line), (object) compilerError.Column, (object) compilerError.ErrorText);
        stringBuilder.AppendFormat("Code :\n{0}\n\n", (object) source);
        throw new Exception(((object) stringBuilder).ToString());
      }
      catch (Exception ex)
      {
        Program.HandleException(ex, Program.parameters.SingleFilename);
      }
    }

    public static int GetScriptLine(int line)
    {
      int num = Enumerable.Count<string>((IEnumerable<string>) Processor.GetUsingStatements().Split(new string[1]
      {
        "\n"
      }, StringSplitOptions.None));
      return line - 5 - num;
    }

    public static string Check_BuildString(string script, string klass, string behaviorStr)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(Processor.GetUsingStatements());
      string args = Processor.GetArgs(Program.GetBehaviorFromString(behaviorStr), klass);
      stringBuilder.Append("namespace UsineEnFolie\r\n{\r\n    public static class Test\r\n    {\r\n        public static void Behavior" + args + "\r\n        {\r\n            " + script + "\r\n        }\r\n    }\r\n}");
      return ((object) stringBuilder).ToString();
    }

    public static void ProcessCompilation(CompilerParameters cparams)
    {
      Console.WriteLine("Debut de la compilation...");
      if (Program.parameters.IsSingle)
        Program.CompileSingle(Program.parameters.SingleFilename, cparams);
      else
        Program.CompileAll(cparams);
      Console.WriteLine("Termine.\nErreurs : " + Program.ErrorsCount.ToString());
    }

    public static void CheckCommandLine()
    {
      string[] commandLineArgs = Environment.GetCommandLineArgs();
      Program.parameters = new Params();
      foreach (string str in commandLineArgs)
      {
        if (str == "-quiet")
          Program.parameters.IsQuiet = true;
        if (str.Contains("-checkonly--"))
        {
          Program.parameters.IsCheckOnly = true;
          Program.parameters.SingleFilename = str.Substring(12);
        }
        if (str.Contains("-single--"))
        {
          Program.parameters.IsSingle = true;
          Program.parameters.SingleFilename = "..\\..\\..\\Content\\Maps\\" + str.Substring(9);
        }
      }
    }

    public static void ReadParams()
    {
      try
      {
        Program.stream = (Stream) File.Open("Params.txt", FileMode.Open);
        Program.reader = new StreamReader(Program.stream);
      }
      catch (FileNotFoundException ex)
      {
        Console.WriteLine("Impossible de charger le fichier de paramètres Params.txt\n" + ex.Message);
        if (Program.parameters.IsQuiet)
          return;
        Console.Read();
        return;
      }
      Program.parameters.GeexRunDll = Program.reader.ReadLine().Split(new char[1]
      {
        ':'
      })[1];
      Program.parameters.ScriptingDll = Program.reader.ReadLine().Split(new char[1]
      {
        ':'
      })[1];
      Program.parameters.MapsFolder = Program.reader.ReadLine().Split(new char[1]
      {
        ':'
      })[1];
      Processor.outputPrefix = Program.reader.ReadLine().Split(new char[1]
      {
        ':'
      })[1];
    }

    public static void CompileSingle(string filename, CompilerParameters cparams)
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

    public static void CompileAll(CompilerParameters cparams)
    {
      foreach (string filename in Directory.EnumerateFiles(Program.parameters.MapsFolder, "*.umap", SearchOption.AllDirectories))
        Program.CompileSingle(filename, cparams);
    }

    public static void HandleException(Exception e, string filename)
    {
      Console.WriteLine("Une erreur est survenue, dans le fichier :" + filename);
      ++Program.ErrorsCount;
      string str = filename.Replace("\\", "_").Replace("/", "_");
      if (str.Length > 40)
        str = str.Substring(str.Length - 30, 30);
      Program.stream = (Stream) File.Open("Log/Erreur -" + str + ".txt", FileMode.Create);
      Program.writer = (TextWriter) new StreamWriter(Program.stream);
      Program.writer.Write(e.Message);
      Program.writer.ToString();
      Program.writer.Close();
      Program.stream.Close();
    }

    public static BehaviorTypes GetBehaviorFromString(string behaviorStr)
    {
      BehaviorTypes behaviorTypes = BehaviorTypes.Always;
      switch (behaviorStr)
      {
        case "OnInit":
          behaviorTypes = BehaviorTypes.OnInit;
          break;
        case "OnEvent":
          behaviorTypes = BehaviorTypes.OnCollideEvent;
          break;
        case "OnHeroFirst":
          behaviorTypes = BehaviorTypes.OnCollideHeroFirst;
          break;
        case "OnHero":
          behaviorTypes = BehaviorTypes.OnCollideHero;
          break;
        case "OnHeroAction":
          behaviorTypes = BehaviorTypes.OnHeroAction;
          break;
        case "Always":
          behaviorTypes = BehaviorTypes.Always;
          break;
        case "OnScreen":
          behaviorTypes = BehaviorTypes.OnScreen;
          break;
        case "OnShoot":
          behaviorTypes = BehaviorTypes.OnCollideShoot;
          break;
      }
      return behaviorTypes;
    }
  }
}
