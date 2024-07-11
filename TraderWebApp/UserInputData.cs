using System.ComponentModel.DataAnnotations;

namespace TraderWebApp
{
    public class UserInputData
    {
        [Required(ErrorMessage = "Type of order is required.")]
        public string? TypeOfOrder { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be a positive number.")]
        public double Amount { get; set; }
    }
}
