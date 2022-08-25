namespace DragunStudio.API.Models.AccountViewModels
{
    public class UpdatePasswordParameterModel
    {
        /// <summary>
        /// The user's Id.
        /// </summary>
        /// <example>31d6810b-7cfc-4a39-9e20-f30cca1a8733</example>
        public string Id { get; set; }

        /// <summary>
        /// The user's current password.
        /// </summary>
        /// <example>oldPassword123!</example>
        public string CurrentPassword { get; set; }

        /// <summary>
        /// The user's new password.
        /// </summary>
        /// <example>newPassword123!</example>
        public string NewPassword { get; set; }
    }
}
