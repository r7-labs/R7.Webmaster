//
//  MainWindow.cs
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
using System.Collections.Generic;
using Gtk;
using R7.Webmaster.Addins.Root;

namespace R7.Webmaster
{
	public partial class MainWindow: Gtk.Window
	{
		protected WidgetAddinManager Addins;

		protected List<IWidgetAddin> AddinPages; 

		public MainWindow () : base (Gtk.WindowType.Toplevel)
		{
			Build ();
		
			Addins = new WidgetAddinManager ();
			AddinPages = new List<IWidgetAddin> ();

			// remove default page
			notebook1.RemovePage (0);

			foreach (var widget in Addins.Widgets)
			{
				notebook1.AppendPage (widget.Instance, new Label (widget.Label));
				AddinPages.Add (widget);
			}

			// wire up SwitchPage here to avoid firing it for default page
			notebook1.SwitchPage += OnNotebook1SwitchPage;
			notebook1.ShowAll ();
		}

		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			Application.Quit ();
			a.RetVal = true;
		}

		protected void OnActionPasteActivated (object sender, EventArgs e)
		{
			throw new NotImplementedException ();
		}

		protected void OnActionProcessActivated (object sender, EventArgs e)
		{
			throw new NotImplementedException ();
		}
	
		protected void OnNotebook1SwitchPage (object o, SwitchPageArgs args)
		{
			Console.WriteLine (args.PageNum);

			var widget = AddinPages [(int) args.PageNum];

			if (widget.FocusWidget != null)
				widget.FocusWidget.GrabFocus ();
				
			// REVIEW: clear or recreate toolbar!

			while (toolbar1.NItems > 0)
			{
				toolbar1.Remove (toolbar1.GetNthItem (0));
			}

			// fill out toolbar
			foreach (var action in widget.Actions)
			{
				var toolitem = (action != null) ? 
					(ToolItem) action.CreateToolItem () :
					new SeparatorToolItem ();

				// insert toolitem to the right
				toolbar1.Insert (toolitem, toolbar1.NItems);
			}

			// show all changes
			toolbar1.ShowAll ();
		}
	}
}