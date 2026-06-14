using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CodeEscape.DTOs.Usuario
{
    public class CriarUsuarioRequest
    {
        [Required]
        [MinLength(3), MaxLength(255)]
        public string Nome { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(50)]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Username deve conter apenas letras, números e _.")]
        [DefaultValue("Username")]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Senha { get; set; }

        [Required]
        public string ConfirmarSenha { get; set; }
        public string AvatarUrl { get; set; }

    }
}
