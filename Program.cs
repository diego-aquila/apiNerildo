
using projetoMakers.Api.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registra o repositório 'UserRepository' no container de injeção de dependência (Dependency Injection 'DI')
// O 'AddScoped' cria uma nova instância do repositório a cada requisição HTTP
// Assim, cada requisição terá seu próprio objeto 'UserRepository'
builder.Services.AddScoped<UserRepository>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
 
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();


app.MapControllers();

app.Run();

/*
 * Rai
 * Eduarda
 * Alexia
 * Benjamin
 * Artur
 * matehus
 * igor
 * stefany
 * igor
 * ryan
 * luiz
 * luiz
 * wagner
 * nicolay
 * italo
 * miguel
 * 
 * */