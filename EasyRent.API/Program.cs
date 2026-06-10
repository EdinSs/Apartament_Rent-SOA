using EasyRent.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ---- Services ----
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Infrastructure: EF Core DbContext + repositories (connection string "Default").
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// ---- HTTP pipeline ----
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
