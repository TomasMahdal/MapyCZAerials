using MapAerials.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MapAerials
{
    class Languages
    {
        private static ResourceDictionary dictionary;

        /// <summary>
        /// Set language of app based on CurrentCulture settings
        /// </summary>
        public static void SetLanguageDictionary()
        {
            dictionary = new ResourceDictionary();
            switch (GetCulture())
            {
                case "cs-CZ":
                    dictionary.Source = new Uri("..\\Resources\\StringResources.cs-CZ.xaml", UriKind.Relative);
                    break;
                default:
                    dictionary.Source = new Uri("..\\Resources\\StringResources.xaml", UriKind.Relative);
                    break;
            }

            Application.Current.Resources.MergedDictionaries.Add(dictionary);
        }

        /// <summary>
        /// Get localized string
        /// </summary>
        /// <param name="s">string id</param>
        /// <returns>localized string</returns>
        public static string GetString(string s)
        {
            return dictionary[s];
        }

        /// <summary>
        /// Get current culture string identificator
        /// </summary>
        /// <returns>culture string identificator</returns>
        public static string GetCulture()
        {
            return Thread.CurrentThread.CurrentCulture.ToString();
        }
    }
}
 