using System;
using System.ComponentModel;
using System.Globalization;

namespace Poke.Api.Extensions
{
    public static class EnumExtensions
    {
        public static DayOfWeek NextDay(this DayOfWeek dayOfWeek)
        {
            if (dayOfWeek == DayOfWeek.Saturday)
            {
                return DayOfWeek.Sunday;
            }
            return dayOfWeek + 1;
        }

        public static DayOfWeek PreviousDay(this DayOfWeek dayOfWeek)
        {
            if (dayOfWeek == DayOfWeek.Sunday)
            {
                return DayOfWeek.Saturday;
            }
            return dayOfWeek - 1;
        }
        public static string GetDescription<T>(this T e) where T : IConvertible
        {
            string description = null;

            if (e is Enum)
            {
                var type = e.GetType();
                var values = Enum.GetValues(type);

                foreach (int val in values)
                {
                    if (val == e.ToInt32(CultureInfo.InvariantCulture))
                    {
                        var memInfo = type.GetMember(type.GetEnumName(val)!);
                        var descriptionAttributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                        if (descriptionAttributes.Length > 0)
                        {
                            description = ((DescriptionAttribute)descriptionAttributes[0]).Description;
                        }

                        break;
                    }
                }
            }

            return description;
        }
    }
}