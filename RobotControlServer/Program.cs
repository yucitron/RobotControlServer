using Microsoft.AspNetCore.HttpOverrides;
using RobotControlServer.Hubs;
using RobotControlServer.Services;

var builder = WebApplication.CreateBuilder(args);

// Console logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddRazorPages();
builder.Services.AddSignalR(o =>
{
    o.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
    o.KeepAliveInterval = TimeSpan.FromSeconds(10);
});

// Robot ayarlarýný oku
builder.Services.Configure<RobotOptions>(builder.Configuration.GetSection("Robot"));

// SAHTE yerine GERÇEK TCP sürücüyü kullan:
builder.Services.AddSingleton<IRobotDriver, TcpRobotDriver>();
// (Geliþtirme aþamasýnda sahte istersek yukarýyý yorumlayýp FakeRobotDriver'ý açarýz.)

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
// app.UseHttpsRedirection();  // Cloudflare ile dýþarýya zaten HTTPS veriyoruz
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseStaticFiles();
app.UseRouting();

app.MapRazorPages();
app.MapHub<RobotControlServer.Hubs.RobotHub>("/robotHub");

app.Run();
