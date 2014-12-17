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
using GtkSourceView;
using R7.Webmaster.Addins.Root;
using System.Runtime.Remoting.Channels;

namespace R7.Webmaster
{
	public partial class MainWindow: Gtk.Window
	{
		protected WidgetAddinManager Addins;

		protected List<IWidgetAddin> AddinPages; 

		protected event EventHandler InputTextChanged;

		protected void OnInputTextChanged (object sender, EventArgs e)
		{
			// propagate event to subscribers
			if (InputTextChanged != null)
				InputTextChanged (InputTextWidget, e);
		}

		public MainWindow () : base (Gtk.WindowType.Toplevel)
		{
			Build ();
		
			Addins = new WidgetAddinManager ();
			AddinPages = new List<IWidgetAddin> ();

			// remove default page
			notebook1.RemovePage (0);

			foreach (var widget in Addins.Widgets)
			{
				notebook1.AppendPage (widget.Instance, new Gtk.Label (widget.Label));
				AddinPages.Add (widget);

				// subscribe text input widgets
				if (widget is ITextInputWidgetAddin)
					InputTextChanged += ((ITextInputWidgetAddin) widget).OnInputTextChanged;
			}

			// wire up SwitchPage here to avoid firing it for default page
			notebook1.SwitchPage += OnNotebook1SwitchPage;
			notebook1.ShowAll ();

			InitTextWidget ();
		}

		protected void InitTextWidget ()
		{
			var linux = true;

			// Linux: GtkSourceViewSharp available
			if (linux)
			{
				var sourceLanguage = SourceLanguageManager.Default.GetLanguage ("xml");
				var sourceStyleSheme = SourceStyleSchemeManager.Default.GetScheme ("tango");

				var sourceView = new SourceView ();
				InputTextWidget = sourceView;

				InputTextWidget.Buffer.Changed += OnInputTextChanged;

				// sourceView.ShowLineNumbers = true;

				((SourceBuffer) sourceView.Buffer).Language = sourceLanguage;
				((SourceBuffer) sourceView.Buffer).HighlightSyntax = true;
				((SourceBuffer) sourceView.Buffer).StyleScheme = sourceStyleSheme;
			}
			else
			// windows: use TextView
			{
				InputTextWidget = new Gtk.TextView ();

			}

			// use monospace font
			InputTextWidget.ModifyFont (Pango.FontDescription.FromString ("Monospace,8"));

			TextScrolledWindow = new Gtk.ScrolledWindow ();
			TextScrolledWindow.Add (InputTextWidget);

			TextScrolledWindow.ShadowType = Gtk.ShadowType.EtchedIn;
			TextScrolledWindow.BorderWidth = 1;

			// place in vbox
			vbox1.Add (TextScrolledWindow);

			((Gtk.Box.BoxChild) vbox1 [TextScrolledWindow]).Position = 1; // below toolbar
			((Gtk.Box.BoxChild) vbox1 [TextScrolledWindow]).Expand = true;
			((Gtk.Box.BoxChild) vbox1 [TextScrolledWindow]).Fill = true;

			vbox1.ShowAll ();
		}

		protected Gtk.ScrolledWindow TextScrolledWindow;

		protected Gtk.TextView InputTextWidget;

		protected void OnDeleteEvent (object sender, Gtk.DeleteEventArgs a)
		{
			Gtk.Application.Quit ();
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
	
		protected void OnNotebook1SwitchPage (object o, Gtk.SwitchPageArgs args)
		{
			Console.WriteLine (args.PageNum);

			var widget = AddinPages [(int) args.PageNum];

			// show textview only to text input widgets
			TextScrolledWindow.Visible = widget is ITextInputWidgetAddin;

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
					(Gtk.ToolItem) action.CreateToolItem () :
					new Gtk.SeparatorToolItem ();

				// insert toolitem to the right
				toolbar1.Insert (toolitem, toolbar1.NItems);
			}

			// show all changes
			toolbar1.ShowAll ();
		}
	}
}