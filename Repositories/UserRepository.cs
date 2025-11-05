// Crie um arquivo em - Repositories/UserRepository.cs
// Importa o namespace que fornece a classe para trabalhar com SQL Server, como SqlConnection e SqlCommand
using Microsoft.Data.SqlClient;
// Importa a classe de configuração do .NET (IConfiguration), usada para ler as connection string e appsettings.json
using Microsoft.Extensions.Configuration;
// Importa o namespace onde está a classe usuário 'User' (Modelo que representa a tabela dbo.Usuario)
using projetoMakers.Api.Data.Models;
// Importa o namespace onde permite a programação assícrona com Task async/await
using System.Threading.Tasks;
// Importa o namespace System, que contém tipos básicos do dotNet(.NET), por exemplo o DateTime usado 
// Vamos utilizar um recurso do 'ArgumentNullException'
using System;
// Define o namespace atual - agrupa classes que lidam com repositórios de dados dentro do projeto
namespace projetoMakers.Api.Repositories
{
    // Define a classe pública 'UserRepository', responsável por manipular registros de usuários no banco (CRUD)
    public class UserRepository
    {
        // Campo privado e somente leitura (_connectionString) que armazenará a string de conexão com banco
        // Inicializado com string.Empty apenas para evitar o aviso de nullabilidade (CS8618)
        // Seu valor real será atribuido no construtor
        private readonly string _connectionString = string.Empty;
        // Construtor da classe que recebe uma instância de 'IConfiguration' injetada pelo sistema (Dependency Injection)
        public UserRepository(IConfiguration configuration)
        {
            // Obtém do arquivo appsttings.json a string de conexão chamada de 'DefaultConnection'
            // Caso não exista ou seja nula, lança uma exceção 'ArgumentNullException', impedindo o uso da classe sem
            // uma conexão valida.
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("Connection string 'DefaultConnection' not found.");
        }
        // Método público e assícrono que cria um novo banco.
        // Recebe um objeto usuário 'User' como parametro.
        // Retorna uma Task (sem valor), indicando operação assíncrona
        public async Task CreateUserAsync(User user)
        {
            // Cria uma nova conexão SQL usando a string de conexão obtida no construtor.
            // O 'using' garante que o objeto seja fechado e descartado automaticamente após o uso.
            using (var connection = new SqlConnection(_connectionString))
            {
                // Abre a conexão com o banco de dados de forma assíncrona (sem travar a thread principal)
                await connection.OpenAsync();
                // QUERY SQL
                // Define o comando SQL que será execultado
                // Está inserindo dados em um novo registro na tabela 'obo.Usuario'
                // O campo CreatedAt será removido pois será inserido pelo banco de dados automaticamente usando como 
                // ex. DEFAULT GETDATE()
                var commandText = @"INSERT INTO Usuarios 
                        (NomeCompleto, Email, PassWordHash, HashPass, DataAtualizacao, StatusId) 
                     VALUES 
                        (@NomeCompleto, @Email, @PassWordHash, @HashPass, @DateUp, @StatusId)"; 
                // Cria um comando SQL associado à conexão de aberta
                // Também está dentro de um bloco 'using' para liberar os recursos do comando após execução
                using (var command = new SqlCommand(commandText, connection))
                {
                    // Adiciona os valores como parâmentros para o comando SQL.
                    // Os protegem contra SQL Injection.
                    command.Parameters.AddWithValue("@NomeCompleto", user.NomeCompleto);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@PassWordHash", user.PassWordHash);
                    command.Parameters.AddWithValue("@HashPass", user.HashPass);
                    // Caso seja nulo no modelo, converte para 'DBNull.Value'
                    // Representação de Null no banco de dados
                    command.Parameters.AddWithValue("@DateUp", (object)user.DateUp ?? DBNull.Value);
                    command.Parameters.AddWithValue("@StatusId", user.StatusId);
                    // Execulta o comando SQL de forma assíncrona
                    // como é um 'INSERT', não retorna linhas (por isso 'ExecuteNonQueryAsync()')
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var commandText = @"SELECT TOP 1 * FROM dbo.Usuarios WHERE Email = @Email";
                using (var command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new User // Mapeia os dados do banco de dados para o objeto Usuario 'User'
                            {
                                UsuarioId = reader.GetInt32(reader.GetOrdinal("UsuarioId")),
                                NomeCompleto = reader.GetString(reader.GetOrdinal("NomeCompleto")),
                                Email = reader.GetString(reader.GetOrdinal("email")),
                                PassWordHash = reader.GetString(reader.GetOrdinal("PassWordHash")),
                                HashPass = reader.GetString(reader.GetOrdinal("HashPass")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("DataCriacao")),
                                DateUp = reader.IsDBNull(reader.GetOrdinal("DataAtualizacao"))
                                        ? null
                                        : reader.GetDateTime(reader.GetOrdinal("DataAtualizacao")),
                                StatusId = reader.GetInt32(reader.GetOrdinal("StatusId"))

                            };
                        }
                    }
                }
            }
            return null;
        }
    }
}
