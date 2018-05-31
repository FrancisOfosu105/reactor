using System.ComponentModel.DataAnnotations;

namespace Reactor.Web.ViewModels.Account
{
    public class LoginModel
    {
        [Required]
        [Display(Name="User Name")]
        public string UserName { get; set; }
        
        [Required]
        public string Password { get; set; }

        public bool RemmemberMe { get; set; }

    }
}