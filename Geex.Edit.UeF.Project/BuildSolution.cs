﻿using Microsoft.Build.BuildEngine;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security;
using BuildEngine;
using MSProject = Microsoft.Build.BuildEngine.Project;
namespace Geex.Edit.UeF.Project
{
    public class SolutionBuilder
    {
        BasicFileLogger b;
        public SolutionBuilder() { }

        [STAThread]
        public bool Compile(string solution_name,string logfile, out string errors)
        {
            b = new BasicFileLogger();

            b.Parameters = logfile;

            b.register();

            Microsoft.Build.BuildEngine.Engine.GlobalEngine.BuildEnabled = true;

            MSProject p = new MSProject (Microsoft.Build.BuildEngine.Engine.GlobalEngine);

            p.BuildEnabled = true;

            p.Load(solution_name);

            bool success = p.Build();

            errors = b.getLogoutput();

            errors += "\n" + b.Warningcount + " Warnings. ";
            errors += "\n" + b.Errorcount + " Errors. ";

            b.Shutdown();

            return success;
        }
    }
}

// This is provided by MSDN 

namespace BuildEngine
{

    public class BasicFileLogger : Logger
    {
        Geex.Edit.UeF.Project.LogWindow sb;

        int warningcount, errorcount;

        public int Errorcount
        {
            get { return errorcount; }
            set { errorcount = value; }
        }

        public int Warningcount
        {
            get { return warningcount; }
            set { warningcount = value; }
        }


        private bool status = true;

        public bool Status
        {
            get { return status; }
            set { status = value; }
        }

   

        public void register()
        {
            warningcount = 0;
            errorcount = 0;
            Microsoft.Build.BuildEngine.Engine.GlobalEngine.RegisterLogger(this);
        }

        public override void Initialize(IEventSource eventSource)
        {

            sb = new Geex.Edit.UeF.Project.LogWindow();
            sb.Show();
            if (null == Parameters)
            {
                throw new LoggerException("Log file was not set.");
            }

            string[] parameters = Parameters.Split(';');

            string logFile = parameters[0];
            if (String.IsNullOrEmpty(logFile))
            {
                throw new LoggerException("Log file was not set.");
            }

            if (parameters.Length > 1)
            {
                throw new LoggerException("Too many parameters passed.");
            }

            try
            {
                //this.streamWriter = new StreamWriter(logFile);
            }
            catch (Exception ex)
            {
                if
                (
                    ex is UnauthorizedAccessException
                    || ex is ArgumentNullException
                    || ex is PathTooLongException
                    || ex is DirectoryNotFoundException
                    || ex is NotSupportedException
                    || ex is ArgumentException
                    || ex is SecurityException
                    || ex is IOException
                )
                {
                    throw new LoggerException("Failed to create log file: " + ex.Message);
                }
                else
                {
                    // Unexpected failure
                    throw;
                }
            }

            eventSource.ProjectStarted += new ProjectStartedEventHandler(eventSource_ProjectStarted);
            //eventSource.TaskStarted += new TaskStartedEventHandler(eventSource_TaskStarted);
            //eventSource.MessageRaised += new BuildMessageEventHandler(eventSource_MessageRaised);
            //eventSource.WarningRaised += new BuildWarningEventHandler(eventSource_WarningRaised);
            eventSource.ErrorRaised += new BuildErrorEventHandler(eventSource_ErrorRaised);
            eventSource.ProjectFinished += new ProjectFinishedEventHandler(eventSource_ProjectFinished);

        }

        void eventSource_ErrorRaised(object sender, BuildErrorEventArgs e)
        {

            string line = String.Format("\n ERROR {0}({1},{2}) \n Message {3} : ", 
                e.File, e.LineNumber, e.ColumnNumber,e.Message);
                        

            string msg = line;

            status = false;

            sb.Append(line);

            sb.Append("\n");        

            errorcount++;

        }

        void eventSource_WarningRaised(object sender, BuildWarningEventArgs e)
        {
           // Console.WriteLine("\n Warning arised here. . " + e.Message + "\n");
    
            string line = String.Format(": Warning {0}({1},{2}): \n Message: ", 
                e.File, e.LineNumber, e.ColumnNumber,e.Message);
            sb.Append(line);    
            warningcount++;

        }

        void eventSource_MessageRaised(object sender, BuildMessageEventArgs e)
        {

            if ((e.Importance == MessageImportance.High && IsVerbosityAtLeast(LoggerVerbosity.Minimal))
                || (e.Importance == MessageImportance.Normal && IsVerbosityAtLeast(LoggerVerbosity.Normal))
                || (e.Importance == MessageImportance.Low && IsVerbosityAtLeast(LoggerVerbosity.Detailed))
                )
            {
                ;
            }
        }

        void eventSource_TaskStarted(object sender, TaskStartedEventArgs e)
        {

        }

        void eventSource_ProjectStarted(object sender, ProjectStartedEventArgs e)
        {
            Console.WriteLine("\n Project compilation is started. . ");
            sb.Append("\n Compiling Project: ");
            sb.Append( e.ProjectFile );            
            indent++;
        }

        void eventSource_ProjectFinished(object sender, ProjectFinishedEventArgs e)
        {
            indent--;
            sb.Append("\n Compilation finished for ");
            sb.Append(e.ProjectFile);
            Console.WriteLine("\n Project compilation is finished. . ");
        }

        public override void Shutdown()
        {
        }

        public string getLogoutput()
        {

            if (sb != null)
                return sb.ToString();
            else
                return null;
        }

        private int indent;
    }
}