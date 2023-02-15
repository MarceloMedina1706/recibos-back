using Template.Helpers;
using Template.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ILiquidacionService, LiquidacionService>();
builder.Services.AddScoped<ITipoLiquidacionService, TipoLiquidacionService>();
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

var provider = builder.Services.BuildServiceProvider();
var cofiguration = provider.GetRequiredService<IConfiguration>();
builder.Services.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var origin = cofiguration.GetValue<string>("FrontUrl");
app.UseCors(options =>
{
    options.WithOrigins(origin);
    options.AllowAnyMethod();
    options.AllowAnyHeader();
});

app.UseMiddleware<JwtMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
