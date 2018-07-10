using System;

namespace RESTful_API_ASP.NET_Core.Helpers
{
    public class DateTimeOffsetExtensions
    {
        public static int GetCurrentAge(DateTimeOffset dateTimeOffset)
        {
            var currentDate = DateTime.UtcNow;
            int age = currentDate.Year - dateTimeOffset.Year;

            if (currentDate < dateTimeOffset.AddYears(age))
            {
                age--;
            }

            return age;
        }
    }
}
