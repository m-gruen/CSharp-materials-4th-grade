namespace MapToSuccess.Examples.Joins;

/// <summary>
/// Joining two in-memory collections is the second great use of a dictionary: it turns a
/// list into a lookup <em>index</em>. The naive nested-loop join re-scans the customers for
/// every order (O(orders × customers)); building an index once and reusing it is
/// O(orders + customers). This is the shape of work an ORM like Entity Framework does for you.
/// </summary>
public static class OrderJoins
{
    /// <summary>Name to use when an order references a customer that does not exist.</summary>
    public const string UnknownCustomer = "(unknown)";

    /// <summary>
    /// Naive join: for each order, scan the entire customer list looking for a match. Correct,
    /// but the work grows with orders × customers.
    /// </summary>
    public static IReadOnlyList<EnrichedOrder> JoinWithNestedLoop(
        IReadOnlyList<Order> orders,
        IReadOnlyList<Customer> customers)
    {
        ArgumentNullException.ThrowIfNull(orders);
        ArgumentNullException.ThrowIfNull(customers);

        // TODO: for each order, search `customers` for the one whose Id == order.CustomerId.
        //       Produce a new EnrichedOrder(order.Id, customer?.Name ?? UnknownCustomer, order.Amount).
        throw new NotImplementedException();
    }

    /// <summary>
    /// Indexed join: build a customer index once with <c>ToDictionary</c>, then every order
    /// resolves its customer in one O(1) lookup.
    /// </summary>
    public static IReadOnlyList<EnrichedOrder> JoinWithDictionary(
        IReadOnlyList<Order> orders,
        IReadOnlyList<Customer> customers)
    {
        ArgumentNullException.ThrowIfNull(orders);
        ArgumentNullException.ThrowIfNull(customers);

        // TODO: build customersById = customers.ToDictionary(c => c.Id) ONCE, then map each order
        //       to EnrichedOrder(order.Id, customersById.GetValueOrDefault(order.CustomerId)?.Name
        //       ?? UnknownCustomer, order.Amount). The result must equal JoinWithNestedLoop's.
        throw new NotImplementedException();
    }

    /// <summary>
    /// One key, many values: group every order under its customer id, giving a
    /// <c>Dictionary&lt;int, List&lt;Order&gt;&gt;</c>.
    /// </summary>
    public static Dictionary<int, List<Order>> GroupOrdersByCustomer(IReadOnlyList<Order> orders)
    {
        ArgumentNullException.ThrowIfNull(orders);

        // TODO: orders.GroupBy(o => o.CustomerId).ToDictionary(g => g.Key, g => g.ToList())
        throw new NotImplementedException();
    }

    /// <summary>Aggregation per key: the total order amount for each customer.</summary>
    public static Dictionary<int, decimal> TotalAmountByCustomer(IReadOnlyList<Order> orders)
    {
        ArgumentNullException.ThrowIfNull(orders);

        // TODO: orders.GroupBy(o => o.CustomerId).ToDictionary(g => g.Key, g => g.Sum(o => o.Amount))
        throw new NotImplementedException();
    }
}
