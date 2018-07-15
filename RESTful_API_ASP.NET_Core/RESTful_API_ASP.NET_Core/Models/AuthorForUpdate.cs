using System.ComponentModel.DataAnnotations;

namespace RESTful_API_ASP.NET_Core.Models
{
    public class AuthorForUpdate : AuthorForManipulation
    {

        [Required(ErrorMessage = "You have to provide the genre of the author")]
        public override string Genre
        {
            get
            {
                return base.Genre;
            }

            set
            {
                base.Genre = value;
            }
        }
    }
}
