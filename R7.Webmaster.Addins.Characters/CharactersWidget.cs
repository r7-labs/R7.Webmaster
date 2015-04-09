//
//  CharactersWidget.cs
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
using Mono.Addins;
using R7.Webmaster.Addins.Root;
using R7.Webmaster.Core;

namespace R7.Webmaster.Addins.Characters
{
	// http://en.wikipedia.org/wiki/List_of_XML_and_HTML_character_entity_references

	[System.ComponentModel.ToolboxItem (true)]
	[Extension(typeof(IWidgetAddin))]
	public partial class CharactersWidget : Gtk.Bin, IWidgetAddin
	{
		#region IWidgetAddin implementation

		public Gtk.Widget Instance { get { return this; }}

		public Gtk.Widget FocusWidget { get { return tableCharacters; }}

		public string Label { get { return "Characters"; }}

		public string SafeName { get { return "characters"; }}

		public string Icon { get { return Gtk.Stock.SelectFont; }}

		public bool IsActive { get; set; }

		public List<Gtk.ToolItem> ToolItems 
		{ 
			get 
			{ 
				return new List<Gtk.ToolItem> () 
				{
					(Gtk.ToolItem) toggleAppend.CreateToolItem (),
					(Gtk.ToolItem) actionClear.CreateToolItem (),
                    new Gtk.SeparatorToolItem (),
                    buttonFilter,
                    new Gtk.SeparatorToolItem (),
                    (Gtk.ToolItem) actionCharmap.CreateToolItem ()
				}; 
			}
		}

		#endregion

        #region MVP-VM references

        protected CharactersViewModel ViewModel;

        protected CharactersPresenter Presenter;

        #endregion

        #region Custom UI elements

		protected ToggleButtonRadioGroup toggleButtonRadioGroup;

        protected Gtk.MenuToolButton buttonFilter;

        #endregion

		public CharactersWidget ()
		{
			this.Build ();

            // create MVP-VM instances
	        ViewModel = new CharactersViewModel ();
            Presenter = new CharactersPresenter (ViewModel);

            // make radiogroup from toggle buttons
			toggleButtonRadioGroup = new ToggleButtonRadioGroup (buttonCopyCharacters, 
				buttonCopyEntities, buttonCopyNumericEntities, buttonCopyHexEntities, buttonCopyUnicode);
            toggleButtonRadioGroup.Activate (0);

            // set buttons metadata
            buttonCopyCharacters.Data.Add ("SelectedFormat", CharacterFormat.Character);
            buttonCopyEntities.Data.Add ("SelectedFormat", CharacterFormat.Entity);
            buttonCopyNumericEntities.Data.Add ("SelectedFormat", CharacterFormat.NumericEntity);
            buttonCopyHexEntities.Data.Add ("SelectedFormat", CharacterFormat.HexEntity);
            buttonCopyUnicode.Data.Add ("SelectedFormat", CharacterFormat.Unicode);

            // create menu toolbutton
            buttonFilter = new Gtk.MenuToolButton ("");
            buttonFilter.IsImportant = true;
            buttonFilter.Menu = MakeCategoriesMenu (
                Presenter.GetCategories (), Presenter.GetMainCategories ());
            buttonFilter.Clicked += (sender, e) => ((Gtk.Menu)((Gtk.MenuToolButton)sender).Menu).Popup ();

            UpdateView ();
		}

        #region UI helpers

        protected Gtk.Menu MakeCategoriesMenu (IEnumerable<string> categories, SortedSet<string> mainCategories)
        {
            var menu = new Gtk.Menu ();
            var submenu = new Gtk.Menu ();
            Gtk.RadioAction actionFirst = null;

            foreach (var category in categories)
            {
                var action = new Gtk.RadioAction (category, category, "", "", 0);

                action.Toggled += CategoryActionToggled;

                if (actionFirst == null)
                    actionFirst = action;
                else
                    action.Group = actionFirst.Group;

                if (mainCategories.Contains (category))
                    menu.Append (action.CreateMenuItem ());
                else
                    submenu.Append (action.CreateMenuItem ());
            }

            menu.Append (new Gtk.SeparatorMenuItem ());

            var submenuItem = new Gtk.MenuItem ("Other");
            submenuItem.Submenu = submenu;
            menu.Append (submenuItem);

            menu.ShowAll ();

            return menu;
        }

