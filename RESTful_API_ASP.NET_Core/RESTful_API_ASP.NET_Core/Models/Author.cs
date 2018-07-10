using System;

namespace RESTful_API_ASP.NET_Core.Models
{
    public class Author
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }

        public string Genre { get; set; }
    }
}
