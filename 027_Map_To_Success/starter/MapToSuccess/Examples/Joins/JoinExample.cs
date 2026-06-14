namespace MapToSuccess.Examples.Joins;

using Spectre.Console;

/// <summary>
/// Shows the dictionary as a join index. The same two lists are joined first with a
/// nested loop and then with a dictionary; the results are identical but the second
/// approach replaces "orders × customers" work with "orders + customers". Also shows
/// the one-key-many-values shape with <c>GroupBy().ToDictionary()</c>.
/// </summary>
public sealed class JoinExample : IExample
{
    private static readonly IReadOnlyList<Customer> _sampleCustomers =
    [
        new Customer(1, "Alice"),
        new Customer(2, "Bob"),
        new Customer(3, "Carol"),
    ];

    private static readonly IReadOnlyList<Order> _sampleOrders =
    [
        new Order(101, CustomerId: 1, Amount: 50.00m),
        new Order(102, CustomerId: 2, Amount: 20.00m),
        new Order(103, CustomerId: 1, Amount: 30.00m),
        new Order(104, CustomerId: 3, Amount: 75.50m),
        new Order(105, CustomerId: 2, Amount: 10.00m),
        new Order(106, CustomerId: 9, Amount: 99.99m), // customer 9 does not exist
    ];

    public string Title => "Joining two lists (the map as an index)";

    public string Description => "Replace a nested-loop join with a dictionary index; one-to-many via GroupBy.";

    public void Run(IAnsiConsole console)
    {
        IReadOnlyList<EnrichedOrder> viaLoop = OrderJoins.JoinWithNestedLoop(_sampleOrders, _sampleCustomers);
        IReadOnlyList<EnrichedOrder> viaDictionary = OrderJoins.JoinWithDictionary(_sampleOrders, _sampleCustomers);

        console.MarkupLine("[bold]Orders enriched with their customer's name[/] (both joins agree):");
        Table joined = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("order")
            .AddColumn("customer")
            .AddColumn("amount", column => column.RightAligned());
        foreach (EnrichedOrder enriched in viaDictionary)
        {
            string name = enriched.CustomerName == OrderJoins.UnknownCustomer
                ? $"[red]{Markup.Escape(enriched.CustomerName)}[/]"
                : Markup.Escape(enriched.CustomerName);
            joined.AddRow(enriched.OrderId.ToString(), name, enriched.Amount.ToString("N2"));
        }

        console.Write(joined);

        RenderAgreement(console, viaLoop.SequenceEqual(viaDictionary));

        int nestedWorstCase = _sampleOrders.Count * _sampleCustomers.Count;
        int indexedWork = _sampleOrders.Count + _sampleCustomers.Count;
        console.MarkupLine(
            $"[bold]Work:[/] nested loop does up to [red]{nestedWorstCase}[/] comparisons "
            + $"(orders × customers), the index does about [green]{indexedWork}[/] (orders + customers). "
            + "With big lists that difference explodes.");

        console.WriteLine();
        console.MarkupLine("[bold]One key, many values[/] — [grey]GroupBy(o => o.CustomerId).ToDictionary(...)[/]:");
        RenderGrouping(console);
    }

    internal static void RenderAgreement(IAnsiConsole console, bool identical) =>
        console.MarkupLine(identical
            ? "[green]✓ the nested-loop join and the dictionary join produced the same rows.[/]"
            : "[red]✗ the two joins disagreed![/]");

    private static void RenderGrouping(IAnsiConsole console)
    {
        Dictionary<int, List<Order>> ordersByCustomer = OrderJoins.GroupOrdersByCustomer(_sampleOrders);
        Dictionary<int, decimal> totalByCustomer = OrderJoins.TotalAmountByCustomer(_sampleOrders);

        Table table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("customer id")
            .AddColumn("order ids")
            .AddColumn("total", column => column.RightAligned());

        foreach ((int customerId, List<Order> orders) in ordersByCustomer.OrderBy(pair => pair.Key))
        {
            string orderIds = string.Join(", ", orders.Select(order => order.Id));
            table.AddRow(customerId.ToString(), orderIds, totalByCustomer[customerId].ToString("N2"));
        }

        console.Write(table);
    }
}
