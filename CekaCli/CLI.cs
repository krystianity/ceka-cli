/*
CekaCli - Command Line Interface Application for Ceka Data Mining Library in C Sharp
Copyright (C) 2015 Christian Fröhlingsdorf, ceka.5cf.de

This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

using Plossum.CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;

using DM.Ceka.Helpers;

namespace CekaCli
{
    /// <summary>
    /// This class defines the CLI options that are passed to the program, Plossum will parse them according the to their attribute mapping.
    /// It contains an additional "Evaluate" function that makes additional custom evaluation and error ouputs.
    /// </summary>
    [CommandLineManager(ApplicationName="Ceka",
    Copyright="Copyright (c) Christian Froehlingsdorf")]
    sealed class CLI
    {

        /* CLI Args */

        [CommandLineOption(Name="l", Description="If set, a logfile will be written", Aliases="log, enable-log, log-enabled")]
        public bool Log = false;

        [CommandLineOption(Name="lf", Description="Logfile to write to; default is: ceka.log", Aliases="logfile")]
        public string LogFile = "ceka.log";

        [CommandLineOption(Name="bw", Description="Prevents the output of the welcome message", Aliases="block-welcome, nc")]
        public bool BlockWelcome = false;

        [CommandLineOption(Name="h", Description="Displays help text", Aliases="help")]
        public bool Help = false;

        [CommandLineOption(Name="v", Description="Display debugging information (measurements for example)", Aliases="verbose")]
        public bool Verbose = false;

        private string mMode;
        [CommandLineOption(Name="m", Description="Specifies the Ceka Modus; choose between: 'mine', 'arff', 'sql'", Aliases="mode", MinOccurs=1)]
        public string Mode
        {
            get { return mMode; }
            set {
                if (String.IsNullOrEmpty(value))
                    throw new InvalidOptionValueException("You have to specify a mode", false);
                else if (value != "mine" && value != "arff" && value != "sql")
                    throw new InvalidOptionValueException("Unknown mode! Choose between: 'mine', 'arff', 'sql'", false);
                else
                    mMode = value;
            }
        }

        private string mInFile;
        [CommandLineOption(Name="i", Description="ARFF input file", Aliases="in, input-file, input, if")]
        public string InFile
        {
            get { return mInFile;  }
            set
            {
                if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                    throw new InvalidOptionValueException("You have to specify an input file (without .arff ending!)", false);

                mInFile = value;
            }
        }

        private string mOutFile;
        [CommandLineOption(Name= "o", Description= "Output filepath", Aliases= "out, output-file, output, of")]
        public string OutFile
        {
            get { return mOutFile; }
            set
            {
                if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                    throw new InvalidOptionValueException("You have to specify an output file", false);

                mOutFile = value;
            }
        }

        private string mAlgorithm;
        [CommandLineOption(Name="a", Description="Data Mining Algorithm that is to be ran", Aliases="algorithm, dma, algo")]
        public string Algorithm
        {
            get { return mAlgorithm; }
            set
            {
                if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                    throw new InvalidOptionValueException("You have to specify an algorithm", false);

                if (!Utils.SupportedAlgorithms.Contains<string>(value))
                {
                    throw new InvalidOptionValueException("The algorithm you specify is not supported! Choose from: " + Utils.SupportedAlgorithmsExploded);
                }

                mAlgorithm = value;
            }
        }

        private string mOutputType;
        [CommandLineOption(Name="ot", Description="Mining result's output-type; choose between: 'json', 'json-file', 'weka', 'weka-file', 'json-pretty', 'json-file-pretty'", Aliases="output-type, result, r")]
        public string OutputType
        {
            get { return mOutputType; }
            set
            {
                if (String.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                    throw new InvalidOptionValueException("You have to specify an output type", false);
                else if (value != "json" && value != "json-file" && value != "weka" && value != "weka-file" && value != "json-pretty" && value != "json-file-pretty")
                    throw new InvalidOptionValueException("Unknown output-type! Choose between: 'json', 'json-file', 'weka', 'weka-file', 'json-pretty', 'json-file-pretty'", false);
                else
                   mOutputType  = value;
            }
        }

        public Dictionary<string, string> ParsedParameters = new Dictionary<string, string>();
        private List<string> mParameters = new List<string>();
        [CommandLineOption(Name="p", Description="Parameters for the chosen modus - see manual for more information", Aliases="parameters, params, pa, ap")]
        public List<string> Parameters
        {
            get { return mParameters; }
            set { mParameters = value; }
        }

        private string mFunction;
        [CommandLineOption(Name="f", Description="The function that you want to call on the arff files content", Aliases="function, func, action")]
        public string Function
        {
            get { return mFunction; }
            set
            {
                if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                    throw new InvalidOptionValueException("You have to specific a function", false);

                if (!Utils.SupportedArffFunctions.Contains<string>(value))
                {
                    throw new InvalidOptionValueException("The algorithm you specify is not supported! Choose from: " + Utils.SupportedArffFunctionExploded);
                }

                mFunction = value;
            }
        }

        private string mConStr;
        [CommandLineOption(Name="cs", Description="Database connection-string e.g. 'SERVER=localhost;DATABASE=uhs;UID=root;PASSWORD=root;'", Aliases="connection-string, con-str, constr, database")]
        public string ConStr
        {
            get { return mConStr; }
            set
            {
                if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                    throw new InvalidOptionValueException("Please specify a connection-string e.g.'SERVER=localhost;DATABASE=uhs;UID=root;PASSWORD=root;'");

                mConStr = value;
            }
        }

        private List<string> mColumns = new List<string>();
        [CommandLineOption(Name = "col", Description = "Columns for the database table", Aliases = "column, columns")]
        public List<string> Columns
        {
            get { return mColumns; }
            set { mColumns = value; }
        }

        /* EOF CLI ARGS */

            /// <summary>
            /// Additional evaluation option that prints error messages according to the main-mode that is used (mine, arff ..)
            /// </summary>
            /// <param name="self">Programm (output & parsing)</param>
            /// <param name="options">CLI object</param>
            /// <param name="parser">CLI Parser</param>
            /// <returns></returns>
        public static int Evaluate(Program program){

            //welcome message
            if (!program.Options.BlockWelcome)
                program.Clout(program.Parser.UsageInfo.GetHeaderAsString(78));

            //debugging information
            if (!program.Options.Verbose)
                DM.Ceka.Helpers.Utils.DEBUG = false;
            else
                DM.Ceka.Helpers.Utils.DEBUG = true;

            //logging
            if (!program.Options.Log)
            {
                DM.Ceka.Helpers.Utils.LOG = false;
            }
            else
            {
                DM.Ceka.Helpers.Utils.LOG = true;
                DM.Ceka.Helpers.Utils.LOGFILE = program.Options.LogFile;
            }

            //check for help or errors
            if (program.Options.Help)
            {
                program.Clout(program.Parser.UsageInfo.GetOptionsAsString(78));
                return -1;
            }
            else if (program.Parser.HasErrors)
            {
                program.Clout(program.Parser.UsageInfo.GetErrorsAsString(78));
                return -2;
            }

            //additional evaluation

            if (program.Options.Mode == "mine")
            {
                if (!IsSet(program.Options.Algorithm))
                {
                    throw new InvalidOptionValueException("This mode requires you to specify an algorithm '-a=apriori' ", false);
                }

                if (!IsSet(program.Options.InFile))
                {
                    throw new InvalidOptionValueException("This mode requires you to specify an input file '-i=filename' ", false);
                }

                if(!IsSet(program.Options.OutputType))
                {
                    throw new InvalidOptionValueException("This mode requires you to specify an output type '-ot=json' ", false);
                }

                if(program.Options.Parameters == null || program.Options.Parameters.Count <= 0){
                    throw new InvalidOptionValueException("This mode requires you to specify parameters '-p=confidence:0.2 -p=support:0.3 -p=apply-confidence:false -p=apply-support:true' ", false);
                }
              
                string[] pa; //doesnt work in set {} block of Parameters
                foreach (string p in program.Options.Parameters)
                {
                    if (!p.Contains<char>(':'))
                    {
                        program.Options.Parameters = null;
                        throw new InvalidOptionValueException("You specified false parameters! Do it like this: -p=p1:v1 -p=p2:v2 -p=p3:v3", false);
                    }

                    pa = p.Split(':');
                    program.Options.ParsedParameters.Add(pa[0], pa[1]);
                }

                return 1;
            }
            else if (program.Options.Mode == "arff")
            {

                if (!IsSet(program.Options.InFile))
                {
                    throw new InvalidOptionValueException("This mode requires you to specify an input file '-i=filename' ", false);
                }

                if (!IsSet(program.Options.Function))
                {
                    throw new InvalidOptionValueException("This mode requires you to specify a function '-f=removePerAttributeValue' ", false);
                }

                if (program.Options.Parameters == null || program.Options.Parameters.Count <= 0)
                {
                    if(!Utils.SupportedArffFunctionsWOP.Contains<string>(program.Options.Function)) //throw only if function requires params
                    {
                        throw new InvalidOptionValueException("This mode requires you to specify parameters '-p=param1 -p=param2 -p=param3' ", false);
                    }
                }

                return 2;
            }
            else if (program.Options.Mode == "sql")
            {

                if (!IsSet(program.Options.ConStr))
                {
                    throw new InvalidOptionValueException("This mode requires you to specify a connection-string '-cs=SERVER=localhost;DATABASE=uhs;UID=root;PASSWORD=root;' ", false);
                }

                if (!IsSet(program.Options.OutFile))
                {
                    throw new InvalidOptionValueException("This mode requires you to specify an output file '-o=uhs.arff' ");
                }

                if (program.Options.Parameters == null || program.Options.Parameters.Count <= 0)
                {
                    throw new InvalidOptionValueException("This mode requires you to specify parameters '-p=table=mytable -p=min-range:-1 -p=max-range:-1 -p=first-column-null:false -p=second-column-null:true' ", false);
                }

                string[] pa; //doesnt work in set {} block of Parameters
                foreach (string p in program.Options.Parameters)
                {
                    if (!p.Contains<char>(':'))
                    {
                        program.Options.Parameters = null;
                        throw new InvalidOptionValueException("You specified false parameters! Do it like this: -p=p1:v1 -p=p2:v2 -p=p3:v3", false);
                    }

                    pa = p.Split(':');
                    program.Options.ParsedParameters.Add(pa[0], pa[1]);
                }

                if (program.Options.Columns == null || program.Options.Columns.Count <= 0)
                {
                    throw new InvalidOptionValueException("This mode requires you to specify columns '-col=column1 -col=column2 -col=column3' ", false);
                }

                return 3;
            }
            else
            {
                throw new InvalidOptionValueException("Internal error", false);
            }
        }

        /// <summary>
        /// Simple helper method checks for Null, Empty or Whitespace in a string
        /// </summary>
        /// <param name="str">The string you want to check.</param>
        /// <returns></returns>
        public static bool IsSet(string str)
        {
            return !(string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str));
        }
    }
}