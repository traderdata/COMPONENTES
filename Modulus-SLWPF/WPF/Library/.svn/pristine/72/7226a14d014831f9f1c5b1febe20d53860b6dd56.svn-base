using System;
using System.Collections.Generic;
using System.Windows;

namespace ModulusFE
{
    internal static class ResourceLoader
    {
        private static readonly Dictionary<string, object> _cachedResources = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
        private static readonly Dictionary<string, ResourceDictionary> _resourceDictionaries = new Dictionary<string, ResourceDictionary>(StringComparer.InvariantCultureIgnoreCase);

#if WPF
        private const string DefaultMediaValues = "/ModulusFE.StockChartX;component/Themes/Brushes.xaml";
#else
		private const string DefaultMediaValues = "/ModulusFE.StockChartX.SL;component/Themes/Brushes.xaml";
#endif

        private static ResourceDictionary GetDictionary(string name)
        {
            ResourceDictionary dictionary;
            if (_resourceDictionaries.TryGetValue(name, out dictionary))
            {
                return dictionary;
            }

            dictionary = new ResourceDictionary { Source = new Uri(name, UriKind.RelativeOrAbsolute) };
            _resourceDictionaries.Add(name, dictionary);

            return dictionary;
        }

        public static T Get<T>(string name)
        {
            if (_cachedResources.ContainsKey(name))
            {
                return (T)_cachedResources[name];
            }

            ResourceDictionary dictionary = GetDictionary(DefaultMediaValues);

            object o = dictionary[name];
            _cachedResources.Add(name, o);

            return (T)o;
        }
    }
}
