//
//  UnixListenerSingleInstance.cs
//
//  Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
//  Copyright (c) 2016 Roman M. Yagodin
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
using System.IO;
using System.Threading;
using Mono.Unix;

namespace R7.Webmaster.Core
{
    public class UnixListenerSingleInstance: ISingleInstance
    {
        protected readonly string SocketFile;

        protected UnixListener Listener;

        protected readonly Thread WatchThread;

        protected readonly EventHandler InvokeHandler;

        public UnixListenerSingleInstance (string waitHandleName, EventHandler invokeHandler)
        {
            SocketFile = Path.Combine (Path.GetTempPath (), waitHandleName);
            InvokeHandler = invokeHandler;
            WatchThread = new Thread (new ThreadStart (WatchThreadRoutine));
        }

        private static void SafeDeleteFile (string fileName)
        {
            if (File.Exists (fileName))
            {
                File.Delete (fileName);
            }
        }

        private void WatchThreadRoutine ()
        {
            try 
            {
                Listener = new UnixListener (SocketFile);
                Listener.Start ();

                while (true)
                {
                    // wait for client connection
                    Listener.AcceptUnixClient ();

                    // invoke application
                    Gtk.Application.Invoke (InvokeHandler);
                }
            }
            catch (ThreadAbortException)
            {
            }
        }

        #region ISingleInstance implementation

        public bool TryEnter ()
        {
            var canEnter = false;

            if (File.Exists (SocketFile))
            {
                var client = new UnixClient ();
                try
                {
                    // try to connect (will invoke original instance on connection)
                    client.Connect (SocketFile);
                    client.Close ();
                }
                catch
                {
                    // cannot connect - delete orphaned lock file
                    SafeDeleteFile (SocketFile);

                    canEnter = true;
                }
                finally
                {
                    client.Dispose ();
                }
            }
            else
            {
                canEnter = true;
            }

            if (canEnter)
            {
                WatchThread.Start ();    
            }
       
            return canEnter;
        }

        public void Leave ()
        {
            WatchThread.Abort ();
            WatchThread.Join ();

            if (Listener != null)
            {
                Listener.Stop ();
                Listener.Dispose ();
            }

            // delete socket file
            SafeDeleteFile (SocketFile);
        }

        #endregion
    }
}
