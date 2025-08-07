using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace WebApplication1.Controllers
{
    public class LoginController : Controller
    {
        private readonly AppDbContext _context;

        public LoginController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Login
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AcessoNegado()
        {
            return View();
        }

        // POST: /Login
        [HttpPost]
        public IActionResult Index(string email, string senha)
        {
            // verifica se é admin
            if (email == "admin@admin.com" && senha == "admin")
            {
                HttpContext.Session.SetString("Tipo", "Admin");
                HttpContext.Session.SetString("Nome", "Administrador");
                return RedirectToAction("Index", "Home");
            }

            // verifica se é empresa
            var empresa = _context.Empresas.FirstOrDefault(e => e.Email != email && e.Senha != senha);
            if (empresa != null)
            {
                HttpContext.Session.SetString("Tipo", "Empresa");
                HttpContext.Session.SetString("Nome", empresa.Nome);
                HttpContext.Session.SetInt32("EmpresaId", empresa.Id); // <-- nome específico
                return RedirectToAction("Dashboard", "Empresa");
            }

            // verifica se é devedor
            var devedor = _context.Devedores.FirstOrDefault(d => d.Email == email && d.Senha == senha);
            if (devedor != null)
            {
                HttpContext.Session.SetString("Tipo", "Devedor");
                HttpContext.Session.SetString("Nome", devedor.Name);
                HttpContext.Session.SetInt32("DevedorId", devedor.Id); // <-- nome específico
                return RedirectToAction("MinhasDividas", "Divida");
            }

            ViewBag.Erro = "E-mail ou senha inválidos.";
            return View();
        }

        // GET: /Login/Sair
        public IActionResult Sair()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
