using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using System.Text.Json; // certifique-se de importar




namespace WebApplication1.Controllers
{
    public class DividaController : Controller
    {
        private readonly AppDbContext _context;

        public DividaController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Divida
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("Tipo") != "Empresa" && HttpContext.Session.GetString("Tipo") != "Admin")
            {
                return RedirectToAction("AcessoNegado", "Login");
            }

            int empresaId = HttpContext.Session.GetInt32("Id") ?? 0;
            var dividas = _context.Dividas
                .Include(d => d.Devedor)
                .Include(d => d.Empresa);

            // Se for empresa, filtra apenas as próprias dívidas
            if (HttpContext.Session.GetString("Tipo") == "Empresa")
            {
                return View(await dividas.Where(d => d.EmpresaID == empresaId).ToListAsync());
            }

            return View(await dividas.ToListAsync()); // Admin vê tudo
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

            // Bloqueia empresa acessando dívida de outra empresa
            if (HttpContext.Session.GetString("Tipo") == "Empresa" &&
                divida.EmpresaID != HttpContext.Session.GetInt32("Id"))
            {
                return RedirectToAction("AcessoNegado", "Login");
            }

            return View(divida);
        }

        // GET: Divida/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("Tipo") != "Admin")
                return RedirectToAction("AcessoNegado", "Login");

            ViewBag.Devedores = new SelectList(_context.Devedores.ToList(), "Id", "Name");
            ViewBag.Empresas = new SelectList(_context.Empresas.ToList(), "Id", "Nome");

            return View(new Divida());
        }

        // POST: Divida/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Divida divida)
        {
            if (HttpContext.Session.GetString("Tipo") != "Admin")
                return RedirectToAction("AcessoNegado", "Login");

            if (ModelState.IsValid)
            {
                try
                {
                    divida.Status = "Pendente";
                    divida.DataPagamento = null;
                    _context.Add(divida);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Dívida criada com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Erro ao salvar a dívida: " + ex.Message);
                }
            }

            ViewBag.Devedores = new SelectList(_context.Devedores.ToList(), "Id", "Name", divida.DevedorID);
            ViewBag.Empresas = new SelectList(_context.Empresas.ToList(), "Id", "Nome", divida.EmpresaID);
            return View(divida);
        }

        // GET: Divida/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (HttpContext.Session.GetString("Tipo") != "Empresa" && HttpContext.Session.GetString("Tipo") != "Admin")
                return RedirectToAction("AcessoNegado", "Login");

            if (id == null) return NotFound();

            var divida = await _context.Dividas.FindAsync(id);
            if (divida == null) return NotFound();

            // Empresa só edita sua própria dívida
            if (HttpContext.Session.GetString("Tipo") == "Empresa" &&
                divida.EmpresaID != HttpContext.Session.GetInt32("Id"))
                return RedirectToAction("AcessoNegado", "Login");

            ViewBag.Devedores = new SelectList(_context.Devedores.ToList(), "Id", "Name", divida.DevedorID);
            ViewBag.Empresas = new SelectList(_context.Empresas.ToList(), "Id", "Nome", divida.EmpresaID);
            return View(divida);
        }

        // POST: Divida/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Divida divida)
        {
            if (id != divida.Id) return NotFound();

            if (HttpContext.Session.GetString("Tipo") != "Empresa" && HttpContext.Session.GetString("Tipo") != "Admin")
                return RedirectToAction("AcessoNegado", "Login");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(divida);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Dívida atualizada com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", "Erro ao atualizar a dívida: " + ex.Message);
                }
            }

            ViewBag.Devedores = new SelectList(_context.Devedores.ToList(), "Id", "Name", divida.DevedorID);
            ViewBag.Empresas = new SelectList(_context.Empresas.ToList(), "Id", "Nome", divida.EmpresaID);
            return View(divida);
        }

        // GET: Divida/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (HttpContext.Session.GetString("Tipo") != "Empresa" && HttpContext.Session.GetString("Tipo") != "Admin")
                return RedirectToAction("AcessoNegado", "Login");

            if (id == null) return NotFound();

            var divida = await _context.Dividas
                .Include(d => d.Devedor)
                .Include(d => d.Empresa)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (divida == null) return NotFound();

            // Empresa só exclui sua própria dívida
            if (HttpContext.Session.GetString("Tipo") == "Empresa" &&
                divida.EmpresaID != HttpContext.Session.GetInt32("Id"))
                return RedirectToAction("AcessoNegado", "Login");

            return View(divida);
        }

        [HttpPost]
        public async Task<IActionResult> Pagar(int id)
        {
            var divida = await _context.Dividas.FindAsync(id);
            if (divida == null || divida.Status == "Pago")
                return NotFound();

            divida.Status = "Pago";
            divida.DataPagamento = DateTime.Now;

            _context.Update(divida);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MinhasDividas));
        }


        // POST: Divida/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString("Tipo") != "Empresa" && HttpContext.Session.GetString("Tipo") != "Admin")
                return RedirectToAction("AcessoNegado", "Login");

            try
            {
                var divida = await _context.Dividas.FindAsync(id);
                if (divida != null)
                {
                    _context.Dividas.Remove(divida);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Dívida excluída com sucesso!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Erro ao excluir a dívida: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Divida/MinhasDividas
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

            // Cria uma lista simplificada (DTO) para evitar referências circulares
            var dividasSimplificadas = dividas.Select(d => new
            {
                d.Id,
                d.Titulo,
                d.Descricao,
                d.Valor,
                d.Status,
                DataVencimento = d.DataVencimento?.ToString("yyyy-MM-dd"),
                DataPagamento = d.DataPagamento?.ToString("yyyy-MM-dd"),
                Empresa = new
                {
                    d.Empresa.Nome
                }
            }).ToList();

            ViewBag.DividasJson = dividasSimplificadas;
            return View(dividas);
        }



    }
}