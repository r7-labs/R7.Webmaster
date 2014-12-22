//
//  TextToHtmlTextProcessing.cs
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
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace R7.Webmaster.Addins.TextCleaner
{
	/// <summary>
	/// Text to HTML-text processing.
	/// </summary>
	public class TextToHtmlTextProcessing : TextCleanerProcessing
	{
		public TextToHtmlTextProcessing () : base ()
		{
		}

		protected override void Build ()
		{
			Command = new CompositeCommand (

				// remove duplicate whitespace
				new RegexReplaceCommand (@"\s+", " "),

				// replace ampersands and quotes
				new CompositeCommand (
					new RegexReplaceCommand (@"(&)(\s+)", "&amp;$2"),
					new ReplaceCommand ("\"", "&quot;"),
					new ReplaceCommand ("'", "&apos;")),

				// replace long dashes with &ndash;, place &nbsp; before it
				new CompositeCommand (
					new ReplaceCommand (" - ", "&nbsp;&ndash; ")),

				// place space after dash
				new CompositeCommand (
					new ReplaceCommand (" -", "&nbsp;&ndash; ")),

				// replace very long dash with long dash
				new CompositeCommand (
					new ReplaceCommand ("\u2014", "&ndash;")),

				// place nbsp before dash
				new CompositeCommand (
					new ReplaceCommand ("\u2013 ", "&nbsp;&ndash; "),
					new ReplaceCommand ("\u2013 ", "&nbsp;&ndash; "),
					new ReplaceCommand (" &nbsp;", "&nbsp;")),

				// emphasize names:
				// Ivanov I.I. => <em>Ivanov I.I.</em>
				// I.I. Ivanov => <em>I.I. Ivanov</em>
				new CompositeCommand (
					new RegexReplaceCommand (@"([ЁА-ЯA-Z]\.)\s*?([А-ЯA-Z]\.)\s*?([ЁёА-Яа-яA-Za-z]{2,})", "<em>$1$2 $3</em>"),
					new RegexReplaceCommand (@"([ЁёА-Яа-яA-Za-z]{2,})\s*?([ЁА-ЯA-Z]\.)\s*?([ЁА-ЯA-Z]\.)", "<em>$1 $2$3</em>")),
					
				// replace URL's with links
				new CompositeCommand (
					new RegexReplaceCommand (@"\b((http|https|ftp|ftps)://.*?)([\s\.,:;!\?]\B)", 
						"<a href=\"${1}\">${1}</a>${3}", RegexOptions.IgnoreCase),
					new RegexReplaceCommand (@"\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b",
						"<a href=\"mailto:$&\">$&</a>", RegexOptions.IgnoreCase))

			);
		}

		public override string Execute (string text, TextCleanerParams textCleanerParams)
		{
			// perform text-to-text processing before converting to HTML
			var textToTextProcessing = new TextToTextProcessing ();
			text = textToTextProcessing.Execute (text, textCleanerParams);

			Params = textCleanerParams;

			return Command.Execute (text);
		}
	}
}

