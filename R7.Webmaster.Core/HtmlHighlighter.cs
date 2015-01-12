//
//  HtmlHighlighter.cs
//
//  Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
//  Copyright (c) 2015 Roman M. Yagodin
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
using System.Text.RegularExpressions;

namespace R7.Webmaster.Core
{
	public class HtmlHighlighter: TextViewHighlighterBase
	{
		public HtmlHighlighter (Gtk.TextBuffer textBuffer, bool applyOnChange = true): base (textBuffer)
		{
			CreateTags ();

			if (applyOnChange)
				textBuffer.Changed += (sender, e) => Highlight ();
		}

		protected void CreateTags ()
		{
			// colors are from tango gtksourceview theme

			// fill buffer tags table
			textBuffer.TagTable.Add (new Gtk.TextTag ("tag-name") { Foreground = "#729fcf" });
			textBuffer.TagTable.Add (new Gtk.TextTag ("tag-bracket") { Foreground = "#729fcf" });
			textBuffer.TagTable.Add (new Gtk.TextTag ("attribute-name") { Foreground = "#4e9a06", Weight = Pango.Weight.Bold });
			textBuffer.TagTable.Add (new Gtk.TextTag ("attribute-value") { Foreground = "#ad7fa8" });
			textBuffer.TagTable.Add (new Gtk.TextTag ("entity") { Foreground = "#8f5902" });
			textBuffer.TagTable.Add (new Gtk.TextTag ("comment") { Foreground = "#204a87", Weight = Pango.Weight.Normal });
		}

		public override void Highlight ()
		{
			// clear current markup
			textBuffer.RemoveAllTags (textBuffer.StartIter, textBuffer.EndIter);

			// find & highlight tags
			foreach (Match match in Regex.Matches (textBuffer.Text, @"(</?)(\w+).*?(/?>)"))
			{
				if (match.Success)
				{
					ApplyTag ("tag-name", match.Groups [2]);
					ApplyTag ("tag-bracket", match.Groups [1]);
					ApplyTag ("tag-bracket", match.Groups [3]);
				}
			}

			// find & highlight attributes
			foreach (Match match in Regex.Matches (textBuffer.Text, @"(\w+\s*?=)\s*?(['""].*?['""])"))
			{
				if (match.Success)
				{
					ApplyTag ("attribute-name", match.Groups [1]);
					ApplyTag ("attribute-value", match.Groups [2]);
				}
			}

			// find & highlight entities
			foreach (Match match in Regex.Matches (textBuffer.Text, @"&[^\s;]+;"))
			{
				if (match.Success)
				{
					ApplyTag ("entity", match.Groups [0]);
				}
			}

			// find & highlight comments
			foreach (Match match in Regex.Matches (textBuffer.Text, @"<\!--.*?-->", RegexOptions.Singleline))
			{
				if (match.Success)
				{
					ApplyTag ("comment", match.Groups [0]);
				}
			}
		}
	}
}

