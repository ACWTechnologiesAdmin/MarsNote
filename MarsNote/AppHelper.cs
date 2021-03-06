﻿using System;
using System.Deployment.Application;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using MahApps.Metro;
using Microsoft.Win32;

namespace MarsNote
{
    public static class AppHelper
    {
        private const string StartupKeyName = "MarsNote";

        private const string StartupSubKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        [Flags]
        public enum WindowCorners
        {
            TopLeft = 1,
            TopRight = 2,
            BottomLeft = 4,
            BottomRight = 8
        }

        /// <summary>
        /// Add a startup key in the registry of the current logged in user.
        /// </summary>
        public static void AddCurrentUserStartup()
        {
            if (!File.Exists(FileHelper.StartupFileLocation))
            {
                FileHelper.CreateFileAndDirectory(FileHelper.StartupFileLocation)?.Close();
            }

            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(StartupSubKey, true))
            {
                key?.SetValue(StartupKeyName, $"\"{FileHelper.StartupFileLocation}\"");
            }
        }

        /// <summary>
        /// Checks whether all selected corners are visible on the user's screen(s).
        /// </summary>
        /// <param name="window">The <see cref="Window"/> to check.</param>
        /// <param name="corners">The corners of the window to check.</param>
        public static bool AllCornersOnScreen(this Window window, WindowCorners corners)
        {
            if (window == null) { throw new ArgumentNullException(nameof(window)); }
            if (corners == 0) { throw new ArgumentOutOfRangeException(nameof(corners), "There are no corners selected."); }

            // For each screen
            foreach (Screen screen in Screen.AllScreens)
            {
                // If the selected corners contains TopLeft
                if (corners.HasFlag(WindowCorners.TopLeft))
                {
                    // Find the exact point of the top left corner of the window
                    var windowTopLeft = new System.Drawing.Point((int)window.Left, (int)window.Top);
                    // If the current screen has that point in its working area, remove the flag from the selected corners
                    if (screen.WorkingArea.Contains(windowTopLeft)) { corners = corners & ~WindowCorners.TopLeft; }
                }
                if (corners.HasFlag(WindowCorners.TopRight))
                {
                    var windowTopRight = new System.Drawing.Point((int)(window.Left + window.Width), (int)window.Top);
                    if (screen.WorkingArea.Contains(windowTopRight)) { corners = corners & ~WindowCorners.TopRight; }
                }
                if (corners.HasFlag(WindowCorners.BottomLeft))
                {
                    var windowBottomLeft = new System.Drawing.Point((int)window.Left, (int)(window.Top + window.Height));
                    if (screen.WorkingArea.Contains(windowBottomLeft)) { corners = corners & ~WindowCorners.BottomLeft; }
                }
                if (corners.HasFlag(WindowCorners.BottomRight))
                {
                    var windowBottomRight = new System.Drawing.Point((int)(window.Left + window.Width), (int)(window.Top + window.Height));
                    if (screen.WorkingArea.Contains(windowBottomRight)) { corners = corners & ~WindowCorners.BottomRight; }
                }
            }

            // If there are no corners that have not been found, return true, else false
            return corners == 0;
        }

        /// <summary>
        /// Checks whether at least one selected corner is visible on the user's screen.
        /// </summary>
        /// <param name="window">The <see cref="Window"/> to check.</param>
        /// <param name="corners">The corners of the window to check.</param>
        public static bool AnyCornersOnScreen(this Window window, WindowCorners corners)
        {
            if (window == null) { throw new ArgumentNullException(nameof(window)); }
            if (corners == 0) { throw new ArgumentOutOfRangeException(nameof(corners), "There are no corners selected."); }

            // For each screen
            foreach (Screen screen in Screen.AllScreens)
            {
                // If the selected corners contains TopLeft
                if (corners.HasFlag(WindowCorners.TopLeft))
                {
                    // Find the exact point of the top left corner of the window
                    var windowTopLeft = new System.Drawing.Point((int)window.Left, (int)window.Top);
                    // If the current screen has that point in its working area, return true
                    if (screen.WorkingArea.Contains(windowTopLeft)) { return true; }
                }
                if (corners.HasFlag(WindowCorners.TopRight))
                {
                    var windowTopRight = new System.Drawing.Point((int)(window.Left + window.Width), (int)window.Top);
                    if (screen.WorkingArea.Contains(windowTopRight)) { return true; }
                }
                if (corners.HasFlag(WindowCorners.BottomLeft))
                {
                    var windowBottomLeft = new System.Drawing.Point((int)window.Left, (int)(window.Top + window.Height));
                    if (screen.WorkingArea.Contains(windowBottomLeft)) { return true; }
                }
                if (corners.HasFlag(WindowCorners.BottomRight))
                {
                    var windowBottomRight = new System.Drawing.Point((int)(window.Left + window.Width), (int)(window.Top + window.Height));
                    if (screen.WorkingArea.Contains(windowBottomRight)) { return true; }
                }
            }

            return false;
        }

        /// <summary>
        /// Centers a window on the current user's primary screen.
        /// </summary>
        /// <param name="window">The <see cref="Window"/> to center.</param>
        public static void CenterOnScreen(this Window window)
        {
            if (window == null) { throw new ArgumentNullException(nameof(window)); }

            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;
            double windowWidth = window.Width;
            double windowHeight = window.Height;
            window.Left = screenWidth / 2 - windowWidth / 2;
            window.Top = screenHeight / 2 - windowHeight / 2;
        }

        /// <summary>
        /// Changes the application accent colour.
        /// </summary>
        /// <param name="accent">The new accent.</param>
        public static void ChangeAccent(Accent accent)
        {
            if (accent != null)
            {
                ThemeManager.ChangeAppStyle(System.Windows.Application.Current, accent, ThemeManager.GetAppTheme("BaseLight"));
            }
        }

        /// <summary>
        /// Changes the application accent colour.
        /// </summary>
        /// <param name="accentName">The name of the new accent.</param>
        public static void ChangeAccent(string accentName)
        {
            ChangeAccent(ThemeManager.GetAccent(accentName));
        }

        /// <summary>
        /// Get the current version of the network deployed application, formatted as "major.minor[.build[.revision]]".
        /// Returns "Unavailable" if the version could not be retrieved.
        /// </summary>
        public static string GetCurrentVersion()
        {
            return ApplicationDeployment.IsNetworkDeployed ? ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString() : "Unavailable";
        }

        /// <summary>
        /// Checks if the current logged in user has a startup key in the registry.
        /// </summary>
        public static bool IsInCurrentUserStartup()
        {
            try
            {
                // Open the registry 'Run' subkey
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(StartupSubKey))
                {
                    // Return true if the 'MarsNote' key exists, and false if it does not
                    return key?.GetValue(StartupKeyName) != null;
                }
            }
            catch (System.Security.SecurityException)
            {
                //The user does not have the permissions required to read the registry key
                return false;
            }
        }

        /// <summary>
        /// Remove the startup key from the registry of the current logged in user, if it exists.
        /// </summary>
        public static void RemoveCurrentUserStartup()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(StartupSubKey, true))
            {
                key?.DeleteValue(StartupKeyName, false);
            }
        }

        /// <summary>
        /// Close the current application instance and start another.
        /// </summary>
        public static void RestartApplication(int exitCode = 0)
        {
            System.Windows.Forms.Application.Restart();
            System.Windows.Application.Current.Shutdown(exitCode);
        }

        /// <summary>
        /// Close the current application instance.
        /// </summary>
        public static void ShutdownApplication(int exitCode = 0)
        {
            System.Windows.Application.Current.Shutdown(exitCode);
        }
    }
}
