﻿//
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
	[System.ComponentModel.ToolboxItem (true)]
	[Extension(typeof(IWidgetAddin))]
	public partial class CharactersWidget : Gtk.Bin, IWidgetAddin, ICharactersView
	{
		public CharactersWidget ()
		{
			this.Build ();
		}

		#region IWidgetAddin implementation

		public Gtk.Widget Instance
		{
			get { return this; }
		}

		public Gtk.Widget FocusWidget
		{
			get { return vbox1; }
		}

		public string Label
		{
			get { return "Characters"; }
		}

		public string SafeName
		{
			get { return "characters"; }
		}

		public string Icon
		{
			get { return Gtk.Stock.SelectFont; }
		}

		public List<Gtk.ToolItem> ToolItems
		{
			get { return new List<Gtk.ToolItem> (); }
		}

		public bool IsActive { get; set; }

		#endregion
	}
}
