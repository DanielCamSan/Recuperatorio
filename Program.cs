using Microsoft.AspNetCore.RateLimiting;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", fixedOptions =>
    {
        fixedOptions.PermitLimit = 100; // 100 requests
        fixedOptions.Window = TimeSpan.FromMinutes(1); // por minuto
        fixedOptions.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        fixedOptions.QueueLimit = 10; // m�ximo 10 en cola
    });
});

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("default", config =>
    {
        config.PermitLimit =3;
        config.Window = TimeSpan.FromMinutes(1);
        config.QueueLimit = 0;

    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseRateLimiter();

app.UseRouting();

// Use Rate Limiting
app.UseRateLimiter();

app.MapControllers().RequireRateLimiting("default");

app.Run();