        protected void ClearButtons (Gtk.Table table)
        {
            foreach (var w in table.Children)
                if (w is Gtk.Button)
                    table.Remove (w);
        }

        protected void MakeButtons (List<CharacterInfo> charList, Gtk.Table table, int columns)
		{
            table.NColumns = (uint)columns;
            table.NRows = Math.Max (3, (uint) Math.Ceiling ((double) charList.Count / columns));

			// attach indexes
			long left = 0;
			long top = 0;
			foreach (var ch in charList)
			{
				var button = new Gtk.Button ();
                button.Label = ch.Label;
                button.TooltipText = ch.Description;
				button.Data.Add ("CharacterCode", ch.Code);
				button.Clicked += CharacterButtonClicked;
				// button.ModifyFont (Pango.FontDescription.FromString ("Monospace,24"));

				table.Attach (button, (uint)left, (uint)(left + 1), (uint)top, (uint)(top + 1),
					Gtk.AttachOptions.Expand | Gtk.AttachOptions.Fill,
					Gtk.AttachOptions.Expand | Gtk.AttachOptions.Fill, 0, 0);

				top = (left + 1 > columns - 1) ? top + 1 : top;
				left = (left + 1) % columns;
			}

			table.ShowAll ();
		}

        #endregion

        #region View-ViewModel sync

        private bool updateViewModel = true;

        /// <summary>
        /// Updates the view from view model state.
        /// </summary>
        /// <param name="updateTable">If set to <c>true</c> update (recreate) characters table.</param>
        protected void UpdateView (bool updateTable = true)
        {
            // suppress handlers which update view model
            updateViewModel = false;

            buttonFilter.Label = ViewModel.Category;

            entryCharacters.Text = ViewModel.CharactersString;
            entryEntities.Text = ViewModel.EntitiesString;
            entryNumericEntities.Text = ViewModel.NumericEntitiesString;
            entryHexEntities.Text = ViewModel.HexEntitiesString;
            entryUnicode.Text = ViewModel.UnicodeString;

            // enable handlers which update view model
            updateViewModel = true;

            if (updateTable)
            {
                ClearButtons (tableCharacters);
                MakeButtons (Presenter.GetCharacters (), tableCharacters, 10);
            }
        }

        /// <summary>
        /// Updates the view model from view (widgets) state.
        /// </summary>
        protected void UpdateViewModel ()
        {
            ViewModel.CharactersString = entryCharacters.Text;
            ViewModel.EntitiesString = entryEntities.Text;
            ViewModel.NumericEntitiesString = entryNumericEntities.Text;
            ViewModel.HexEntitiesString = entryHexEntities.Text;
            ViewModel.UnicodeString = entryUnicode.Text;
        }

        #endregion

        #region Event handlers

        protected void CategoryActionToggled (object sender, EventArgs e)
        {
            var action = (Gtk.RadioAction) sender;

            // simulate click-like behaviour
            if (action.Active)
            {
                ViewModel.Category = action.Name;

                UpdateView ();
            }
        }

		protected void CharacterButtonClicked (object sender, EventArgs e)
		{
			var button = (Gtk.Button) sender;

            var character = Presenter.GetCharacter ((int) button.Data ["CharacterCode"]);
            if (character != null)
            {
                // format chars and put them to the entries
                ViewModel.PutChar (character, toggleAppend.Active);

                UpdateView (false);

                // copy selected entry content to clipboard
                Presenter.CopyToClipboard ();
            }
		}

		protected void OnActionClearActivated (object sender, EventArgs e)
		{
            ViewModel.Clear ();

            UpdateView ();
		}

		protected void OnButtonCopyToggled (object sender, EventArgs e)
		{
			var button = (Gtk.ToggleButton) sender;

			// simulate click-like behaviour
			if (button.Active)
            {
                ViewModel.CharacterFormat = (CharacterFormat) button.Data ["SelectedFormat"];
                Presenter.CopyToClipboard ();
            }
        }
       
        protected void OnActionCharmapActivated (object sender, EventArgs e)
        {
            Presenter.InvokeCharmapApplication ();
        }

        protected void OnEntryCharactersChanged (object sender, EventArgs e)
        {
            if (updateViewModel)
                UpdateViewModel ();
        }

        #endregion
	}
}
