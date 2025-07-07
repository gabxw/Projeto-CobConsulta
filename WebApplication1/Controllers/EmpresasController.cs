using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class EmpresasController : Controller
    {
        private readonly AppDbContext _context;

        public EmpresasController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var lista = _context.Empresas.ToList();
            return View(lista);
        }

        public IActionResult Cadastrar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Cadastrar(Empresa empresa)
        {
            if (ModelState.IsValid)
            {
                _context.Empresas.Add(empresa);
                _context.SaveChanges();
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

            return View(empresa);
        }
    }
}
