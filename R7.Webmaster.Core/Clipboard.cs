//
//  MyClass.cs
//
//  Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
//  Copyright (c) 2014 Roman M. Yagodin
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
using System.Text;

namespace R7.Webmaster.Core
{
	public static class Clipboard
	{
		public static string Text
		{
			get
			{
				var clipboard = Gtk.Clipboard.Get (Gdk.Atom.Intern ("CLIPBOARD", true));

				if (clipboard.WaitIsTextAvailable ())
					return clipboard.WaitForText ();

				return string.Empty;
			}

			set
			{
				var clipboard = Gtk.Clipboard.Get (Gdk.Atom.Intern ("CLIPBOARD", true));
				clipboard.Text = value;
				clipboard.Store ();
			}
		}

		public static string Html
		{
			get 
			{ 
				var clipboard = Gtk.Clipboard.Get (Gdk.Atom.Intern ("CLIPBOARD", true));
				var target = Gdk.Atom.Intern ("text/html", true);

				var selection = clipboard.WaitForContents (target);
				if (selection != null)
				{
					var text = Encoding.UTF8.GetString (selection.Data, 0, selection.Data.Length);
					Console.WriteLine (text);
					return text;
				}

				return Text;
			}
		}
	}
}

