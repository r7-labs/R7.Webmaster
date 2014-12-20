//
//  Program.cs
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
using R7.Webmaster.Core;

namespace R7.Webmaster
{
	class Program
	{
		public static readonly AppConfig AppConfig;

		protected static readonly IInvocableSingleInstance AppInstance;

		protected static MainWindow MainWindow;

		protected static void OnInvoke (object sender, EventArgs e)
		{
			MainWindow.Restore ();
		}
	
		static Program ()
		{
			AppConfig = new AppConfig ();
			AppInstance = new InvocableSingleInstance ("R7.Webmaster", OnInvoke);
		}

		public static void Main (string[] args)
		{
			if (AppInstance.TryEnter ())
			{
				try
				{
					Gtk.Application.Init ();
					MainWindow = new MainWindow ();
					MainWindow.Show ();
					Gtk.Application.Run ();
				}
				catch (Exception ex)
				{
					throw ex;
				}
				finally
				{
					AppInstance.Leave ();

					#if DEBUG
					Console.WriteLine ("Exiting...");
					#endif
				}
			}
			else
			{
				// send signal to running instance
				AppInstance.Invoke ();
			}
		}
	}
}
