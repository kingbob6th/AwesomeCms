using System.ComponentModel.DataAnnotations;

namespace AwesomeCms.HelperModels.Requests
{
    public class RegisterRequest
    {
        public string Username { get; set; }
        [RegularExpression(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z")]
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
