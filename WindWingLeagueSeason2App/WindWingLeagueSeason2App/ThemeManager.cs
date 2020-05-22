using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace WindWingLeagueSeason2App
{
    public static class ThemeManager
    {
        public const string DynamicButtonColor = nameof(DynamicButtonColor);

        public const string DynamicPrimaryTextColor = nameof(DynamicPrimaryTextColor);
        public const string DynamicSecondaryTextColor = nameof(DynamicSecondaryTextColor);

        public const string DynamicNavigationPrimary = nameof(DynamicNavigationPrimary);
        public const string DynamicBackgroundColor = nameof(DynamicBackgroundColor);
        public const string DynamicBarTextColor = nameof(DynamicBarTextColor);

        public const string DynamicTopShadow = nameof(DynamicTopShadow);
        public const string DynamicBottomShadow = nameof(DynamicBottomShadow);

        public const string DynamicHasShadow = nameof(DynamicHasShadow);

        public const string Elevation4dpColor = nameof(Elevation4dpColor);

        static bool _darkMode;
        public static bool darkMode
        {
            get
            {
                return _darkMode;
            }
            set
            {
                if (value)
                {
                    SetDarkMode();
                }
                else
                {
                    SetLightMode();
                }
            }
        }



        public static void SetDynamicResource(string targetResourceName, string sourceResourceName)
        {
            if (!Application.Current.Resources.TryGetValue(sourceResourceName, out var value))
            {
                throw new InvalidOperationException($"key {sourceResourceName} not found in the resource dictionary");
            }

            Application.Current.Resources[targetResourceName] = value;
        }

        public static void SetDynamicResource<T>(string targetResourceName, T value)
        {
            Application.Current.Resources[targetResourceName] = value;
        }

        public static void SetDarkMode()
        {
            SetDynamicResource(DynamicSecondaryTextColor, "DarkThemeSecondaryTextColor");

            SetDynamicResource(DynamicPrimaryTextColor, "DarkThemeTextColor");
            SetDynamicResource(DynamicNavigationPrimary, "DarkNavigationPrimary");
            SetDynamicResource(DynamicBackgroundColor, "DarkSurface");

            _darkMode = true;

            MessagingCenter.Send(new object(), "UpdateTheme");
        }

        public static void SetLightMode()
        {
            SetDynamicResource(DynamicSecondaryTextColor, "LightThemeSecondaryTextColor");

            SetDynamicResource(DynamicPrimaryTextColor, "LightThemeTextColor");
            SetDynamicResource(DynamicNavigationPrimary, "LightNavigationPrimary");
            SetDynamicResource(DynamicBackgroundColor, "LightSurface");

            _darkMode = false;

            MessagingCenter.Send(new object(), "UpdateTheme");
        }
    }
}

