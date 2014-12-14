//
//  CaseChanger.cs
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

namespace R7.Webmaster.Addins.CaseChanger
{
	public class CaseChangerModel
	{
		public CaseChangerModel ()
		{
		}

		public string InvertedCase (string s)
		{
			var upper = s.ToUpper ();
			var lower = s.ToLower ();
			var chars = s.ToCharArray();

			for (var i = 0; i < s.Length; i++)
			{
				if (upper[i] == s[i])
					chars[i] = lower[i];
				else if (lower[i] == s[i])
					chars[i] = upper[i];
				else
					chars[i] = s[i];
			}

			return new string(chars); 
		}

		public string SentenceCase (string s)
		{
			var guid = "_" + Guid.NewGuid().ToString() + "_";
			var upper = s.ToUpper();

			var sentence = Regex.Replace (". " + s.ToLower(), @"([\.\!\?\u2026]\s*?)(\b\w)(\w*?)", 
				string.Format ("$1{0}$3", guid))
				.Substring (2);

			var index = -1;
			do
			{
				index = sentence.IndexOf(guid);
				if (index >= 0)
				{	
					sentence = sentence.Remove(index, guid.Length);
					sentence = sentence.Insert(index, upper[index].ToString());
				}

			} while (index >= 0);

			return sentence;




			/*
			var sentences = s.Split ('.', '!', '?', (char)8230);
			for (var i=0; i<sentences.Length; i++)
			{
				if (!string.IsNullOrWhiteSpace(sentences[i].Trim()))
				{
					sentences[i] = sentences[i].ToLower();
					var chars = sentences[i].ToCharArray();

					// Uppercase first character
					if (chars.Length > 0) 
					{
						chars[0] = chars[0].ToString().ToUpper()[0];
						sentences[i] = new string(chars);
					}
				}
			}

			return string.Join(" ", sentences);*/


		}

		public string WordFirstLetterUpperCase (string s)
		{
			var guid = "_" + Guid.NewGuid().ToString() + "_";
			var upper = s.ToUpper();

			var sentence = Regex.Replace(s.ToLower(), @"(\b\w)(\w*?)", 
				string.Format ("$2{0}", guid));

			var index = -1;
			do
			{
				index = sentence.IndexOf(guid);
				if (index >= 0)
				{	
					sentence = sentence.Remove(index, guid.Length);
					sentence = sentence.Insert(index, upper[index].ToString());
				}

			} while (index >= 0);

			return sentence;
		}

	}
}

