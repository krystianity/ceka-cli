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

namespace CekaCli.Runners
{
    /// <summary>
    /// Abstract "runner" class used to give a base for the different "modes" of the application
    /// </summary>
    abstract class CliRunner
    {
        protected CLI options;
        protected Program program;

        protected CliRunner(Program p) //<- abstract class (RC)
        {
            program = p;
            options = p.Options; //could be changed and called via program
        }

        protected void clout(string msg) //<- abstract class (RC)
        {
            program.Clout(msg);
        }

        public abstract int Run();
    }
}