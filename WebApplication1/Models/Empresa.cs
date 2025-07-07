using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Empresa
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome da empresa é obrigatório")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O CNPJ é obrigatório")]
        public string Cnpj { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória")]
        public string Senha { get; set; }

        [Required(ErrorMessage = "O telefone é obrigatório")]
        public string Telefone { get; set; }

        [ValidateNever]
        public List<Divida> Dividas { get; set; } = new List<Divida>();
    }
}
