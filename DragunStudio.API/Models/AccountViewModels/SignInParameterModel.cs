namespace DragunStudio.API.Models.AccountViewModels
{
    public class SignInParameterModel
    {
        /// <summary>
        /// The user's email.
        /// </summary>
        /// <example>John.Doe@example.com</example>
        public string Email { get; set; }

        /// <summary>
        /// The user's password.
        /// </summary>
        /// <example>oldPassword123!</example>
        public string Password { get; set; }
    }
}
