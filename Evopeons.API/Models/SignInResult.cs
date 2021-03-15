namespace Evopeons.API.Models
{
    public class SignInResult
    {
        public bool IsSuccessfull { get; set; }
        public string Token { get; set; }
        public string Username { get; set; }
    }
}