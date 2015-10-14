﻿/*
 * FOG Service : A computer management client for the FOG Project
 * Copyright (C) 2014-2015 FOG Project
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 3
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Zazzles;

namespace FOG
{
    /// <summary>
    ///     Coordinate all user specific FOG modules
    /// </summary>
    internal class main
    {
        private const string LogName = "UserService";
        private static AbstractService _fogService;

        public static void Main(string[] args)
        {
            Log.FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "fog_user.log");

            AppDomain.CurrentDomain.UnhandledException += Log.UnhandledException;

            // Wait for the main service to spawn
            while (Process.GetProcessesByName("FOGService").Length == 0)
            {
                Thread.Sleep(500);
            }
            Thread.Sleep(1000);

            Eager.Initalize();

            Log.Entry(LogName, "Initializing");

            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "updating.info")))
            {
                Log.Entry(LogName, "Update.info found, exiting program");
                UpdateWaiterHelper.SpawnUpdateWaiter(Settings.Location);
                Environment.Exit(0);
            }

            _fogService = new FOGUserService();
            _fogService.Start();

            if (Settings.Get("Tray").Equals("1") && Settings.OS == Settings.OSType.Windows)
                StartTray();
        }

        private static void StartTray()
        {
            ProcessHandler.RunClientEXE("FOGTray.exe", "", false);
        }
    }
}