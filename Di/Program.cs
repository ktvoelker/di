//  
//  Main.cs
//  
//  Author:
//       Karl Voelker <ktvoelker@gmail.com>
// 
//  Copyright (c) 2010 Karl Voelker
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
// 
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.IO;
using Gtk;

namespace Di
{
    class Program
    {
        public static void Main(string[] args)
        {
            string rootPath;
            if (args.Length == 0)
            {
                string env = Environment.GetEnvironmentVariable("DI_PROJECT");
                if (string.IsNullOrEmpty(env))
                {
                    rootPath = Environment.CurrentDirectory;
                }
                else
                {
                    rootPath = env;
                }
            }
            else
            {
                rootPath = args[0];
            }
            Application.Init();
            var model = new Model.Main(new DirectoryInfo(rootPath));
            var ctl = new Controller.Main(model);
            var view = new View.Main(ctl);
            view.ShowAll();
            ctl.Ready.Handler();
            Application.Run();
        }
    }
}

