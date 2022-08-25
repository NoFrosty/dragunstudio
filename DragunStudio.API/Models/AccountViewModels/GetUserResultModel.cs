using DragunStudio.API.Enums;

namespace DragunStudio.API.Models.AccountViewModels
{
    /// <summary>
    /// User's Data.
    /// </summary>
    public class GetUserResultModel
    {
        /// <summary>
        /// The user's Id.
        /// </summary>
        /// <example>31d6810b-7cfc-4a39-9e20-f30cca1a8733</example>
        public string Id { get; set; }

        /// <summary>
        /// The user's first name.
        /// </summary>
        /// <example>John</example>
        public string? FirstName { get; set; }

        /// <summary>
        /// The user's last name.
        /// </summary>
        /// <example>Doe</example>
        public string? LastName { get; set; }

        /// <summary>
        /// The user's living country.
        /// </summary>
        /// <example>France</example>
        public string? Country { get; set; }

        /// <summary>
        /// The user's birth day.
        /// </summary>
        /// <example>2022-08-24T16:44:11.327Z</example>
        public DateTimeOffset? Birthday { get; set; }

        /// <summary>
        /// The user's last login date.
        /// </summary>
        /// <example>2022-08-24T16:44:11.327Z</example>
        public DateTimeOffset? LastLoginDateTime { get; set; }

        /// <summary>
        /// The user's account creation date.
        /// </summary>
        /// <example>2022-08-24T16:44:11.327Z</example>
        public DateTimeOffset? CreationDateTime { get; set; }

        /// <summary>
        /// The user's username.
        /// </summary>
        /// <example>JDoe</example>
        public string UserName { get; set; }

        /// <summary>
        /// The user's username.
        /// </summary>
        /// <example>JDoe</example>
        public string NormalizedUserName { get; set; }

        /// <summary>
        /// The user's email.
        /// </summary>
        /// <example>John.Doe@example.com</example>
        public string Email { get; set; }

        /// <summary>
        /// The user's email.
        /// </summary>
        /// <example>John.Doe@example.com</example>
        public string NormalizedEmail { get; set; }

        /// <summary>
        /// The user's phone number.
        /// </summary>
        /// <example>0033612345678</example>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// The user's phone number is confirmed.
        /// </summary>
        /// <example>true</example>
        public bool PhoneNumberConfirmed { get; set; }

        /// <summary>
        /// The user's two factor is enabled.
        /// </summary>
        /// <example>true</example>
        public bool TwoFactorEnabled { get; set; }

        /// <summary>
        /// The user's account lock out end date.
        /// </summary>
        /// <example>2022-08-24T16:44:11.327Z</example>
        public DateTimeOffset? LockoutEnd { get; set; }

        /// <summary>
        /// The user's account is lock out.
        /// </summary>
        /// <example>true</example>
        public bool LockoutEnabled { get; set; }

        /// <summary>
        /// The user's account failed acces count.
        /// </summary>
        /// <example>2</example>
        public int AccessFailedCount { get; set; }

        /// <summary>
        /// The user's address 1.
        /// </summary>
        /// <example>105 Road Francky</example>
        public string? Address1 { get; set; }

        /// <summary>
        /// The user's address 1.
        /// </summary>
        /// <example>105 Road Francky</example>
        public string? Address2 { get; set; }

        /// <summary>
        /// The user's post code.
        /// </summary>
        /// <example>31000</example>
        public string? PostCode { get; set; }

        /// <summary>
        /// The user's account status.
        /// </summary>
        /// <example>Active</example>
        public AccountStatus AccountStatus { get; set; }
    }
}
