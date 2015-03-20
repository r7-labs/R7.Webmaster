//
//  CharactersViewModel.cs
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
using System.Collections.Generic;

namespace R7.Webmaster.Addins.Characters
{
    // ViewModel should not depend on Model!

    // REVIEW: Implement ICharactersViewModel

    public class CharactersViewModel
    {
        public CharactersViewModel ()
        {
            CharacterFormat = CharacterFormat.Character;
            AllCategories = "All";

            Clear ();
        }

        public readonly string AllCategories;
      
        // public CharacterInfo SelectedCharacter { get; set; }

        public CharacterFormat CharacterFormat { get; set; }

        public string CharactersString { get; set; }

        public string EntitiesString { get; set; }

        public string NumericEntitiesString { get; set; }

        public string HexEntitiesString { get; set; }

        public string UnicodeString { get; set; }

        public string Category { get; set; }

        // REVIEW: Move to Presenter?
        public void PutChar (CharacterInfo character, bool append)
        {
            // format chars and put them to the entries
            CharactersString = PutString (CharactersString, ((char) character.Code).ToString (), append);
            EntitiesString = PutString (EntitiesString, character.Entity, append);
            NumericEntitiesString = PutString (NumericEntitiesString, character.ToDecEntity (), append);
            HexEntitiesString = PutString (HexEntitiesString, character.ToHexEntity (), append);
            UnicodeString = PutString (UnicodeString, character.ToUnicode (), append);
        }

        private string PutString (string text, string newValue, bool append)
        {
            return append ? text + newValue : newValue;
        }

        public void Clear ()
        {
            Category = AllCategories;

            CharactersString = "";
            EntitiesString = "";
            NumericEntitiesString = "";
            HexEntitiesString = "";
            UnicodeString = "";
        }
    }
}

