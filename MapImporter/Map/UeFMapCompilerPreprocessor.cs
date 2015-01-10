using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
namespace MapImporter
{
    /// <summary>
    /// Pré-processeur de code pour UeFMapCompiler.
    /// </summary>
    public static class UeFMapCompilerPreprocessor
    {
        /// <summary>
        /// Lance l'exécution du pré-processeur sur le fragment de code donné.
        /// </summary>
        /// <returns></returns>
        public static string Run(string code)
        {
            string regexp = "(?<begin>" + Regex.Escape("#script") + ")" + @"(?<script>[^€]*?)" + "(?<end>" + Regex.Escape("#endscript") + ")";
            Regex reg = new Regex(regexp);
            var matches = reg.Matches(code);
            foreach(Match match in matches)
            {
                var begin = match.Groups["begin"];
                var end = match.Groups["end"];
                var script = match.Groups["script"];
                code = code.Replace(begin.Value + script.Value + end.Value, Process(script.Value));
            }
            string r = "(?<par1>" + Regex.Escape(")") + ")";
            r += "(?<virg>" + Regex.Escape(",") + ")";
            r += "(?<middle>[" + "\\s" + "]*)";
            r += "(?<par2>" + Regex.Escape(")") + ")";
            reg = new Regex(r);
            matches = reg.Matches(code);
            while (matches.Count != 0)
            {
                foreach (Match match in matches)
                {
                    string oldStr = match.Value;
                    string newStr = match.Groups["par1"].Value + match.Groups["middle"].Value + match.Groups["par2"].Value;
                    code = code.Replace(oldStr, newStr);
                }
                matches = reg.Matches(code);
            }
            return code;
        }
        /// <summary>
        /// Effectue le pré-processing sur le string donné.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static string Process(string str)
        {
            
            string newstr = "new CommandSet(";
            str = str.Replace("#condition", "new Condition(delegate(GameEvent evt, CommandSetContext context) {");
            str = str.Replace("#endcondition", "})");
            str = str.Replace("$", "CommandSet.ConditionFactory.");
            str = str.Replace("@if", "CommandSet.FlowFactory.If(");
            str = str.Replace("@elsif", "CommandSet.FlowFactory.Elsif,");
            str = str.Replace("@else", "CommandSet.FlowFactory.Else");
            str = str.Replace("@endif", ")");
            str = str.Replace("@", "CommandSet.ActionFactory.");
            str = str.Replace(";", ",");

            newstr += str;
            newstr += ")";
            return newstr;
        }
    }
}
