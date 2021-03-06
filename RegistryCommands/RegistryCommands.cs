﻿//==========================================================
//Created by Derrick Ducote - admin@pcaffinity.com -
//Original ModifyRegistry.cs from Francesco Natali - fn.varie@libero.it -
//==========================================================

namespace PCAFFINITY
{
    using Microsoft.Win32;
    using System;
    using System.Collections.Generic;

    public class RegistryCommands
    {
        public RegistryCommands(RegistryView registryView, RegistryKey baseKey, string subKey, bool throwErrors = false, string remotePC = null)
        {
            BaseRegistryView = registryView;
            BaseRegistryKey = baseKey;
            SubKey = FixSubKey(subKey);
            ThrowErrors = throwErrors;
            RemotePC = remotePC;
        }

        public RegistryCommands(RegistryKey baseKey, string subKey, bool throwErrors = false, string remotePC = null)
        {
            BaseRegistryKey = baseKey;
            SubKey = FixSubKey(subKey);
            ThrowErrors = throwErrors;
            RemotePC = remotePC;
        }

        public RegistryCommands(string subKey, RegistryView registryView)
        {
            BaseRegistryView = registryView;
            SubKey = FixSubKey(subKey);
        }

        public RegistryCommands(string subKey)
        {
            SubKey = FixSubKey(subKey);
        }

        public string SubKey { get; set; } = "SOFTWARE\\" + System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
        private RegistryKey BaseRegistryKey { get; } = Registry.LocalMachine;
        private RegistryView BaseRegistryView { get; }
        private string RemotePC { get; }
        private bool ThrowErrors { get; }

        public bool DeleteKey(string KeyName)
        {
            try
            {
                using RegistryKey rk = OpenNewBaseKey();
                using RegistryKey sk1 = rk.CreateSubKey(SubKey);
                if (sk1 == null)
                {
                    return true;
                }
                else
                {
                    sk1.DeleteValue(KeyName);
                }

                return true;
            }
            catch (Exception e)
            {
                if (ThrowErrors)
                {
                    throw new InvalidOperationException($"Err Deleting: {SubKey}{Environment.NewLine}{e.Message}");
                }

                return false;
            }
        }

        public bool DeleteSubKeyTree(string KeyName, bool throwOnMissing = false)
        {
            try
            {
                using RegistryKey rk = OpenNewBaseKey();
                using RegistryKey sk1 = rk.CreateSubKey(SubKey);
                if (sk1 == null)
                {
                    return true;
                }
                else
                {
                    sk1.DeleteSubKeyTree(KeyName, throwOnMissing);
                }

                return true;
            }
            catch (Exception e)
            {
                if (ThrowErrors)
                {
                    throw new InvalidOperationException($"Err Deleting: {SubKey}{Environment.NewLine}{e.Message}");
                }

                return false;
            }
        }

        public bool DeleteSubKeyTree(bool throwOnMissing = false)
        {
            try
            {
                using RegistryKey rk = OpenNewBaseKey();
                using RegistryKey sk1 = rk.OpenSubKey(SubKey);
                if (sk1 != null)
                {
                    rk.DeleteSubKeyTree(SubKey, throwOnMissing);
                }

                return true;
            }
            catch (Exception e)
            {
                if (ThrowErrors)
                {
                    throw new InvalidOperationException($"Err Deleting: {SubKey}{Environment.NewLine}{e.Message}");
                }

                return false;
            }
        }

        public string[] GetAllSubKeys()
        {
            using RegistryKey rk = OpenNewBaseKey();
            using RegistryKey sk1 = rk.OpenSubKey(SubKey);
            try
            {
                List<string> strings = new List<string>();
                foreach (string sub in sk1.GetSubKeyNames())
                {
                    strings.Add(sub);
                }

                return strings.ToArray();
            }
            catch (Exception e)
            {
                if (ThrowErrors)
                {
                    throw new InvalidOperationException($"Err Reading keys of: {SubKey}{Environment.NewLine}{e.Message}");
                }

                return null;
            }
        }

        public string Read(string KeyName)
        {
            using RegistryKey rk = OpenNewBaseKey();
            using RegistryKey sk1 = rk.OpenSubKey(SubKey);

            if (sk1 == null)
            {
                return null;
            }
            else
            {
                try
                {
                    return (string)sk1.GetValue(KeyName);
                }
                catch (Exception e)
                {
                    if (ThrowErrors)
                    {
                        throw new InvalidOperationException($"Err Reading: {SubKey}{Environment.NewLine}{e.Message}");
                    }

                    return null;
                }
            }
        }

        public string ReadSubFolderKey(string subKeyFolder, string KeyName)
        {
            using RegistryKey rk = OpenNewBaseKey();
            using RegistryKey sk1 = rk.OpenSubKey($@"{SubKey}\{subKeyFolder}");
            if (sk1 == null)
            {
                return null;
            }
            else
            {
                try
                {
                    return (string)sk1.GetValue(KeyName);
                }
                catch (Exception e)
                {
                    if (ThrowErrors)
                    {
                        throw new InvalidOperationException($"Err Reading: {SubKey}{Environment.NewLine}{e.Message}");
                    }

                    return null;
                }
            }
        }

        public int SubKeyCount()
        {
            try
            {
                using RegistryKey rk = OpenNewBaseKey();
                using RegistryKey sk1 = rk.OpenSubKey(SubKey);
                if (sk1 != null)
                {
                    return sk1.SubKeyCount;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception e)
            {
                if (ThrowErrors)
                {
                    throw new InvalidOperationException($"Err Reading keys of: {SubKey}{Environment.NewLine}{e.Message}");
                }

                return 0;
            }
        }

        public int ValueCount()
        {
            try
            {
                using RegistryKey rk = OpenNewBaseKey();
                using RegistryKey sk1 = rk.OpenSubKey(SubKey);
                if (sk1 != null)
                {
                    return sk1.ValueCount;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception e)
            {
                if (ThrowErrors)
                {
                    throw new InvalidOperationException($"Err Reading keys of: {SubKey}{Environment.NewLine}{e.Message}");
                }

                return 0;
            }
        }

        public bool Write(string KeyName, object Value)
        {
            try
            {
                using RegistryKey rk = OpenNewBaseKey();
                using RegistryKey sk1 = rk.CreateSubKey(SubKey);
                sk1.SetValue(KeyName, Value);
                return true;
            }
            catch (Exception e)
            {
                if (ThrowErrors)
                {
                    throw new InvalidOperationException($"Err Writing: {SubKey}{Environment.NewLine}{e.Message}");
                }

                return false;
            }
        }

        private string FixSubKey(string skey)
        {
            if (skey == null)
            {
                return string.Empty;
            }

            if (skey.StartsWith("\\"))
            {
                skey = skey.Substring(1, skey.Length - 1);
            }

            return skey.Trim();
        }

        private RegistryKey OpenNewBaseKey()
        {
            RegistryHive key = BaseRegistryKey.ToString().ToUpper() switch
            {
                "HKEY_CLASSES_ROOT" => RegistryHive.ClassesRoot,
                "HKEY_USERS" => RegistryHive.Users,
                "HKEY_CURRENT_USER" => RegistryHive.CurrentUser,
                "HKEY_LOCAL_MACHINE" => RegistryHive.LocalMachine,
                _ => RegistryHive.LocalMachine,
            };

            if (RemotePC != null)
            {
                return RegistryKey.OpenRemoteBaseKey(key, RemotePC, BaseRegistryView);
            }
            else
            {
                return RegistryKey.OpenBaseKey(key, BaseRegistryView);
            }
        }
    }
}