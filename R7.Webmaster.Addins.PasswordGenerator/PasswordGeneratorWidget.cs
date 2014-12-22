//
//  PasswordGeneratorWidget.cs
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
using Mono.Addins;
using R7.Webmaster.Addins.Root;


namespace R7.Webmaster.Addins.PasswordGenerator
{
	[System.ComponentModel.ToolboxItem (true)]
	[Extension(typeof(IWidgetAddin))]
	public partial class PasswordGeneratorWidget : Gtk.Bin, IPasswordGeneratorView, IWidgetAddin
	{
		private PasswordGeneratorModel model;

		#region IWidgetAddin implementation

		public Gtk.Widget Instance { get { return this; } }

		public Gtk.Widget FocusWidget { get { return spinbutton1; } }

		public string Label { get { return "Password Generator"; } }

		public string SafeName { get { return "passwordgenerator"; } }

		public string Icon 
		{ 
			get { return Gtk.Stock.DialogAuthentication; } 
		}

		public List<Gtk.Action> Actions 
		{
			get 
			{
				return new List<Gtk.Action> () { actionGenerate, null, actionGuid };
			}
		}

		#endregion

		public PasswordGeneratorWidget ()
		{
			this.Build ();

			model = new PasswordGeneratorModel();

			Pango.FontDescription font = Pango.FontDescription.FromString ("Monospace");
			textview1.ModifyFont (font);

			// if (AppConfig.OnUnix)
			// 	seahorseAction.Sensitive = true;
		}

		protected void OnActionGuidActivated (object sender, System.EventArgs e)
		{
			// generate N guids
			var lastGuid = "";

			if (checkInsertSeparator.Active)
			{
				var separator = "\n".PadLeft(Guid.Empty.ToString().Length + 1, '-') ;
				textview1.Buffer.Text += separator;
			}

			for (var i = 0; i < spinNPasswords.ValueAsInt; i++)
			{
				lastGuid = model.GenerateGuid ();
				textview1.Buffer.Text += lastGuid + "\n";

				// try to scroll to the end of text
				// CHECK: we make it for every line, cause if we add more than 1 line, scrollbar position is wrong
				textview1.ScrollToIter(textview1.Buffer.EndIter, 0, false, 0, 0);
				GtkScrolledWindow.Vadjustment.Value = GtkScrolledWindow.Vadjustment.Upper;
			}
			entry1.Text = lastGuid;
		}

		protected void OnActionSaveAsActivated (object sender, EventArgs e)
		{
			/*
			var sd = new FileChooserDialog (Stock.SaveAs, Program.mainForm, FileChooserAction.Save,
				Stock.SaveAs, ResponseType.Ok, Stock.Cancel, ResponseType.Cancel);
			if (sd.Run () == (int)ResponseType.Ok)
				File.WriteAllText(sd.Filename, textview1.Buffer.Text);

			sd.Destroy();*/
		}

		protected void OnCheckIncludeCustomToggled (object sender, EventArgs e)
		{
			entryCustomChars.Sensitive = checkIncludeCustom.Active;
		}

		protected void OnSeahorseActionActivated(object sender, EventArgs e)
		{
			// TODO: add password management software to config
			// if (AppConfig.OnUnix)
			//	System.Diagnostics.Process.Start("seahorse");
		}

		protected void OnActionGenerateActivated (object sender, EventArgs e)
		{
			// generate N passwords
			var lastPassword = "";

			if (checkInsertSeparator.Active)
			{
				var separator = "\n".PadLeft(spinbutton1.ValueAsInt + 1, '-') ;
				textview1.Buffer.Text += separator;
			}

			for (var i = 0; i < spinNPasswords.ValueAsInt; i++)
			{
				lastPassword = model.Generate (spinbutton1.ValueAsInt, 
					checkAllowRepeated.Active,
					checkIncludeUnderscore.Active, 
					checkIncludeSpecialChars.Active,
					checkIncludeDigits.Active,
					checkIncludeUppercase.Active,
					checkIncludeLowercase.Active,
					checkIncludeCustom.Active,
					entryCustomChars.Text);

				textview1.Buffer.Text += lastPassword + "\n";

				// try to scroll to the end of text
				// CHECK: we make it for every line, cause if we add more than 1 line, scrollbar position is wrong
				textview1.ScrollToIter(textview1.Buffer.EndIter, 0, false, 0, 0);
				GtkScrolledWindow.Vadjustment.Value = GtkScrolledWindow.Vadjustment.Upper;
			}
			entry1.Text = lastPassword;
		}
	}
}

