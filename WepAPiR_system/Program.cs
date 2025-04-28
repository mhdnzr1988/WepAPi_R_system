using WepAPiR_system.CommonUtility;
using WepAPiR_system.Repository;
using WepAPiR_system.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy =>
        {
            policy
                .WithOrigins("http://localhost:4200") // Replace with your Angular app URL
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();
// Register Repository and Service
builder.Services.AddScoped<IHackerNewsRepository, HackerNewsRepository>();
builder.Services.AddScoped<IHackerNewsService,HackerNewsService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // You can pass options here to customize UI
}
app.UseMiddleware<GlobalExceptionHandingClass>();
app.UseHttpsRedirection();
app.UseCors("AllowAngularApp");
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();
app.Run();
