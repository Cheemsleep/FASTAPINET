using FastApiMvc.Api.Extensions;
using FastApiMvc.Common;
using FastApiMvc.Service.Interfaces;
using FastApiMvc.Service.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add custom services
builder.Services.AddDatabase(builder.Configuration)
                .AddRedisCache(builder.Configuration)
                .AddAutoMapper()
                .AddValidation()
                .AutoRegisterServices()
                .AutoRegisterRepositories()
                .AutoRegisterValidators();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseGlobalExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
