// Controllers/AccountController.cs
// Fornece os recursos para criar controladores de endpoints em APIs

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
// Usado para capturar erros específicos do SQL Server
using Microsoft.Data.SqlClient;
// Importa os modelos (ViewModels) usados nas requisições, como RegisterRequestModel
using projetoMakers.Api.Models;
// Importa o repositoório responsável pelo acesso ao banco de dados
using projetoMakers.Api.Repositories;
// Importa o namespace System, que contém tipos básicos do dotNet(.NET)
using System;
using System.Linq.Expressions;


//  ----- ADICIONADO PARA CRIPTOGRAFIA
using System.Security.Cryptography;
using System.Text;
// --- ADICIONADO O BCRYPT
using BCrypt.Net;


// Importa o namespace onde permite a programação assícrona com Task async/await
using System.Threading.Tasks;
// Cria um apelido (alias) para evitar conflito entre modelos com o mesmo nome
// Aqui, 'DbUsuario' faz a referência ao modelo de usuário do banco de dados
using DbUsuario = projetoMakers.Api.Data.Models.User;

namespace projetoMakers.Api.Controllers
{
    // Indica que esta classe é um controlador de API
    [ApiController]

    // Define a rota base do controller
    // Exemplo: 'api/account' chamará este controller
    [Route("api/[controller]")]

    public class AccountController: ControllerBase
    {
        // Declaração do reposítório de usuário para interagir com o banco de dados
        private readonly UserRepository _userRepository;

        // Construtor de controller - injeta o repositório via Injeção de Dependência (DI)
        public AccountController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // =======================================
        // MÉTODO DE REGISTRO DE USUÁRIO
        // =======================================

        [HttpPost("register")]

        // Define um endpoint POST acessível em 'api/account/register'
        public async Task<IActionResult> Register([FromBody] RegisterRequestModel model)
        {
            try
            {
                // Cria um novo objeto de usuário para salvar no banco de dados
                string PassWordHash = ComputeSha256Hash(model.PassWordHash);
                string Email = ComputeSha256Hash(model.Email);

                string dataString = DateTime.Now.ToString();// dia/mes/ano hora/min/seg
                string apiKey = "mangaPara_todos"; // criando a palavra chave para criptografia

                // montando a string para a criptografia
                string PassWordHash2 = PassWordHash + Email + apiKey;
                string HashPass2 = Email + PassWordHash + dataString + apiKey;

                // --- criptografando usando o Bcrypt
                string passWordHash = BCrypt.Net.BCrypt.HashPassword(PassWordHash2);
                string HashPass = BCrypt.Net.BCrypt.HashPassword(HashPass2);

                var novoUser = new DbUsuario
                {
                    // Nome completo vindo do corpo da requisição
                    NomeCompleto = model.NomeCompleto,
                    // Email informado no registro
                    Email = model.Email,
                    // Senha 'PassWordHash' e 'HashPass' criptografada
                    PassWordHash = passWordHash,
                    HashPass = HashPass,
                    // Data atual para o controle de atualização
                    DateUp = DateTime.Now,
                    StatusId = 2
                };

                // Chama o repositório para inserir o novo usuário no banco de dados
                await _userRepository.CreateUserAsync(novoUser);

                // Retorna sucesso com a menssagem amigavel em "JSON"
                return Ok(new { 
                    erro = false,
                    Message = "Usuário cadastrado com sucesso!" });
            }
            // Tratamento de erro de duplicidade (chave única) no email no banco de dados.
            catch (SqlException ex)  when (ex.Number == 2627 || ex.Number == 2601)
            {
                return Conflict(new {
                    erro = true,
                    Message = "Este email já está em uso!" });
            }
            // Captura outros erros genéricos e retorna código 500 (erro interno SRV)
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Erro: {ex.Message}" });
            }
        }
        // =======================================
        // MÉTODO DE LOGIN DE USUÁRIO
        // =======================================
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestModel model)
        {
            // 1. Buscar o usuário no banco de dados
            var user = await _userRepository.GetUserByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized(new { message = "Usuário ou senha inválidos!" });
            }
            // 2. Recria a hash de login exatamente como no registro
            string PassWordHash = ComputeSha256Hash(model.PassWordHash);
            string Email = ComputeSha256Hash(model.Email);
            string apiKey = "mangaPara_todos"; // criando a palavra chave para criptografia
            // montando a string para a criptografia
            string PassWordHash2 = PassWordHash + Email + apiKey;
            // 3. Veridicar o hash usando BCrypt
            // Compara o hash recém-criado com o hash salvo no banco (user.PassWordHas)
            bool isPasswordValid;
            try
            {
                isPasswordValid = BCrypt.Net.BCrypt.Verify(PassWordHash2, user.PassWordHash);
            }
            catch (Exception)
            {
                isPasswordValid = false;
            }
            if (!isPasswordValid)
            {
                return Unauthorized(new { 
                    erro = true,
                    message = "Usuário ou senha inválidos!"
                    
                });
            }
            // 4. SUCESSO!!! (próxima etapa inserir o gerador do JWT)
            return Ok(new { 
                erro = false,
                message = "Login realizado com sucesso!",
                
            });
        }
        // -----  MÉTODO DE HASHING DO SHA256 -----
        private string ComputeSha256Hash(string rawData)
        {
            // Cria uma instância de SHA256
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Computa o hash dos dados de entrada 'string'
                // e retorna o resultado como um 'Array' de bytes
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Converte o 'Array' de bytes em uma string hexadecimal
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }   
    }
}