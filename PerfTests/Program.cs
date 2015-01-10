using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;
namespace PerfTests
{
    class Program
    {
        public class Foo
        {

        }

        static void Main(string[] args)
        {
            StringBuilder build = new StringBuilder();
            /*build.AppendLine("int truc = 0;");
            build.AppendLine("#script");
            build.AppendLine("@if $Switch(\"A\");");
		    build.AppendLine("    @MoveLeft;");
            build.AppendLine("    @Suck;");
            build.AppendLine("    @if $Switch()");
            build.AppendLine("        @Suck;");
            build.AppendLine("    @endif");
            build.AppendLine("@elsif $Caca('B');");
            build.AppendLine("    @Fuck;");
            build.AppendLine("@elsif #condition return context.Switch == 5 #endcondition;");
            build.AppendLine("    @Fuck;");
            build.AppendLine("@else");
            build.AppendLine("    @MoveRight;");
            build.AppendLine("    @DontSuck;");
            build.AppendLine("@endif");
            build.AppendLine("#endscript");
            build.AppendLine("int machin = 5;");*/
            build.AppendLine("gladele(),");
            build.AppendLine("gloade(),");
            build.AppendLine(") caca");

            //MapImporter.UeFMapCompilerPreprocessor prep = new MapImporter.UeFMapCompilerPreprocessor(build.ToString());
            //string script = prep.Run();
        }
    }
}
