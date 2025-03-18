using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MiniHotel.API;
using MiniHotel.Application.Interfaces.IRepository;
using MiniHotel.Application.Interfaces.IService;
using MiniHotel.Infrastructure.Data;
using MiniHotel.Infrastructure.Identity;
using MiniHotel.Infrastructure.Mapping;
using MiniHotel.Infrastructure.Services;
using MiniHotel.Infrastructure.Reposiitories;
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

builder.Services.AddScoped<IRoomRepository, RoomRepository>();

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddAutoMapper(typeof(MappingConfig));

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await RoleSeeder.SeedRolesAsync(roleManager);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
