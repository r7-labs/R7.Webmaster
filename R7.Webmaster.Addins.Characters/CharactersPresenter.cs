//
//  CharactersPresenter.cs
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
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Diagnostics;
using R7.Webmaster.Core;

namespace R7.Webmaster.Addins.Characters
{
	public class CharactersPresenter
	{
        // Presenter gets data from Model, 
        // operates with ViewModel and environment,
        // contains BLL and DAL functions

        #region MVP-VM references

        protected CharactersModel Model { get; set; }

        protected CharactersViewModel ViewModel { get; set; }

        #endregion

        public CharactersConfig Config { get; protected set; }

        public CharactersPresenter (CharactersViewModel viewModel)
		{
            // load config
            Config = new CharactersConfig ();

            // base (original) and user data files
            var baseDataFile = Path.Combine (Path.GetDirectoryName (Assembly.GetExecutingAssembly ().Location), "characters.xml");
            var userDataFile = Path.Combine (Config.ApplicationData, "characters.xml");

            // copy base data file to the user data file
            if (!File.Exists (userDataFile))
                File.Copy (baseDataFile, userDataFile);

            // load characters
            Model = new CharactersModel ();
            Model.CharactersList = new CharacterList ();
            Model.CharactersList.LoadFromFile (Path.Combine (Config.ApplicationData, "characters.xml"));

            ViewModel = viewModel;
		}

        public SortedSet<string> GetCategories ()
        {
            return Model.CharactersList.Categories;
        }

        public SortedSet<string> GetMainCategories ()
        {
            return Model.CharactersList.MainCategories;
        }

        public CharacterInfo GetCharacter (int code)
        {
            return Model.CharactersList.FindByCode (code);
        }

        public List<CharacterInfo> GetCharacters ()
        {
            if (!string.IsNullOrEmpty (ViewModel.Category) && ViewModel.Category != ViewModel.AllCategories)
                return Model.CharactersList.FilterByCategories (ViewModel.Category);

            return Model.CharactersList.Characters;
        }

        public void InvokeCharmapApplication ()
        {
            Process.Start (Config.CharMapApplication);
        }

        // REVIEW: Must return string instead of copying?
        public void CopyToClipboard ()
        {
            switch (ViewModel.CharacterFormat)
            {
                case CharacterFormat.Character: 
                    Clipboard.Text = ViewModel.CharactersString;
                    break;
                case CharacterFormat.Entity: 
                    Clipboard.Text = ViewModel.EntitiesString;
                    break;
                case CharacterFormat.NumericEntity:
                    Clipboard.Text = ViewModel.NumericEntitiesString;
                    break;
                case CharacterFormat.HexEntity:
                    Clipboard.Text = ViewModel.HexEntitiesString;
                    break;
                case CharacterFormat.Unicode:
                    Clipboard.Text = ViewModel.UnicodeString;
                    break;
            }
        }
	}
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
            //  xser.Serialize (xmlWriter, ch);

            xser.Serialize (stream, charList);

            //xmlWriter.Close ();
            stream.Close ();
        }*/