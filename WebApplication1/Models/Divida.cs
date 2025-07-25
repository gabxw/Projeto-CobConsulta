using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Divida
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "O título é obrigatório")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "O valor é obrigatório")]
        public int Valor { get; set; }

        [Required(ErrorMessage = "A data de vencimento é obrigatória")]
        public DateTime? DataVencimento { get; set; }

        //[Required(ErrorMessage = "A data de pagamento é obrigatória")]
        public DateTime? DataPagamento { get; set; }

        [Required(ErrorMessage = "O status é obrigatório")]
        public string Status { get; set; } = "Pendente";
        public int DevedorID { get; set; }

        [ValidateNever]
        public Devedor Devedor{ get; set; }
        public int EmpresaID { get; set; }

        [ValidateNever]
        public Empresa Empresa { get; set; }

    }
}
