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

namespace R7.Webmaster.Addins.TextCleaner
{
	[System.ComponentModel.ToolboxItem (true)]
	[Extension(typeof(IWidgetAddin))]
	public partial class TextCleanerWidget : Gtk.Bin, ITextCleanerView, IWidgetAddin
	{

		#region IWidgetAddin implementation

		public Gtk.Widget Instance { get { return this; } }

		public Gtk.Widget FocusWidget { get { return txvSource; } }

		public string Label { get { return "Text Cleaner"; } }

		public List<Gtk.Action> Actions 
		{
			get 
			{
				return new List<Gtk.Action> () { actionProcess, null, actionPaste, actionPasteHTML, null, 
					actionPrevSource, actionNextSource, null, actionCopy, actionCopyMarkup };
			}
		}

		#endregion

		Stack<string> PrevSources;
		Stack<string> NextSources;
		 
		protected void CopyResults ()
		{
			if (chkAutoCopy.Active)
			{
				//Gtk.Clipboard cb = txvResult.GetClipboard(...).Get(...);
				if (rbnHtmlOut.Active)
					Clipboard.Text = txvResult.Buffer.Text;
				else
					Clipboard.Text = textviewText.Buffer.Text;
			}
		}

		private TextCleanerModel Model;

		public TextCleanerWidget ()
		{
			this.Build ();

			this.Model = new TextCleanerModel ();

			PrevSources = new Stack<string> ();
			PrevSources.Push (string.Empty); // preview
			PrevSources.Push (txvSource.Buffer.Text); // current
			NextSources = new Stack<string> ();

			txvSource.Buffer.Changed += OnSourceChanged;

			Pango.FontDescription font = Pango.FontDescription.FromString ("Monospace");
			txvResult.ModifyFont (font);
			txvSource.ModifyFont (font);
			textviewText.ModifyFont (font);

			DefaultState ();
			ProcessState ();
		}

		protected void DefaultState()
		{
			//rbnAutoIn.Active = true;
			rbnHtmlOut.Active = true;
			//expanderClearTables.Expanded = false;
		}

		// protected string PrevSourceText = string.Empty;

		protected virtual void OnSourceChanged (object sender, EventArgs e)
		{
			//var PrevSourceText = PrevSources.Peek ();

			var needProcess = chkAutoProcess.Active;
			/*&&
				(txvSource.Buffer.Text.Length != PrevSourceText.Length ||
				txvSource.Buffer.Text != PrevSourceText);*/

			if (needProcess)
				OnActionProcessActivated (sender, e);

			if (!string.IsNullOrWhiteSpace (txvSource.Buffer.Text))
				PrevSources.Push (txvSource.Buffer.Text);

			ProcessState ();
		}

		protected void OnChkClearTablesToggled (object sender, System.EventArgs e)
		{
			ProcessState ();
		}

		protected void OnChkAutoCopyToggled (object sender, System.EventArgs e)
		{
			ProcessState ();
		}

		protected void OnNewActionActivated (object sender, EventArgs e)
		{
			// txvResult.Buffer.Text = Model.Parse(txvSource.Buffer.Text);
			// txvResult.Buffer.Text = HtmlTidy.Process(txvSource.Buffer.Text);
		}		

		/*
		protected void OnTextToSeoActionActivated (object sender, EventArgs e)
		{
			// find root window:
			var w = this.Parent;

			while (!(w is Window))
				w = w.Parent;

			var main = w as MainForm;

			// activate SEO tab
			main.SetActivePage(2);
			// move text to SEO input 
			main.SetSeoText(textviewText.Buffer.Text);

		}*/

		protected void ProcessState()
		{
			tableClearTablesOptions.Sensitive = chkClearTables.Active;

			hboxAutoCopyFormat.Sensitive = chkAutoCopy.Active;

			actionPrevSource.Sensitive = PrevSources.Count > 2;
			actionNextSource.Sensitive = NextSources.Count > 0;

			// make copy buttons active, if there are something to copy
			actionCopyMarkup.Sensitive = !string.IsNullOrWhiteSpace (txvResult.Buffer.Text);
			actionCopy.Sensitive = !string.IsNullOrWhiteSpace (textviewText.Buffer.Text);
		}


		protected void OnActionProcessActivated (object sender, EventArgs e)
		{
			TextCleanerParams param = new TextCleanerParams ()
			{
				TableCSSClass = entryTableClass.Text,
				TableWidth = spinTableWidth.ValueAsInt,
				TableWidthUnits = comboWidthUnits.ActiveText
			};			

			/*	
			txvResult.Buffer.Text =
				this.Model.TextClean (txvSource.Buffer.Text,
				    (rbnAutoIn.Active) ? 
						this.Model.IsHtml (txvSource.Buffer.Text) : 
						rbnHtmlIn.Active,
				    rbnHtmlOut.Active,
				    chkFixRssDate.Active, chkClearTables.Active, param);
			*/

			/* pre 141214
			txvResult.Buffer.Text =
				this.Model.TextClean (
					txvSource.Buffer.Text,
					this.Model.IsHtml (txvSource.Buffer.Text),
					true, // XML / HTML
					chkFixRssDate.Active, chkClearTables.Active,
					checkEmNames.Active, param);

			textviewText.Buffer.Text =
				this.Model.TextClean (
					txvSource.Buffer.Text,
					this.Model.IsHtml (txvSource.Buffer.Text),
					false, // Plain text
					chkFixRssDate.Active, chkClearTables.Active, 
					checkEmNames.Active, param);
			*/

			txvResult.Buffer.Text = Model.TextClean (txvSource.Buffer.Text,
				new TextCleanerParams {
					HtmlOut = true,
					EmNames = checkEmNames.Active 
				}
			);
					
			textviewText.Buffer.Text = Model.TextClean (txvSource.Buffer.Text,
				new TextCleanerParams {
					HtmlOut = false,
					EmNames = false
				}
			);


			CopyResults ();

			ProcessState ();
		}

		protected void OnActionPasteActivated (object sender, EventArgs e)
		{
			txvSource.Buffer.Text = Clipboard.Text;
		}

		protected void OnActionPasteHTMLActivated (object sender, EventArgs e)
		{
			// Try paste HTML, else paste text

			var clip = Gtk.Clipboard.Get (Gdk.Atom.Intern ("CLIPBOARD", true));
			var target = Gdk.Atom.Intern ("text/html", true);

			var selection = clip.WaitForContents (target);
			if (selection != null)
			{
				// Console.WriteLine (selection.Data.Length);
				// Console.WriteLine (selection.Type.Name); // text/html

				txvSource.Buffer.Text = System.Text.Encoding.UTF8.GetString (selection.Data, 0, selection.Data.Length);

			}
			else if (clip.WaitIsTextAvailable ())
			{
				var text = clip.WaitForText ();
				// Console.WriteLine ("Text");
				txvSource.Buffer.Text = text;
			}
			//else
			//	Console.WriteLine ("No selection"); 
		}

		protected void OnActionCopyActivated (object sender, EventArgs e)
		{
			Clipboard.Text = textviewText.Buffer.Text;
		}

		protected void OnActionCopyMarkupActivated (object sender, EventArgs e)
		{
			Clipboard.Text = txvResult.Buffer.Text;
		}

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
		}
	}
}

