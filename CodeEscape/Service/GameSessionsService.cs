using CodeEscape.Common;
using CodeEscape.DTOs.GameSession;
using CodeEscape.Models;
using System.Security.Claims;

namespace CodeEscape.Service
{
    public class GameSessionsService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CodeEscapeContext db;

        public GameSessionsService(CodeEscapeContext _db, IHttpContextAccessor httpContextAccessor)
        {
            db = _db;
            _httpContextAccessor = httpContextAccessor;
        }

        // so user logado
        public ResultadoPadrao<StartGameSessionResponse> Start(StartGameSessionRequest dto)
        {
            var UserId = ObterIdUsuarioLogado();

            var sala = db.TabelaRooms.FirstOrDefault(r => r.Id == dto.RoomId && r.IsAtiva && r.TabelaDesafios.Any(e => e.IsAtivo));

            if(sala == null) 
                return ResultadoPadrao<StartGameSessionResponse>.Falha("Sala não encontrada", 404);

            var GameSessionAtiva = db.Gamesessions.FirstOrDefault(g => g.RoomId == dto.RoomId && g.UserId == UserId && !g.Finalizada);

            if(GameSessionAtiva != null)
            {
                var ContinuarGameSession = new StartGameSessionResponse
                {
                    GameSessionId = GameSessionAtiva.Id,
                    Pontuacao = GameSessionAtiva.Pontuacao,
                    EnigmaAtual = GameSessionAtiva.EnigmaAtual,
                    Continuando = true
                };

                return ResultadoPadrao<StartGameSessionResponse>.Ok(ContinuarGameSession);
            }
            else
            {
                var newGameSession = new Gamesession
                {
                    UserId = UserId,
                    RoomId = dto.RoomId,
                    Pontuacao = 0,
                    EnigmaAtual = 1,
                    DataInicio = DateTime.UtcNow,
                    Finalizada = false,
                };

                db.Gamesessions.Add(newGameSession);
                db.SaveChanges();

                var response = new StartGameSessionResponse
                {
                    GameSessionId = newGameSession.Id,
                    Pontuacao = newGameSession.Pontuacao,
                    EnigmaAtual = newGameSession.EnigmaAtual,
                    Continuando = false
                };

                return ResultadoPadrao<StartGameSessionResponse>.Ok(response);
            }
            

        }


        public ResultadoPadrao<ObterEnigmaAtualResponse> ObterEnigmaAtual(int gameSessionId)
        {
            var UserId = ObterIdUsuarioLogado();

            var gameSession = db.Gamesessions.FirstOrDefault(g => g.Id == gameSessionId && !g.Finalizada && g.UserId == UserId);

            if (gameSession == null)
                return ResultadoPadrao<ObterEnigmaAtualResponse>.Falha("Sessao não encontrada");

            var enigmaAtual = db.TabelaDesafios.Where(d => d.RoomId == gameSession.RoomId && d.Ordem == gameSession.EnigmaAtual && d.IsAtivo).Select(a => new ObterEnigmaAtualResponse
            {
                Id = a.Id,
                Titulo = a.Titulo,
                Pergunta = a.Pergunta,
                Ordem = a.Ordem
            }).FirstOrDefault();

            if(enigmaAtual == null)
                return ResultadoPadrao<ObterEnigmaAtualResponse>.Falha("Enigma não encontrado");

            return ResultadoPadrao<ObterEnigmaAtualResponse>.Ok(enigmaAtual);
        
        }

        public ResultadoPadrao<ResponderEnigmaResponse> ResponderEnigma(int id, ResponderEnigmaRequest dto)
        {
            var UserId = ObterIdUsuarioLogado();

            var gameSession = db.Gamesessions.FirstOrDefault(g => g.Id == id && !g.Finalizada && g.UserId == UserId);

            if (gameSession == null)
                return ResultadoPadrao<ResponderEnigmaResponse>.Falha("Sessao não encontrada");

            var enigmaAtual = db.TabelaDesafios.Where(d => d.RoomId == gameSession.RoomId && d.Ordem == gameSession.EnigmaAtual && d.IsAtivo).FirstOrDefault();

            if (enigmaAtual == null)
                return ResultadoPadrao<ResponderEnigmaResponse>.Falha("Enigma não encontrado");

            if(enigmaAtual.Resposta.Trim().ToLower() == dto.Resposta.Trim().ToLower())
            {
                // acertou, somar pontos e avançar desafio e salvar
                gameSession.Pontuacao += 100;
                var nextEnigma = db.TabelaDesafios.Any(d => d.RoomId == gameSession.RoomId && d.Ordem == enigmaAtual.Ordem + 1 && d.IsAtivo);
                if (nextEnigma)
                    gameSession.EnigmaAtual++;
                else if (!nextEnigma) 
                {
                    gameSession.Finalizada = true;
                    gameSession.DataFim = DateTime.UtcNow;

                } 
                db.SaveChanges();

                var dadosAcerto = new ResponderEnigmaResponse
                {
                    Acertou = true,
                    Pontuacao = gameSession.Pontuacao,
                    EnigmaAtual = gameSession.EnigmaAtual,
                    Finalizada = gameSession.Finalizada,
                };
                return ResultadoPadrao<ResponderEnigmaResponse>.Ok(dadosAcerto);
            } else
            {
                // subtrair os pontos
                gameSession.Pontuacao -= 10;
                db.SaveChanges();

                var dadosErro = new ResponderEnigmaResponse
                {
                    Acertou = false,
                    Pontuacao = gameSession.Pontuacao,
                    EnigmaAtual = gameSession.EnigmaAtual,
                    Finalizada = gameSession.Finalizada,
                };

                return ResultadoPadrao<ResponderEnigmaResponse>.Ok(dadosErro);
            }
        }

        public ResultadoPadrao<PedirDicaResponse> PedirDica(int GameSessionId)
        {
            var UserId = ObterIdUsuarioLogado();

            var gameSession = db.Gamesessions.Where(g => g.Id == GameSessionId && g.UserId == UserId && !g.Finalizada).FirstOrDefault();

            if (gameSession == null)
                return ResultadoPadrao<PedirDicaResponse>.Falha("Sessao não encontrada");

            var enigmaAtual = db.TabelaDesafios.Where(d => d.RoomId == gameSession.RoomId && d.Ordem == gameSession.EnigmaAtual && d.IsAtivo).FirstOrDefault();

            if (enigmaAtual == null)
                return ResultadoPadrao<PedirDicaResponse>.Falha("Enigma não encontrado");

            var jaPediuDica = db.Gamesessiondicas.Any(d => d.GameSessionId ==  GameSessionId && d.OrdemEnigma == enigmaAtual.Ordem);

            if (jaPediuDica)
                return ResultadoPadrao<PedirDicaResponse>.Falha("Você já utilizou a dica deste enigma");

            gameSession.Pontuacao -= 25;

            var Gamesessiondica = new Gamesessiondica
            {
                GameSessionId = GameSessionId,
                OrdemEnigma = enigmaAtual.Ordem
            };

            db.Gamesessiondicas.Add(Gamesessiondica);

            db.SaveChanges();
            var Dica = new PedirDicaResponse
            {
                Dica = enigmaAtual.Dica,
                Pontuacao = gameSession.Pontuacao
            };
            return ResultadoPadrao<PedirDicaResponse>.Ok(Dica);
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
