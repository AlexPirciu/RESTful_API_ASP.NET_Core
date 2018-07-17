using System.Reflection;

namespace RESTful_API_ASP.NET_Core.Services
{
    public class TypeHelperService : ITypeHelperServices
    {
        public bool TypeHasProperties<T>(string fields)
        {
            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            var fieldsAfterSplit = fields.Split(",");
            foreach (var field in fieldsAfterSplit)
            {
                var propertyName = field.Trim();
                var propertInfo = typeof(T)
                    .GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propertInfo == null)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
