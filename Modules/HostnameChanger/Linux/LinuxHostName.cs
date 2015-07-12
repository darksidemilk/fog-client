﻿using System;
using System.IO;
using System.Linq;
using FOG.Handlers;
using FOG.Handlers.Middleware;

namespace FOG.Modules.HostnameChanger.Linux
{
    class LinuxHostName : IHostName
    {
        private string Name = "HostnameChanger";
        private string currentHostName;

        public void RenameComputer(string hostname)
        {
            currentHostName = Environment.MachineName;
            
            BruteForce(hostname);
        }

        private void BruteForce(string hostname)
        {
            Log.Entry(Name, "Brute forcing hostname change...");
            UpdateHostname(hostname);
            UpdateHOSTNAME(hostname);
            UpdateHosts(hostname);
            UpdateNetwork(hostname);
        }

        public bool RegisterComputer(Response response)
        {
            throw new NotImplementedException();
        }

        public void UnRegisterComputer(Response response)
        {
            throw new NotImplementedException();
        }

        public void ActivateComputer(string key)
        {
            throw new NotImplementedException();
        }

        private void ReplaceAll(string file, string hostname)
        {
            if (!File.Exists(file))
            {
                Log.Error(Name, "--> Did not find " + file);
                return;
            }

            try
            {
                File.WriteAllText(file, hostname);
                Log.Entry(Name, "--> Success " + file);
            }
            catch (Exception ex)
            {
                Log.Error(Name, "--> Failed " + file);
                Log.Error(Name, "----> " + ex.Message);
            }
        }

        private void UpdateHostname(string hostname)
        {
            ReplaceAll(@"/etc/hostname", hostname);
        }

        private void UpdateHOSTNAME(string hostname)
        {
            ReplaceAll(@"/etc/HOSTNAME", hostname);
        }

        private void UpdateHosts(string hostname)
        {
            var file = @"/etc/hosts";

            if (!File.Exists(file))
            {
                Log.Error(Name, "--> Did not find " + file);
                return;
            }

            try
            {
                var lines = File.ReadAllLines(file);

                for (var i = 0; i < lines.Length; i++)
                {
                    var ip = "127.0.1.1";

                    if (!lines[i].Contains(ip)) continue;

                    lines[i] = ip + "   " + hostname;

                }

                File.WriteAllLines(file, lines);
                Log.Entry(Name, "--> Success " + file);
            }
            catch (Exception ex)
            {
                Log.Error(Name, "--> Failed " + file);
                Log.Error(Name, "----> " + ex.Message);
            }
        }

        private void UpdateNetwork(string hostname)
        {
            var file = @"/etc/sysconfig/network";

            if (!File.Exists(file))
            {
                Log.Error(Name, "--> Did not find " + file);
                return;
            }

            try
            {
                var lines = File.ReadAllLines(file);

                for (var i = 0; i < lines.Length; i++)
                {
                    if (!lines[i].Trim().Contains("HOSTNAME=")) continue;
                    var parts = lines[i].Split('=');

                    if (parts.Length < 2) return;

                    if (parts[1].Contains("."))
                    {
                        var dots = parts[1].Split('.');
                        dots[0] = hostname;
                        parts[1] = string.Join(".", dots);
                    }
                    else
                    {
                        parts[1] = hostname;
                    }

                    lines[i] = parts[0] + "=" + parts[1];
                }

                File.WriteAllLines(file, lines);
                Log.Entry(Name, "--> Success " + file);
            }
            catch (Exception ex)
            {
                Log.Error(Name, "--> Failed " + file);
                Log.Error(Name, "----> " + ex.Message);
            }
        }
    }
}
