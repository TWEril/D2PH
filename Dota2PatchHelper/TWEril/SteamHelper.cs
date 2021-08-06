using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWEril
{
    internal static class SteamHelper
    {
        private const string WindowsUninstallRegPathX86_X64 = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
        private const string WindowsUninstallRegPathX86 = @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall";

        private const string Dota2RegPathX86_X64 = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 570";
        private const string SteamRegPathX86 = @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\Steam";

        /// <summary>
        /// 获取Steam 根目录
        /// </summary>
        public static string GetSteamPath()
        {
            RegistryKey curkey = null;
            if (Environment.Is64BitOperatingSystem)
            {
                try
                {
                    curkey = Registry.LocalMachine.OpenSubKey(SteamRegPathX86);
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                try
                {
                    curkey = Registry.LocalMachine.OpenSubKey(SteamRegPathX86);
                }
                catch
                {
                    return null;
                }
            }

            if (curkey != null)
            {
                string p = null;
                try
                {
                    var uapp = curkey.GetValue("UninstallString")?.ToString();
                    if (uapp != null)
                    {
                        p = uapp.Replace("uninstall.exe", "");
                    }
                }
                catch
                {
                    return null;
                }
                return p;
            }

            return null;
        }

        /// <summary>
        /// 获取dota2 根目录
        /// </summary>
        public static string GetDota2Path()
        {
            RegistryKey curkey = null;
            try
            {
                curkey = Registry.LocalMachine.OpenSubKey(Dota2RegPathX86_X64);
            }
            catch
            {
                return null;
            }
            if (curkey != null)
            {
                string p = null;
                try
                {
                    p = curkey.GetValue("InstallLocation")?.ToString();
                }
                catch
                {
                    return null;
                }
                return p;
            }

            return null;
        }
    }
}
