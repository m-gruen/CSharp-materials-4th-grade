namespace MapToSuccess.Examples.Joins;

/// <summary>An order with its customer's name filled in (the result of a join).</summary>
public sealed record EnrichedOrder(int OrderId, string CustomerName, decimal Amount);
