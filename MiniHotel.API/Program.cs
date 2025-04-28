using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MiniHotel.API.Configuration;
using MiniHotel.API.Middlewares;
using MiniHotel.Application.Interfaces;
using MiniHotel.Application.Interfaces.IRepository;
using MiniHotel.Application.Interfaces.IService;
using MiniHotel.Application.Services;
using MiniHotel.Infrastructure.Data;
using MiniHotel.Infrastructure.Data.Seed;
using MiniHotel.Infrastructure.Identity;
using MiniHotel.Infrastructure.Mapping;
using MiniHotel.Infrastructure.Reposiitories;
using MiniHotel.Infrastructure.Services;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<MiniHotelDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultPostgresSQLConnection"));
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<MiniHotelDbContext>()
    .AddDefaultTokenProviders();

// Configure authentication options
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // TODO: change to 'true' in product
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

// Add CORS policy
var allowedOrigins = builder.Configuration.GetValue<string>("allowedOrigins")!.Split(",");

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IRoomTypeRepository, RoomTypeRepository>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IFileService, FileService>();

builder.Services.AddTransient<ISeeder, RoleSeeder>();
builder.Services.AddTransient<ISeeder, HotelUserSeeder>();
builder.Services.AddTransient<ISeeder, RoomTypeSeeder>();
builder.Services.AddTransient<ISeeder, RoomSeeder>();
builder.Services.AddTransient<ISeeder, ServiceSeeder>();
builder.Services.AddTransient<ISeeder, BookingSeeder>();
builder.Services.AddTransient<ISeeder, InvoiceSeeder>();

builder.Services.AddAutoMapper(typeof(MappingConfig));

builder.Services.AddTransient<ExceptionHandlingMiddleware>();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.Configure<LiqPayOptions>(builder.Configuration.GetSection("LiqPay"));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "MiniHotel API", Version = "v1" });

    options.AddServer(new Microsoft.OpenApi.Models.OpenApiServer
    {
        Description = "Development server",
        Url = "https://localhost:7252"
    });

    options.CustomOperationIds(apiDesc => $"{apiDesc.ActionDescriptor.RouteValues["action"]}");

    var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileName));

    // Add security definition for JWT
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "¬вед≥ть токен €к: Bearer {token}"
    });

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

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors();

app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

// If the path is not found - return index.html (for SPA routes)
app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == 404 &&
        !Path.HasExtension(context.Request.Path.Value) &&
        !context.Request.Path.Value.StartsWith("/api"))
    {
        context.Request.Path = "/index.html";
        context.Response.StatusCode = 200;
        await next();
    }
});

// Migrate the database
using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<MiniHotelDbContext>();
await db.Database.MigrateAsync();

// Seed the database
var seeders = scope.ServiceProvider.GetServices<ISeeder>();
foreach (var seeder in seeders)
{
    await seeder.SeedAsync();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
