using System.ComponentModel.DataAnnotations;

namespace RESTful_API_ASP.NET_Core.Models
{
    public class BookForUpdate : BookForManipulation
    {
        [Required(ErrorMessage = "You should provide a description for the book")]
        public override string Description
        {
            get
            {
                return base.Description;
            }

            set
            {
                base.Description = value;
            }
        }
    }
}
