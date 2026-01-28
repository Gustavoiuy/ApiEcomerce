using System.Text;
using ApiEcommerce.Constants;
using Infrastructure.Data;
using Domain.Entities;
using Infrastructure.Repository;
using Application.IRepository;
using Application.Maping;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using Mapster;
using Asp.Versioning;



var builder = WebApplication.CreateBuilder(args);
var dbConnectionString = builder.Configuration.GetConnectionString("ConexionSql");
// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(options =>
  options.UseSqlServer(dbConnectionString)
  .UseSeeding((context, _) =>
  {
      var appContext = (ApplicationDbContext)context;
      DataSeeder.SeedData(appContext);
      appContext.SaveChanges();
  })
);
builder.Services.AddResponseCaching(options =>
{
    options.MaximumBodySize = 1024 * 1024;
    options.UseCaseSensitivePaths = true;
});


builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddSingleton(TypeAdapterConfig.GlobalSettings);
MapsterConfig.RegisterMappings(TypeAdapterConfig.GlobalSettings);
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// var secretKey = builder.Configuration.GetValue<string>("ApiSettings:SecretKey");
// if (string.IsNullOrEmpty(secretKey))
// {
//     throw new InvalidOperationException("SecretKey no esta configurada");
// }
var secretKey = builder.Configuration.GetValue<string>("ApiSettings:SecretKey") ?? "ClaveTemporalDePrueba12345678901234567890";
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
})
;

builder.Services.AddControllers(
    option =>
{
    option.CacheProfiles.Add(CacheProfiles.Default10, CacheProfiles.Profile10);
    option.CacheProfiles.Add(CacheProfiles.Default20, CacheProfiles.Profile20);
}
);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
      options =>
  {

      options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
      {
          Description = "Nuestra API utiliza la Autenticación JWT usando el esquema Bearer. \n\r\n\r" +
                      "Ingresa la palabra a continuación el token generado en login.\n\r\n\r" +
                      "Ejemplo: \"12345abcdef\"",
          Name = "Authorization",
          In = ParameterLocation.Header,
          Type = SecuritySchemeType.Http,
          Scheme = "Bearer"
      });
      options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
      {
        new OpenApiSecurityScheme
        {
          Reference = new OpenApiReference
          {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
          },
          Scheme = "oauth2",
          Name = "Bearer",
          In = ParameterLocation.Header
        },
        new List<string>()
      }
    });
      options.SwaggerDoc("v1", new OpenApiInfo
      {
          Version = "v1",
          Title = "API Ecommerce",
          Description = "API para gestionar productos y usuarios",
          TermsOfService = new Uri("http://example.com/terms"),
          Contact = new OpenApiContact
          {
              Name = "TrejoDev24",
              Url = new Uri("https://trejodev24.vercel.app/")
          },
          License = new OpenApiLicense
          {
              Name = "Licencia de uso",
              Url = new Uri("http://example.com/license")
          }
      });
      options.SwaggerDoc("v2", new OpenApiInfo
      {
          Version = "v2",
          Title = "API Ecommerce V2",
          Description = "API para gestionar productos y usuarios",
          TermsOfService = new Uri("http://example.com/terms"),
          Contact = new OpenApiContact
          {
              Name = "TrejoDev24",
              Url = new Uri("https://trejodev24.vercel.app/")
          },
          License = new OpenApiLicense
          {
              Name = "Licencia de uso",
              Url = new Uri("http://example.com/license")
          }
      }
      );


  }
);



var apiVersioningBuilder = builder.Services.AddApiVersioning(option =>
{
    option.AssumeDefaultVersionWhenUnspecified = true;
    option.DefaultApiVersion = new ApiVersion(1, 0);
    option.ReportApiVersions = true;
    // option.ApiVersionReader = ApiVersionReader.Combine(new QueryStringApiVersionReader("api-version")); //?api-version
});
apiVersioningBuilder.AddApiExplorer(option =>
{
    option.GroupNameFormat = "'v'VVV"; // v1,v2,v3...
    option.SubstituteApiVersionInUrl = true; // api/v{version}/products
});


//add configuration the cors
builder.Services.AddCors(options =>
  {
      options.AddPolicy("AllowSpecificOrigin",
      builder =>
      {
          builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
      }
      );
  }
);

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(options =>
  {
      options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
      options.SwaggerEndpoint("/swagger/v2/swagger.json", "v2");
  });
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{


}

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigin");

app.UseResponseCaching();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
