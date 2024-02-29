using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitiesManager.Core.DTO
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Email can't be blank")]
        public string? PersonName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email can't be blank")]
        [EmailAddress(ErrorMessage = "Email should be in a proper format")]
        [Remote(action: "IsEmailAlreadyRegistered", controller: "Account", ErrorMessage = "Email is already in use")]
        public string? Email { get; set; } = string.Empty;


        [Required(ErrorMessage = "Email can't be blank")]
        [RegularExpression("^[0-9]*$", ErrorMessage = " Phone number should containt only digits")]
        public string? PhoneNumber { get; set; } = string.Empty;


        [Required(ErrorMessage = "Password can't be blank")]
        public string? Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password can't be blank")]
        [Compare(nameof(Password), ErrorMessage = "Password and Confirm Password do not match")]
        public string? ConfirmationPassword { get; set; } = string.Empty;

    }
}
