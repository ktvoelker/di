using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Karl;

namespace Di
{
    public static class Extensions
    {
        public static CharIter Item(this Gtk.TextBuffer buffer, int index)
        {
            return new CharIter(buffer.GetIterAtOffset(index));
        }

        public static CharIter GetCursorIter(this Gtk.TextBuffer buffer)
        {
            return buffer.Item(buffer.CursorPosition);
        }

        public static void Delete(this Gtk.TextBuffer buffer, Range r)
        {
            var start = r.Start.GtkIter;
            var end = r.End.GtkIter;
            buffer.DeleteInteractive(ref start, ref end, true);
        }

        public static void Insert(this Gtk.TextBuffer buffer, CharIter c, string text)
        {
            var ti = c.GtkIter;
            buffer.Insert(ref ti, text);
        }

        public static string GetName(this IEnumerable<Controller.WindowMode> mode)
        {
            return string.Join("-", mode.Where(m => !m.Hidden).OrderByDescending(m => m.Priority).Select(m => m.Name).ToArray());
        }

        public static Gdk.Size GetSize(this Pango.FontDescription font)
        {
            var widget = new Gtk.TextView();
            widget.ModifyFont(font);
            var layout = widget.CreatePangoLayout("W");
            int width, height;
            layout.GetPixelSize(out width, out height);
            return new Gdk.Size() { Width = width, Height = height };
        }

        public static string ProjectRelativeFullName(this Model.Meta.IEntry node)
        {
            if (node == node.Root.Root)
            {
                return ".";
            }
            else
            {
                return node.FullName.Substring(node.Root.Root.FullName.Length + 1);
            }
        }

        public static bool ContainsFocus(this Gtk.Widget w)
        {
            var v = w as View.IContainFocus;
            return (v == null ? w : v.FocusWidget).HasFocus;
        }

        public static void GiveFocus(this Gtk.Widget w)
        {
            var v = w as View.IContainFocus;
            (v == null ? w : v.FocusWidget).GrabFocus();
        }

        public static Ini.IIniSection GetSectionOrEmpty(this Ini.IIniFile ini, string name)
        {
            return ini.GetWithDefault(name, Ini.IniParser.CreateEmptyIniSection());
        }
    }
}
