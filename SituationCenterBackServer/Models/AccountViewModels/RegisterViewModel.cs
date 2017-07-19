using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        [JsonProperty("email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [JsonProperty("password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        [JsonProperty("confirm")]
        public string ConfirmPassword { get; set; }

        
        [Required]
        [StringLength(11, ErrorMessage = "Please enter phone number is format 89008007654")]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone number")]
        [JsonProperty("phone")]
        public string PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Name")]
        [JsonProperty("name")]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Surname")]
        [JsonProperty("surname")]
        public string Surname { get; set; }
    }
}
