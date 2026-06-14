# Map To Success — your Dictionary assignment

In this assignment you will learn the **`Dictionary<TKey, TValue>`** (a *hash map*) by
**building the logic yourself**. The app's plumbing is already written for you — the menu,
the colourful output, the data generators, and a **full set of unit tests**. What is
missing is the interesting part: every method that actually *uses* or *implements* a
dictionary currently looks like this:

```csharp
public bool ContainsKey(TKey key)
{
    // TODO: return true when the key is present, otherwise false.
    throw new NotImplementedException();
}
```

**Your job:** replace each `throw new NotImplementedException()` with a real
implementation, guided by the `// TODO` comment above it, until **all tests pass**.

## The one-paragraph mental model (read this first)

A dictionary stores **key → value** pairs. Ask it for a key and it returns the value
*immediately* — it does **not** scan every item. It runs the key through `GetHashCode()`
to choose a *bucket* (a small slot) and only looks inside that one bucket. That is why a
lookup stays fast (`O(1)` on average) even with millions of entries, while searching a
`List` gets slower as it grows (`O(n)`). The price: every key needs a sensible
`Equals`/`GetHashCode`, and keys must be unique. Everything in this assignment is a
variation on that single idea.

## How to work

1. Open the solution and run the tests — they are **red** to begin with:
   ```bash
   dotnet test
   ```
2. Pick an example below, open its files, and implement each stubbed method.
3. Re-run the tests for that area until they are **green**. The tests are your
   specification — read them when a `// TODO` is unclear.
4. When everything is green, run the app and watch your work:
   ```bash
   dotnet run --project MapToSuccess
   ```
5. **Goal:** all tests pass *and* code coverage stays at **100 %**:
   ```bash
   dotnet test --collect:"XPlat Code Coverage"
   ```

> You only edit the **logic** files listed below. You should not need to touch the menu,
> the `*Example` renderer classes, or the test project.

Do the examples roughly in order — #1 builds the intuition that makes the rest click.

---

## 1 · MiniDictionary — build the map yourself
**Why:** if you implement a hash map once, it stops being magic. You will *see* why a
lookup is fast and why keys need a hash code.

**File:** `MapToSuccess/Examples/MiniDictionary/MiniDictionary.cs`

Implement the array-of-buckets hash map (separate chaining):

- `GetBucketIndex(key)` — turn the key's hash code into a bucket index. Mask off the sign
  bit (`& int.MaxValue`) so it is never negative, then `% BucketCount`. *Why: a hash code
  can be negative, but an array index cannot.*
- `Add`, the indexer `this[key]`, `TryGetValue`, `ContainsKey`, `Remove` — each must look
  only inside the **one** bucket the key hashes to. *Why: scanning a single short chain
  instead of the whole map is exactly what makes it fast.*
- Growing: when the table passes ~75 % full, double the bucket count and **re-hash every
  entry**. *Why: a bigger array changes `hash % length`, so entries must move; and short
  chains keep lookups fast.*

**Done when:** `MiniDictionaryTests` and `MiniDictionaryExampleTests` pass — including the
test that proves two colliding keys share a bucket.

## 2 · Counting words — what a map is *for*
**Why:** "I have a key and want to update the value stored under it" is the single most
common use of a dictionary, and it is awkward with a `List`.

**File:** `MapToSuccess/Examples/WordFrequency/WordFrequencyCounter.cs`

Implement the same word-count four ways so you can compare the APIs:

- `CountWithContainsKey` — `ContainsKey` then the indexer (the beginner version; note it
  looks the key up twice).
- `CountWithTryGetValue` — one lookup with `TryGetValue` (`out` gives `0` when missing).
- `CountWithGetValueOrDefault` — the compact `counts[w] = counts.GetValueOrDefault(w) + 1`.
- `CountWithLinq` — declarative: `GroupBy(word => word).ToDictionary(g => g.Key, g => g.Count())`.
- `FirstSeenPositions` — use `TryAdd` to record only the **first** position of each word.
- `Tokenize` / `TopWords` — split text into words, and sort by count.

*Why so many?* They all produce the same answer; seeing them side by side teaches the API
surface and shows that `GroupBy().ToDictionary()` is the same shape you will later write
against a database with Entity Framework.

**Done when:** `WordFrequencyCounterTests` (which assert all four agree) and
`WordFrequencyExampleTests` pass.

## 3 · Fast lookup — *why it matters*
**Why:** this is the headline. You will measure, on your own machine, a dictionary beating
a list by a factor of thousands.

