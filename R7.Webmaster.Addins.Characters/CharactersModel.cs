//
//  PasswordGeneratorModel.cs
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

namespace R7.Webmaster.Addins.Characters
{
	public class CharactersModel
	{	
		public CharacterList Characters;

		public CharactersModel ()
		{
			Characters = new CharacterList ();
			Characters.LoadFromFile ("characters.xml");
		}

		/*
		private void TestingDatabase ()
		{
			var charList = new CharacterList ();

			charList.Characters.Add (new CharacterInfo () { Code = (int)'…' });
			charList.Characters.Add (new CharacterInfo () { Label = "Non-breakable space", Code = 160, Entity = "&nbsp;" });
			charList.Characters.Add (new CharacterInfo () { Code = (int)'Z' });

			var stream = new FileStream ("characters.xml", FileMode.Create, FileAccess.Write, FileShare.Read);
			// var xmlWriter = new XmlTextWriter (stream, System.Text.Encoding.UTF8);
			//xmlWriter.Settings.Indent = true;
			//xmlWriter.Settings.IndentChars = "\t";
			//xmlWriter.Settings.NewLineOnAttributes = true;
			//xmlWriter.Settings.NewLineChars = "\n";

			var xser = new XmlSerializer (typeof (CharacterList) );
			//var xser = new XmlSerializer (typeof (List<CharacterInfo>), new Type [] { typeof(CharacterInfo) } );
			//var xser = new XmlSerializer (typeof(CharacterInfo));

			//foreach (var ch in chars)
			//	xser.Serialize (xmlWriter, ch);

			xser.Serialize (stream, charList);

			//xmlWriter.Close ();
			stream.Close ();
		}*/
	}
}

