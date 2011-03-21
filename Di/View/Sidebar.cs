//  
//  Sidebar.cs
//  
//  Author:
//       Karl Voelker <ktvoelker@gmail.com>
// 
//  Copyright (c) 2011 Karl Voelker
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
using System.Collections.Generic;
namespace Di.View
{
    public abstract class Sidebar : Gtk.VBox, IContainFocus
    {
        public delegate Sidebar SidebarCreator(Controller.Task task);

        private static IDictionary<Type, SidebarCreator> Sidebars;

        static Sidebar()
        {
            Sidebars = new Dictionary<Type, SidebarCreator>();
            Register<Controller.FsChooser<Model.File>>(task => new FsChooserView<Model.File>(task));
            Register<Controller.FsChooser<Model.Directory>>(task => new FsChooserView<Model.Directory>(task));
        }

        public static void Register<T>(Func<T, Sidebar> f) where T : Controller.Task
        {
            Sidebars[typeof(T)] = task => f((T) task);
        }

        public readonly Controller.Task Controller;

        public Sidebar(Controller.Task task)
        {
            Controller = task;
        }

        public static Sidebar Create(Controller.Task task)
        {
            return Sidebars[task.GetType()](task);
        }

        public virtual Gtk.Widget FocusWidget
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }
    }
}

