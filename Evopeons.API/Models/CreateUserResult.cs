namespace Evorine.API.Models
{
    public class CreateUserResult
    {
        public bool IsSuccessfull { get; set; }
        public bool IsUsernameTaken { get; set; }
        public bool IsEmailTaken { get; set; }
    }
}