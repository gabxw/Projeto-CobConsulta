using WebApplication1.Models;

namespace WebApplication1.ViewModels
{
    public class DividaImportada
    {
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public int Valor { get; set; }
        public string Status { get; set; }
        public DateTime? DataVencimento { get; set; }

        // Dados do devedor
        public string NomeDevedor { get; set; }
        public string EmailDevedor { get; set; }
        public string CpfDevedor { get; set; }
        public string TelefoneDevedor { get; set; }
        public string SenhaDevedor { get; set; }
    }

    public class ImportacaoDividaViewModel
    {
        public List<DividaImportada> Dividas { get; set; } = new();
        public List<string> Erros { get; set; } = new();
    }
}
