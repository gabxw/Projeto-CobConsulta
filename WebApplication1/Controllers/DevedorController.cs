using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using System.Threading.Tasks;
using System.Linq;

namespace WebApplication1.Controllers
{
    public class DevedorController : Controller
    {
        private readonly AppDbContext _context;

        public DevedorController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Devedor/Create
        public IActionResult Create()
        {
            var tipo = HttpContext.Session.GetString("Tipo");
            if (tipo != "Empresa" && tipo != "Admin")
            {
                TempData["Mensagem"] = "Acesso restrito!";
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Devedor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Devedor devedor)
        {
            if (ModelState.IsValid)
            {
                _context.Devedores.Add(devedor);
                await _context.SaveChangesAsync();
                TempData["Mensagem"] = "Devedor cadastrado com sucesso!";
                return RedirectToAction("Dashboard", "Empresa");
            }
            return View(devedor);
        }
    }
}
