using CodeEscape.Common;
using CodeEscape.DTOs.Ranking;
using CodeEscape.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeEscape.Service
{
    public class RankingService
    {
        private readonly CodeEscapeContext db;

        public RankingService(CodeEscapeContext _db)
        {
            db = _db;
        }

        public ResultadoPadrao<List<RankingSalaResponse>> RankingPorSala(int roomId)
        {
            var ranking = db.Gamesessions
                .Where(r => r.RoomId == roomId && r.Finalizada)
                .Include(r => r.User)
                .ToList()
                .GroupBy(g => g.UserId)
                .Select(grupo => grupo
                    .OrderByDescending(p => p.Pontuacao)
                    .ThenBy(p => p.DataFim - p.DataInicio)
                    .First()
                )
                .OrderByDescending(r => r.Pontuacao)
                .ThenBy(p => p.DataFim - p.DataInicio)
                .Where(r => r != null)
                .Select(r => new RankingSalaResponse
                {

                    Usuario = r.User.Username,
                    AvatarUrl = r.User.AvatarUrl,
                    Pontuacao = r.Pontuacao,
                    TempoSegundos = (r.DataFim - r.DataInicio).Value.TotalSeconds
                })
                .ToList();

            

            return ResultadoPadrao<List<RankingSalaResponse>>.Ok(ranking);
        }
    }    
}
