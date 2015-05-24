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

using DM.Ceka;
using DM.Ceka.Algorithms.Associaters;
using DM.Ceka.Loader;
using DM.Ceka.Saver;

namespace CekaCli
{
    sealed class MineRunner : CliRunner
    {
        private ArffLoader loader;
        private ArffInstance instance;

        public MineRunner(Program self, ref CLI options)
            : base(self, options)
        {
        }

        public override int Run()
        {

            switch (options.Algorithm)
            {
                case "apriori": this.runApriori(); break;
                default: throw new NotImplementedException("not yet");
            }

            return 0;
        }

        private void loadArffFile()
        {
            this.loader = new ArffLoader(options.InFile);
            this.loader.loadArff();
            this.instance = this.loader.getInstance();
        }

        private void runApriori()
        {
            //defaults
            float support = 0.1f;
            float confidence = 0.1f;
            bool usupport = false;
            bool uconfidence = false;

            //try to set
            int c = 0;
            try
            {
                foreach (KeyValuePair<string, string> param in options.ParsedParameters)
                {
                    switch (param.Key)
                    {
                        case "support": support = float.Parse(param.Value); c++; break;
                        case "confidence": confidence = float.Parse(param.Value); c++; break;
                        case "apply-support": usupport = bool.Parse(param.Value); c++; break;
                        case "apply-confidence": uconfidence = bool.Parse(param.Value); c++; break;
                        default: if (options.Verbose) self.Clout("Parameter " + param.Key + " is not supported for Apriori algorithm!"); break;
                    }
                }
            }
            catch (Exception ex)
            {
                if (options.Verbose) self.Clout(ex.Message);
            }

            if (c != 4 && options.Verbose)
                self.Clout("Parameter set was not full! Probably using default parameters for Apriori!");

            //defaults
            bool write_to_file = false;
            string outputfile = options.InFile;
            DM.Ceka.Saver.AprioriSaveTypes outputformat = DM.Ceka.Saver.AprioriSaveTypes.WEKA;

            //try to set
            if (!string.IsNullOrEmpty(options.OutFile) && !string.IsNullOrWhiteSpace(options.OutFile)) //is not necessary thats why we have to check it
                outputfile = options.OutFile;

            switch (options.OutputType)
            {
                case "json": outputformat = AprioriSaveTypes.JSON; break;
                case "json-pretty": outputformat = AprioriSaveTypes.JSON_PRETTY; break;
                case "weka": outputformat = AprioriSaveTypes.WEKA; break;
                case "json-file": outputformat = AprioriSaveTypes.JSON; write_to_file = true; break;
                case "weka-file": outputformat = AprioriSaveTypes.WEKA; write_to_file = true; break;
                case "json-file-pretty": outputformat = AprioriSaveTypes.JSON_PRETTY; write_to_file = true; break;
            }

            //run
            this.loadArffFile();
            new Apriori(this.instance, support, confidence, usupport, uconfidence, outputformat, (write_to_file ? false : true), outputfile);
        }
    }
}