using WepAPiR_system.CommonUtility;
using WepAPiR_system.Repository;
using WepAPiR_system.Services;

var builder = WebApplication.CreateBuilder(args);

//Sets up your app to allow CORS from Angular 
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

builder.Services.AddControllers(); //support for controller-based API endpoints
builder.Services.AddSwaggerGen(); //Swagger/OpenAPI support to generate interactive API documentation.
builder.Services.AddHttpClient(); //Registers the HttpClient factory for making HTTP requests
builder.Services.AddMemoryCache(); //Enables in-memory caching for performance optimization
// Register Repository and Service
builder.Services.AddScoped<IHackerNewsRepository, HackerNewsRepository>();  //Repository layer with scoped lifetimes
builder.Services.AddScoped<IHackerNewsService,HackerNewsService>(); //service layer with scoped lifetimes

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // You can pass options here to customize UI
}
app.UseMiddleware<GlobalExceptionHandingClass>();  //Adds custom global exception-handling middleware to catch and handle unhandled exceptions globally.
app.UseHttpsRedirection();
app.UseCors("AllowAngularApp"); //Enable CORS for Angular App
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers(); //Maps controller routes to the HTTP pipeline
app.Run();