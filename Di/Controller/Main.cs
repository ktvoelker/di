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
using Gdk;

namespace Di.Controller
{
    public partial class Main
    {
        public readonly Model.Main Model;

        public readonly BindList<Window> Windows;
        public readonly BindList<Window>.Events WindowsEvents;

        public readonly Bind<Window> FocusedWindow = new Bind<Window>(null);

        public readonly IDictionary<string, IEnumerable<WindowMode>> WindowModes;

        public readonly Event1<Task> BeginTask = new Event1<Task>();

        private readonly IDictionary<string, IEnumerable<ICommand>> commandMacros = new Dictionary<string, IEnumerable<ICommand>>();

        private static readonly Regex ModeSectionName = new Regex(@"^mode\s+(?<name>\w+)$");

        private static readonly IDictionary<string, Gdk.ModifierType> ModifierNames = new Dictionary<string, Gdk.ModifierType>();

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
                    mode.Name = section.Value.GetWithDefault<string, string>("display-name", modeKey);
                    mode.Hidden = section.Value.GetBoolWithDefault("hidden", false);
                    var map = new KeyMap();
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

            // TODO remove old code until END (after putting these bindings into a config file)
                        /*

            // Command mode bindings (0)
            var commandMode = new KeyMap() { Priority = 5 };
            commandMode.Add(Key.i, new Command.ClearWindowMode(), new Command.AddWindowMode("Insert"));
            commandMode.Add(Key.h, new Command.Down());
            commandMode.Add(Key.t, new Command.Up());
            commandMode.Add(Key.d, new Command.Left());
            commandMode.Add(Key.n, new Command.Right());
            commandMode.Add(Key.Down, new Command.Down());
            commandMode.Add(Key.Up, new Command.Up());
            commandMode.Add(Key.Left, new Command.Left());
            commandMode.Add(Key.Right, new Command.Right());
            commandMode.Add(Key.o, new Command.OpenFile());
            commandMode.Add(Key.e, new Command.NewFile());
            commandMode.Add(Key.w, new Command.ClearWindowMode(), new Command.AddWindowMode("Window"), new Command.AddWindowMode("Common"));
            WindowModes.Add("Command", new WindowMode { Name = "Command", KeyMap = commandMode });
            
            // Insert mode bindings (1)
            var insertMode = new KeyMap();
            insertMode.SetDefault(new Command.InsertKey());
            insertMode.Add(Key.Return, new Command.InsertChar('\n'));
            insertMode.Add(Key.Escape,
                           new Command.DiscardInput(),
                           new Command.ClearWindowMode(),
                           new Command.AddWindowMode("Command"),
                           new Command.AddWindowMode("Number"),
                           new Command.AddWindowMode("Common"));
            insertMode.Add(Key.BackSpace, new Command.Delete(), new Command.Backspace());
            insertMode.Add(Key.Tab, new Command.Tab());
            insertMode.Add(Key.Down, new Command.Down());
            insertMode.Add(Key.Up, new Command.Up());
            insertMode.Add(Key.Left, new Command.Left());
            insertMode.Add(Key.Right, new Command.Right());
            insertMode.Add(Key.Delete, new Command.Delete(), new Command.Right());
            WindowModes.Add("Insert", new WindowMode { Name = "Insert", KeyMap = insertMode });

            // Number mode bindings (2)
            var numberMode = new KeyMap();
            numberMode.Add(Key.Key_0, new NumCommand());
            numberMode.Add(Key.Key_1, new NumCommand());
            numberMode.Add(Key.Key_2, new NumCommand());
            numberMode.Add(Key.Key_3, new NumCommand());
            numberMode.Add(Key.Key_4, new NumCommand());
            numberMode.Add(Key.Key_5, new NumCommand());
            numberMode.Add(Key.Key_6, new NumCommand());
            numberMode.Add(Key.Key_7, new NumCommand());
            numberMode.Add(Key.Key_8, new NumCommand());
            numberMode.Add(Key.Key_9, new NumCommand());
            WindowModes.Add("Number", new WindowMode { Name = "Number", Hidden = true, KeyMap = numberMode });

            // Common bindings (3)
            var commonMode = new KeyMap();
            commonMode.Add(Key.Escape, new Command.DiscardInput());
            WindowModes.Add("Common", new WindowMode { Name = "Common", Hidden = true, KeyMap = commonMode });

            // Window mode bindings (4)
            var windowMode = new KeyMap();
            windowMode.Add(Key.a,
                           new Command.OpenFileInNewWindow(),
                           new Command.ClearWindowMode(),
                           new Command.AddWindowMode("Command"),
                           new Command.AddWindowMode("Number"),
                           new Command.AddWindowMode("Common"));
            windowMode.Add(Key.c,
                           new Command.CloseWindow(),
                           new Command.ClearWindowMode(),
                           new Command.AddWindowMode("Command"),
                           new Command.AddWindowMode("Number"),
                           new Command.AddWindowMode("Common"));
            windowMode.Add(Key.e,
                           new Command.NewFileInNewWindow(),
                           new Command.ClearWindowMode(),
                           new Command.AddWindowMode("Command"),
                           new Command.AddWindowMode("Number"),
                           new Command.AddWindowMode("Common"));
            WindowModes.Add("Window", new WindowMode { Name = "Window", KeyMap = windowMode });

            Window.DefaultMode.Add(WindowModes["Command"]);
            Window.DefaultMode.Add(WindowModes["Number"]);
            Window.DefaultMode.Add(WindowModes["Common"]);

            // END TODO
            */
        
            Windows = new BindList<Window>();
            if (Model.Buffers.HasAny())
            {
                var window = new Window(this, Model.Buffers.Item(0));
                Windows.Add(window);
                FocusedWindow.Value = window;
            }
            WindowsEvents = Windows.Event;
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
                        Where(t => t.IsClass && t.IsSubclassOf(typeof(ICommand)) && t.Name == command).ToList();
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

        private Window CreateWindow()
        {
            var window = new Window(this, Model.CreateBuffer());
            Windows.Add(window);
            return window;
        }

        private Window CreateWindow(Di.Model.File file)
        {
            var window = new Window(this, Model.FindOrCreateBuffer(file));
            Windows.Add(window);
            return window;
        }

        public Window FindWindow(Di.Model.File file)
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

        public Window FindOrCreateWindow(Di.Model.File file)
        {
            var window = FindWindow(file);
            return window == null ? CreateWindow(file) : window;
        }
    }
}

