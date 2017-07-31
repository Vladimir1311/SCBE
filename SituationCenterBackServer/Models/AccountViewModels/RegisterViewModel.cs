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
        [StringLength(14, ErrorMessage = "Please enter phone number is format 89008007654", MinimumLength =12)]
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

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Birthday")]
        //[CustomValidation()]
        [JsonProperty("birthday")]
        public string Birthday { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Sex")]
        [JsonProperty("isMan")]
        public bool Sex { get; set; }


        public DateTime ParsedBirthday()
        {
            var numbers = Birthday
                .Split('/')
                .Select(T => int.Parse(T))
                .Reverse()
                .ToArray();
            return new DateTime(numbers[0], numbers[1], numbers[2]);
        }
    }
}
