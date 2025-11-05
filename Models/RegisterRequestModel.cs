// Crie o arquivo novo - Models/RegisterRequestModel.cs

// Importa o namespace que contém os atributos de validação de dados,
// como '[Required]', '[EmailAddress]', '[StringLength]', etc.
// Esses atributos são usados para validar automaticamente as propriedades de um modelo
using System.ComponentModel.DataAnnotations;
// Define o namespace do arquivo
// Agrupa as classes do projeto relacionadas à camanda 'Models' da API
namespace projetoMakers.Api.Models
{
    // Declaração da classe publica 'RegisterRequestModel'
    // Este modelo é usado para representar os dados enviados ao cadastrar um novo usuário
    // Normalmente em corpo de (JSON) da requisição em um endPoint POST /register
    public class RegisterRequestModel
    {
        // Atributo de validação: indica que a propriedade é obrigatória
        // Se o valor vier NULL (nulo) ou Empty (vazio) na requisição, a validação
        // falhará e a API retornará erro 400 (Bad Request)
        [Required(ErrorMessage = "Nome é obrigatório!")]
        // Propriedade que representa o nome completo do usuário
        // Obs.: Está informação será validada como obrigatória devido ao [Required]
        public string NomeCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email é obrigatório!")]
        // O Atributo adicional de validação: verifica se o valor segue o
        // formato de um endereço de email válido
        // Ex: 'usuario@dominio.com.br' -> Válido | usuario@ -> inválido
        [EmailAddress(ErrorMessage = "O email informado não é valido!")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Senha é obrigatória!")]
        public string PassWordHash { get; set; } = string.Empty;
    }
}
