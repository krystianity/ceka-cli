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
using DM.Ceka.Loader;
using DM.Ceka.Saver;

namespace CekaCli
{
    sealed class ArffRunner : CliRunner
    {
        private ArffLoader loader;
        private ArffInstance instance;

        public ArffRunner(Program self, ref CLI options)
            : base(self, options)
        {
        }

        private void loadArffFile()
        {
            this.loader = new ArffLoader(options.InFile);
            this.loader.loadArff();
            this.instance = this.loader.getInstance();
        }

        public override int Run()
        {
            this.loadArffFile();

            string msg = "none";
            int result = 1;
            try
            {
                switch (options.Function)
                {
                    case "removePerAttributeValue": instance.removeDatasetsPerAttributeValue(options.Parameters[0], options.Parameters[1]); break;
                    case "rebuildAttributeAsRanged": instance.rebuildAttributeValueByRange(int.Parse(options.Parameters[0]), int.Parse(options.Parameters[1])); break;
                    case "removePatternMatchRows": instance.deletePatternMatchingDatasets(options.Parameters.ToList<string>()); break;
                    case "removeUnusedValues": instance.removeUnusedAttributeValues(); break;
                    case "refineAllRangedAttributes": instance.refineBackAllRangedAttributes(int.Parse(options.Parameters[0]), int.Parse(options.Parameters[1])); break;
                    case "getMemorySize": result = instance.GetMemorySize(); break;
                }
            }
            catch (Exception ex)
            {
                if (options.Verbose)
                    self.Clout("Failed to execute function, have you defined enough params? ('-p=param1 -p=param2'); " + ex.Message);

                result = 0;
                msg = ex.Message;
            }

            if (result > 0) //only override (save) the instance, if the function was successfull
            {
                new ArffSaver(this.instance).saveInstance(options.InFile);
            }

            self.Clout("{ \"result\": \"" + result + "\", \"msg\": \"" + msg + "\" }"); //give cli status, so that it could be parsed if needed
            
            return 0;
        }
    }
}