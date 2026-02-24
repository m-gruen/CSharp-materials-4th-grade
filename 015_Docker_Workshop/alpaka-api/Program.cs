using System.Text.Json.Serialization;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain
           .Insert(0, AlpakaJsonSerializerContext.Default);
});

var app = builder.Build();

string serviceName = app.Configuration["ALPAKA_API_NAME"] ?? "Alpaka Herd API";

List<Alpaka> herd =
[
    new(1, "Momo", 4, "White"),
    new(2, "Pablo", 6, "Brown"),
    new(3, "Luna", 2, "Black")
];

var sync = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
int nextId = herd.Max(item => item.Id) + 1;

app.MapGet("/", () => TypedResults.Ok(new ApiStatus(serviceName, "ok")));

app.MapGet("/api/alpakas", () =>
{
    sync.EnterReadLock();
    try
    {
        return TypedResults.Ok(new AlpakaCollectionResponse(serviceName, herd.ToList()));
    }
    finally
    {
        sync.ExitReadLock();
    }
});

app.MapGet("/api/alpakas/{id:int}", IResult (int id) =>
{
    sync.EnterReadLock();
    try
    {
        var alpaka = herd.FirstOrDefault(item => item.Id == id);

        return alpaka is null
            ? TypedResults.NotFound(new ErrorResponse($"No alpaka found with id {id}."))
            : TypedResults.Ok(alpaka);
    }
    finally
    {
        sync.ExitReadLock();
    }
});

app.MapPost("/api/alpakas", IResult (CreateAlpakaRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.Name))
    {
        return TypedResults.BadRequest(new ErrorResponse("Name is required."));
    }

    if (request.Age < 0)
    {
        return TypedResults.BadRequest(new ErrorResponse("Age must be zero or greater."));
    }

    if (string.IsNullOrWhiteSpace(request.Color))
    {
        return TypedResults.BadRequest(new ErrorResponse("Color is required."));
    }

    sync.EnterWriteLock();
    try
    {
        var created = new Alpaka(nextId, request.Name.Trim(), request.Age, request.Color.Trim());
        herd.Add(created);
        nextId++;

        return TypedResults.Created($"/api/alpakas/{created.Id}", created);
    }
    finally
    {
        sync.ExitWriteLock();
    }
});

await app.RunAsync();

internal sealed record ApiStatus(string ServiceName, string Status);

internal sealed record Alpaka(int Id, string Name, int Age, string Color);

internal sealed record AlpakaCollectionResponse(string ServiceName, List<Alpaka> Items);

internal sealed record CreateAlpakaRequest(string Name, int Age, string Color);

internal sealed record ErrorResponse(string Message);

[JsonSerializable(typeof(ApiStatus))]
[JsonSerializable(typeof(Alpaka))]
[JsonSerializable(typeof(List<Alpaka>))]
[JsonSerializable(typeof(AlpakaCollectionResponse))]
[JsonSerializable(typeof(CreateAlpakaRequest))]
[JsonSerializable(typeof(ErrorResponse))]
internal sealed partial class AlpakaJsonSerializerContext : JsonSerializerContext { }
