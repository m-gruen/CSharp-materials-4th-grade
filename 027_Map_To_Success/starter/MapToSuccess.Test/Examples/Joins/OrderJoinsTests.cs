namespace MapToSuccess.Test.Examples.Joins;

using global::MapToSuccess.Examples.Joins;

public sealed class OrderJoinsTests
{
    private static readonly IReadOnlyList<Customer> Customers =
    [
        new Customer(1, "Alice"),
        new Customer(2, "Bob"),
    ];

    private static readonly IReadOnlyList<Order> Orders =
    [
        new Order(101, CustomerId: 1, Amount: 50m),
        new Order(102, CustomerId: 2, Amount: 20m),
        new Order(103, CustomerId: 1, Amount: 30m),
        new Order(104, CustomerId: 9, Amount: 5m), // unknown customer
    ];

    [Fact]
    public void BothJoins_ProduceIdenticalResults()
    {
        IReadOnlyList<EnrichedOrder> viaLoop = OrderJoins.JoinWithNestedLoop(Orders, Customers);
        IReadOnlyList<EnrichedOrder> viaDictionary = OrderJoins.JoinWithDictionary(Orders, Customers);

        viaDictionary.Should().Equal(viaLoop);
    }

    [Fact]
    public void Join_FillsInCustomerNames_AndMarksUnknownCustomers()
    {
        IReadOnlyList<EnrichedOrder> enriched = OrderJoins.JoinWithDictionary(Orders, Customers);

        enriched.Should().Equal(
            new EnrichedOrder(101, "Alice", 50m),
            new EnrichedOrder(102, "Bob", 20m),
            new EnrichedOrder(103, "Alice", 30m),
            new EnrichedOrder(104, OrderJoins.UnknownCustomer, 5m));
    }

    [Fact]
    public void GroupOrdersByCustomer_PutsEveryOrderUnderItsCustomer()
    {
        Dictionary<int, List<Order>> grouped = OrderJoins.GroupOrdersByCustomer(Orders);

        grouped.Should().ContainKey(1);
        grouped[1].Select(order => order.Id).Should().Equal(101, 103);
        grouped[2].Select(order => order.Id).Should().Equal(102);
        grouped[9].Select(order => order.Id).Should().Equal(104);
    }

    [Fact]
    public void TotalAmountByCustomer_SumsEachCustomersOrders()
    {
        Dictionary<int, decimal> totals = OrderJoins.TotalAmountByCustomer(Orders);

        totals[1].Should().Be(80m);
        totals[2].Should().Be(20m);
        totals[9].Should().Be(5m);
    }

    [Fact]
    public void Methods_RejectNullArguments()
    {
        IReadOnlyList<Order> nullOrders = null!;
        IReadOnlyList<Customer> nullCustomers = null!;

        ((Action)(() => OrderJoins.JoinWithNestedLoop(nullOrders, Customers))).Should().Throw<ArgumentNullException>();
        ((Action)(() => OrderJoins.JoinWithNestedLoop(Orders, nullCustomers))).Should().Throw<ArgumentNullException>();
        ((Action)(() => OrderJoins.JoinWithDictionary(nullOrders, Customers))).Should().Throw<ArgumentNullException>();
        ((Action)(() => OrderJoins.JoinWithDictionary(Orders, nullCustomers))).Should().Throw<ArgumentNullException>();
        ((Action)(() => OrderJoins.GroupOrdersByCustomer(nullOrders))).Should().Throw<ArgumentNullException>();
        ((Action)(() => OrderJoins.TotalAmountByCustomer(nullOrders))).Should().Throw<ArgumentNullException>();
    }
}
