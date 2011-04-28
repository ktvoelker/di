//  
//  Input.cs
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using Gdk;

namespace Di.Controller
{
    public partial class Main
    {
        public readonly Model.Main Model;

        public readonly IdleHandler Idle;

        public readonly BindListWithCurrent<Window> Windows;

        public readonly IDictionary<string, IEnumerable<WindowMode>> WindowModes;

        public readonly Event1<Task> BeginTask = new Event1<Task>();

        private readonly IDictionary<string, IEnumerable<ICommand>> commandMacros = new Dictionary<string, IEnumerable<ICommand>>();

        private static readonly Regex ModeSectionName = new Regex(@"^mode\s+(?<name>\S+)$");

        private static readonly IDictionary<string, Gdk.ModifierType> ModifierNames = new Dictionary<string, Gdk.ModifierType>();

        public readonly Event0 Ready = new Event0();

        public readonly Event0 Saving = new Event0();

        static Main()
        {
            ModifierNames.Add("C", Gdk.ModifierType.ControlMask);
            ModifierNames.Add("S", Gdk.ModifierType.ShiftMask);
            ModifierNames.Add("W", Gdk.ModifierType.SuperMask);
            ModifierNames.Add("M", Gdk.ModifierType.MetaMask);
            ModifierNames.Add("A", Gdk.ModifierType.Mod1Mask);
            ModifierNames.Add("L", Gdk.ModifierType.LockMask);
        }

        public Main(Model.Main m)
        {
            Model = m;
            Idle = new IdleHandler(this);
            
            WindowModes = new Dictionary<string, IEnumerable<WindowMode>>();

            // Load command macros from config file
            foreach (var entry in Model.Config.GetSectionOrEmpty("macros"))
            {
                commandMacros[entry.Key] = ParseCommands(entry.Value);
            }

            // Load window modes from config file
            foreach (var section in Model.Config)
            {
                var match = ModeSectionName.Match(section.Key);
                if (match.Success)
                {
                    var modeKey = match.Groups["name"].Value;
                    var mode = new WindowMode();
                    mode.Key = modeKey;
                    mode.Name = section.Value.GetWithDefault<string, string>("display-name", modeKey);
                    mode.Hidden = section.Value.GetBoolWithDefault("hidden", false);
                    var map = new KeyMap();
                    if (section.Value.ContainsKey("priority"))
                    {
                        map.Priority = sbyte.Parse(section.Value["priority"]);
                    }
                    if (section.Value.ContainsKey("default"))
                    {
                        map.SetDefault(ParseCommands(section.Value["default"]));
                    }
                    foreach (var entry in section.Value)
                    {
                        if (string.IsNullOrEmpty(entry.Key) || entry.Key == "display-name" || entry.Key == "hidden" || entry.Key == "default")
                        {
                            continue;
                        }
                        var mod = Gdk.ModifierType.None;
                        var tokens = entry.Key.Tokenize().ToList();
                        var key = (Gdk.Key) (Gdk.Keyval.FromName(tokens.Last()));
                        tokens.RemoveAt(tokens.Count - 1);
                        foreach (var modName in tokens)
                        {
                            mod |= ModifierNames[modName];
                        }
                        map.Add(key, mod, ParseCommands(entry.Value));
                    }
                    mode.KeyMap = map;
                    WindowModes.Add(modeKey, new WindowMode[] { mode });
                }
            }

            // Load mode sets from config file
            foreach (var entry in Model.Config.GetSectionOrEmpty("mode-sets"))
            {
                WindowModes[entry.Key] = entry.Value.Tokenize().Select(k => WindowModes[k]).Flatten();
            }

            // Load default window modes from config file
            Model.Config[""]["default-modes"].Tokenize().ForEach(k => WindowModes[k].ForEach(Window.DefaultMode.Add));

            Windows = new BindListWithCurrent<Window>();
            if (Model.Buffers.HasAny())
            {
                Windows.Add(new Window(this, Model.Buffers.Item(0)));
            }

            Action openFile = () => Command.FileCommand.OpenFile(this, Command.FileCommand.InNewWindow, false);

            // Open a file at startup (but it won't work until the view has attached itself to our events)
            Ready.Add(openFile);

            // Open a file whenever all other files have been closed
            Windows.Changed.Add(() =>
            {
                if (Windows.Count == 0)
                {
                    openFile();
                }
            });
        }

