//
//  ConfigBase.cs
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
using System.IO;
using System.Reflection;
using System.Configuration;
using System.Collections.Specialized;
using Nini.Config;

namespace R7.Webmaster.Core
{
	public abstract class ConfigBase
	{
		protected IConfigSource ConfigSource;

		protected IConfig CommonConfig;

		protected IConfig PlatformConfig;

		protected string ConfigName;
	
		protected string ConfigSubFolder;

		protected ConfigBase (string configName, string configSubFolder)
		{
			ConfigSubFolder = configSubFolder;
			ConfigName = configName;

			// base (original) and user config files
			var baseConfigFile = Path.Combine (Path.GetDirectoryName (Assembly.GetExecutingAssembly ().Location), configName + ".config");
			var userConfigFile = Path.Combine (ApplicationData, configName + ".config");

			#if DEBUG

			// create app data directory, if needed
			if (!Directory.Exists (ApplicationData))
				Directory.CreateDirectory (ApplicationData);

			// always replace user config file with original one
			File.Copy (baseConfigFile, userConfigFile, true);

			#endif

			// copy base config to the user config
			if (!File.Exists (userConfigFile))
			{
				// create app data directory, if needed
				if (!Directory.Exists (ApplicationData))
					Directory.CreateDirectory (ApplicationData);

				File.Copy (baseConfigFile, userConfigFile);
			}

			// get config source
			ConfigSource = new DotNetConfigSource (userConfigFile);

			// get configs (config sections)
 			CommonConfig = ConfigSource.Configs ["common"];
			PlatformConfig = ConfigSource.Configs [Platform + "Platform"];
		}

		/// <summary>
		/// Saves changes to user config
		/// </summary>
		public void Save()
		{
			ConfigSource.Save ();
		}

		public string ApplicationData
		{
			get 
			{ 
				return Path.Combine ( 
					Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData), ConfigSubFolder); 
			}
		}

		#region Platform

		private string platform;

		public string Platform
		{
			get 
			{ 
				if (platform == null)
				{
					var platformId = Environment.OSVersion.Platform;

					switch (platformId)
					{
						case PlatformID.Win32Windows:
						case PlatformID.Win32NT:
						case PlatformID.WinCE: 
							platform = "windows";
							break;

						case PlatformID.Unix:
							platform = "unix";
							break;

						case PlatformID.Xbox:
							platform = "xbox";
							break;

						case PlatformID.MacOSX:
							platform = "macosx";
							break;

						default:
							platform = "unknown";
							break;
					}
				}

				return platform;
			}
		}

		public bool OnUnix
		{
			get { return Platform == "unix"; }
		}

		public bool OnWindows
		{
			get { return Platform == "windows"; }
		}

		#endregion
	}
}



