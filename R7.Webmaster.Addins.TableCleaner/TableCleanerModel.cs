//
//  TableCleanerModel.cs
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
using R7.Webmaster.Addins.TextCleaner;

namespace R7.Webmaster.Addins.TableCleaner
{
	public class TableCleanerParams: TextCleanerParams
	{
		public bool SetWidth;
		public bool SetCssClass;
		public string TableCssClass;
		public int TableWidth;
		public string TableWidthUnits;
		public bool ApplyResultFormat;
		public string ResultFormat;
	}

	public class TableCleanerModel
	{
		public TableCleanerModel ()
		{
		}

		public string TableClean (string text, TableCleanerParams tableCleanParams)
		{
			tableCleanParams.HtmlIn = HtmlUtils.IsHtml (text);

			if (tableCleanParams.HtmlIn && HtmlUtils.HasStartTag (text, "table"))
			{
				var resultText = new TableCleanProcessing ().Execute (text, tableCleanParams);

				if (tableCleanParams.ApplyResultFormat)
				{
					resultText = string.Format (tableCleanParams.ResultFormat, resultText);
				}

				return resultText;
			}

			return text;
		}
	}
}
