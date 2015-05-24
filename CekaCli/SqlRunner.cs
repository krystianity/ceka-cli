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
using DM.Ceka.Saver;
using DM.Ceka.Database;

namespace CekaCli
{
    sealed class SqlRunner : CliRunner
    {
        public SqlRunner(Program self, ref CLI options)
            : base(self, options)
        {
        }

        public override int Run()
        {
            int min_range = -1;
            int max_range = -1;
            bool first_col_null = false;
            bool second_col_null = true;
            string table = "";

            int c = 0;
            try
            {
                foreach (KeyValuePair<string, string> param in options.ParsedParameters)
                {
                    switch (param.Key)
                    {
                        case "min-range": min_range = int.Parse(param.Value); c++; break;
                        case "max-range": max_range = int.Parse(param.Value); c++; break;
                        case "first-column-null": first_col_null = bool.Parse(param.Value); c++; break;
                        case "second-column-null": second_col_null = bool.Parse(param.Value); c++; break;
                        case "table": table = param.Value; c++; break;
                        default: if (options.Verbose) self.Clout("Parameter " + param.Key + " is not supported for SQL building!"); break;
                    }
                }
            }
            catch (Exception ex)
            {
                if (options.Verbose) self.Clout(ex.Message);
            }

            if (c != 5 && options.Verbose)
                self.Clout("Parameter set was not full! Probably using default parameters for SQL building!");

            CekaMySQL sql = new CekaMySQL(options.ConStr);
            ArffInstance uhsAi = sql.tableToInstance(table, options.Columns.ToArray<string>(), min_range, max_range, first_col_null, second_col_null);
            uhsAi.integrityCheck();
            new ArffSaver(uhsAi).saveInstance(options.OutFile);

            return 0;
        }
    }
}