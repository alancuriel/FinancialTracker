using System;
using CsvHelper.Configuration;

namespace FinancialTracker.Api.Model;

public class CopilotTransaction
{
    public string Date { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string ParentCategory { get; set; } = string.Empty;
    public bool Excluded { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Account { get; set; } = string.Empty;
    public int AccountMask { get; set; }
    public string Note { get; set; } = string.Empty;
    public string Recurring { get; set; } = string.Empty;
}
