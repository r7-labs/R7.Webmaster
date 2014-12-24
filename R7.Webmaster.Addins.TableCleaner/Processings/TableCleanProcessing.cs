//
//  HtmlToHtmlProcessing.cs
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
using R7.Webmaster.Addins.TextCleaner;

namespace R7.Webmaster.Addins.TableCleaner
{
	public class TableCleanProcessing : TextCleanerProcessing
	{
		public TableCleanProcessing () : base ()
		{
		}

		protected override void Build ()
		{
			Command = new CompositeCommand (

				// remove unnessesary open and closing tags
				new CompositeCommand (
					new RegexReplaceCommand (@"<colgroup\b[^>]*>|<p\b[^>]*>|<div\b[^>]*>|<span\b[^>]*>|<font\b[^>]*>|<i\b[^>]*>|<em\b[^>]*>|<b\b[^>]*>|<strong\b[^>]*>|<col\b[^>]*>", "", RegexOptions.IgnoreCase),
					new RegexReplaceCommand (@"</colgroup>|</p>|</div>|</span>|</i>|</b>|</em>|</strong>|</font>|</col>", "", RegexOptions.IgnoreCase)),
			
				// enclose all attribute values in the quotes
				new CompositeCommand (
					new RegexReplaceCommand ("(\\s+\\w+)\\s*=\\s*(\\w+%?)", "${1}=\"${2}\"")),

				// add @@@@@ before span attributes
				new CompositeCommand (
					new RegexReplaceCommand ("(?=(row|col)span=[\"']\\d+[\"'])", "@@@@@", RegexOptions.IgnoreCase)),

				// remove any other attributes - just because they do not begans with @@@@@
				new CompositeCommand (
					new RegexReplaceCommand ("\\s+\\w+:?\\w+=[\"'].*?[\"']", "")),

				// remove MS Excel non-standard export attributes
				new CompositeCommand (
					new RegexReplaceCommand ("x:num|x:str", "")),

				// remove @@@@@ before span attributes
				new CompositeCommand (
					new ReplaceCommand ("@@@@@", "")),

				// remove spaces in open tags like "<TD >" => "<TD>" -
				// those spaces appears after removing attributes
				new CompositeCommand (
					new RegexReplaceCommand ("(<\\w+)\\s+>", "${1}>")),

				// remove spaces before and after open and close tags	
				new CompositeCommand (
					new RegexReplaceCommand (@"\s+<", "<"),
					new RegexReplaceCommand (@">\s+", ">")),

				// remove duplicate spaces
				new CompositeCommand (
					new RegexReplaceCommand (@"\s+", " ")),

				// lowercase all tags and span attributes with MatchEvaluator
				new CompositeCommand (
					new CustomCommand (delegate (string value) {
						return Regex.Replace (value, @"<(/?\w+)|(row|col)span", m => m.Value.ToLower (), RegexOptions.IgnoreCase);
					}))
			);
		}

		/*
		public override string Execute (string text, TextCleanerParams textCleanerParams)
		{
			return Execute (text, (TableCleanerParams) textCleanerParams);
		}
		*/

		public string Execute (string text, TableCleanerParams tableCleanerParams)
		{
			// apply HTML-to-HTML processing first
			var htmlToHtmlProcessing = new HtmlToHtmlProcessing ();
			text = htmlToHtmlProcessing.Execute (text, (TextCleanerParams) tableCleanerParams);

			text = Command.Execute (text);

			if (tableCleanerParams.SetCssClass)
				text = text.Replace ("<table", string.Format (
					"<table class=\"{0}\"", tableCleanerParams.TableCssClass));

			if (tableCleanerParams.SetWidth)
				text = text.Replace ("<table", string.Format (
					"<table width=\"{0}{1}\"", tableCleanerParams.TableWidth, tableCleanerParams.TableWidthUnits));

			return text;
		}
	}
}

