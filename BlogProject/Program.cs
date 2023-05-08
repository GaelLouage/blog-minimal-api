using BlogProject.Endpoints;
using BlogProject.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//add services for mongodb
builder.Services.ConfigureServicesExtensions(builder.Configuration);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// user endpoints
app.RegisterUserEndPoints();
// blog endpoints
app.RegisterBlogEndPoints();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
