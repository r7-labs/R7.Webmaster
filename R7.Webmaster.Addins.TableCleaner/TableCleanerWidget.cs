//
//  TableCleanerWidget.cs
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
using Mono.Addins;
using R7.Webmaster.Core;
using R7.Webmaster.Addins.Root;
using R7.Webmaster.Addins.TextCleaner;

namespace R7.Webmaster.Addins.TableCleaner
{
	[System.ComponentModel.ToolboxItem (true)]
	[Extension(typeof(IWidgetAddin))]
	public partial class TableCleanerWidget : Gtk.Bin, ITableCleanerView, ITextInputWidgetAddin
	{
		#region ITextInputWidgetAddin implementation

		public Gtk.Widget Instance { get { return this; } }

		public Gtk.Widget FocusWidget { get { return txvResult; } }

		public ITextInputWidgetAddinHost Host { get; set; }

		public string Label { get { return "Table Cleaner"; } }

		public string SafeName { get { return "tablecleaner"; } }

		public EventHandler OnInputTextChanged 
		{
			get { return OnSourceChanged; }
		}

		public TextInputAction SupportedActions 
		{
			get { return TextInputAction.Paste | TextInputAction.PasteHtml | TextInputAction.AutoProcess; }
		}

		public bool IsActive { get; set; }

		public string Icon 
		{ 
			get { return Gtk.Stock.Clear; } 
		}

		public List<Gtk.ToolItem> ToolItems
		{
			get 
			{ 
				return new List<Gtk.ToolItem> () {
					(Gtk.ToolItem) actionProcess.CreateToolItem (),
					new Gtk.SeparatorToolItem (),
					buttonCopy 
				}; 
			}
		}

		#endregion
		 
		protected void AutoCopyResults ()
		{
			if (toggleAutoCopy.Active)
			{
				CopyResults ();
			}
		}

		protected void CopyResults ()
		{
			Clipboard.Text = txvResult.Buffer.Text;
		}

		private TableCleanerModel Model;

		protected TableCleanerConfig Config;

		protected Gtk.MenuToolButton buttonCopy;

		public TableCleanerWidget ()
		{
			this.Build ();

			this.Model = new TableCleanerModel ();

			Config = new TableCleanerConfig ();

			Pango.FontDescription font = Pango.FontDescription.FromString ("Monospace");
			txvResult.ModifyFont (font);

			// fill table CSS classes comboentry
			foreach (var tableCssClass in Config.TableCssClasses)
				centryCssClass.AppendText (tableCssClass);
			centryCssClass.Active = 0;

			UIManager.AddUiFromResource ("R7.Webmaster.Addins.TableCleaner.ui.CopyMenu.xml");

			buttonCopy = new Gtk.MenuToolButton (Gtk.Stock.Copy);
			buttonCopy.IsImportant = true;
			buttonCopy.Label = "Copy";
			buttonCopy.Menu = UIManager.GetWidget ("/copyMenu");
			buttonCopy.Clicked += OnButtonCopyClicked;

			ProcessState ();
		}

		protected virtual void OnSourceChanged (object sender, EventArgs e)
		{
			if (IsActive && Host.AutoProcess)
				OnActionProcessActivated (sender, e);

			ProcessState ();
		}

		protected void ProcessState()
		{
			// empty
		}

		protected void OnActionProcessActivated (object sender, EventArgs e)
		{
			var tableCleanerParams = new TableCleanerParams ()
			{
				SetWidth = checkSetWidth.Active,
				SetCssClass = checkSetCssClass.Active,
				TableCssClass = centryCssClass.ActiveText,
				TableWidth = spinWidth.ValueAsInt,
				TableWidthUnits = comboWidthUnits.ActiveText,

				// TextCleanerParams:
				HtmlOut = true,
				EmNames = true
			};			

			txvResult.Buffer.Text = Model.TableClean (Host.InputText, tableCleanerParams);

			AutoCopyResults ();

			ProcessState ();
		}

		protected void OnButtonCopyClicked (object sender, EventArgs e)
		{
			CopyResults ();
		}

		protected void OnToggleAutoCopyToggled (object sender, EventArgs e)
		{
			ProcessState ();
		}
	}
}
