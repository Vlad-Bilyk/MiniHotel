using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MiniHotel.API;
using MiniHotel.API.Configuration;
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
using System.Security.Claims;
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
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

builder.Services.AddTransient<ISeeder, RoleSeeder>();
builder.Services.AddTransient<ISeeder, HotelUserSeeder>();
builder.Services.AddTransient<ISeeder, RoomTypeSeeder>();
builder.Services.AddTransient<ISeeder, RoomSeeder>();
builder.Services.AddTransient<ISeeder, ServiceSeeder>();
builder.Services.AddTransient<ISeeder, BookingSeeder>();
builder.Services.AddTransient<ISeeder, InvoiceSeeder>();

builder.Services.AddAutoMapper(typeof(MappingConfig));

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.Configure<LiqPayOptions>(builder.Configuration.GetSection("LiqPay"));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomOperationIds(apiDesc => $"{apiDesc.ActionDescriptor.RouteValues["controller"]
         + apiDesc.ActionDescriptor.RouteValues["action"]}");

    var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileName));
});

var app = builder.Build();

app.UseCors("AllowAllOrigins");

app.UseMiddleware<ExceptionHandlingMiddleware>();

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// TODO: Remove this middleware in production
app.Use(async (context, next) =>
{
    var identity = new ClaimsIdentity("Fake");
    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, "e09dc253-eaec-4204-80f2-e62025881eca"));
    identity.AddClaim(new Claim(ClaimTypes.Name, "TestUser"));
    var principal = new ClaimsPrincipal(identity);

    context.User = principal;
    await next();
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
