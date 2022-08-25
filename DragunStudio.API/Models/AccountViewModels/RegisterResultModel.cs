using Microsoft.AspNetCore.Identity;

namespace DragunStudio.API.Models.AccountViewModels
{
    public class RegisterResultModel
    {
        public bool IsSuccessfull { get; set; }
        public bool IsEmailTaken { get; set; }
        public bool IsUsernameTaken { get; set; }
        public IdentityResult? IdentityResult { get; set; }
    }
}
