using System.ComponentModel.DataAnnotations;

namespace CobrancaPro.Models
{
    public class BillingRecord
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Cliente")]
        public string Client { get; set; }

        [Required]
        [Display(Name = "Valor")]
        [DataType(DataType.Currency)]
        public decimal Value { get; set; }

        [Required]
        [Display(Name = "Vencimento")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [Required]
        [Display(Name = "Status")]
        public BillingStatus Status { get; set; }

        [Display(Name = "Data de Criação")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public enum BillingStatus
    {
        Pago = 1,
        Pendente = 2,
        Atrasado = 3
    }
}