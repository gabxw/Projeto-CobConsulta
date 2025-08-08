namespace WebApplication1.ViewModels
{
    public class DividaImportada
    {
        public string Nome { get; set; }
        public string CPF { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
        public string Status { get; set; }
        public DateTime? DataVencimento { get; set; }
    }

    public class ImportacaoDividaViewModel
    {
        public List<DividaImportada> Dividas { get; set; } = new();
        public List<string> Erros { get; set; } = new();
    }
}
