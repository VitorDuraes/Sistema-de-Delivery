using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Sistema_de_Delivery.src.Data;
using Sistema_de_Delivery.src.Services;
using System.Text;
using System.Reflection;


var builder = WebApplication.CreateBuilder(args);

// Configurações para a conexão com o banco de dados
builder.Services.AddDbContext<DeliveryContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuração de autenticação JWT
builder.Services.AddScoped<JwtService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                    };
                });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact",
        builder => builder.WithOrigins("http://localhost:3000") // URL do frontend React
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials());

});
// Adiciona os controllers ao serviço
builder.Services.AddControllers();

// Adiciona a documentação do Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Sistema de Delivery API",
        Version = "v1",
        Description = "API RESTful para gerenciamento de clientes em um sistema de delivery. Desenvolvida com ASP.NET Core, utiliza autenticação via JWT e segue boas práticas de segurança, versionamento e organização em camadas. A API expõe endpoints para cadastro, login e consulta de clientes. A base de dados relacional armazena informações como nome, e-mail e senha criptografada. Ideal para integração com sistemas de pedidos e gerenciamento de usuários."
    });

    // Carregar os comentários XML para cada assembly
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    // Configuração de autenticação no Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Insira o token JWT com o prefixo 'Bearer'",
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new List<string>()
        }
    });
});

// Injeção de dependências para o serviço de PDF
builder.Services.AddScoped<PedidoPdfService>();

var app = builder.Build();

// Configuração do pipeline de requisições HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowReact");
app.UseAuthentication();  // Adiciona autenticação
app.UseAuthorization();   // Adiciona autorização
app.UseHttpsRedirection();
// Mapeia os controladores
app.MapControllers();

app.Run();
