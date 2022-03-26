using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore; //UseSqlServer
using Microsoft.OpenApi.Models; //OpenApiInfo
using WebApiCanciones.Filtros;
using WebApiCanciones.Middlewares;
using WebApiCanciones.Services;

namespace WebApiCanciones
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
            services.AddControllers(opciones =>
            {
                opciones.Filters.Add(typeof(FiltroDeExcepcion));
            }).AddJsonOptions (x =>
            x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);

            //Se encarga de configurara ApplicationDbContext como un servicio
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));

            services.AddTransient<IService, ServiceA>(); //Cuando se utiliza la interfaz IService, la instancia es ServiceA
            
            services.AddTransient<ServiceTransient>();
                //Cada petición es una nueva instancia

            services.AddScoped<ServiceScoped>();
                //Mima instancia entre peticiones, pero diferente entre usuarios
           
            services.AddSingleton<ServiceSingleton>();
                //Misma instancia siempre para todos los usuarios

            services.AddTransient<FiltroDeAccion>();
            
            services.AddHostedService<EscribirEnArchivo>();
            

            services.AddResponseCaching(); //Filtros

            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebAPICanciones", Version = "v1" });

            });

            services.AddAutoMapper(typeof(Startup));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env /*,ILogger<Startup> logger*/)
        {

            /*//Use no afecta los demás como run
            app.Use(async (context, siguiente) =>
            {
                using (var ms = new MemoryStream())
                {
                    //Se asigna el body del response en una variable y se le da el valor de memorystream
                    var bodyOriginal = context.Response.Body;
                    context.Response.Body = ms;

                    //Permite continuar con la línea
                    await siguiente.Invoke();

                    //Guardamos lo que le respondemos al cliente en el string
                    ms.Seek(0, SeekOrigin.Begin);
                    string response = new StreamReader(ms).ReadToEnd();
                    ms.Seek(0, SeekOrigin.Begin);

                    //Leemos el stream y lo colocamos como estaba
                    await ms.CopyToAsync(bodyOriginal);
                    context.Response.Body = bodyOriginal;

                    logger.LogInformation(response);
                }
            });*/

            app.Map("/maping", app =>
            {
                app.Run(async context =>
                {
                    await context.Response.WriteAsync("Interceptando las peticiones");
                });
            });
            
            
            app.UseMiddleware<ResponseHttpMiddleware>();
            
            // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseResponseCaching(); //Filtros

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
