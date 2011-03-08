//  
//  Font.cs
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
namespace Di
{
    public enum FontFamily
    {
        Serif,
        SansSerif,
        Monospace
    }

    public class Font
    {
        public readonly uint Size;
        public readonly FontFamily Family;

        private readonly string PangoFamily;

        public Font(uint _size, FontFamily _family)
        {
            Size = _size;
            Family = _family;
            switch (Family)
            {
                case FontFamily.Serif:
                    PangoFamily = "serif";
                    break;
                case FontFamily.SansSerif:
                    PangoFamily = "sans-serif";
                    break;
                case FontFamily.Monospace:
                    PangoFamily = "monospace";
                    break;
            }
        }

        public static implicit operator Pango.FontDescription(Font font)
        {
            return new Pango.FontDescription {
                Family = font.PangoFamily,
                Size = (int) (font.Size * Pango.Scale.PangoScale)
            };
        }
    }
}

