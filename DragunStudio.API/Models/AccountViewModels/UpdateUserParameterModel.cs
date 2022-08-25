namespace DragunStudio.API.Models.AccountViewModels
{
    public class UpdateUserParameterModel
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
        /// The user's username.
        /// </summary>
        /// <example>JDoe</example>
        public string UserName { get; set; }

        /// <summary>
        /// The user's email.
        /// </summary>
        /// <example>John.Doe@example.com</example>
        public string Email { get; set; }

        /// <summary>
        /// The user's phone number.
        /// </summary>
        /// <example>0033612345678</example>
        public string? PhoneNumber { get; set; }

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
    }
}
