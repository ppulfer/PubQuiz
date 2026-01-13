using Microsoft.EntityFrameworkCore;
using PubQuiz.Web.Components;
using PubQuiz.Web.Data;
using PubQuiz.Web.Hubs;
using PubQuiz.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PubQuizDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<GameService>();
builder.Services.AddSingleton<ScoringService>();
builder.Services.AddSingleton<WordleService>();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSignalR();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapHub<GameHub>("/gamehub");

await SeedData.InitializeAsync(app.Services);

app.Run();
