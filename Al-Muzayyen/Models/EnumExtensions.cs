using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Al_Muzayyen.Models
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            var displayAttribute = enumValue.GetType()
                                            .GetField(enumValue.ToString())?
                                            .GetCustomAttribute<DisplayAttribute>();
            return displayAttribute != null ? displayAttribute.Name : enumValue.ToString();
        }
    }
}
