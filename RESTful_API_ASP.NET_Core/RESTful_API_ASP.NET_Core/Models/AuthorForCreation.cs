using System.Collections.Generic;

namespace RESTful_API_ASP.NET_Core.Models
{
    public class AuthorForCreation : AuthorForManipulation
    {
        public ICollection<BookForCreation> Books { get; set; } = new List<BookForCreation>();
    }
}
