
using CodeEscape.Common;
using CodeEscape.DTOs.Room;
using CodeEscape.Enums;
using CodeEscape.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeEscape.Service
{
    public class RoomService
    {
        private readonly CodeEscapeContext db;

        public RoomService(CodeEscapeContext _db)
        {
            db = _db;
        }

        public ResultadoPadrao<List<DadosRoomResponse>> ListarSalas() 
        {
            var rooms = db.TabelaRooms.Where(r => r.IsAtiva && r.TabelaDesafios.Any(e => e.IsAtivo)).ToList();

            var dados = rooms.Select(d =>
            {
                var quantidadeEnigmas = db.TabelaDesafios.Count(de => de.IsAtivo && de.RoomId == d.Id);

                return new DadosRoomResponse
                {
                    Id = d.Id,
                    Nome = d.Nome,
                    Descricao = d.Descricao,
                    CapaUrl = string.IsNullOrWhiteSpace(d.CapaUrl) ? "uploads/capas/defaultCapa.png" : d.CapaUrl,
                    Criador = d.Criador == null ? "admin" : d.Criador.Nome,
                    QuantidadeEnigma = quantidadeEnigmas,
                    Dificuldade = ObterDificuldade(quantidadeEnigmas)
                };
            }).ToList();

            if (dados.Count == 0)
                return ResultadoPadrao<List<DadosRoomResponse>>.Falha("Nenhuma sala encontrada", 404);

            return ResultadoPadrao<List<DadosRoomResponse>>.Ok(dados);
        }
        

        public ResultadoPadrao<DadosRoomResponse> ListarSalasPorId(int id)
        {
            var quantidadeEnigmas = db.TabelaDesafios.Count(d => d.IsAtivo && d.RoomId == id);

            var room = db.TabelaRooms.Where(r => r.IsAtiva && r.Id == id && r.TabelaDesafios.Any(e => e.IsAtivo)).Select(d => new DadosRoomResponse
            {
                Id = d.Id,
                Nome = d.Nome,
                Descricao = d.Descricao,
                CapaUrl = string.IsNullOrWhiteSpace(d.CapaUrl) ? "uploads/capas/defaultCapa.png" : d.CapaUrl,
                Criador = d.Criador == null ? "admin" : d.Criador.Nome,
                QuantidadeEnigma = quantidadeEnigmas,
                Dificuldade = ObterDificuldade(quantidadeEnigmas)
            }).FirstOrDefault();


            if (room == null)
                return ResultadoPadrao<DadosRoomResponse>.Falha("Nenhuma sala encontrada", 404);

            return ResultadoPadrao<DadosRoomResponse>.Ok(room);
        }

       private DificuldadeRoom ObterDificuldade(int quantidadeEnigmas)
        {
            if (quantidadeEnigmas <= 5)
                return DificuldadeRoom.Fácil;
            else if (quantidadeEnigmas <= 10)
                return DificuldadeRoom.Média;
            
            return DificuldadeRoom.Difícil;
        }
    }
}
