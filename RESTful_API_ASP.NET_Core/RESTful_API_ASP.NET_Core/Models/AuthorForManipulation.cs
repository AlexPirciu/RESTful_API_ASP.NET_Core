using System;
using System.ComponentModel.DataAnnotations;

namespace RESTful_API_ASP.NET_Core.Models
{
    public abstract class AuthorForManipulation
    {
        [Required(ErrorMessage = "You have to provide the first name of the author")]
        [MaxLength(50, ErrorMessage = "The first name of the author must be 50 characters long")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "You have to provide the last name of the author")]
        [MaxLength(50, ErrorMessage = "The last name of the author must be 50 characters long")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "You have to provide the birth date of the author")]
        public DateTimeOffset DateOfBirth { get; set; }

        [MaxLength(25, ErrorMessage = "The genre of the author must be 25 characters long")]
        public virtual string Genre { get; set; }
    }
}
