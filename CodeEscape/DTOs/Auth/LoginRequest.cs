using System.ComponentModel.DataAnnotations;

namespace CodeEscape.DTOs.Auth
{
    public class LoginRequest
    {
        [Required]
        public string Login { get; set; }

        [Required]
        public string Senha { get; set; }
    }
}
