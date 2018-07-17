using System.Collections.Generic;

namespace RESTful_API_ASP.NET_Core.Services
{
    public class PropertyMappingValue
    {
        public IEnumerable<string> DestinationProperty { get; private set; }

        public bool Revert { get; private set; }

        public PropertyMappingValue(IEnumerable<string> destinationProperty, bool revert = false)
        {
            DestinationProperty = destinationProperty;
            Revert = revert;
        }
    }
}
