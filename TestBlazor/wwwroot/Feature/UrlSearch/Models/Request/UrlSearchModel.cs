using System.ComponentModel.DataAnnotations;
namespace UserInputValidationWithRegex.Models
{
    public class SearchUrlModel
    {
        [Required]
        [Url]
        public string SearchUrl { get; set; }
    }
}