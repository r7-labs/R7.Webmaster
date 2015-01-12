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
			var tagTag = new Gtk.TextTag ("tag");
			tagTag.Foreground = "gray";

			textBuffer.TagTable.Add (tagTag);
		}

		public override void Highlight ()
		{
			// clear current markup
			textBuffer.RemoveAllTags (textBuffer.StartIter, textBuffer.EndIter);

			var tagMatches = Regex.Matches (textBuffer.Text, @"<\w+.*?>", RegexOptions.IgnoreCase);

			foreach (Match tagMatch in tagMatches)
			{
				if (tagMatch.Success)
				{
					var startIter = textBuffer.GetIterAtOffset (tagMatch.Index);
					var endIter = textBuffer.GetIterAtOffset (tagMatch.Index + tagMatch.Length);

					textBuffer.ApplyTag ("tag", startIter, endIter);
				}
			}
		}
	}
}

