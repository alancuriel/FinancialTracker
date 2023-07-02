using System;
using CsvHelper.Configuration;
using FinancialTracker.Api.Model;

namespace FinancialTracker.Api.Mappers.CsvMappers;

public sealed class CopilotTransactionMap : ClassMap<CopilotTransaction>
{
    public CopilotTransactionMap()
    {
        Map(t => t.Date).Name("date");
        Map(t => t.Name).Name("name");
        Map(t => t.Amount).Name("amount");
        Map(t => t.Status).Name("status");
        Map(t => t.Category).Name("category");
        Map(t => t.ParentCategory).Name("parent category");
        Map(t => t.Type).Name("type");
        Map(t => t.Account).Name("account");
        Map(t => t.AccountMask).Name("account mask").Default(0, true);
        Map(t => t.Note).Name("note");
        Map(t => t.Recurring).Name("recurring");
        Map(t => t.Excluded).Name("excluded");
    }
}

