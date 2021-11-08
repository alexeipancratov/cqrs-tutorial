using Api.Utils;
using Logic.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var config = new Config(3);
            services.AddSingleton(config);

            var connectionString = new ConnectionString(Configuration["ConnectionString"]);
            services.AddSingleton(connectionString);

            services.AddSingleton(new SessionFactory(connectionString));
            //services.AddScoped<ICommandHandler<EditPersonalInfoCommand>>(provider =>
            //   new AuditLoggingDecorator<EditPersonalInfoCommand>(
            //       new DatabaseRetryDecorator<EditPersonalInfoCommand>(
            //           new EditPersonalInfoCommandHandler(provider.GetService<SessionFactory>()),
            //           provider.GetService<Config>())
            //   ));
            //services.AddScoped<ICommandHandler<DisenrollCommand>, DisenrollCommandHandler>();
            //services.AddScoped<ICommandHandler<EnrollCommand>, EnrollCommandHandler>();
            //services.AddScoped<ICommandHandler<RegisterCommand>, RegisterCommandHandler>();
            //services.AddScoped<ICommandHandler<TransferCommand>, TransferCommandHandler>();
            //services.AddScoped<ICommandHandler<UnregisterCommand>, UnregisterCommandHandler>();
            //services.AddScoped<IQueryHandler<GetListQuery, List<StudentDto>>, GetListQueryHandler>();
            services.AddScoped<Messages>();
            services.AddHandlers();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Api", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
