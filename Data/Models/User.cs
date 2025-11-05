// este arquivo é criado em
// Data/Models/User.cs

// Importa o namespace System, que contém tipos básicos do dotNet(.NET), por exemplo o DateTime usado 
using System;
// Declare o namespace do arquivo, agrupa logicamente as classes do projeto (aqui; API, camada de dados, modelos)
namespace projetoMakers.Api.Data.Models
{
    // Declaração da classe publica 'User', o 'public' permite que outras classes ou partes da aplicação acessem diretamente
    public class User
    {
        // Propriedade inteira que representa a chave primária (ID) do usuário na tabela
        // Conversão comum: nome + 'Id'. Usada pelo EF Core para mapear automáticamente como PK
        public int UsuarioId { get; set; }
        // Propriedade string que armazena o 'NomeCompleto' do usuário
        // Vamos iniciar como String Vazia '= string.Empty', que evita valores nulos e garante valor não-nulo
        public string NomeCompleto { get; set; } = string.Empty;
        // Propriedade string que armazena o 'Email' do usuário
        // Vamos iniciar como String Vazia '= string.Empty', que evita valores nulos e garante o valor não-nulo
        public string Email { get; set; } = string.Empty;
        // Propriedade String que armazena a senha 'PassWordHas' do usuário
        // A senha dentro do backEnd será realizada uma crypitografia (resultado de um algoritmo de hashing 
        // ex: PKDF2, bycrypt, Argon2
        // Vamos iniciar como String Vazia '= string.Empty', que evita valores nulos e garante o valor não-nulo
        public string PassWordHash { get; set; } = string.Empty;
        // Propriedade String que armazena a recuperacao de senha 'HashPass' do usuário
        // A crypitografia, responsavel para validar a recuparacao da senha futura caso o usuário esqueça
        // Vamos iniciar como String Vazia '= string.Empty', que evita valores nulos e garante o valor não-nulo
        public string HashPass { get; set; } = string.Empty;
        // Armazena a data e hora que o registro foi criado. DateTime não anulável significa que sempre deve ter um valor
        // Normalmente preenchido pelo servidor ao criar um novo usuário
        public DateTime CreatedAt { get; set; }

        // Armazena a data e hora que o registro foi alterado. O '?' indica que é um tipo anulável (DateTime?)
        // Permitindo que seja NULL quando nunca houver atualização
        public DateTime? DateUp { get; set; }

        // Inteiro para a referência a um status (ex: ativo, inativo, pendente, etc.)
        // Provavelmente uma chave estrangeira para a tabela Usuariostatus ou apenas Status.
        // Representa o estado atual do usuário no sistema
        public int StatusId { get; set; }
    }
}