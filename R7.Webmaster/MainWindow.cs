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
using R7.Webmaster.Core;

namespace R7.Webmaster
{
	public partial class MainWindow: Gtk.Window, ITextInputWidgetAddinHost
	{
		protected WidgetAddinManager AddinManager;

		protected List<IWidgetAddin> Addins; 

		protected event EventHandler InputTextChanged;

		protected void OnInputTextChanged (object sender, EventArgs e)
		{
			// propagate event to subscribers
			if (InputTextChanged != null)
				InputTextChanged (InputTextWidget, e);
		}

		#region ITextInputAddinHost implementation

		public string InputText
		{
			get { return InputTextWidget.Buffer.Text; }
		}
			
		public bool AutoProcess
		{
			get { return toggleAutoProcess.Active; }
		}

		#endregion

		public MainWindow () : base (Gtk.WindowType.Toplevel)
		{
			Build ();
		
			InitAddins ();

			InitNotebook ();
			InitTextWidget ();
			InitTrayIcon ();
		}

		protected void InitAddins ()
		{
			AddinManager = new WidgetAddinManager ();
			Addins = OrderAddins (AddinManager.Widgets, Program.AppConfig.AddinsOrder);

			foreach (var widget in Addins)
			{
				// bind text input widgets
				if (widget is ITextInputWidgetAddin)
				{
					var textInputWidget = (ITextInputWidgetAddin) widget;

					textInputWidget.Host = this;
					InputTextChanged += textInputWidget.OnInputTextChanged;
				}
			}
		}

		protected List<IWidgetAddin> OrderAddins (IWidgetAddin [] addins, string [] addinsOrder)
		{
			var unorderedAddins = new List<IWidgetAddin> (addins);
			var orderedAddins = new List<IWidgetAddin> (unorderedAddins.Count);

			for (var i = 0; i < addinsOrder.Length; i++)
			{
				var addin = unorderedAddins.Find (a => a.SafeName == addinsOrder [i]);

				if (addin != null)
				{
					// move founded addin to its place 
					orderedAddins.Add (addin);
					unorderedAddins.Remove (addin);
				}
			}

			// add remained items
			orderedAddins.AddRange (unorderedAddins);

			return orderedAddins;
		}

		#region Notebook

		protected void InitNotebook ()
		{
			// restore tabs position
			notebook1.TabPos = Program.AppConfig.TabsPosition;

			// remove default page
			notebook1.RemovePage (0);

			foreach (var widget in Addins)
			{
				AppendPage (notebook1, widget.Instance, widget.Label, widget.Icon);
			}

			// wire up SwitchPage manually to avoid firing it for default page
			notebook1.SwitchPage += OnNotebook1SwitchPage;
			notebook1.ShowAll ();
		}

		protected void OnNotebook1SwitchPage (object o, Gtk.SwitchPageArgs args)
		{
			#if DEBUG
			Console.WriteLine (args.PageNum);
			#endif

			var widget = Addins [(int) args.PageNum];

			// show textview only to text input widgets
			TextScrolledWindow.Visible = widget is ITextInputWidgetAddin;

			if (widget.FocusWidget != null)
				widget.FocusWidget.GrabFocus ();
				
			// REVIEW: clear or recreate toolbar!

			while (toolbar1.NItems > 0)
			{
				toolbar1.Remove (toolbar1.GetNthItem (0));
			}

			// starting insert position
			var pos = 0;
		
			// add common ITextInputWidgetAddin actions
			if (widget is ITextInputWidgetAddin)
			{
				toolbar1.Insert ((Gtk.ToolItem) actionPaste.CreateToolItem (), pos++);
				toolbar1.Insert ((Gtk.ToolItem) toggleAutoProcess.CreateToolItem (), pos++);
				toolbar1.Insert (new Gtk.SeparatorToolItem (), pos++);
			}
		
			// fill out toolbar
			foreach (var action in widget.Actions)
			{
				var toolitem = (action != null) ? 
					(Gtk.ToolItem) action.CreateToolItem () :
					new Gtk.SeparatorToolItem ();

				// insert toolitem to the right
				toolbar1.Insert (toolitem, pos++);
			}

			// show all changes
			toolbar1.ShowAll ();
		}

		protected void AppendPage (Gtk.Notebook nb, Gtk.Widget child, string title, string iconName)
		{
			var label = new Gtk.Label (title);
			var header = new Gtk.HBox ();

			// FIXME: Windows have not default GTK theme? So LoadIcon() failed for most icons
			// var image = new Image (IconTheme.Default.LoadIcon(iconName, 16, IconLookupFlags.UseBuiltin));

			var image = new Gtk.Image (RenderIcon (iconName, Gtk.IconSize.Menu, ""));

			// var rcStyle = new RcStyle ();
			// rcStyle.Xthickness = rcStyle.Ythickness = 0;
			// label.ModifyStyle (rcStyle);
			// image.ModifyStyle (rcStyle);

			label.Justify = Gtk.Justification.Left;
			header.BorderWidth = 0;

			header.PackStart (image, false, false, 4);
			header.PackStart (label, false, false, 0);
			header.ShowAll ();

			nb.AppendPage (child, header);
		}

		#endregion

		#region Tray icon and menu

		protected Gtk.StatusIcon TrayIcon;

		protected void InitTrayIcon ()
		{
			// load menu structure
			UIManager.AddUiFromResource("R7.Webmaster.ui.TrayMenu.xml");

			// REVIEW: Use icon resource instead of icon name
			TrayIcon = new Gtk.StatusIcon (RenderIcon ("application", Gtk.IconSize.Dialog, ""));
			TrayIcon.Visible = true;

			// icon tooltip
			TrayIcon.Tooltip = Title;

			// on left-click
			TrayIcon.Activate += OnActionRestoreActivated;

			// on right-click
			TrayIcon.PopupMenu += OnTrayIconPopup;
		}

		void OnTrayIconPopup (object o, EventArgs args)
		{
			// ensure that menu is loaded
			UIManager.EnsureUpdate();

			// get menu widget
			var trayMenu = (Gtk.Menu) UIManager.GetWidget ("/trayMenu");
			trayMenu.ShowAll();

			// popup the menu
			trayMenu.Popup();
		}

		public void Restore ()
		{
			// restore window from tray
			Visible = true;

			// move window to the front
			Present();
		}

		protected void OnActionRestoreActivated (object sender, EventArgs e)
		{
			Restore ();
		}

		protected void OnDeleteEvent (object sender, Gtk.DeleteEventArgs a)
		{
			// hide the window to the tray
			Visible = false;

			// event was processed?
			a.RetVal = true;
		}

		protected void OnActionQuitActivated (object sender, EventArgs e)
		{
			Gtk.Application.Quit ();
		}

		#endregion

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

			// set text wrap mode
			InputTextWidget.WrapMode = Gtk.WrapMode.Word;

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

		protected void OnActionPasteActivated (object sender, EventArgs e)
		{
			InputTextWidget.Buffer.Text = Clipboard.Text;
		}






	}
}