using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace korjornalen.Utils
{
    public class CultureHelper
    {
        // Culture implementation based on this article: http://afana.me/post/aspnet-mvc-internationalization.aspx

        private static readonly List<string> _implementedCultures = new List<string>() {
            "en",
            "sv"
        };
        

        /// <summary>
        /// Returns an implemented culture name string, based on the "cultureName" parameter.
        /// </summary>
        /// <param name="name" />Culture's name (e.g. en-US)</param>
        public static string GetImplementedCulture(string cultureName)
        {
            if (string.IsNullOrEmpty(cultureName))
                return GetDefaultCulture();

            if (_implementedCultures.Where(c => c.Equals(cultureName, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return cultureName; // Auto-accept culture names that match an already implemented culture.

            return GetClosestImplementedCulture(cultureName);
        }

        public static bool IsSupported(string languageCode)
        {
            return _implementedCultures.Contains(languageCode);
        }

        /// <summary>
        /// Tries to find a culture that shares the same language, if it doesn't it just returns the default culture for the application.
        /// </summary>
        /// <param name="cultureName"></param>
        /// <returns></returns>
        private static string GetClosestImplementedCulture(string cultureName) {
            var n = GetNeutralCulture(cultureName);
            foreach (var c in _implementedCultures)
                if (c.StartsWith(n))
                    return c;

            return GetDefaultCulture();
        }


        /// <summary>
        /// Receives a culture name string (Example: en-GB) and removes the locale part of the name (From previous example that would be the "-GB"-part), if it exists.
        /// </summary>
        /// <param name="cultureName"></param>
        /// <returns></returns>
        private static string GetNeutralCulture(string cultureName) {
            if (!cultureName.Contains('-'))
                return cultureName;

            return cultureName.Split('-')[0];
        }

        /// <summary>
        /// Returns the default culture for this application.
        /// </summary>
        /// <returns></returns>
        private static string GetDefaultCulture()
        {
            return _implementedCultures[0];
        }
    }
}