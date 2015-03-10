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
using System.Diagnostics;
using Mono.Addins;
using R7.Webmaster.Addins.Root;
using R7.Webmaster.Core;

namespace R7.Webmaster.Addins.Characters
{
	// http://en.wikipedia.org/wiki/List_of_XML_and_HTML_character_entity_references

	[System.ComponentModel.ToolboxItem (true)]
	[Extension(typeof(IWidgetAddin))]
	public partial class CharactersWidget : Gtk.Bin, IWidgetAddin, ICharactersView
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

		protected CharactersModel Model;

		protected ToggleButtonRadioGroup toggleButtonRadioGroup;

        protected Gtk.MenuToolButton buttonFilter;

		public CharactersWidget ()
		{
			this.Build ();

			Model = new CharactersModel ();

            MakeButtons (Model.Characters.Characters, tableCharacters, 10);

			toggleButtonRadioGroup = new ToggleButtonRadioGroup (buttonCopyCharacters, 
				buttonCopyEntities, buttonCopyNumericEntities, buttonCopyHexEntities, buttonCopyUnicode);

			toggleButtonRadioGroup.Activate (0);

            buttonFilter = new Gtk.MenuToolButton ("");
            buttonFilter.Label = "All";
            buttonFilter.IsImportant = true;
            buttonFilter.Menu = MakeCategoriesMenu (Model.Characters);
		}

        protected Gtk.Menu MakeCategoriesMenu (CharacterList characters)
        {
            var menu = new Gtk.Menu ();
            var submenu = new Gtk.Menu ();
            Gtk.RadioAction actionFirst = null;

            foreach (var category in characters.Categories)
            {
                var action = new Gtk.RadioAction (category, category, "", "", 0);

                action.Toggled += CategoryActionToggled;

                if (actionFirst == null)
                    actionFirst = action;
                else
                    action.Group = actionFirst.Group;

                if (characters.MainCategories.Contains (category))
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

        protected void CategoryActionToggled (object sender, EventArgs e)
        {
            var action = (Gtk.RadioAction) sender;

            buttonFilter.Label = action.Name;
            MakeButtons (Model.Characters.FilterByCategories (action.Name), tableCharacters, 10);
        }

        protected void ClearButtons (Gtk.Table table)
        {
            foreach (var w in table.Children)
                if (w is Gtk.Button)
                    table.Remove (w);
        }

        protected void MakeButtons (List<CharacterInfo> charList, Gtk.Table table, int columns)
		{
            ClearButtons (table);

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

		protected void CharacterButtonClicked (object sender, EventArgs e)
		{
			var button = (Gtk.Button) sender;
			var character = Model.Characters.FindByCode ((int) button.Data ["CharacterCode"]);

			if (character != null)
			{
				// need to append or replace entry text 
				var append = toggleAppend.Active;

				// format chars and put them to the entiries
				TextToEntry (entryCharacters, ((char)character.Code).ToString (), append);
				TextToEntry (entryEntities, character.Entity, append);
				TextToEntry (entryNumericEntities, "&#" + character.Code + ";", append);
				TextToEntry (entryHexEntities, "&#x" + character.Code.ToString("X") + ";", append);
				TextToEntry (entryUnicode, "U+" + character.Code.ToString ("X4"), append);

				// copy entry content to clipboard
				TextToClipboard (toggleButtonRadioGroup.Active);
			}
		}

		protected void TextToEntry (Gtk.Entry entry, string text, bool append)
		{
			entry.Text = (append ? entry.Text : "") + text;
		}

		protected void TextToClipboard (Gtk.ToggleButton button)
		{
			if (button == buttonCopyCharacters)
				Clipboard.Text = entryCharacters.Text;
			else if (button == buttonCopyEntities)
				Clipboard.Text = entryEntities.Text;
			else if (button == buttonCopyHexEntities)
				Clipboard.Text = entryHexEntities.Text;
			else if (button == buttonCopyNumericEntities)
				Clipboard.Text = entryNumericEntities.Text;
			else if (button == buttonCopyUnicode)
				Clipboard.Text = entryUnicode.Text;
		}

		protected void OnActionClearActivated (object sender, EventArgs e)
		{
			entryCharacters.Text = "";
			entryEntities.Text = "";
			entryNumericEntities.Text = "";
			entryHexEntities.Text = "";
			entryUnicode.Text = "";

            buttonFilter.Label = "All";
            MakeButtons (Model.Characters.Characters, tableCharacters, 10);
		}

		protected void OnButtonCopyClicked (object sender, EventArgs e)
		{
			var button = (Gtk.ToggleButton) sender;

			// simulate click-like behaviour
			if (button.Active)
				TextToClipboard (button);
		}
       
        protected void OnActionCharmapActivated (object sender, EventArgs e)
        {
            Process.Start (Model.Config.CharMapApplication);
        }
	}
}
