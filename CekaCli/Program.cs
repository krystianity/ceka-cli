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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Plossum.CommandLine;

namespace CekaCli
{
    class Program
    {
        private CLI options;
        private CommandLineParser parser;
        private CliRunner runner;

        static int Main(string[] args)
        {
            DM.Ceka.Saver.ArffSaver.OVERRIDE = true;
           
            Program self = new Program();
            self.parseValues();
            int result = self.evaluateOptions();

            if (result <= 0)
                return result;
            else
                return self.executeValues(result);
        }

        private void parseValues()
        {
            this.options = new CLI();
            this.parser = new CommandLineParser(this.options);
            this.parser.Parse();
        }

        private int evaluateOptions()
        {
            try
            {
                return CLI.Evaluate(this, ref this.options, ref this.parser);
            }
            catch (InvalidOptionValueException error)
            {
                //catch additional exceptions that are thrown by custom evaluation
                this.Clout("Errors:");
                this.Clout("   * " + error.Message);
                this.Clout("");
                return -3;
            }
        }

        public void Clout(string msg)
        {
            Console.WriteLine(msg);
        }

        private int executeValues(int result)
        {
            switch (result)
            {
                case 1: this.runner = new MineRunner(this, ref this.options); break;
                case 2: this.runner = new ArffRunner(this, ref this.options); break;
                case 3: this.runner = new SqlRunner(this, ref this.options); break;
                default: return result;
            }
            
            return this.runner.Run();
        }
    }
}