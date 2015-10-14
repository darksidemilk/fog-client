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
using Zazzles;
using Zazzles.Modules.Updater;

namespace FOG
{
    internal class MacUpdate : IUpdate
    {
        private const string LogName = "UpdateHelper";

        public void ApplyUpdate()
        {
            ProcessHandler.RunClientEXE("UnixInstaller.exe",
                $"{Settings.Get("Server")} {Settings.Get("Tray")} {Settings.Get("Company")} {Settings.Get("RootLog")} {Settings.Get("HTTPS")}");
        }

        public void StartService()
        {
            ProcessHandler.Run("launchctl", "load -w /Library/LaunchDaemons/org.freeghost.daemon.plist");
            ProcessHandler.Run("launchctl", "load -w /Library/LaunchAgents/org.freeghost.useragent.plist");
        }

        public void StopService()
        {
            ProcessHandler.Run("launchctl", "unload -w /Library/LaunchDaemons/org.freeghost.daemon.plist");
            ProcessHandler.Run("launchctl", "unload -w /Library/LaunchAgents/org.freeghost.useragent.plist");
        }
    }
}