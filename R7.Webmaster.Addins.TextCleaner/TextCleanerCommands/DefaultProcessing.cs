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
	public class DefaultProcessing : TextCleanerProcessing
	{
		public DefaultProcessing () : base ()
		{
		}

		protected override void Build ()
		{
			Commands = new List<ITextCleanerCommand> () {

				// add spaces after punctuation
				new CompositeCommand (
					new ReplaceCommand (",", ", "),
					new ReplaceCommand ("!", "! ")),

				// enclose all text in the para
				new CompositeCommand (
					new PrependCommand ("<p>"),
					new AppendCommand ("</p>")).When (() => Params.HtmlOut && !Params.HtmlIn),

				// normalize endlines
				new CompositeCommand (
					new ReplaceCommand ("\r", "\n"),
					new ReplaceCommand ("\n\n", "\n")),

				// replace endlines with paras
				new CompositeCommand (
					new ReplaceCommand ("\n", "</p><p>")).When (() => Params.HtmlOut && !Params.HtmlIn),

				// remove spaces before and after para tags
				new CompositeCommand (
					new ReplaceCommand ("<p> ", "<p>"),
					new ReplaceCommand (" </p>", "</p>")).When (() => Params.HtmlOut),

				// remove duplicate whitespace
				new RegexReplaceCommand (@"\s+", " "),

				// remove extra and empty paras
				new CompositeCommand (
					new CustomCommand ( delegate (string value) 
					{
						var buffer_t = string.Empty;
						var once = true;
						while (buffer_t.Length != value.Length)
						{
							if (once)
								once = false;
							else
								value = buffer_t;

							buffer_t = value.Replace ("</p><p></p><p>", "</p><p>");
						}

						return buffer_t;
					}),
					new ReplaceCommand ("<p></p>", ""),
					new ReplaceCommand ("<p> </p>", ""),
					new ReplaceCommand ("<p>\u00A0</p>", ""),
					new ReplaceCommand ("<p>&#160;</p>", ""),
					new ReplaceCommand ("<p>&nbsp;</p>", ""),
					new ReplaceCommand ("<p><p>", "<p>"),
					new ReplaceCommand ("</p></p>", "</p>")
				).When (() => Params.HtmlOut),

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

				// replace ampersands and quotes // &apos;?
				new CompositeCommand (
					new ReplaceCommand ("&", "&amp;"),
					new ReplaceCommand ("\"", "&quot;"),
					new ReplaceCommand ("'", "&apos;")).When (() => Params.HtmlOut && !Params.HtmlIn),

				// replace figure quotes in html output
				new CompositeCommand (
					new ReplaceCommand ("«", "&quot;"),
					new ReplaceCommand ("»", "&quot;")).When (() => Params.HtmlOut),

				// replace figure quotes in text output
				new CompositeCommand (
					new ReplaceCommand ("«", "\""),
					new ReplaceCommand ("»", "\"")).When (() => !Params.HtmlOut),

				// remove hyphens
				new ReplaceCommand ("¬", ""),

				// replace long dashes with &ndash;, place &nbsp; before it
				new CompositeCommand (
					new ReplaceCommand (" - ", "&nbsp;&ndash; ")).When (() => Params.HtmlOut),

				// place space after dash
				new CompositeCommand (
					new ReplaceCommand (" -", "&nbsp;&ndash; ")).When (() => Params.HtmlOut),

				// replace very long dash with long dash
				new CompositeCommand (
					new ReplaceCommand ("\u2014", "\u2013"),
					new ReplaceCommand ("&mdash;", "&ndash;")).When (() => Params.HtmlOut),

				// place nbsp before dash
				new CompositeCommand (
					new ReplaceCommand ("\u2013 ", "&nbsp;&ndash; "),
					new ReplaceCommand ("\u2013 ", "&nbsp;&ndash; "),
					new ReplaceCommand (" &nbsp;", "&nbsp;")).When (() => Params.HtmlOut),

				// emphasize names:
				// Ivanov I.I. => <em>Ivanov I.I.</em>
				// I.I. Ivanov => <em>I.I. Ivanov</em>
				new CompositeCommand (
					new RegexReplaceCommand (@"([ЁА-ЯA-Z]\.)\s*?([А-ЯA-Z]\.)\s*?([ЁёА-Яа-яA-Za-z]{2,})", "<em>$1$2 $3</em>"),
					new RegexReplaceCommand (@"([ЁёА-Яа-яA-Za-z]{2,})\s*?([ЁА-ЯA-Z]\.)\s*?([ЁА-ЯA-Z]\.)", "<em>$1 $2$3</em>"))
					.When (() => Params.HtmlOut && Params.EmNames), 

					// fix some common typos
					new CompositeCommand (
						new ReplaceCommand ("г.г.", "гг."),
						new ReplaceCommand ("с\\х", "с.-х."),
						new ReplaceCommand ("с/х", "с.-х."),
						new ReplaceCommand ("с.х.", "с.-х.")),

					// replace URL's with links
					new CompositeCommand (
						new RegexReplaceCommand (@"\b((http|https|ftp|ftps)://.*?)([\s\.,:;!\?]\B)", 
							"<a href=\"${1}\">${1}</a>${3}", RegexOptions.IgnoreCase),
						new RegexReplaceCommand (@"\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b",
							"<a href=\"mailto:$&\">$&</a>", RegexOptions.IgnoreCase)).When (() => Params.HtmlOut && !Params.HtmlIn)

				}; // end list

		}
	}
}

