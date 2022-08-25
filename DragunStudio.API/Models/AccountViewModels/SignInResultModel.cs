namespace DragunStudio.API.Models.AccountViewModels
{
    public class SignInResultModel
    {
        /// <summary>
        /// Success of the sign in.
        /// </summary>
        /// <example>true</example>
        public bool IsSuccessfull { get; set; }

        /// <summary>
        /// The user's token.
        /// </summary>
        /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiYWRtaW4iOnRydWV9.dyt0CoTl4WoVjAHI9Q_CwSKhl6d_9rhM3NrXuJttkao</example>
        public string Token { get; set; }

        /// <summary>
        /// The user's username.
        /// </summary>
        /// <example>John</example>
        public string UserName { get; set; }
    }
}
