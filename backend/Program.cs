using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=movies.db"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", p =>
        p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    if (!db.Movies.Any())
    {
        var a1 = new Actor { Name = "Кристиан Бейл", Bio = "Известен с физическите си трансформации." };
        var a2 = new Actor { Name = "Хийт Леджър", Bio = "Легендарен с ролята си на Джокера." };
        var a3 = new Actor { Name = "Леонардо Ди Каприо", Bio = "Носител на Оскар." };

        db.Movies.AddRange(
            new Movie { Title = "Тъмният рицар", ReleaseYear = 2008, Actors = new() { a1, a2 } },
            new Movie { Title = "Генезис (Inception)", ReleaseYear = 2010, Actors = new() { a1, a3 } }
        );
        db.SaveChanges();
    }
}

app.UseCors("AllowReact");
app.MapControllers();
app.Run();
