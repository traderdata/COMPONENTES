using System;
using System.Collections.Generic;
using System.Reflection;

namespace ModulusFE
{
    internal static class Enum
    {
        public static IEnumerable<string> GetNames(Type enumType)
        {
            FieldInfo[] fieldInfos = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (var info in fieldInfos)
            {
                yield return info.Name;
            }
        }
    }
}