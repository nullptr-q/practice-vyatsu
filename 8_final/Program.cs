using Microsoft.EntityFrameworkCore;
using TimeTrackingApi.Services;
using TimeTrackingApi.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Настройка SQLite
builder.Services.AddDbContext<TimeTrackingDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<TimeEntryService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API учета рабочего времени",
        Version = "v1",
        Description = "Сервис для регистрации трудозатрат сотрудников крупной компании"
    });

    // Настраиваем использование XML-комментариев.
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var app = builder.Build();

// Настройка Swagger для удобства останется на /swagger.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Time Tracking API v1");
});

app.UseDefaultFiles(); // Запуск index.html из wwwroot по умолчанию
app.UseStaticFiles();

app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TimeTrackingDbContext>();
    db.Database.EnsureCreated();
}

app.Run();