namespace MapToSuccess.Examples.Joins;

/// <summary>An order placed by the customer with id <see cref="CustomerId"/>.</summary>
public sealed record Order(int Id, int CustomerId, decimal Amount);
