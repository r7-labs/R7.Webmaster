//
//  TextCleanerModel.cs
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
using System.Text.RegularExpressions;
using R7.Webmaster.Core;

namespace R7.Webmaster.Addins.TextCleaner
{

	public class TextCleanerModel
	{
		public TextCleanerModel ()
		{
		}

		public string TextClean (string text, TextCleanerParams textCleanParams)
		{
			textCleanParams.HtmlIn = HtmlUtils.IsHtml (text);

			if (!textCleanParams.HtmlIn && textCleanParams.HtmlOut)
			{
                return new TextToHtmlProcessing ().Execute (text, textCleanParams.Copy ());
			}

			if (!textCleanParams.HtmlIn && !textCleanParams.HtmlOut)
			{
                return new TextToTextProcessing ().Execute (text, textCleanParams.Copy ());
			}

			if (textCleanParams.HtmlIn && !textCleanParams.HtmlOut)
			{
                return new HtmlToTextProcessing ().Execute (text, textCleanParams.Copy ());
			}

			if (textCleanParams.HtmlIn && textCleanParams.HtmlOut)
			{
                return new HtmlToHtmlProcessing ().Execute (text, textCleanParams.Copy ());
			}

			return text;
		}
	}
}
