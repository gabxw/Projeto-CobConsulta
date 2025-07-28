using CobrancaPro.Models;

namespace CobrancaPro.Services
{
    public class BillingService : IBillingService
    {
        private static List<BillingRecord> _billingRecords = new List<BillingRecord>
        {
            new BillingRecord { Id = 1, Client = "Empresa ABC Ltda.", Value = 2450.00m, DueDate = new DateTime(2023, 6, 15), Status = BillingStatus.Pago },
            new BillingRecord { Id = 2, Client = "Comércio XYZ S.A.", Value = 3210.50m, DueDate = new DateTime(2023, 6, 18), Status = BillingStatus.Pendente },
            new BillingRecord { Id = 3, Client = "Indústria 123 Ltda.", Value = 5670.00m, DueDate = new DateTime(2023, 6, 10), Status = BillingStatus.Atrasado },
            new BillingRecord { Id = 4, Client = "Serviços QWERTY EIRELI", Value = 1890.00m, DueDate = new DateTime(2023, 6, 20), Status = BillingStatus.Pendente },
            new BillingRecord { Id = 5, Client = "Distribuidora ASDFG S.A.", Value = 4320.75m, DueDate = new DateTime(2023, 6, 5), Status = BillingStatus.Atrasado },
            new BillingRecord { Id = 6, Client = "Construtora ZXCVB Ltda.", Value = 7890.25m, DueDate = new DateTime(2023, 6, 25), Status = BillingStatus.Pago },
            new BillingRecord { Id = 7, Client = "Transportadora HJKL S.A.", Value = 2340.80m, DueDate = new DateTime(2023, 6, 12), Status = BillingStatus.Pendente },
            new BillingRecord { Id = 8, Client = "Farmácia UIOP Ltda.", Value = 1560.45m, DueDate = new DateTime(2023, 6, 8), Status = BillingStatus.Atrasado }
        };

        public KPIData GetKPIData()
        {
            var totalToReceive = _billingRecords.Where(r => r.Status != BillingStatus.Pago).Sum(r => r.Value);
            var receivedThisMonth = _billingRecords.Where(r => r.Status == BillingStatus.Pago).Sum(r => r.Value);
            var overdue = _billingRecords.Where(r => r.Status == BillingStatus.Atrasado).Sum(r => r.Value);
            var defaultClients = _billingRecords.Where(r => r.Status == BillingStatus.Atrasado).Count();

            return new KPIData
            {
                TotalToReceive = totalToReceive,
                ReceivedThisMonth = receivedThisMonth,
                Overdue = overdue,
                DefaultClients = defaultClients,
                TotalToReceiveChange = 12,
                ReceivedThisMonthChange = 8,
                OverdueChange = 5
            };
        }

        public List<BillingRecord> GetBillingRecords()
        {
            return _billingRecords.OrderByDescending(r => r.CreatedAt).ToList();
        }

        public BillingRecord GetBillingRecord(int id)
        {
            return _billingRecords.FirstOrDefault(r => r.Id == id);
        }

        public void CreateBillingRecord(BillingRecord record)
        {
            record.Id = _billingRecords.Max(r => r.Id) + 1;
            _billingRecords.Add(record);
        }

        public void UpdateBillingRecord(BillingRecord record)
        {
            var existingRecord = _billingRecords.FirstOrDefault(r => r.Id == record.Id);
            if (existingRecord != null)
            {
                existingRecord.Client = record.Client;
                existingRecord.Value = record.Value;
                existingRecord.DueDate = record.DueDate;
                existingRecord.Status = record.Status;
            }
        }

        public void DeleteBillingRecord(int id)
        {
            var record = _billingRecords.FirstOrDefault(r => r.Id == id);
            if (record != null)
            {
                _billingRecords.Remove(record);
            }
        }
    }
}