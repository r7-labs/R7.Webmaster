//
//  Clipboard.cs
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

		/// <summary>
		/// Guesses the encoding of binary data.
		/// </summary>
		/// <returns>The encoding.</returns>
		/// <param name="data">binary data.</param>
		private static Encoding GuessEncoding (byte [] data)
		{
			var encodings = new [] {
				Encoding.UTF8,
				Encoding.Unicode,
				Encoding.BigEndianUnicode,
				Encoding.UTF32
			};

			foreach (var encoding in encodings)
			{
				var match = true;
				var preamble = encoding.GetPreamble ();
				for (var i = 0; i < preamble.Length; i++)
				{
					if (preamble [i] != data [i])
					{
						match = false;
						break;
					}
				}

				if (match)
					return encoding;
			}

			// REVIEW: This may not be true...
			return Encoding.UTF8;
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

					string text;

					// try to guess selection data encoding
					var encoding = GuessEncoding (selection.Data);

					if (encoding != null)
					{
						text = encoding.GetString (selection.Data);

						#if DEBUG
						Console.WriteLine (encoding.GetType ().Name);
						Console.WriteLine (text);
						#endif

						return text;
					}

					return string.Empty;
				}

				return Text;
			}
		}
	}
}

