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
	public class TextToTextProcessing : TextCleanerProcessing
	{
		public TextToTextProcessing () : base ()
		{
		}

		protected override void Build ()
		{
			Command = new CompositeCommand (

				// add spaces after punctuation
				new CompositeCommand (
					new ReplaceCommand (",", ", "),
					new ReplaceCommand ("!", "! ")),

				// normalize endlines
				new CompositeCommand (
					new ReplaceCommand ("\r", "\n"),
					new ReplaceCommand ("\n\n", "\n")),

				// remove duplicate whitespace
				new RegexReplaceCommand (@"\s+", " "),

				// remove spaces before "closing" punctuation
				new CompositeCommand (
					new ReplaceCommand (" .", "."),
					new ReplaceCommand (" ,", ","),
					new ReplaceCommand (" ;", ";"),
					new ReplaceCommand (" :", ":"),
					new ReplaceCommand (" )", ")"),
					new ReplaceCommand (" ]", "]"),
					new ReplaceCommand (" ?", "?"),
					new ReplaceCommand (" !", "!")),

				// remove extra punctuation before closing parenthesis
				new ReplaceCommand (".).", ".)"),

				// replace figure quotes in text output
				new CompositeCommand (
					new ReplaceCommand ("«", "\""),
					new ReplaceCommand ("»", "\"")),

				// remove hyphens
				new ReplaceCommand ("¬", string.Empty),

				// fix some common typos
				new CompositeCommand (
					new ReplaceCommand ("г.г.", "гг."),
					new ReplaceCommand ("с\\х", "с.-х."),
					new ReplaceCommand ("с/х", "с.-х."),
					new ReplaceCommand ("с.х.", "с.-х.")),

				// trim text
				new CompositeCommand (
					new TrimCommand ())

			);
		}
	}
}

