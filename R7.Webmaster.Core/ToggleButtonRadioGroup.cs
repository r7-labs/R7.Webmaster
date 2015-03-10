//
//  ToggleButtonRadioGroup.cs
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
using System.Collections;
using System.Collections.Generic;
using Gtk;
using System.Diagnostics;
using System.Security.Cryptography;
using Pango;
using System.Runtime.InteropServices;

namespace R7.Webmaster.Core
{
	// TODO: Sync with https://github.com/roman-yagodin/Gtk.R7/blob/master/Gtk.R7.Accordeon.cs

	public class ToggleButtonRadioGroup: IEnumerable<Gtk.ToggleButton>
	{
		protected List<Gtk.ToggleButton> ToggleButtons { get; set; }

		public ToggleButtonRadioGroup ()
		{
		}

		public ToggleButtonRadioGroup (params Gtk.ToggleButton [] buttons)
		{
			ToggleButtons = new List<Gtk.ToggleButton> (buttons);
			foreach (var button in ToggleButtons )
				button.Toggled += OnToggleButtonActivated;
		}

		public void Add (IEnumerable<Gtk.ToggleButton> buttons)
		{
			ToggleButtons.AddRange (buttons);
			foreach (var button in ToggleButtons)
				button.Toggled += OnToggleButtonActivated;
		}

		public void Activate (int index)
		{
			var i = 0;
			foreach (var button in ToggleButtons)
				button.Active = (i++ == index);
		}

		public Gtk.ToggleButton Active
		{
			get
			{ 
				foreach (var button in ToggleButtons)
					if (button.Active)
						return button;

				return null;
			}
		}

		private bool enableEventProcessing = true;

		private void OnToggleButtonActivated (object sender, EventArgs e)
		{
			if (enableEventProcessing && sender is ToggleButton)
			{
				foreach (var button in ToggleButtons)
				{
					// disable processing of this event,
					// as we trigger it by setting Active property
					enableEventProcessing = false;
					button.Active = (button == sender);
					enableEventProcessing = true;
				}
			}
		}

		#region IEnumerable implementation

		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns>
		/// The enumerator.
		/// </returns>
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return ToggleButtons.GetEnumerator();
		}

		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns>
		/// The enumerator.
		/// </returns>
		public IEnumerator<Gtk.ToggleButton> GetEnumerator ()
		{
			return ToggleButtons.GetEnumerator();
		}

		#endregion
	}
}
