using System;

namespace RESTful_API_ASP.NET_Core.Models
{
    public class AuthorForCreation
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTimeOffset DateOfBirth { get; set; }

        public string Genre { get; set; }
    }
}
