using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        // GET: Divida
        public async Task<IActionResult> Index()
        {
            var dividas = _context.Dividas
                .Include(d => d.Devedor)
                .Include(d => d.Empresa);
            return View(await dividas.ToListAsync());
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

            return View(divida);
        }

        // GET: Divida/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("Tipo") != "Admin")
            {
                return RedirectToAction("AcessoNegado", "Login");
            }
            ViewBag.Devedores = new SelectList(_context.Devedores.ToList(), "Id", "Name");
            ViewBag.Empresas = new SelectList(_context.Empresas.ToList(), "Id", "Nome");

            return View(new Divida());
        }

        // POST: Divida/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Divida divida)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    divida.Status = "Pendente";
                    divida.DataPagamento = null; // Deve ser null, não DataTime.MinValue
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
            foreach (var erro in ModelState)
            {
                foreach (var subErro in erro.Value.Errors)
                {
                    Console.WriteLine($"Erro em {erro.Key}: {subErro.ErrorMessage}");
                }
            }
            return View(divida);
        }

        // GET: Divida/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var divida = await _context.Dividas.FindAsync(id);
            if (divida == null) return NotFound();

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
            if (id == null) return NotFound();

            var divida = await _context.Dividas
                .Include(d => d.Devedor)
                .Include(d => d.Empresa)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (divida == null) return NotFound();

            return View(divida);
        }

        // POST: Divida/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
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
    }
}
