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

namespace R7.Webmaster.Addins.TextCleaner
{
	public class MatchGroup
	{
		public int Index;
		public int Length;
		public string Value;
		public string NewValue;
	}

	public class HtmlToHtmlProcessing : TextCleanerProcessing
	{
		public HtmlToHtmlProcessing () : base ()
		{
		}

		protected override void Build ()
		{
			Command = new CompositeCommand (
				new CustomCommand (HtmlToHtml)
			);
		}

		public override string Execute (string text, TextCleanerParams textCleanerParams)
		{
			text = Command.Execute (text);

			return text;
		}

		private string HtmlToHtml (string text) //, TextCleanParams param)
		{
			var text2HtmlTextProcessing = new TextToHtmlTextProcessing ();

			#region Attributes

			// get match collections, attrs @title, @alt, @summary going first
			// THINK: More precise and universal regex for attr match
			var attrs = Regex.Matches (text, @"(title|alt|summary)=[""'](.*?)[""']", RegexOptions.IgnoreCase | RegexOptions.Singleline);

			// calculate real matches count, without empty and non-successful 
			var attrsCount = CountValidMatches (attrs, 2);

			// make an array of MatchGroups to store new attribute values
			var attrGroups = new MatchGroup [attrsCount];
			CopyValidMatches (attrs, attrGroups, 2);

			// pass all matched values to cleanup
			foreach (var _group in attrGroups)
				_group.NewValue = text2HtmlTextProcessing.Execute (_group.Value, null);

			// now, we need to apply changes back to original text,
			// before proceed with tags and values
			text = ApplyMatchGroups (text, attrGroups);

			#endregion

			#region Tag values

			// get values
			var values = Regex.Matches (text, ">(.*?)<", RegexOptions.Singleline);
			var valuesCount = CountValidMatches (values, 1);

			var valueGroups = new MatchGroup [valuesCount];
			CopyValidMatches (values, valueGroups, 1);

			foreach (var _group in valueGroups)
				_group.NewValue = text2HtmlTextProcessing.Execute (_group.Value, null);

			text = ApplyMatchGroups (text, valueGroups);

			#endregion

			return text;
		}

		/// <summary>
		/// Applies the [changed] match groups back to text, base on their original index and length
		/// </summary>
		/// <returns>
		/// Resulting text
		/// </returns>
		/// <param name='text'>
		/// Base text
		/// </param>
		/// <param name='matchGroups'>
		/// Match groups array.
		/// </param>
		private string ApplyMatchGroups (string text, MatchGroup [] matchGroups)
		{
			var offset = 0;
			var newText = text;		

			foreach (var _group in matchGroups)
			{
				newText = newText.Remove (_group.Index + offset, _group.Length);
				newText = newText.Insert (_group.Index + offset, _group.NewValue);
				offset = newText.Length - text.Length; 
			}

			return newText;
		}


		private int CountValidMatches (MatchCollection matches, int nGroup)
		{
			var matchCount = 0;

			foreach (Match match in matches)
				if (match.Success && !string.IsNullOrWhiteSpace (match.Groups [nGroup].Value))
					matchCount++;

			return matchCount;
		}

		private void CopyValidMatches (MatchCollection matches, MatchGroup[] matchArray, int groupNum)
		{
			var matchCount = 0;

			foreach (Match match in matches)
				if (match.Success && !string.IsNullOrWhiteSpace (match.Groups [groupNum].Value))
				{
					matchArray [matchCount] = new MatchGroup ();
					matchArray [matchCount].Index = match.Groups [groupNum].Index;
					matchArray [matchCount].Length = match.Groups [groupNum].Length;
					matchArray [matchCount].Value = match.Groups [groupNum].Value;
					matchCount++;
				}

		}
	}
}

