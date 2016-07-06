using System;
using System.ComponentModel;

namespace ns.Base.Extensions {

    /// <summary>
    /// Extension methods to work with Enums.
    /// </summary>
    public static class EnumExtension {

        public static string GetDescription(this Enum value) {
            DescriptionAttribute attribute = System.Attribute.GetCustomAttribute(value.GetType().GetField(value.ToString()), typeof(DescriptionAttribute)) as DescriptionAttribute;
            return attribute != null ? attribute.Description : value.ToString();
        }
    }
}