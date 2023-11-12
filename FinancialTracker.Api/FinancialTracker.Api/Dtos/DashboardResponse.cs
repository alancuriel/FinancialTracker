namespace FinancialTracker.Api;

public class DashboardResponse
{
    public decimal Income { get; set; }
    public decimal IncomeToLastMonth { get; set; }
    public decimal Expenses { get; set; }
    public decimal ExpensesToLastMonth { get; set; }
    public string TopCategory { get; set; } = string.Empty;
    public decimal AmountSpentOnCategory { get; set; }
    public int TransactionsThisMonth { get; set; }
    public int TransactionsLastMonth { get; set; }
    public List<SpentInDay> SpentThisMonth { get; set; } = new(31);
    public List<DashboardTransaction> RecentTransactions { get; set; } = new();
}

public class SpentInDay
{
    public int Day { get; set; }
    public decimal Spent { get; set; }
}

public class DashboardTransaction
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Category { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
}


