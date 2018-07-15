using System.ComponentModel.DataAnnotations;

namespace RESTful_API_ASP.NET_Core.Models
{
    public abstract class BookForManipulation
    {
        [Required(ErrorMessage = "You should provide a title for the book.")]
        [MaxLength(100, ErrorMessage = "The title shouldn't have more than 100 characters.")]
        public string Title { get; set; }

        [MaxLength(500, ErrorMessage = "The title shouldn't have more than 100 characters.")]
        public virtual string Description { get; set; }
    }
}
