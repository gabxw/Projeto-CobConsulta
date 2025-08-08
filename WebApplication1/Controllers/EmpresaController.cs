using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication1.ViewModels;
using WebApplication1.Helpers;
using System.IO;


namespace WebApplication1.Controllers
{
    public class EmpresaController : Controller
    {
        private readonly AppDbContext _context;

        public EmpresaController(AppDbContext context)
        {
            _context = context;
        }

        private bool EhEmpresaLogada()
        {
            return HttpContext.Session.GetString("Tipo") == "Empresa";
        }

        public IActionResult AcessoNegado()
        {
            return View();
        }

        public async Task<IActionResult> Dividas()
        {
            if (!EhEmpresaLogada())
                return RedirectToAction("AcessoNegado", "Login");

            int empresaId = HttpContext.Session.GetInt32("EmpresaId") ?? 0;

            var dividas = await _context.Dividas
                .Include(d => d.Devedor)
                .Where(d => d.EmpresaID == empresaId)
                .OrderByDescending(d => d.DataCriacao)
                .ToListAsync();

            return View(dividas);
        }

        public async Task<IActionResult> DetalhesDivida(int id)
        {
            if (!EhEmpresaLogada())
                return RedirectToAction("AcessoNegado", "Login");

            var divida = await _context.Dividas
                .Include(d => d.Devedor)
                .Include(d => d.Empresa)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (divida == null)
                return NotFound();

            return View(divida);
        }

        [HttpGet]
        public async Task<IActionResult> ExcluirDivida(int id)
        {
            if (!EhEmpresaLogada())
                return RedirectToAction("AcessoNegado", "Login");

            var divida = await _context.Dividas
                .Include(d => d.Devedor)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (divida == null)
                return NotFound();

            return View(divida);
        }

