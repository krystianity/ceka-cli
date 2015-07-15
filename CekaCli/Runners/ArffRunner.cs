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
using System.Linq;

using DM.Ceka;
using DM.Ceka.Loader;
using DM.Ceka.Saver;

namespace CekaCli.Runners
{
    /// <summary>
    /// The executive for used in the "arff" mode (uses Ceka to refine arff files)
    /// </summary>
    sealed class ArffRunner : CliRunner
    {
        private ArffLoader loader;
        private ArffInstance instance;

        public ArffRunner(Program p)
            : base(p)
        {
        }

        private void loadArffFile()
        {
            loader = new ArffLoader(options.InFile);
            loader.loadArff();
            instance = loader.getInstance();
        }

        /// <summary>
        /// additional parsing & execution of the appropriate ceka function
        /// </summary>
        /// <returns></returns>
        public override int Run()
        {
            loadArffFile();

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
                    clout("Failed to execute function, have you defined enough params? ('-p=param1 -p=param2'); " + ex.Message);

                result = 0;
                msg = ex.Message;
            }

            if (result > 0) //only override (save) the instance, if the function was successfull
            {
                new ArffSaver(instance).saveInstance(options.InFile);
            }

            //make a simple JSON result cli-output
            clout("{ \"result\": \"" + result + "\", \"msg\": \"" + msg + "\" }");
            
            return 0;
        }
    }
}