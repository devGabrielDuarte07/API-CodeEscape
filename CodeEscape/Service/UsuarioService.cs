using CodeEscape.Common;
using CodeEscape.DTOs.Usuario;
using CodeEscape.Models;
using System.Security.Claims;

namespace CodeEscape.Service
{
    public class UsuarioService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CodeEscapeContext db;

        public UsuarioService(CodeEscapeContext _db, IHttpContextAccessor httpContextAccessor)
        {
            db = _db;
            _httpContextAccessor = httpContextAccessor;
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

        public ResultadoPadrao<DadosUsuarioResponse> DadosPerfil()
        {
            var userId = ObterIdUsuarioLogado();

            var usuario = db.TabelaUsuarios.FirstOrDefault(u => u.Id == userId);

            if (usuario == null)
            {
                return ResultadoPadrao<DadosUsuarioResponse>.Falha("Usuário não encontrado");
            }

            var salasConcluidas = db.Gamesessions.Where(g => g.UserId == userId && g.Finalizada).Count();

            var pontuacaoTotal = db.Gamesessions.Where(g => g.UserId == userId && g.Finalizada).Sum(x => x.Pontuacao);

            var salaMaisJogada = db.Gamesessions
                .Where(g => g.UserId == userId && g.Finalizada)
                .GroupBy(g => g.RoomId)
                .Select(g => new
                {
                    RoomId = g.Key,
                    Quantidade = g.Count()
                })
                .OrderByDescending(g => g.Quantidade)
                .FirstOrDefault();

            if (salaMaisJogada == null)
            {
                var dados2 = new DadosUsuarioResponse
                {
                    Nome = usuario.Nome,
                    Username = usuario.Username,
                    Email = usuario.Email,
                    AvatarUrl = usuario.AvatarUrl,
                    DataCadastro = usuario.CriadoEm,

                    SalasConcluidas = 0,
                    PontuacaoTotal = 0,

                    SalaMaisJogada = null,
                    MelhorTempo = 0
                };

                return ResultadoPadrao<DadosUsuarioResponse>.Ok(dados2);
            }

            var sala = db.TabelaRooms.FirstOrDefault(s => s.Id == salaMaisJogada.RoomId);


            var melhorTempo = db.Gamesessions.Where(g => g.UserId == userId && g.RoomId == salaMaisJogada.RoomId && g.Finalizada && g.DataInicio != null && g.DataFim != null)
                .AsEnumerable()
                .Select(g => (g.DataFim - g.DataInicio).Value.TotalSeconds).Min();

            var dados = new DadosUsuarioResponse
            {
                Nome = usuario.Nome,
                Username = usuario.Username,
                Email = usuario.Email,
                AvatarUrl = usuario.AvatarUrl,
                DataCadastro = usuario.CriadoEm,
                SalasConcluidas = salasConcluidas,
                PontuacaoTotal = pontuacaoTotal,
                SalaMaisJogada = sala?.Nome,
                MelhorTempo = melhorTempo
            };
            return ResultadoPadrao<DadosUsuarioResponse>.Ok(dados);
        }

        private int ObterIdUsuarioLogado()
        {

            var user = _httpContextAccessor.HttpContext?.User;

            var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;


            if (!int.TryParse(userId, out int id))
                throw new Exception("Usuário não autenticado ou ID inválido");

            return id;
        }
    }
}
