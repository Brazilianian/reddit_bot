using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace reddit_bor.util
{
    public static class EnumUtil
    {
        public static string GetDescription(this Enum value)
        {
            return value.GetType()
                    .GetMember(value.ToString())
                    .FirstOrDefault()?
                    .GetCustomAttribute<DescriptionAttribute>()
                    ?.Description ?? value.ToString();
        }
    }
}
