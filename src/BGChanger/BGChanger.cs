using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace BGChanger
{
    /// <summary>
    /// Change the Windows background settings.
    /// </summary>
    public static class BGChanger
    {
        private const string DefaultBackground = @"C:\Windows\Web\Wallpaper\Windows\img0.jpg";
        private const BackgroundStyle DefaultBackgroundStyle = BackgroundStyle.Center;
        private const string DefaultSolidColor = "0 0 0";
        private const string DefaultSlideshowDirectory = @"C:\Windows\Web\Wallpaper\Theme1";
        private const int DefaultSlideshowInterval = 600000;

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

            if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
            {
                throw new ArgumentOutOfRangeException(nameof(background), background, "The background should be a JPG, JPEG or PNG.");
            }
        }

        /// <summary>
        /// Check if the provided color is valid.
        /// </summary>
        /// <param name="color">The color string to be checked.</param>
        private static void CheckColor(string color)
        {
            if (string.IsNullOrWhiteSpace(color))
            {
                throw new ArgumentNullException(nameof(color));
            }

            // TODO: Make the regular expression more robust.
            var regex = new Regex(@"\d{1,3} \d{1,3} \d{1,3}");
            var match = regex.Match(color);

            if (!match.Success)
            {
                throw new ArgumentOutOfRangeException(nameof(color), color, $"The color should match this {regex} regular expression.");
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
        /// <param name="color">The fallback color string to be used.</param>
        public static void SetPictureBackground(string background = DefaultBackground, BackgroundStyle style = DefaultBackgroundStyle, string color = DefaultSolidColor)
        {
            CheckBackground(background);
            CheckColor(color);

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
            key2?.SetValue(@"Background", color, RegistryValueKind.String);
            key2?.Close();

            SystemParametersInfo(0x14, 0, background, 0x01 | 0x02);
        }
        
        /// <summary>
        /// Set a solid color as the desktop background.
        /// </summary>
        /// <param name="color">The color string to be used.</param>
        public static void SetSolidColorBackground(string color = DefaultSolidColor)
        {
            CheckColor(color);

            var key1 = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", writable: true);
            key1?.DeleteValue(@"WallPaper");
            key1?.Close();

            var key2 = Registry.CurrentUser.OpenSubKey(@"Control Panel\Colors", writable: true);
            key2?.SetValue(@"Background", color, RegistryValueKind.String);
            key2?.Close();
        }

        /// <summary>
        /// WIP: Set a slideshow as the desktop background.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="interval">Interval to change pictures, can be any number of milliseconds.</param>
        /// <param name="shuffle"></param>
        /// <param name="runOnBattery"></param>
        /// <param name="style">The background style to be used.</param>
        /// <param name="color">The fallback color string to be used.</param>
        public static void SetSlideshowBackground(string directory = DefaultSlideshowDirectory, int interval = DefaultSlideshowInterval, bool shuffle = false, bool runOnBattery = false, BackgroundStyle style = DefaultBackgroundStyle, string color = DefaultSolidColor)
        {
            CheckSlideshowDirectory(directory);
            CheckColor(color);

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
            
            throw new NotImplementedException();
        }
    }
}
