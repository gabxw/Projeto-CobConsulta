namespace WebApplication1.Models
{
    public class Devedor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Senha { get; set; }
        public string Email { get; set; }
        public string Cpf { get; set; }


        public List<Divida> Dividas { get; set; }

    }
}