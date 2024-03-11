// Opsætning af ASP.NET Core webapplikationen med middleware og services
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Json;

// Import af egne namespaces
using Data; 
using Service; 
using Model; 

var builder = WebApplication.CreateBuilder(args); 

var AllowSomeStuff = "_AllowSomeStuff"; // 

// Konfiguration af CORS for at tillade kommunikation med andre domæner
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowSomeStuff, builder =>
    {
        builder.AllowAnyOrigin() 
               .AllowAnyHeader() 
               .AllowAnyMethod(); 
    });
});

// Konfiguration af databasekontekst (SQLite)
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("ContextSQLite")));

// Registrering af DataService i Dependency Injection containeren
builder.Services.AddScoped<DataService>();

// Konfiguration af JSON serialization for at undgå cyklisk referencefejl
builder.Services.Configure<JsonOptions>(options =>
{
    // Ignorer cykliske referencer ved serialisering
    options.SerializerOptions.ReferenceHandler =
        System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

var app = builder.Build(); 

using (var scope = app.Services.CreateScope()) 
{
    var dataService = scope.ServiceProvider.GetRequiredService<DataService>(); // Hentning af DataService fra containeren
    dataService.SeedData(); // Kald af SeedData metoden for at initialisere data
}

app.UseHttpsRedirection(); // Middleware for at omdirigere HTTP til HTTPS
app.UseCors(AllowSomeStuff); // Middleware til at bruge CORS politik

{
    // Definering af endpoints

    // Velkomstbesked
    app.MapGet("/", (DataService service) =>
    {
        return new { message = "Velkommen til Reddit!" };
    });

    // Hentning af alle indlæg
    app.MapGet("/api/posts/", (DataService service) =>
    {
        return service.GetPosts().Select(b => new
        {
            ID = b.Id,
            Content = b.Content,
            Username = b.User.Username,
            CreationDate = b.CreationDate,
            Votes = b.Upvotes,
            Author = new 
            {
                
            }
        });
    });

    // Hentning af specifikt indlæg
    app.MapGet("/api/posts/{id}", (DataService service, int id) =>
    {
        return service.GetPost(id);
    });

    // Opret et nyt indlæg
    app.MapPost("/api/posts", async (DataService service, Post post) =>
    {
        return service.CreatePost(post.Title, post.Content, post.UserId);
    });

    // Opret en ny kommentar til et indlæg
    app.MapPost("/api/posts/{id}/comments", async (DataService service, int id, Comment comment) =>
    {
        return await service.CreateComment(comment.Content, id, comment.UserId);
    });

    // Opstemning af et indlæg
    app.MapPut("/api/posts/{id}/upvote", async (DataService service, int id) =>
    {
        return await service.UpvotePost(id);
    });

    // Nedstemning af et indlæg
    app.MapPut("/api/posts/{id}/downvote", async (DataService service, int id) =>
    {
        return await service.DownvotePost(id);
    });

    // Opstemning af en kommentar
    app.MapPut("/api/posts/{postId}/comments/{commentId}/upvote", async (DataService service, int postId, int commentId) =>
    {
        return await service.UpvoteComment(commentId, postId);
    });

    // Nedstemning af en kommentar
    app.MapPut("/api/posts/{postId}/comments/{commentId}/downvote", async (DataService service, int postId, int commentId) =>
    {
        return await service.DownvoteComment(commentId, postId);
    });

}; 
app.Run(); 
