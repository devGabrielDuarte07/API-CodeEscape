using CodeEscape.Common;
using CodeEscape.DTOs.Usuario;
using CodeEscape.Models;

namespace CodeEscape.Service
{
    public class CadastroService
    {
        private readonly CodeEscapeContext db;

        public CadastroService(CodeEscapeContext _db) 
        {
            db = _db;
        }


        public ResultadoPadrao<object> CriarUsuario(CriarUsuarioRequest dto)
        {
            var existe = db.TabelaUsuarios.Any(u => u.Username == dto.Username || u.Email == dto.Email);

            if (existe)
                return ResultadoPadrao<object>.Falha("Username ou Email já cadastrado", 400);


            if (dto.Senha != dto.ConfirmarSenha)
                return ResultadoPadrao<object>.Falha("As senha não se coicidem");

            var newUser = new TabelaUsuario
            { 
                Nome = dto.Nome,
                Username = dto.Username,
                Email = dto.Email,
                Senha = BCrypt.Net.BCrypt.HashPassword(dto.Senha),
                AvatarUrl = string.IsNullOrWhiteSpace(dto.AvatarUrl) ? "/uploads/avatars/default.png" : dto.AvatarUrl
            };

            db.TabelaUsuarios.Add(newUser);
            db.SaveChanges();

            return ResultadoPadrao<object>.Ok("Usuario criado com sucesso");
        }

        
    }
}
