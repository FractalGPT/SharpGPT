using System.ComponentModel;
using System.Reflection;

namespace FractalGPT.SharpGPTLib.Infrastructure.Extensions;

public static class EnumExtensions
{
    public static string GetDescription(this Enum value)
    {
        FieldInfo field = value.GetType().GetField(value.ToString());
        if (field == null)
        {
            return value.ToString();
        }

        // Пытаемся получить атрибут Description
        DescriptionAttribute attribute = field.GetCustomAttribute<DescriptionAttribute>();

        // Если атрибут есть, возвращаем его значение, иначе — имя члена enum
        return attribute != null ? attribute.Description : value.ToString();
    }
}