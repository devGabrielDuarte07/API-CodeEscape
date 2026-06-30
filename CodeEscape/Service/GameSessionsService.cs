using CodeEscape.Common;
using CodeEscape.DTOs.GameSession;
using CodeEscape.Models;
using Microsoft.EntityFrameworkCore;
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

            var totalEnigmas = db.TabelaDesafios.Where(d => d.RoomId == gameSession.RoomId).Count();


            var enigmaAtual = db.TabelaDesafios.Where(d => d.RoomId == gameSession.RoomId && d.Ordem == gameSession.EnigmaAtual && d.IsAtivo).Select(a => new ObterEnigmaAtualResponse
            {
                Id = a.Id,
                Titulo = a.Titulo,
                Pergunta = a.Pergunta,
                Ordem = a.Ordem,
                TotalEnigmas = totalEnigmas
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

        public ResultadoPadrao<List<MinhasPartidasResponse>> MinhasPartidas()
        {
            var userId = ObterIdUsuarioLogado();

            var partidas = db.Gamesessions
                .Where(g => g.UserId == userId)
                .OrderByDescending(t => t.DataInicio)
                .Select(a => new
                {
                    a.Id,
                    Sala = a.Room.Nome,
                    a.Pontuacao,
                    a.Finalizada,
                    a.DataInicio,
                    a.DataFim
                }).ToList()
                .Select(a => new MinhasPartidasResponse
                {
                    GameSessionId = a.Id,
                    Sala = a.Sala,
                    Pontuacao = a.Pontuacao,
                    Finalizada = a.Finalizada,
                    DataInicio = a.DataInicio,
                    DataFim = a.DataFim,
                    TempoSegundos = a.DataFim.HasValue && a.DataInicio.HasValue 
                    ? (a.DataFim - a.DataInicio).Value.TotalSeconds 
                    : null
                }).ToList();

            if (partidas.Count == 0)
                return ResultadoPadrao<List<MinhasPartidasResponse>>.Falha("Nenhuma partida encontrada");

            return ResultadoPadrao<List<MinhasPartidasResponse>>.Ok(partidas);
        }

        public ResultadoPadrao<ResultadoPartidaResponse> ResultadoPartida(int idGameSession)
        {
            var UserId = ObterIdUsuarioLogado();

            var gameSession = db.Gamesessions
                .Include(g => g.Room)
                .FirstOrDefault(g =>
                    g.Id == idGameSession &&
                    g.UserId == UserId && 
                    g.Finalizada
                );

                if (gameSession == null)
                {
                    return ResultadoPadrao<ResultadoPartidaResponse>.Falha("Partida não encontrada.");
                }

            var ranking = db.Gamesessions
                .Where(g =>
                    g.RoomId == gameSession.RoomId &&
                    g.Finalizada)
                .ToList()
                .GroupBy(g => g.UserId)
                .Select(grupo => grupo
                    .OrderByDescending(p => p.Pontuacao)
                    .ThenBy(p => p.DataFim - p.DataInicio)
                    .First()
                )
                .OrderByDescending(g => g.Pontuacao)
                .ThenBy(g => g.DataFim - g.DataInicio)
                .ToList();

            var posicao = ranking.FindIndex(g => g.UserId == UserId) + 1;

            var resultadoFinal = new ResultadoPartidaResponse   
            {
                NomeSala = gameSession.Room.Nome,
                Pontuacao = gameSession.Pontuacao,
                TempoSegundos = (gameSession.DataFim - gameSession.DataInicio).Value.TotalSeconds,
                MelhorPosicaoRanking = posicao,
                CodigoSala = gameSession.RoomId
            };

            return ResultadoPadrao<ResultadoPartidaResponse>.Ok(resultadoFinal);
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
