using System.ComponentModel.DataAnnotations;

namespace SituationCenterBackServer.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}