        [HttpPost, ActionName("ExcluirDivida")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirConfirmado(int id)
        {
            var divida = await _context.Dividas.FindAsync(id);

            if (divida != null)
            {
                _context.Dividas.Remove(divida);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Dividas");
        }


        [HttpGet]
        public async Task<IActionResult> EditDivida(int id)
        {
            if (!EhEmpresaLogada())
                return RedirectToAction("AcessoNegado", "Login");

            var divida = await _context.Dividas.FindAsync(id);

            if (divida == null)
                return NotFound();

            var devedores = _context.Dividas
                .Include(d => d.Devedor)
                .Where(d => d.EmpresaID == divida.EmpresaID && d.Devedor != null)
                .Select(d => d.Devedor)
                .Distinct()
                .ToList();

            ViewBag.Devedores = new SelectList(devedores, "Id", "Name", divida.DevedorID);

            return View(divida);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDivida(Divida model)
        {
            if (!EhEmpresaLogada())
                return RedirectToAction("AcessoNegado", "Login");

            if (!ModelState.IsValid)
            {
                int empresaId = HttpContext.Session.GetInt32("EmpresaId") ?? 0;
                var devedores = _context.Dividas
                    .Include(d => d.Devedor)
                    .Where(d => d.EmpresaID == empresaId && d.Devedor != null)
                    .Select(d => d.Devedor)
                    .Distinct()
                    .ToList();

                ViewBag.Devedores = new SelectList(devedores, "Id", "Name", model.DevedorID);
                return View(model);
            }

            var dividaExistente = await _context.Dividas.FindAsync(model.Id);
            if (dividaExistente == null)
                return NotFound();

            // Atualiza apenas os campos editáveis
            dividaExistente.Titulo = model.Titulo;
            dividaExistente.Descricao = model.Descricao;
            dividaExistente.Valor = model.Valor;
            dividaExistente.DataVencimento = model.DataVencimento;
            dividaExistente.DataPagamento = model.DataPagamento;
            dividaExistente.Status = model.Status;
            dividaExistente.DevedorID = model.DevedorID;

            await _context.SaveChangesAsync();
            return RedirectToAction("Dividas");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDivida(Divida model)
        {
            if (!EhEmpresaLogada())
                return RedirectToAction("AcessoNegado", "Login");

            int empresaId = HttpContext.Session.GetInt32("EmpresaId") ?? 0;

            var devedorEhValido = await _context.Dividas
                .AnyAsync(d => d.DevedorID == model.DevedorID && d.EmpresaID == empresaId);

            if (!devedorEhValido)
            {
                ModelState.AddModelError("DevedorID", "Esse devedor não está associado à sua empresa.");
            }

            if (ModelState.IsValid)
            {
                model.EmpresaID = empresaId;
                model.Status = "Pendente";
                model.DataCriacao = DateTime.Now;

                _context.Dividas.Add(model);
                await _context.SaveChangesAsync();

                return RedirectToAction("Dividas");
            }

            // Recarrega dropdown para repintar a view com erro
            var devedores = _context.Dividas
                .Include(d => d.Devedor)
                .Where(d => d.EmpresaID == empresaId && d.Devedor != null)
                .Select(d => d.Devedor)
                .Distinct()
                .ToList();

            ViewBag.Devedores = new SelectList(devedores, "Id", "Name", model.DevedorID);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> CreateDivida()
        {
            if (!EhEmpresaLogada())
                return RedirectToAction("AcessoNegado", "Login");

            int empresaId = HttpContext.Session.GetInt32("EmpresaId") ?? 0;

            var devedores = await _context.Dividas
                .Include(d => d.Devedor)
                .Where(d => d.EmpresaID == empresaId && d.Devedor != null)
                .Select(d => d.Devedor)
                .Distinct()
                .ToListAsync();

            ViewBag.Devedores = new SelectList(devedores, "Id", "Name");

            return View();
        }

        [HttpGet]
        public IActionResult ImportarDividas()
        {
            if (!EhEmpresaLogada())
                return RedirectToAction("AcessoNegado", "Login");

            return View(new ImportacaoDividaViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> ImportarDividas(IFormFile excelFile)
        {
            if (!EhEmpresaLogada())
                return RedirectToAction("AcessoNegado", "Login");

            if (excelFile == null || excelFile.Length == 0)
            {
                ModelState.AddModelError("", "Selecione um arquivo Excel.");
                return View();
            }

            int empresaId = HttpContext.Session.GetInt32("EmpresaId") ?? 0;

            using var stream = new MemoryStream();
            await excelFile.CopyToAsync(stream);
            stream.Position = 0;

            var resultado = ExcelImportHelper.ProcessarExcel(stream, empresaId);

            if (resultado.Erros.Any())
            {
                var vmErro = new ImportacaoDividaViewModel
                {
                    Erros = resultado.Erros
                };
                return View("ImportarDividas", vmErro);
            }

            var dividasImportadas = resultado.Dividas.Select(d => new DividaImportada
            {
                Nome = d.Devedor.Name,
                Email = d.Devedor.Email,
                Telefone = d.Devedor.Telefone,
                CPF = d.Devedor.Cpf,
                Titulo = d.Titulo,
                Descricao = d.Descricao,
                Valor = d.Valor,
                Status = d.Status,
                DataVencimento = d.DataVencimento
            }).ToList();

            var vm = new ImportacaoDividaViewModel
            {
                Dividas = dividasImportadas
            };

            return View("ConfirmarImportacao", vm);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmarImportacao(List<DividaImportada> dividasConfirmadas)
        {
            int empresaId = HttpContext.Session.GetInt32("EmpresaId") ?? 0;

            if (dividasConfirmadas == null || !dividasConfirmadas.Any())
            {
                TempData["MensagemErro"] = "Nenhuma dívida foi confirmada para importação.";
                return RedirectToAction("ImportarDividas");
            }

            foreach (var item in dividasConfirmadas)
            {
                var devedorExistente = await _context.Devedores
                    .FirstOrDefaultAsync(d => d.Cpf == item.CPF);

                if (devedorExistente == null)
                {
                    devedorExistente = new Devedor
                    {
                        Name = item.Nome,
                        Email = item.Email,
                        Cpf = item.CPF,
                        Senha = "importado",
                        Telefone = item.Telefone
                    };

                    _context.Devedores.Add(devedorExistente);
                    await _context.SaveChangesAsync();
                }

                var novaDivida = new Divida
                {
                    EmpresaID = empresaId,
                    DevedorID = devedorExistente.Id,
                    Titulo = item.Titulo,
                    Descricao = item.Descricao,
                    Valor = (int)item.Valor,
                    DataVencimento = item.DataVencimento,
                    DataCriacao = DateTime.Now,
                    Status = item.Status
                };

                _context.Dividas.Add(novaDivida);
            }

            await _context.SaveChangesAsync();

            TempData["MensagemSucesso"] = "Importação concluída com sucesso.";
            return RedirectToAction("Dividas");
        }


        public async Task<IActionResult> Dashboard()
        {
            if (!EhEmpresaLogada())
                return RedirectToAction("AcessoNegado", "Login");

            int empresaId = HttpContext.Session.GetInt32("EmpresaId") ?? 0;
            var hoje = DateTime.Today;

            var dividasDaEmpresa = await _context.Dividas
                .Include(d => d.Devedor)
                .Where(d => d.EmpresaID == empresaId)
                .ToListAsync();

            var devedores = dividasDaEmpresa
                .Select(d => d.Devedor)
                .Where(d => d != null)
                .Distinct()
                .ToList();

            var devedoresComPendencia = dividasDaEmpresa
                .Where(d => d.Status == "Pendente" || d.Status == "Vencido")
                .Select(d => d.Devedor)
                .Where(d => d != null)
                .Distinct()
                .ToList();

            int totalDevedores = devedores.Count;
            int comPendencia = devedoresComPendencia.Count;
            int semPendencia = totalDevedores - comPendencia;

            int pendentes = dividasDaEmpresa.Count(d => d.Status == "Pendente");
            int atrasadas = dividasDaEmpresa.Count(d => d.DataVencimento.HasValue
                                                        && d.DataVencimento.Value.Date < hoje
                                                        && !(d.Status == "Quitado" || d.Status == "Finalizado"));
            int quitadas = dividasDaEmpresa.Count(d => d.Status == "Quitado" || d.Status == "Finalizado");

            decimal faturamento = dividasDaEmpresa
                .Where(d => d.Status == "Quitado" || d.Status == "Finalizado")
                .Sum(d => d.Valor);

            decimal valorEmAberto = dividasDaEmpresa
                .Where(d => (d.Status == "Pendente" || d.Status == "Vencido") || (d.DataVencimento.HasValue && d.DataVencimento.Value.Date < hoje && !(d.Status == "Quitado" || d.Status == "Finalizado")))
                .Sum(d => d.Valor);

            var topDevedores = dividasDaEmpresa
                .Where(d => d.Status == "Pendente" || d.Status == "Vencido")
                .GroupBy(d => d.Devedor)
                .Select(g => new EmpresaDashboardViewModel.TopDevedorItem
                {
                    Devedor = g.Key!,
                    TotalDevido = g.Sum(x => x.Valor)
                })
                .OrderByDescending(x => x.TotalDevido)
                .Take(5)
                .ToList();

            var agingBuckets = new Dictionary<string, decimal>
            {
                ["0-30"] = dividasDaEmpresa
                    .Where(d => d.DataVencimento.HasValue
                                && d.DataVencimento.Value.Date < hoje
                                && !(d.Status == "Quitado" || d.Status == "Finalizado")
                                && (hoje - d.DataVencimento.Value.Date).Days <= 30)
                    .Sum(d => d.Valor),
                ["31-60"] = dividasDaEmpresa
                    .Where(d => d.DataVencimento.HasValue
                                && d.DataVencimento.Value.Date < hoje
                                && !(d.Status == "Quitado" || d.Status == "Finalizado")
                                && (hoje - d.DataVencimento.Value.Date).Days is int days && days >= 31 && days <= 60)
                    .Sum(d => d.Valor),
                [">60"] = dividasDaEmpresa
                    .Where(d => d.DataVencimento.HasValue
                                && d.DataVencimento.Value.Date < hoje
                                && !(d.Status == "Quitado" || d.Status == "Finalizado")
                                && (hoje - d.DataVencimento.Value.Date).Days > 60)
                    .Sum(d => d.Valor),
            };

            var recentes = dividasDaEmpresa
                .OrderByDescending(d => d.DataVencimento)
                .Take(5)
                .Select(d => new EmpresaDashboardViewModel.RecentDividaItem
                {
                    Titulo = d.Titulo,
                    Devedor = d.Devedor,
                    Valor = d.Valor,
                    Status = d.Status,
                    DataVencimento = d.DataVencimento,
                    DataPagamento = d.DataPagamento
                })
                .ToList();

            decimal totalCobradoOuAberto = faturamento + valorEmAberto;
            var taxaRecuperacao = totalCobradoOuAberto > 0
                ? (faturamento / totalCobradoOuAberto) * 100
                : 0;

            var pagamentosQuitados = dividasDaEmpresa
            .Where(d =>
                (d.Status == "Quitado" || d.Status == "Finalizado") &&
                d.DataPagamento.HasValue &&
                d.DataCriacao.HasValue &&
                d.DataCriacao.Value > DateTime.MinValue)
            .Select(d => (d.DataPagamento.Value - d.DataCriacao.Value).TotalDays)
            .ToList();

            double mediaDias = pagamentosQuitados.Any() ? pagamentosQuitados.Average() : 0;



            var primeiroDiaMesAtual = new DateTime(hoje.Year, hoje.Month, 1);
            var primeiroDiaMesAnterior = primeiroDiaMesAtual.AddMonths(-1);
            var fimMesAnterior = primeiroDiaMesAtual.AddDays(-1);

            decimal faturamentoMesAtual = dividasDaEmpresa
                .Where(d => (d.Status == "Quitado" || d.Status == "Finalizado") && d.DataPagamento.HasValue)
                .Where(d => d.DataPagamento.Value.Date >= primeiroDiaMesAtual.Date && d.DataPagamento.Value.Date <= hoje)
                .Sum(d => d.Valor);

            decimal faturamentoMesAnterior = dividasDaEmpresa
                .Where(d => (d.Status == "Quitado" || d.Status == "Finalizado") && d.DataPagamento.HasValue)
                .Where(d => d.DataPagamento.Value.Date >= primeiroDiaMesAnterior.Date && d.DataPagamento.Value.Date <= fimMesAnterior.Date)
                .Sum(d => d.Valor);

            var alertas = dividasDaEmpresa
                .Where(d => d.DataVencimento.HasValue
                            && d.DataVencimento.Value.Date < hoje
                            && !(d.Status == "Quitado" || d.Status == "Finalizado")
                            && (hoje - d.DataVencimento.Value.Date).Days > 30)
                .Select(d => new EmpresaDashboardViewModel.DividaAlertItem
                {
                    Titulo = d.Titulo,
                    DevedorNome = d.Devedor?.Name ?? "",
                    Valor = d.Valor,
                    DiasAtraso = (hoje - d.DataVencimento.Value.Date).Days
                })
                .ToList();

            // notificações
            var notificacoes = new List<string>();

            var pagosRecentes = dividasDaEmpresa
                .Where(d => (d.Status == "Quitado" || d.Status == "Finalizado")
                            && d.DataPagamento.HasValue
                            && (hoje - d.DataPagamento.Value.Date).TotalDays <= 2)
                .OrderByDescending(d => d.DataPagamento)
                .Take(5)
                .ToList();

            foreach (var d in pagosRecentes)
            {
                var dias = (int)(hoje - d.DataPagamento.Value.Date).TotalDays;
                var quando = dias == 0 ? "hoje" : $"{dias} dia(s) atrás";
                notificacoes.Add($"Devedor {d.Devedor?.Name} quitou R$ {d.Valor.ToString("N2")} em \"{d.Titulo}\" {quando}.");
            }

            var virouVencida = dividasDaEmpresa
                .Where(d => d.DataVencimento.HasValue
                            && d.DataVencimento.Value.Date < hoje
                            && !(d.Status == "Quitado" || d.Status == "Finalizado")
                            && (hoje - d.DataVencimento.Value.Date).Days == 1)
                .Take(5);

            foreach (var d in virouVencida)
            {
                notificacoes.Add($"Dívida \"{d.Titulo}\" de {d.Devedor?.Name} venceu ontem e ainda está em aberto.");
            }

            var proximas = dividasDaEmpresa
                .Where(d => d.DataVencimento.HasValue
                            && (d.Status == "Pendente")
                            && (d.DataVencimento.Value.Date - hoje).TotalDays is double diff && diff >= 0 && diff <= 3)
                .OrderBy(d => d.DataVencimento)
                .Take(5);

            foreach (var d in proximas)
            {
                var dias = (int)(d.DataVencimento.Value.Date - hoje).TotalDays;
                notificacoes.Add($"Dívida \"{d.Titulo}\" de {d.Devedor?.Name} vence em {dias} dia(s).");
            }

            var empresa = await _context.Empresas.FindAsync(empresaId);


            var vm = new EmpresaDashboardViewModel
            {
                TotalDevedores = totalDevedores,
                ComPendencia = comPendencia,
                SemPendencia = semPendencia,

                Pendentes = pendentes,
                Atrasadas = atrasadas,
                Quitadas = quitadas,

                Faturamento = faturamento,
                ValorEmAberto = valorEmAberto,

                FaturamentoMesAtual = faturamentoMesAtual,
                FaturamentoMesAnterior = faturamentoMesAnterior,

                TotalDividas = dividasDaEmpresa.Count,
                ValorTotalDividas = dividasDaEmpresa.Sum(d => d.Valor),

                ValorPendente = dividasDaEmpresa
        .Where(d => d.Status == "Pendente")
        .Sum(d => d.Valor),

                ValorAtrasado = dividasDaEmpresa
        .Where(d => d.Status == "Vencido")
        .Sum(d => d.Valor),

                ValorQuitado = dividasDaEmpresa
        .Where(d => d.Status == "Quitado" || d.Status == "Finalizado")
        .Sum(d => d.Valor),

                TopDevedores = topDevedores,
                Aging = agingBuckets,
                Recentes = recentes,
                Alertas = alertas,
                NotificacoesRecentes = notificacoes,

                TaxaRecuperacaoPercent = Math.Round(taxaRecuperacao, 1),
                TempoMedioPagamentoDias = (decimal)Math.Round(mediaDias, 1),

                NomeEmpresa = empresa?.Nome ?? "Empresa"

            };

            Console.WriteLine("Qtd de pagamentos analisados: " + pagamentosQuitados.Count);
            foreach (var dias in pagamentosQuitados)
            {
                Console.WriteLine($"Diferença: {dias}");
            }
            Console.WriteLine("Média calculada: " + mediaDias);

            Console.WriteLine($"Tempo médio de pagamento: {mediaDias}");


            return View(vm);

        }

    }
}
