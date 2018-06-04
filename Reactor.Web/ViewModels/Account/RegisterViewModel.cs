using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Reactor.Web.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name="User Name")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name ="Last Name")] 
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Email Address")]
        [EmailAddress]
        public string EmailAddress { get; set; }
        
        [Required]
        [MinLength(6, ErrorMessage = "{0} should be a minimum of {1} characters.")]
        public string Password { get; set; }
        
        [Compare(nameof(Password), ErrorMessage = "Password does not match")]
        public string ConfirmPassword { get; set; }

        public IFormFile File { get; set; }
        
        public string ReturnUrl { get; set; }
    }
}