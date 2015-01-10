using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace Geex.Edit.Common.Tools
{
    /// <summary>
    /// Class used to test performance of sections of code, and store it in files.
    /// </summary>
    class Benchmark
    {
        /// <summary>
        /// String used internally to store data
        /// </summary>
        StringBuilder m_string;
        /// <summary>
        /// Used for indentation
        /// </summary>
        int m_indentLevel = 0;
        /// <summary>
        /// Time used to register the starting of the benchmark
        /// </summary>
        DateTime time;
        /// <summary>
        /// Initalizes the benchmark.
        /// </summary>
        public Benchmark()
        {
            m_string = new StringBuilder();
        }
        /// <summary>
        /// Start entering a category. This will change identation level and is only used for lisibility.
        /// </summary>
        /// <param name="name"></param>
        public void StartCategory(string name)
        {
            for (int i = 0; i < m_indentLevel; i++)
            {
                m_string.Append("--");
            }
            m_indentLevel += 1;
            m_string.AppendLine(" " + name + " :");
        }
        /// <summary>
        /// Ends a category.
        /// </summary>
        /// <param name="name"></param>
        public void EndCategory()
        {
            m_indentLevel -= 1;
            m_string.AppendLine("");
        }
        /// <summary>
        /// Tells the benchmark it starts evaluation
        /// </summary>
        public void StartEvaluating()
        {
            time = DateTime.Now;
        }
        /// <summary>
        /// Ends the evaluation.
        /// </summary>
        /// <param name="message">The text that will appear next to the time taken by the task</param>
        public void EndEvaluating(string message)
        {
            int finalTime = (DateTime.Now - time).Milliseconds;
            for (int i = 0; i < m_indentLevel; i++)
            {
                m_string.Append("--");
            }
            m_string.AppendLine(" " + message + " : " + finalTime.ToString() + "ms");
        }
        /// <summary>
        /// Ends the benchmark, and store it in the given file.
        /// </summary>
        /// <param name="filename">Filename of the file where to store the benchmark</param>
        public void EndBenchmark(string filename)
        {
            string benchmarkFilename = AppDomain.CurrentDomain.BaseDirectory + "benchmark_" + filename + ".txt";
            FileStream stream = System.IO.File.Open(benchmarkFilename, FileMode.Create);
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(m_string.ToString());
            writer.Close();
            stream.Close();
        }
    }
}
