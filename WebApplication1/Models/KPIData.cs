namespace CobrancaPro.Models
{
    public class KPIData
    {
        public decimal TotalToReceive { get; set; }
        public decimal ReceivedThisMonth { get; set; }
        public decimal Overdue { get; set; }
        public int DefaultClients { get; set; }
        public decimal TotalToReceiveChange { get; set; }
        public decimal ReceivedThisMonthChange { get; set; }
        public decimal OverdueChange { get; set; }
    }
}