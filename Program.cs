
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.Data.SqlClient;
using sib_api_v3_sdk.Client;

namespace railway_eservice_api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Configuration.Default.ApiKey.Add("api-key", "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");

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    policy =>
                    {
                        policy.AllowAnyOrigin()
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                    });
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors();


            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

    }
}
