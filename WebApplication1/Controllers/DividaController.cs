using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using System.Text.Json;

namespace WebApplication1.Controllers
{
    public class DividaController : Controller
    {
        private readonly AppDbContext _context;

        public DividaController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Divida/MinhasDividas
        public async Task<IActionResult> MinhasDividas()
        {
            if (HttpContext.Session.GetString("Tipo") != "Devedor")
                return RedirectToAction("AcessoNegado", "Login");

            int devedorId = HttpContext.Session.GetInt32("DevedorId") ?? 0;

            var dividas = await _context.Dividas
                .Include(d => d.Empresa)
                .Where(d => d.DevedorID == devedorId)
                .ToListAsync();

            var dividasSimplificadas = dividas.Select(d => new
            {
                d.Id,
                d.Titulo,
                d.Descricao,
                d.Valor,
                d.Status,
                DataVencimento = d.DataVencimento?.ToString("yyyy-MM-dd"),
                DataPagamento = d.DataPagamento?.ToString("yyyy-MM-dd"),
                Empresa = new { d.Empresa.Nome }
            }).ToList();

            ViewBag.DividasJson = JsonSerializer.Serialize(dividasSimplificadas);
            return View(dividas);
        }

        // GET: Divida/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var divida = await _context.Dividas
                .Include(d => d.Devedor)
                .Include(d => d.Empresa)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (divida == null) return NotFound();

            if (HttpContext.Session.GetString("Tipo") == "Empresa" &&
                divida.EmpresaID != HttpContext.Session.GetInt32("Id"))
            {
                return RedirectToAction("AcessoNegado", "Login");
            }

            return View(divida);
        }

        // GET: Divida/Pagar/5
        public async Task<IActionResult> Pagar(int id)
        {
            var divida = await _context.Dividas
                .Include(d => d.Devedor)
                .Include(d => d.Empresa)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (divida == null)
            {
                return NotFound();
            }

            if (divida.Status == "Pago")
            {
                TempData["Mensagem"] = "Esta dívida já foi paga.";
                return RedirectToAction("MinhasDividas");
            }

            return View(divida);
        }

        // POST: Divida/ConfirmarPagamento
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarPagamento(int id, string metodo)
        {
            var divida = await _context.Dividas.FirstOrDefaultAsync(d => d.Id == id);

            if (divida == null)
            {
                return NotFound();
            }

            if (divida.Status == "Pago")
            {
                TempData["Mensagem"] = "Esta dívida já foi paga.";
                return RedirectToAction("MinhasDividas");
            }

            divida.Status = "Pago";
            divida.DataPagamento = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["Mensagem"] = $"Pagamento por {metodo} realizado com sucesso!";
            return RedirectToAction("MinhasDividas");
        }
    }
}
