//  
//  Buffer.cs
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
using Gtk;
using Pango;
namespace Di.View
{
    public class Buffer : HBox
    {
        private Controller.Buffer ctl;

        public Buffer(Controller.Buffer _ctl)
        {
            ctl = _ctl;
            Homogeneous = true;
            Spacing = 20;
            BorderWidth = 20;
            var textView = new BufferTextView(ctl);
            Add(textView);
        }

        private class BufferTextView : TextView
        {
            private Controller.Buffer ctl;

            public BufferTextView(Controller.Buffer _ctl) : base(_ctl.GtkTextBuffer)
            {
                ctl = _ctl;
                ModifyFont(new FontDescription {
                    Family = "monospace",
                    Size = (int) (14 * Pango.Scale.PangoScale)
                });
            }

            protected override bool OnKeyPressEvent(Gdk.EventKey e)
            {
                ctl.KeyPressedHandler(e);
                return true;
            }
        }
    }
}
