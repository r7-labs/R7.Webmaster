//
//  HtmlUtils.cs
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
using System.Text.RegularExpressions;

namespace R7.Webmaster.Core
{
	public static class HtmlUtils
	{
		public static bool IsHtml (string text)
		{
			string stripped = 
				text.Replace ("</", "")
					.Replace ("/>", "");

			int delta = text.Length - stripped.Length;

			return delta > 0;
		}

		public static string GetHtmlBody (string text)
		{
			var bodyStart = Regex.Match (text, @"<body.*?>", RegexOptions.IgnoreCase);
			var bodyEnd = Regex.Match (text, @"</body>", RegexOptions.IgnoreCase);

			if (bodyStart.Success && bodyEnd.Success)
			{
				text = text.Substring (bodyStart.Index + bodyStart.Length, 
					bodyEnd.Index - (bodyStart.Index + bodyStart.Length)).Trim ();
			}

			return text;
		}
	}
}
