using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;


namespace WebApplication1.Controllers
{
    public class TesteController : Controller
    {
        public IActionResult Index()
        {
            var lista = _context.Devedores.ToList();
            return View(lista); // <-- aqui você manda a lista pra view
        }

        private readonly AppDbContext _context;

        public TesteController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Inserir()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Inserir(Devedor cliente)
        {
            if (ModelState.IsValid)
            {
                _context.Devedores.Add(cliente);
                _context.SaveChanges();
                Console.WriteLine("SALVO COM SUCESSO");
                return RedirectToAction("Index");
            }

            Console.WriteLine("FORMULÁRIO INVÁLIDO");

            // ADICIONA: mostrar os erros um por um no terminal
            foreach (var erro in ModelState)
            {
                foreach (var subErro in erro.Value.Errors)
                {
                    Console.WriteLine($"Erro em {erro.Key}: {subErro.ErrorMessage}");
                }
            }

            return View(cliente);
        }
    }
}
