using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks; // Añadido para Task
using System.Text;
using Identity.Api.Model;
using Identity.Api.Persistence.DataBase;
using Identity.Api.Services;
using Identity.Api.Interfaces;
using Microsoft.Extensions.Options;
using Modelo.laconcordia.Modelo.Database;


var builder = WebApplication.CreateBuilder(args);
// Define la URL base según el entorno
string baseApiUrl;
if (builder.Environment.IsDevelopment())
{
    baseApiUrl = "http://localhost:5191";
}
else
{
    baseApiUrl = "https://api.laconcordia.compugtech.com";
}

// Agregamos la URL como un valor de configuración disponible para toda la aplicación
builder.Configuration["ApiBaseUrl"] = baseApiUrl;

// Define la variable para CORS
string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// CORS CORREGIDO
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                    policy =>
                    {
                        if (builder.Environment.IsDevelopment())
                        {
                            // Desarrollo: permitir orígenes específicos de localhost
                            policy.WithOrigins(
                                    "https://localhost:7180",      // Tu Blazor HTTPS
                                    "http://localhost:7180",       // Tu Blazor HTTP
                                    "https://localhost:7171",      // Puerto alternativo HTTPS
                                    "http://localhost:5047",       // Blazor Server
                                    "http://localhost:5000",       // Puerto alternativo
                                    "http://localhost:3000" ,      // Otro puerto común
                                    "http://localhost:5213",        // Otro puerto común
                                     "https://localhost:44377"        // Otro puerto común
                                  )
                                  .AllowAnyHeader()
                                  .WithExposedHeaders("totalAmountPages")
                                  .AllowAnyMethod()
                                  .AllowCredentials();
                        }
                        else
                        {
                            // Producción: dominios específicos
                            policy.WithOrigins(
                                    "https://lconcordia.compugtech.com",    // Tu dominio principal
                                    "http://lconcordia.compugtech.com",     // HTTP fallback
                                    "https://www.lconcordia.compugtech.com", // Con www
                                    "http://www.lconcordia.compugtech.com" ,  // Con www HTTP
                                    "https://localhost:7180",      // Tu Blazor HTTPS
                                    "http://localhost:7180",       // Tu Blazor HTTP
                                    "https://localhost:7171",      // Puerto alternativo HTTPS
                                    "http://localhost:5047",       // Blazor Server
                                    "http://localhost:5000",       // Puerto alternativo
                                    "http://localhost:3000",      // Otro puerto común
                                    "http://localhost:5213"       // Otro puerto común
                                    
                                  )
                                  .AllowAnyHeader()
                                  .WithExposedHeaders("totalAmountPages")
                                  .AllowAnyMethod()
                                  .AllowCredentials();
                        }
                    });
});


// DbContext
builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDBContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

builder.Services.AddRazorPages();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["jwt:key"])),
        ClockSkew = TimeSpan.Zero
    });


builder.Services.AddScoped<IMenuInfo, MenuInfoServices>();
builder.Services.AddScoped<IMenuInfo, MenuInfoServices>();
builder.Services.AddScoped<INavigation, NavigationServices>();
builder.Services.AddScoped<ICargo, CargoServices>();
builder.Services.AddScoped<IUnidad, UnidadServices>();
// Registrar servicios de permisos
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IAdvancedPermissionService, AdvancedPermissionService>();
builder.Services.AddScoped<IParentesco, ParentescoServices>();
builder.Services.AddScoped<IEstadoCivil, EstadoCivilServices>();
builder.Services.AddScoped<IEmpresa, EmpresaServices>();    
builder.Services.AddScoped<ITipolicencium, TipolicenciumServices>();
builder.Services.AddScoped<INacionalidad, NacionalidadServices>();
builder.Services.AddScoped<INiveleducacion, NiveleducacionServices>();
builder.Services.AddScoped<ICargo, CargoServices>();
builder.Services.AddScoped<IParentesco, ParentescoServices>();
builder.Services.AddScoped<IDuenopuesto, DuenopuestoServices>();



builder.Services.AddControllers();

// Construir la aplicación
var app = builder.Build();

// Logging para debug
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation($"🔧 Entorno: {app.Environment.EnvironmentName}");
logger.LogInformation($"🌐 API Base URL: {baseApiUrl}");


// Configure the HTTP request pipeline (equivalente a Configure)
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    logger.LogInformation("🔧 Modo desarrollo - CORS permisivo habilitado");
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios.
    app.UseHsts();
    logger.LogInformation("🔧 Modo producción - CORS específico habilitado");
}

// ORDEN CORRECTO DEL PIPELINE
app.UseCors(MyAllowSpecificOrigins); // CORS PRIMERO
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

logger.LogInformation("🚀 API iniciada correctamente");
app.Run();