//
//  ITextCleanCommand.cs
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

namespace R7.Webmaster.Addins.TextCleaner
{
	public class ReplaceCommand: TextCleanerCommand
	{
		public ReplaceCommand (string pattern, string replacement) : base ()
		{
			Pattern = pattern;
			Replacement = replacement;
		}

		public string Pattern { get; set; }

		public string Replacement { get; set; }

		public override string Execute (string value)
		{
			if (IsEnabled)
			{
				return value.Replace (Pattern, Replacement);
			}

			return value; 
		}
	}

	public class TrimCommand: TextCleanerCommand
	{
		public TrimCommand (params char [] trimChars) : base ()
		{
			if (trimChars != null && trimChars.Length > 0)
				TrimChars = trimChars;
		}

		public char [] TrimChars { get; set; }

		public override string Execute (string value)
		{
			if (IsEnabled)
			{
				if (TrimChars != null && TrimChars.Length > 0)
					return value.Trim (TrimChars);

				return value.Trim ();
			}

			return value;
		}
	}

	public class AppendCommand: TextCleanerCommand
	{
		public AppendCommand (string after) : base ()
		{
			After = after;
		}

		public string After { get; set; }

		public override string Execute (string value)
		{
			if (IsEnabled)
			{
				return value + After;
			}

			return value;
		}
	}

	public class PrependCommand: TextCleanerCommand
	{
		public PrependCommand (string before) : base ()
		{
			Before = before;
		}

		public string Before { get; set; }

		public override string Execute (string value)
		{
			if (IsEnabled)
			{
				return Before + value;
			}

			return value;
		}
	}
}

