using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

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

            var dividasSimplificadas = dividas.Select(d => new {
                d.Id,
                d.Titulo,
                d.Descricao,
                d.Valor,
                d.Status,
                DataVencimento = d.DataVencimento?.ToString("dd/MM/yyyy"),
                DataPagamento = d.DataPagamento?.ToString("dd/MM/yyyy"),
                Empresa = new { d.Empresa.Nome }
            }).ToList();
            ViewBag.DividasJson = System.Text.Json.JsonSerializer.Serialize(dividasSimplificadas);

            return View(dividas);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PagarCartao(int id, string Nome, string Numero, string Validade, string CVV)
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
            // Validação básica dos dados do cartão
            if (string.IsNullOrWhiteSpace(Nome) ||
                string.IsNullOrWhiteSpace(Numero) ||
                string.IsNullOrWhiteSpace(Validade) ||
                string.IsNullOrWhiteSpace(CVV) ||
                Numero.Replace(" ", "").Length != 16 ||
                !System.Text.RegularExpressions.Regex.IsMatch(Validade, "^(0[1-9]|1[0-2])\\/[0-9]{2}$") ||
                !System.Text.RegularExpressions.Regex.IsMatch(CVV, "^[0-9]{3,4}$"))
            {
                TempData["Mensagem"] = "Dados do cartão inválidos.";
                return RedirectToAction("Pagar", new { id });
            }
            divida.Status = "Pago";
            divida.DataPagamento = DateTime.Now;
            await _context.SaveChangesAsync();
            TempData["Mensagem"] = "Pagamento realizado com sucesso!";
            return RedirectToAction("MinhasDividas");
        }
        // ...outros métodos existentes...
    }
}
