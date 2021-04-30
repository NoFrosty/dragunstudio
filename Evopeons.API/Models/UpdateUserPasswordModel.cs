using System;
namespace Evorine.API.Models
{
    public class UpdateUserPasswordModel
    {
        public Guid UserID { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
