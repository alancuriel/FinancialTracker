namespace FinancialTracker.Api.Helpers;

public static class FinancialHelper
{
    private static readonly string[] colors =
        { "blue", "green", "red", "orange", "yellow" , "purple", "cyan"};

    public static void SetCatColors(this IEnumerable<Category> categories)
    {
        short i = 0;

        foreach (var category in categories)
        {
            category.Color = colors[i % colors.Length];
            i += 1;
        }
    }
}
