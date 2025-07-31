using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class EmpresaController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult Devedores()
        {
            return View();
        }

        public IActionResult Cobrancas()
        {
            return View();
        }

        public IActionResult Funcionarios()
        {
            return View();
        }
    }
}

