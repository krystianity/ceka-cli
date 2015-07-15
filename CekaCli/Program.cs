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

using System;

using Plossum.CommandLine;
using CekaCli.Runners;

namespace CekaCli
{
    sealed class Program
    {
        public CLI Options;
        public CommandLineParser Parser;
        public CliRunner Runner;

        static int Main(string[] args)
        {
            DM.Ceka.Saver.ArffSaver.OVERRIDE = true; //enabled Ceka's Lib overwrite
           
            Program self = new Program(); //calling self (rel->static)
            self.parseValues(); //parse
            int result = self.evaluateOptions(); //evaluate

            return result <= 0 ? result : self.executeValues(result); //execute
        }

        private void parseValues()
        {
            Options = new CLI();
            Parser = new CommandLineParser(Options);
            Parser.Parse();
        }

        private int evaluateOptions()
        {
            try
            {
                return CLI.Evaluate(this);
            }
            catch (InvalidOptionValueException error)
            {
                //catch additional exceptions that are thrown by custom evaluation
                Clout("Errors: \n  * " + error.Message + "\n");
                return -3;
            }
        }

        private int executeValues(int result)
        {
            switch (result)
            {
                case 1: Runner = new MineRunner(this); break;
                case 2: Runner = new ArffRunner(this); break;
                case 3: Runner = new SqlRunner(this); break;
                default: return result;
            }
            
            return Runner.Run();
        }

        /// <summary>
        /// Log/Output (command line ouput)
        /// </summary>
        /// <param name="msg">Message to be displayed in a new line</param>
        public void Clout(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}