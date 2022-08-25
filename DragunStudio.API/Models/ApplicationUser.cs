using DragunStudio.API.Enums;
using Microsoft.AspNetCore.Identity;

namespace DragunStudio.API.Models
{
    /// <summary>
    /// User's Data.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
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
