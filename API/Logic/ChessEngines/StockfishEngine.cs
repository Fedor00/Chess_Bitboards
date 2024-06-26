using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using API.Interfaces;

namespace API.Logic.ChessEngines
{
    public class StockfishEngine : IChessEngine
    {
        private Process _stockfishProcess;
        public StockfishEngine()
        {
            _stockfishProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = @"C:\Users\fedor\OneDrive\Documents\Personal\Chess\Stockfish\stockfish-windows-x86-64.exe",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    CreateNoWindow = true
                }
            };
            _stockfishProcess.Start();
        }
        public Task<string> GetBestMoveAsync(string fen, int depth)
        {
            if (!_stockfishProcess.StartInfo.FileName.EndsWith("exe") || !_stockfishProcess.Start())
            {
                throw new InvalidOperationException("Failed to start Stockfish process.");
            }
            _stockfishProcess.StandardInput.WriteLine($"position fen {fen}");
            _stockfishProcess.StandardInput.WriteLine($"go depth {depth}");
            Console.WriteLine($"go depth {depth}");

            string output;
            string bestMoveLine = "bestmove";
            while ((output = _stockfishProcess.StandardOutput.ReadLine()) != null)
            {
                if (output.StartsWith(bestMoveLine))
                {
                    return Task.FromResult(output.Replace(bestMoveLine, "").Trim().Split(' ')[0]);
                }
            }
            throw new InvalidOperationException("Stockfish did not return a best move.");
        }
    }
}