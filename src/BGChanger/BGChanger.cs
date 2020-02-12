using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Microsoft.Win32;

namespace BGChanger
{
    /// <summary>
    /// Change the Windows background settings.
    /// </summary>
    public static class BGChanger
    {
        /// <summary>
        /// </summary>
        [DllImport("user32.dll")]
        public static extern bool SetSysColors(int cElements, int[] lpaElements, int[] lpaRgbValues);
        
        /// <summary>
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(uint action, uint uParam, string vParam, uint winIni);

        /// <summary>
        /// Check if the provided background is valid.
        /// </summary>
        /// <param name="background">The background file path to be checked.</param>
        private static void CheckBackground(string background)
        {
            if (string.IsNullOrWhiteSpace(background))
            {
                throw new ArgumentNullException(nameof(background));
            }

            if (!File.Exists(background))
            {
                throw new FileNotFoundException();
            }

            var extension = Path.GetExtension(background).ToLower();

            if (extension != ".bmp" && extension != ".jpg" && extension != ".jpeg" && extension != ".png")
            {
                throw new ArgumentOutOfRangeException(nameof(background), background, "The background should be a BMP, JPG, JPEG or PNG.");
            }
        }

        /// <summary>
        /// Check if the current user has the administrator role.
        /// </summary>
        private static void CheckIsAdmin()
        {
            var isAdmin = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

            if (!isAdmin)
            {
                throw new UnauthorizedAccessException();
            }
        }

        /// <summary>
        /// Checks if the provided directory can be used as a slideshow directory.
        /// </summary>
        /// <param name="directory">The directory path to be checked.</param>
        private static void CheckSlideshowDirectory(string directory)
        {
            if (string.IsNullOrWhiteSpace(directory))
            {
                throw new ArgumentNullException(nameof(directory));
            }

            if (!Directory.Exists(directory))
            {
                throw new DirectoryNotFoundException();
            }
        }

        /// <summary>
        /// Reset the default Windows desktop background.
        /// </summary>
        public static void ResetBackground()
        {
            SetPictureBackground();
        }
        
        /// <summary>
        /// Set a picture as the desktop background.
        /// </summary>
        /// <param name="background">The background file path to be used.</param>
        /// <param name="style">The background style to be used.</param>
        /// <param name="color">The fallback color to be used.</param>
        public static void SetPictureBackground(string background = @"C:\Windows\Web\Wallpaper\Windows\img0.jpg", BackgroundStyle style = BackgroundStyle.Center, Color color = default)
        {
            CheckBackground(background);

            background = Path.GetFullPath(background);

            var key1 = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", writable: true);
            key1?.SetValue(@"WallPaper", background, RegistryValueKind.String);

            switch (style)
            {
                case BackgroundStyle.Center:
                    key1?.SetValue(@"WallpaperStyle", "0", RegistryValueKind.String);
                    key1?.SetValue(@"TileWallpaper", "0", RegistryValueKind.String);
                    break;
                case BackgroundStyle.Fill:
                    key1?.SetValue(@"WallpaperStyle", "10", RegistryValueKind.String);
                    key1?.SetValue(@"TileWallpaper", "0", RegistryValueKind.String);
                    break;
                case BackgroundStyle.Fit:
                    key1?.SetValue(@"WallpaperStyle", "6", RegistryValueKind.String);
                    key1?.SetValue(@"TileWallpaper", "0", RegistryValueKind.String);
                    break;
                case BackgroundStyle.Span:
                    key1?.SetValue(@"WallpaperStyle", "22", RegistryValueKind.String);
                    key1?.SetValue(@"TileWallpaper", "0", RegistryValueKind.String);
                    break;
                case BackgroundStyle.Stretch:
                    key1?.SetValue(@"WallpaperStyle", "2", RegistryValueKind.String);
                    key1?.SetValue(@"TileWallpaper", "0", RegistryValueKind.String);
                    break;
                case BackgroundStyle.Tile:
                    key1?.SetValue(@"WallpaperStyle", "0", RegistryValueKind.String);
                    key1?.SetValue(@"TileWallpaper", "1", RegistryValueKind.String);
                    break;
            }

            key1?.Close();

            var key2 = Registry.CurrentUser.OpenSubKey(@"Control Panel\Colors", writable: true);
            key2?.SetValue(@"Background", string.Format(CultureInfo.InvariantCulture, "{0} {1} {2}", color.R, color.G, color.B), RegistryValueKind.String);
            key2?.Close();
            
            // Set the new desktop background for the current session.
            SystemParametersInfo(0x14, 0, background, 0x01 | 0x02);
        }

        /// <summary>
        /// Set a solid color as the desktop background.
        /// </summary>
        /// <param name="color">The color to be used.</param>
        public static void SetSolidColorBackground(Color color = default)
        {
            var key1 = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", writable: true);
            key1?.DeleteValue(@"WallPaper", throwOnMissingValue: false);
            key1?.Close();

            var key2 = Registry.CurrentUser.OpenSubKey(@"Control Panel\Colors", writable: true);
            key2?.SetValue(@"Background", string.Format(CultureInfo.InvariantCulture, "{0} {1} {2}", color.R, color.G, color.B), RegistryValueKind.String);
            key2?.Close();

            // Set the new desktop solid color for the current session.
            int[] elements = { 1 };
            int[] colors = { ColorTranslator.ToWin32(color) };
            SetSysColors(elements.Length, elements, colors);
        }

        /// <summary>
        /// Set a slideshow as the desktop background.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="interval">Interval to change pictures, can be any number of milliseconds.</param>
        /// <param name="shuffle"></param>
        /// <param name="runOnBattery"></param>
        /// <param name="style">The background style to be used.</param>
        /// <param name="color">The fallback color to be used.</param>
        public static void SetSlideshowBackground(string directory = @"C:\Windows\Web\Wallpaper\Theme1", int interval = 600000, bool shuffle = false, bool runOnBattery = false, BackgroundStyle style = BackgroundStyle.Center, Color color = default)
        {
            throw new NotImplementedException();
            
            CheckSlideshowDirectory(directory);

            var slideshowIniFile = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                @"Microsoft\Windows\Themes\slideshow.ini"
            );
            var encryptedPath = "";

            // Write information into the slideshow.ini file.
            File.Delete(slideshowIniFile);
            File.Create(slideshowIniFile);
            using (var sw = File.CreateText(slideshowIniFile)) 
            {
                sw.WriteLine(@"[Slideshow]");
                sw.WriteLine($"ImagesRootPIDL={encryptedPath}");
            }	

            var key2 = Registry.CurrentUser.OpenSubKey(@"Control Panel\Personalization\Desktop Slideshow", writable: true);
            key2?.SetValue("Interval", interval, RegistryValueKind.DWord);
            key2?.SetValue("Shuffle", (shuffle) ? 1 : 0, RegistryValueKind.DWord);
        }
    }
}
