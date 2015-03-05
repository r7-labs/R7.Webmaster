//
//  CharacterList.cs
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
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace R7.Webmaster.Addins.Characters
{
	[XmlRoot ("Characters")]
	public class CharacterList
	{	
        private SortedSet<string> categories;

		#region Properties

		[XmlElement ("Character", typeof (CharacterInfo))]
		public List<CharacterInfo> Characters { get; set; } 

        [XmlIgnore]
        public SortedSet<string> Categories 
        {
            get
            { 
                if (categories == null)
                {
                    categories = new SortedSet<string> ();

                    var splitChars = new [] { ',' };
                    foreach (var character in Characters)
                    {
                        var catStrings = character.Categories.Split (splitChars, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var catString in catStrings)
                            categories.Add (catString);
                    }
                }

                return categories;
            }
        }

		#endregion

		public CharacterList ()
		{
			Characters = new List<CharacterInfo> ();

		}

		#region Methods

		public void LoadFromFile (string fileName)
		{
			FileStream stream = null;

			try 
			{
				stream = new FileStream (fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
				var serializer = new XmlSerializer (GetType ());

				Characters = ((CharacterList) serializer.Deserialize (stream)).Characters;
			}
			finally
			{
				if (stream != null)
					stream.Close ();
			}
		}

		public CharacterInfo FindByCode (int code)
		{
			foreach (var character in Characters)
				if (character.Code == code)
					return character; 

			return null;
		}

        public List<CharacterInfo> FilterByCategory (string category)
        {
            var result = new List<CharacterInfo> ();

            foreach (var character in Characters)
                if (character.Categories.Contains (category))
                    result.Add (character);

            return result;
        }

        #endregion
	}
}

