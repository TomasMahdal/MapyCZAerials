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
        /// <summary>
        /// Set language of app based on CurrentCulture settings
        /// </summary>
        public static void SetLanguageDictionary()
        {
            ResourceDictionary dictionary = new ResourceDictionary();
            switch (Thread.CurrentThread.CurrentCulture.ToString())
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
    }
}
