
using CodeEscape.Configurations;
using CodeEscape.Models;
using CodeEscape.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// 🔹 Swagger + JWT
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.OrderActionsBy(apiDesc =>
    {
        var methodOrder = apiDesc.HttpMethod switch
        {
            "GET" => "1",
            "POST" => "2",
            "PUT" => "3",
            "DELETE" => "4",
            _ => "5"
        };

        return methodOrder;
    });
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Code Escape API",
        Version = "v1.0",
        Description = """
        
        API para gerenciamento do game Code Escape
        

        Desenvolvido por Gabriel Duarte
        
        """,
        Contact = new OpenApiContact
        {
            Name = "Gabriel Duarte"
        }
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Digite o token JWT sem 'Bearer'"
    });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    options.IncludeXmlComments(xmlPath);

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
// 🔹 DbContext
builder.Services.AddDbContext<CodeEscapeContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    )
);

// Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// email

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

// services
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<FeedbackService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<GameSessionsService>();
builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<RankingService>();
builder.Services.AddScoped<RoomService>();
builder.Services.AddScoped<UploadService>();
builder.Services.AddHttpContextAccessor();

// 🔹 JWT Authentication
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

var app = builder.Build();

// 🔹 Swagger
app.UseSwagger();
app.UseSwaggerUI();

// arquivo
app.UseStaticFiles();

// cors
app.UseCors("CorsPolicy");
// 🔹 Pipeline
app.UseAuthentication(); // ⚠️ sempre antes
app.UseAuthorization();

app.MapControllers();

app.Run();