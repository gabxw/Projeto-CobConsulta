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
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Cpf { get; set; }
        public string Telefone { get; set; }
        public string Senha{ get; set; }
    }

    public class ImportacaoDividaViewModel
    {
        public List<DividaImportada> Dividas { get; set; } = new();
        public List<string> Erros { get; set; } = new();
    }
}
