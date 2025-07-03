namespace WebApplication1.Models
{
    public class Divida
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public int Valor { get; set; }
        public DateTime DataVencimento { get; set; }
        public DateTime DataPagamento { get; set; }
        public string Status { get; set; } = "Pendente";
        public int DevedorID { get; set; }
        public Devedor Devedor{ get; set; }
        public int EmpresaID { get; set; }          
        public Empresa Empresa { get; set; }

    }
}