**Files:**
- `MapToSuccess/Examples/RegistryLookup/LinearRegistry.cs` — `FindById` by scanning the
  list (`FirstOrDefault`). `O(n)`.
- `MapToSuccess/Examples/RegistryLookup/DictionaryRegistry.cs` — build an index with
  `people.ToDictionary(p => p.Id)` in the constructor, then `FindById` is one lookup. `O(1)`.
- `MapToSuccess/Examples/RegistryLookup/LookupBenchmark.cs` — `Measure` runs every query
  id against a registry, counts hits, and sums the found ids into a checksum.

*Why a checksum?* It proves both strategies return **exactly the same results**, and stops
the compiler from optimising the "useless" lookups away.

**Done when:** `RegistryDataTests`, `LookupBenchmarkTests` and `RegistryLookupExampleTests`
pass. Then run the app: 10 million people, the list takes ~10 s, the dictionary < 1 ms.

## 4 · Joining two lists — the map as an *index*
**Why:** replacing nested loops with a lookup is one of the biggest real-world wins, and it
is exactly what an ORM does for you under the hood.

**File:** `MapToSuccess/Examples/Joins/OrderJoins.cs`

- `JoinWithNestedLoop` — for each order, scan all customers to find its customer.
  `orders × customers` work.
- `JoinWithDictionary` — build a customer index once with `ToDictionary`, then each order
  resolves in `O(1)`. `orders + customers` work.
- `GroupOrdersByCustomer` — one key, many values:
  `GroupBy(o => o.CustomerId).ToDictionary(g => g.Key, g => g.ToList())`.
- `TotalAmountByCustomer` — aggregation per key: `GroupBy(...).ToDictionary(..., g => g.Sum(...))`.

*Why both joins?* They must return identical rows — proving the fast version is also the
*correct* version. Handle orders whose customer does not exist (use `OrderJoins.UnknownCustomer`).

**Done when:** `OrderJoinsTests` and `JoinExampleTests` pass.

## 5 · Designing good keys — equality & hashing
**Why:** the dictionary finds entries via the key's `GetHashCode`/`Equals`. Get these
wrong and your lookups silently fail. This is where most real bugs come from.

**Files:**
- `MapToSuccess/Examples/Keys/Coordinate.cs` — a **good** key. Implement `IEquatable<Coordinate>`
  (`Equals` by value) and `GetHashCode` with `HashCode.Combine(Latitude, Longitude)`.
  *Why: equal keys must return the same hash, or the map looks in the wrong bucket.*
- `MapToSuccess/Examples/Keys/MutableKey.cs` — `Equals`/`GetHashCode` based on `Id`
  (deliberately mutable, to demonstrate the bug).
- `MapToSuccess/Examples/Keys/KeyDemonstrations.cs` — small methods that each return a
  boolean proving a point: a value key is found via a *fresh* instance; a reference key is
  **not**; mutating a stored key **strands** its entry; `StringComparer.OrdinalIgnoreCase`
  makes lookups case-insensitive; a NodaTime `LocalDate` works as a key for free.

*`ReferenceKey` is intentionally left without equality overrides* — your demonstration
should prove that two equal-looking instances are treated as different keys.

**Done when:** `KeyTypesTests`, `KeyDemonstrationsTests` and `KeysExampleTests` pass.

## 6 · Memoization — compute once, reuse
**Why:** a dictionary makes a perfect cache. Wrap an expensive function so it runs **once
per distinct key**.

**File:** `MapToSuccess/Examples/Memoization/Memoizer.cs`

Implement `Get(key)`: if the key is cached, return it and count a **hit**; otherwise count
a **miss**, run the wrapped function, store the result, and return it. *Why: the whole
point is to call the expensive function as few times as possible — exactly once per key.*

**Done when:** `MemoizerTests` (which assert the function runs once per distinct key) and
`MemoizationExampleTests` pass.

---

## What you should *not* change

These are provided so you can focus on dictionaries:

- `Program.cs`, `ExampleRunner`, `ExampleCatalog` — the menu and wiring.
- The `*Example` renderer classes — they print results with Spectre.Console.
- `PersonGenerator` / `RegistryWorkload` (test data) and the whole test project.

## Checklist before you hand in

- [ ] `dotnet build` succeeds with **no warnings** (warnings are treated as errors).
- [ ] `dotnet test` — **all green**.
- [ ] Coverage is **100 %** (`dotnet test --collect:"XPlat Code Coverage"`).
- [ ] You can run the app and explain, for each example, *why* a dictionary is the right
      tool.
