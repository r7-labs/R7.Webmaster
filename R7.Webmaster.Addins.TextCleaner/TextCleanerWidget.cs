//
//  TextCleanerWidget.cs
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
using R7.Webmaster.Addins.Root;
using R7.Webmaster.Core;
using System.Runtime.Remoting.Lifetime;

namespace R7.Webmaster.Addins.TextCleaner
{
	[System.ComponentModel.ToolboxItem (true)]
	[Extension(typeof(IWidgetAddin))]
	public partial class TextCleanerWidget : Gtk.Bin, ITextCleanerView, ITextInputWidgetAddin
	{
		#region ITextInputWidgetAddin implementation

		public Gtk.Widget Instance { get { return this; } }

		public Gtk.Widget FocusWidget { get { return notebook1; } }

		public ITextInputWidgetAddinHost Host { get; set; }

		public string Label { get { return "Text Cleaner"; } }

		public string SafeName { get { return "textcleaner"; } }

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
			get { return Gtk.Stock.FindAndReplace; } 
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

		// TODO: Implement in the host application
		// Stack<string> PrevSources;
		// Stack<string> NextSources;
		 
		protected void AutoCopyResults ()
		{
			if (toggleAutoCopy.Active)
			{
				CopyResults ();
			}
		}

		protected void CopyResults ()
		{
			if (radioCopyHtml.Active)
			{
				Clipboard.Text = txvResult.Buffer.Text;
			}
			else if (radioCopyText.Active)
			{
				Clipboard.Text = textviewText.Buffer.Text;
			}
			else // if (radioCopyActiveTab.Active)
			{
				if (notebook1.Page == 0)
					Clipboard.Text = txvResult.Buffer.Text;
				else
					Clipboard.Text = textviewText.Buffer.Text;
			}
		}

		private TextCleanerModel Model;

		protected Gtk.MenuToolButton buttonCopy;

		public TextCleanerWidget ()
		{
			this.Build ();

			this.Model = new TextCleanerModel ();

			// TODO: Implement in the host application
			// PrevSources = new Stack<string> ();
			// PrevSources.Push (string.Empty); // preview
			// PrevSources.Push (txvSource.Buffer.Text); // current
			// NextSources = new Stack<string> ();

			Pango.FontDescription font = Pango.FontDescription.FromString ("Monospace");
			txvResult.ModifyFont (font);
			textviewText.ModifyFont (font);

			UIManager.AddUiFromResource ("R7.Webmaster.Addins.TextCleaner.ui.CopyMenu.xml");

			buttonCopy = new Gtk.MenuToolButton (Gtk.Stock.Copy);
			buttonCopy.IsImportant = true;
			// buttonCopy.Sensitive = false;
			buttonCopy.Menu = UIManager.GetWidget ("/copyMenu");
			buttonCopy.Clicked += OnButtonCopyClicked;

			// activate copy HTML button to set copy button label
			radioCopyActiveTab.Active = true;

			ProcessState ();
		}

		// protected string PrevSourceText = string.Empty;

		protected virtual void OnSourceChanged (object sender, EventArgs e)
		{
			//var PrevSourceText = PrevSources.Peek ();

			var needProcess = IsActive && Host.AutoProcess;
			/*&&
				(txvSource.Buffer.Text.Length != PrevSourceText.Length ||
				txvSource.Buffer.Text != PrevSourceText);*/

			if (needProcess)
				OnActionProcessActivated (sender, e);

			ProcessState ();
		}

		protected void OnChkClearTablesToggled (object sender, System.EventArgs e)
		{
			ProcessState ();
		}

		protected void ProcessState()
		{
			// TODO: Implement in the host application
			// actionPrevSource.Sensitive = PrevSources.Count > 2;
			// actionNextSource.Sensitive = NextSources.Count > 0;

			// make copy button sensitive
			/* buttonCopy.Sensitive = 
				!string.IsNullOrWhiteSpace (txvResult.Buffer.Text) || !string.IsNullOrWhiteSpace (textviewText.Buffer.Text);*/
		}

		protected void OnActionProcessActivated (object sender, EventArgs e)
		{
			var textCleanerParams = new TextCleanerParams ()
			{
				EmNames = true
			};			

			textCleanerParams.HtmlOut = true;
			txvResult.Buffer.Text = Model.TextClean (Host.InputText, textCleanerParams);
					
			textCleanerParams.HtmlOut = false;
			textviewText.Buffer.Text = Model.TextClean (Host.InputText, textCleanerParams);

			AutoCopyResults ();

			ProcessState ();
		}

		protected void OnButtonCopyClicked (object sender, EventArgs e)
		{
			CopyResults ();
		}

		// TODO: Implement in the host application
		/*
		protected void OnActionNextSourceActivated (object sender, EventArgs e)
		{
			txvSource.Buffer.Changed -= OnSourceChanged;
			PrevSources.Push(NextSources.Pop ()); // pop current
			txvSource.Buffer.Text = PrevSources.Peek (); // peek preview
			txvSource.Buffer.Changed += OnSourceChanged;

			ProcessState ();
		}

		protected void OnActionPrevSourceActivated (object sender, EventArgs e)
		{
			txvSource.Buffer.Changed -= OnSourceChanged;
			NextSources.Push(PrevSources.Pop ()); // pop current
			txvSource.Buffer.Text = PrevSources.Peek (); // peek preview
			txvSource.Buffer.Changed += OnSourceChanged;

			ProcessState ();
		}*/
	
		protected void OnToggleAutoCopyToggled (object sender, EventArgs e)
		{
			ProcessState ();
		}

		protected void OnRadioCopyActivated (object sender, EventArgs e)
		{
			buttonCopy.Label = ((Gtk.RadioAction) sender).Label;
		}
	}
}
