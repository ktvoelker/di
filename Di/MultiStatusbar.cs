using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Di
{
    public struct StatusbarItem
    {
        public int Width;

        public Func<string> GetText;

        public Action<Action> Event;

        private StatusbarItem(int width, Func<string> getText, Action<Action> evt)
        {
            Width = width;
            GetText = getText;
            Event = evt;
        }

        public static StatusbarItem Create(int width, Func<string> getText, Action<Action> evt)
        {
            return new StatusbarItem(width, getText, evt);
        }

        public static StatusbarItem Create(int width, Func<string> getText, IEvent evt)
        {
            return new StatusbarItem(width, getText, evt.AddNullary);
        }

        public static StatusbarItem Create(int width, string message)
        {
            return new StatusbarItem(width, () => message, a => { });
        }

        public static StatusbarItem Create<T>(int width, Bind<T> message, Func<T, string> proj)
        {
            return new StatusbarItem(width, () => proj(message.Value), a => message.Changed += o => a());
        }

        public static StatusbarItem Create<T>(int width, Bind<T> message)
        {
            return new StatusbarItem(width, () => message.Value.ToString(), a => message.Changed += o => a());
        }
    }

    public class MultiStatusbar : Gtk.HBox
    {
        private void Add(StatusbarItem item, bool expand)
        {
            var bar = new Gtk.Statusbar();
            bar.HasResizeGrip = false;
            bar.Push(0, item.GetText());
            bar.SetSizeRequest(item.Width, bar.HeightRequest);
            if (expand)
            {
                PackEnd(bar, true, true, 0);
            }
            else
            {
                PackStart(bar, false, true, 0);
            }
            item.Event(() =>
            {
                bar.Pop(0);
                bar.Push(0, item.GetText());
            });
        }

        public void Add(StatusbarItem item)
        {
            Add(item, false);
        }

        public void AddLast(StatusbarItem item)
        {
            Add(item, true);
        }
    }
}
