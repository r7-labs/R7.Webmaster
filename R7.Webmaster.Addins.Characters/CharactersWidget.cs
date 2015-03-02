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
			get { return new List<Gtk.ToolItem> () { (Gtk.ToolItem) toggleXmlMode.CreateToolItem () }; }
		}

		#endregion

		protected CharactersModel Model;

		public CharactersWidget ()
		{
			this.Build ();

			Model = new CharactersModel ();

			MakeButtons (Model.Characters, tableCharacters, 5);
		}

		protected void MakeButtons (CharacterList charList, Gtk.Table table, int columns)
		{
			table.NColumns = (uint)columns;
			table.NRows = (uint)Math.Ceiling ((double)charList.Characters.Count / columns);

			// attach indexes
			long left = 0;
			long top = 0;
			foreach (var ch in charList.Characters)
			{
				var button = new Gtk.Button ();
				button.Label = ch.Label;
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
				if (toggleXmlMode.Active)
				{
					entryCharacters.Text += character.Entity;
				}
				else
				{
					entryCharacters.Text += (char)character.Code;
				}
			}
		}
	}
}
