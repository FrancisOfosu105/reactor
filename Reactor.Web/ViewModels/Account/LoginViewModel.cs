using System.ComponentModel.DataAnnotations;

namespace Reactor.Web.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name="User Name")]
        public string UserName { get; set; }
        
        [Required]
        public string Password { get; set; }

        public bool RemmemberMe { get; set; }
        
        public string ReturnUrl { get; set; }

    }
}