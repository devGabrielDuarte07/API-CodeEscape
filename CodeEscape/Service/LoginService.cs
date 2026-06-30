using CodeEscape.Common;
using CodeEscape.DTOs.Auth;
using CodeEscape.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CodeEscape.Service
{
    public class LoginService
    {
        private readonly CodeEscapeContext db;
        private readonly IConfiguration _config;

        public LoginService(CodeEscapeContext _db, IConfiguration config)
        {
            db = _db;
            _config = config;
        }

        public ResultadoPadrao<object> Login(LoginRequest dto) 
        {
            TabelaUsuario usuario;

            if (dto.Login.Contains("@"))
            {
                usuario = db.TabelaUsuarios.FirstOrDefault(u => u.Email == dto.Login && u.IsAtivo);
            }
            else
            {
                usuario = db.TabelaUsuarios.FirstOrDefault(u => u.Username == dto.Login && u.IsAtivo);
            }

            if (usuario == null)
                return ResultadoPadrao<object>.Falha("Login ou senha inválidos", 404);

            if (!BCrypt.Net.BCrypt.Verify(dto.Senha, usuario.Senha))
                return ResultadoPadrao<object>.Falha("Login ou senha inválidos", 404);

            var token = GerarToken(usuario.Id, usuario.Perfil);

            return ResultadoPadrao<object>.Ok(token);
        }

        private string GerarToken(int idUsuario, string role)
        {
            var claims = new List<Claim> {
                        new Claim(ClaimTypes.NameIdentifier, idUsuario.ToString()),
                        new Claim(ClaimTypes.Role, role)
                    };
            var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_config["Jwt:Key"])
            );
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.UtcNow.AddDays(7),
                    signingCredentials: creds
            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenString;
        }
    }
}
