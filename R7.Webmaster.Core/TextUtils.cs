//
//  TextUtils.cs
//
//  Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
//  Copyright (c) 2015 Roman M. Yagodin
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
	public static class TextUtils
	{
		/// <summary>
		/// Guesses the encoding of binary data.
		/// </summary>
		/// <returns>The encoding.</returns>
		/// <param name="data">binary data.</param>
		public static Encoding GuessEncoding (byte [] data, Encoding defaultEncoding)
		{
			var encodings = new [] {
				Encoding.UTF8,
				Encoding.Unicode,
				Encoding.BigEndianUnicode,
				Encoding.UTF32
			};

			foreach (var encoding in encodings)
			{
				var match = true;
				var preamble = encoding.GetPreamble ();

				if (preamble.Length <= data.Length)
				{
					for (var i = 0; i < preamble.Length; i++)
					{
						if (preamble [i] != data [i])
						{
							match = false;
							break;
						}
					}
				}

				if (match)
					return encoding;
			}

			return defaultEncoding;
		}
	}
}

