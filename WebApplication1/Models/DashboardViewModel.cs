namespace CobrancaPro.Models
{
    public class DashboardViewModel
    {
        public KPIData KPIData { get; set; }
        public List<BillingRecord> BillingRecords { get; set; }
        public string SearchTerm { get; set; } = "";
        public string StatusFilter { get; set; } = "all";
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
        public string ActiveMenuItem { get; set; } = "dashboard";
    }
}