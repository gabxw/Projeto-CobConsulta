using CobrancaPro.Models;

namespace CobrancaPro.Services
{
    public interface IBillingService
    {
        KPIData GetKPIData();
        List<BillingRecord> GetBillingRecords();
        BillingRecord GetBillingRecord(int id);
        void CreateBillingRecord(BillingRecord record);
        void UpdateBillingRecord(BillingRecord record);
        void DeleteBillingRecord(int id);
    }
}