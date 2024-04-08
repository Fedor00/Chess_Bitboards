
using API.Controllers;
using API.Interfaces;
using API.Repository;
using API.Services;
using DeviceMicroservice.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using UserMicroservice.Interfaces;
using UserMicroservice.Repositories;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
            var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
            var dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "chess-db";
            var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "postgres";
            var dbPass = Environment.GetEnvironmentVariable("DB_PASS") ?? "root";

            var connectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPass};";
            Console.WriteLine(connectionString);
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddScoped<ChessEngineService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IGameRepository, GameRepository>();
            services.AddScoped<IChessEngineRepository, ChessEngineRepository>();
            services.AddScoped<GameService>();
            services.AddHttpClient();
            services.AddSignalR();

            services.AddDbContextFactory<DataContext>(opt =>
            {
                opt.UseNpgsql(connectionString);
            });

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            return services;
        }
    }
}