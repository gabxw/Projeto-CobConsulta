using Microsoft.AspNetCore.Mvc;
using CobrancaPro.Models;
using CobrancaPro.Services;

namespace CobrancaPro.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBillingService _billingService;
        private const int RecordsPerPage = 5;

        public HomeController(IBillingService billingService)
        {
            _billingService = billingService;
        }

        public IActionResult Index(string searchTerm = "", string statusFilter = "all", int page = 1)
        {
            var kpiData = _billingService.GetKPIData();
            var allRecords = _billingService.GetBillingRecords();

            // Aplicar filtros
            var filteredRecords = allRecords.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                filteredRecords = filteredRecords.Where(r =>
                    r.Client.ToLower().Contains(searchTerm.ToLower()));
            }

            if (statusFilter != "all")
            {
                if (Enum.TryParse<BillingStatus>(statusFilter, true, out var status))
                {
                    filteredRecords = filteredRecords.Where(r => r.Status == status);
                }
            }

            var totalRecords = filteredRecords.Count();
            var totalPages = (int)Math.Ceiling((double)totalRecords / RecordsPerPage);

            var paginatedRecords = filteredRecords
                .Skip((page - 1) * RecordsPerPage)
                .Take(RecordsPerPage)
                .ToList();

            var viewModel = new DashboardViewModel
            {
                KPIData = kpiData,
                BillingRecords = paginatedRecords,
                SearchTerm = searchTerm,
                StatusFilter = statusFilter,
                CurrentPage = page,
                TotalPages = totalPages,
                TotalRecords = totalRecords,
                ActiveMenuItem = "dashboard"
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Search(string searchTerm, string statusFilter)
        {
            return RedirectToAction("Index", new { searchTerm, statusFilter, page = 1 });
        }

        public IActionResult Clientes()
        {
            var viewModel = new DashboardViewModel
            {
                ActiveMenuItem = "clientes"
            };
            return View(viewModel);
        }

        public IActionResult Cobrancas()
        {
            var viewModel = new DashboardViewModel
            {
                ActiveMenuItem = "cobrancas"
            };
            return View(viewModel);
        }

        public IActionResult Relatorios()
        {
            var viewModel = new DashboardViewModel
            {
                ActiveMenuItem = "relatorios"
            };
            return View(viewModel);
        }

        public IActionResult Configuracoes()
        {
            var viewModel = new DashboardViewModel
            {
                ActiveMenuItem = "configuracoes"
            };
            return View(viewModel);
        }
    }
}