        public void Save()
        {
            Idle.LastSave = DateTime.Now;
            new Thread(Model.Save) { IsBackground = true }.Start();
            Saving.Handler();
        }

        public void Quit()
        {
            Save();
            Gtk.Application.Quit();
        }

        private IEnumerable<ICommand> ParseCommandOrMacro(string text)
        {
            var tokens = text.Tokenize();
            var command = tokens.FirstOrDefault();
            if (command == null)
            {
                return new ICommand[] { new Command.Ignore() };
            }
            else
            {
                var args = tokens.Skip(1).ToList();
                if (command == "macro")
                {
                    if (args.Count != 1)
                    {
                        throw new ConfigProblem(string.Format("Wrong number of arguments in macro reference `{0}'", text));
                    }
                    return commandMacros[args[0]];
                }
                else
                {
                    var types = Assembly.GetExecutingAssembly().GetTypes().
                        Where(t => t.IsClass && t.GetInterfaces().Contains(typeof(ICommand)) && t.Name == command).ToList();
                    if (types.Count == 0)
                    {
                        throw new ConfigProblem(string.Format("No command class named {0} found", command));
                    }
                    else if (types.Count != 1)
                    {
                        throw new ConfigProblem(string.Format("Multiple command classes named {0} found", command));
                    }
                    else
                    {
                        ConstructorInfo bestCtor = null;
                        foreach (var ctor in types[0].GetConstructors())
                        {
                            if (ctor.GetParameters().Length == args.Count)
                            {
                                if (bestCtor == null)
                                {
                                    bestCtor = ctor;
                                }
                                else
                                {
                                    throw new ConfigProblem(string.Format("Multiple constructors with arity {0} found for command {1}", args.Count, command));
                                }
                            }
                        }
                        if (bestCtor == null)
                        {
                            throw new ConfigProblem(string.Format("No constructor with arity {0} found for command {1}", args.Count, command));
                        }
                        var parms = bestCtor.GetParameters();
                        object[] oArgs = new object[parms.Length];
                        for (int i = 0; i < args.Count; ++i)
                        {
                            oArgs[i] = ParseCommandArg(args[i], parms[i].ParameterType);
                        }
                        return new ICommand[] { (ICommand) (bestCtor.Invoke(oArgs)) };
                    }
                }
            }
        }

        private object ParseCommandArg(string text, Type type)
        {
            if (type == typeof(string))
            {
                return text;
            }
            else if (type == typeof(char))
            {
                if (text.Length == 2 && text[0] == '\\')
                {
                    switch (text[1])
                    {
                        case 'n': return '\n';
                        case 'r': return '\r';
                        case 'f': return '\f';
                        case 'v': return '\v';
                        case 't': return '\t';
                        case '\\': return '\\';
                    }
                }
                return char.Parse(text);
            }
            else if (type == typeof(int))
            {
                return int.Parse(text);
            }
            else
            {
                throw new ConfigProblem(string.Format("Unsupported parameter type {0}", type));
            }
        }

        private IEnumerable<ICommand> ParseCommands(string text)
        {
            return text.Split(';').Select(ParseCommandOrMacro).Flatten();
        }

        private Window CreateWindow(Model.File file)
        {
            var window = new Window(this, Model.FindOrCreateBuffer(file));
            Windows.Add(window);
            return window;
        }

        public Window FindWindow(Model.File file)
        {
            foreach (var window in Windows)
            {
                if (window.Model.Value.File == file)
                {
                    return window;
                }
            }
            return null;
        }

        public Window FindOrCreateWindow(Model.File file)
        {
            var window = FindWindow(file);
            return window == null ? CreateWindow(file) : window;
        }
    }
}

