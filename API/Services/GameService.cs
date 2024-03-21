using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Interfaces;
using API.Logic;
using API.Models.Entities;

namespace API.Services
{
    public class GameService
    {
        private readonly IGameRepository _gameRepository;

        public GameService(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }
        public async Task<Game> CreateOrJoinGame(long playerId)
        {
            var playingGame = await GetGame(playerId);
            // if already playing a game or created one and waiting , return that game
            if (playingGame != null) return playingGame;
            var game = await _gameRepository.GetWaitingGame();
            // if there is no game to join create one
            if (game == null) return await CreateGame(playerId);
            // join the waiting game
            game.TopPlayerId = playerId;
            game.Status = "playing";
            _gameRepository.UpdateGame(game);
            await _gameRepository.SaveChangesAsync();
            return game;
        }

        private async Task<Game> CreateGame(long playerId)
        {
            Game game = new()
            {
                Status = "waiting",
                Fen = Utils.ChessHelpers.INITIAL_FEN,
                BottomPlayerId = playerId
            };
            await _gameRepository.AddGameAsync(game);
            await _gameRepository.SaveChangesAsync();
            return game;
        }
        public async Task<Game> GetGame(long playerId)
        {
            return await _gameRepository.GetGameForPlayerAsync(playerId);
        }
    }
}
