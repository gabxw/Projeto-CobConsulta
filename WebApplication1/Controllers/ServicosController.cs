using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class ServicosController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ConsulteDebito()
        {
            return View();
        }
        public IActionResult EmitirSegundaVia()
        {
            return View();
        }
        public IActionResult AnexarDocumentos()
        {
            return View();
        }
    }
}
