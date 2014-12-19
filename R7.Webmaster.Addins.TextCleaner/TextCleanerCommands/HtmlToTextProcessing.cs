//
//  TextToTextProcessing.cs
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
	public class HtmlToTextProcessing : TextCleanerProcessing
	{
		public HtmlToTextProcessing () : base ()
		{
		}

		protected override void Build ()
		{
			Commands = new List<ITextCleanerCommand> () {

				// replace entities
				new CompositeCommand (
					new RegexReplaceCommand (@"&nbsp;", " ", RegexOptions.IgnoreCase),
					new RegexReplaceCommand (@"&amp;", "'", RegexOptions.IgnoreCase),
					new RegexReplaceCommand (@"&quot;", "\"", RegexOptions.IgnoreCase)),

				// add endlines
				new CompositeCommand (
					new RegexReplaceCommand (@"<br\s*?/?>", "\n", RegexOptions.IgnoreCase),
					new RegexReplaceCommand (@"</p>", "\n", RegexOptions.IgnoreCase),
					new RegexReplaceCommand (@"</li>", "\n", RegexOptions.IgnoreCase),
					new RegexReplaceCommand (@"</tr>", "\n", RegexOptions.IgnoreCase),
					new RegexReplaceCommand (@"</h\d>", "\n", RegexOptions.IgnoreCase)),

				// REVIEW: Add tabs instead of spaces?
				// add spaces
				new CompositeCommand (
					new RegexReplaceCommand (@"</td>", " ", RegexOptions.IgnoreCase),
					new RegexReplaceCommand (@"</th>", " ", RegexOptions.IgnoreCase)),

				// strip HTML tags
				new CompositeCommand (
					new RegexReplaceCommand (@"<.*?>", " "))

			}; // end list
		}

		public override string Execute (string text, TextCleanerParams textCleanerParams)
		{
			Params = textCleanerParams;

			foreach (ITextCleanerCommand command in Commands)
				text = command.Execute (text);

			// perform text-to-text processing after converting from HTML
			var textToTextProcessing = new TextToTextProcessing ();
			text = textToTextProcessing.Execute (text, textCleanerParams);

			return text;
		}
	}
}

