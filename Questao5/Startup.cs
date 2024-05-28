using MediatR;
using System.Data;
using Microsoft.Data.Sqlite;
using Questao5.Application.Commands;

namespace Questao5
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen();

            // Configure SQLite connection
            services.AddSingleton<IDbConnection>(sp => new SqliteConnection("Data Source=database.db"));

            // Configure MediatR
            services.AddMediatR(typeof(Startup).Assembly);

            // Register command and query handlers
            services.AddTransient<IRequestHandler<CreateMovementCommand, Guid>, CreateMovementCommandHandler>();
            services.AddTransient<IRequestHandler<GetBalanceQuery, AccountBalanceDto>, GetBalanceQueryHandler>();

            // Other service registrations
            // ...
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
