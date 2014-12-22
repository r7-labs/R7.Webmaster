//
//  InvocableSingleInstance.cs
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
using System.Threading;

namespace R7.Webmaster.Core
{
	public class InvocableSingleInstance: IInvocableSingleInstance
	{
		protected readonly Semaphore WaitHandle;

		protected readonly Thread WatchThread;

		public InvocableSingleInstance (string waitHandleName, EventHandler invokeHandler)
		{
			WaitHandle = new Semaphore (1, 1, waitHandleName);
			WatchThread = new Thread (new ThreadStart (WatchThreadRoutine));
			InvokeHandler = invokeHandler;
		}

		private void WatchThreadRoutine ()
		{
			try 
			{
				// wait for signal, invoke and block again
				while (WaitHandle.WaitOne ()) 
				{
					Gtk.Application.Invoke (InvokeHandler);
				}
			}
			catch (ThreadAbortException ex)
			{
			}
		}

		#region IInvocableSingleInstance implementation

		public bool TryEnter ()
		{
			if (WaitHandle.WaitOne (0))
			{
				WatchThread.Start ();

				return true;
			}

			return false;
		}

		public void Leave ()
		{
			try
			{
				WatchThread.Abort ();
				WaitHandle.Release ();
			}
			finally
			{
				WaitHandle.Close ();
			}
		}

		public void Invoke ()
		{
			// decrement semaphore counter to send signal 
			// to the watch thread of the running instance
			WaitHandle.Release ();
		}

		#endregion
	}
}

