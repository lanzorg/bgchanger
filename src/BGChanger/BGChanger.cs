using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Microsoft.Win32;

namespace BGChanger
{
    public static class BGChanger
    {
        const string DefaultBackground = @"C:\Windows\Web\Wallpaper\Windows\img0.jpg";
        const string DefaultSolidColor = "0 0 0";
        const BackgroundStyle DefaultBackgroundStyle = BackgroundStyle.Center;
        const SlideshowDuration DefaultSlideshowDuration = SlideshowDuration.TenMinutes;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(uint action, uint uParam, string vParam, uint winIni);

        private static void CheckIsAdmin()
        {
            var isAdmin = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

            if (!isAdmin)
            {
                throw new UnauthorizedAccessException();
            }
        }

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
                throw new ArgumentOutOfRangeException(nameof(background), "The background should be a JPEG or PNG.");
            }
        }

        private static void CheckSolidColor(string color)
        {
            throw new NotImplementedException();
        }

        public static void ResetCurrentBackground()
        {
            SetCurrentBackground();
        }

        public static void ResetMachineBackground()
        {
            CheckIsAdmin();

            throw new NotImplementedException();
        }

        public static void SetCurrentBackground(string background = DefaultBackground, BackgroundStyle style = DefaultBackgroundStyle, string color = DefaultSolidColor)
        {
            CheckBackground(background);
            CheckSolidColor(color);

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

        public static void SetCurrentSolidColor(string color = DefaultSolidColor)
        {
            CheckSolidColor(color);

            var key1 = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", writable: true);
            key1?.DeleteValue(@"WallPaper");
            key1?.Close();

            var key2 = Registry.CurrentUser.OpenSubKey(@"Control Panel\Colors", writable: true);
            key2?.SetValue(@"Background", color, RegistryValueKind.String);
            key2?.Close();
        }

        public static void SetCurrentSlideshow(string directory, SlideshowDuration duration = DefaultSlideshowDuration, bool shuffle = false, bool runOnBattery = false, BackgroundStyle style = DefaultBackgroundStyle, string color = DefaultSolidColor)
        {
            CheckSolidColor(color);

            throw new NotImplementedException();
        }

        public static void SetMachineBackground(string background = DefaultBackground, BackgroundStyle style = DefaultBackgroundStyle, string color = DefaultSolidColor)
        {
            CheckIsAdmin();
            CheckBackground(background);
            CheckSolidColor(color);

            background = Path.GetFullPath(background);

            throw new NotImplementedException();
        }

        public static void SetMachineSolidColor(string color = DefaultSolidColor)
        {
            CheckIsAdmin();
            CheckSolidColor(color);

            throw new NotImplementedException();
        }

        public static void SetMachineSlideshow(string directory, SlideshowDuration duration = DefaultSlideshowDuration, bool shuffle = false, bool runOnBattery = false, BackgroundStyle style = DefaultBackgroundStyle, string color = DefaultSolidColor)
        {
            CheckIsAdmin();
            CheckSolidColor(color);

            throw new NotImplementedException();
        }
    }
}
