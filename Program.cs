using Npgsql;
using CorpBankingDashboard.Components;
using WebDashboard.Repositories;

var builder = WebApplication.CreateBuilder(args);

var connString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' tidak ditemukan.");

builder.Services.AddNpgsqlDataSource(connString);
builder.Services.AddScoped<AnalyticsRepository>();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

// Minimal API Endpoints (JSON)
app.MapGet("/api/periods", async (AnalyticsRepository repo) => Results.Ok(await repo.GetAvailablePeriodsAsync()));
app.MapGet("/api/summary/{date}", async (DateOnly date, AnalyticsRepository repo) =>
{
    var summary = await repo.GetMonthlySummaryAsync(date);
    return summary is null ? Results.NotFound() : Results.Ok(summary);
});
app.MapGet("/api/audit/jobs", async (AnalyticsRepository repo) => Results.Ok(await repo.GetRecentJobLogsAsync()));

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();