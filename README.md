# PosApp461 — POS Console App (.NET Framework 4.6.1)

A port of PosApp with .NET 8.0 (that I made and then converted to 4.6.1) targeting .NET Framework 4.6.1, using C# 7.3. Same conditional compilation pattern (`#if CLIENT_A` / `#if CLIENT_B`), same BDD scenarios, same business logic — just targeting the classic .NET Framework runtime.

## Client Variants

| | Client A | Client B |
|---|---|---|
| Extra field | `Notes` | `DebitCard` |
| On submit | Transaction immediately **Completed** | Transaction saved as **Pending** |
| Extra menu | — | "Process Pending Payments" (select by ID) |
| DB file | `clienta.db` | `clientb.db` |
| Output | `bin\ClientA\PosApp.ClientA.exe` | `bin\ClientB\PosApp.ClientB.exe` |

## Prerequisites

**Build machine:** .NET SDK 6+ (cross-compiles to net461 on any OS)

**Runtime machine:** Windows with [.NET Framework 4.6.1](https://dotnet.microsoft.com/download/dotnet-framework/net461) installed

> Note: The original net8.0 version is in the sibling `PosApp/` folder.

## Solution Structure

```
PosApp461.sln
├── src/
│   ├── Domain/                        # PosApp.Domain.csproj — entities, repository interface
│   └── Console/                       # Shared source — compiled by both client csproj files
│       ├── PosApp.ClientA461.csproj   # DefineConstants=CLIENT_A → bin\ClientA\PosApp.ClientA.exe
│       ├── PosApp.ClientB461.csproj   # DefineConstants=CLIENT_B → bin\ClientB\PosApp.ClientB.exe
│       ├── Services/TransactionService.cs
│       ├── Infrastructure/SqliteTransactionRepository.cs
│       ├── Menus/MainMenu.cs
│       └── Program.cs
└── tests/
    └── PosApp.Tests461.csproj         # SpecFlow + xUnit BDD tests (net461)
```

## Build

```bash
dotnet publish src/Console/PosApp.ClientA461.csproj -c Release -r win-x64 --self-contained false -o bin/ClientA
dotnet publish src/Console/PosApp.ClientB461.csproj -c Release -r win-x64 --self-contained false -o bin/ClientB
```

> Use `dotnet publish` (not `dotnet build`) — publish ensures the native `e_sqlite3.dll` is copied to the output folder alongside the `.exe`.

## Run (Windows)

```cmd
bin\ClientA\PosApp.ClientA.exe
bin\ClientB\PosApp.ClientB.exe
```

## Test

```bash
dotnet test tests/PosApp.Tests461.csproj
```

> On Linux/macOS, running net461 tests requires Mono. On Windows with .NET Framework 4.6.1, tests run natively.

## Conditional Compilation

Same pattern as PosApp — two `.csproj` files share the same source, each defining a different constant:

```xml
<!-- PosApp.ClientA461.csproj -->
<DefineConstants>CLIENT_A</DefineConstants>

<!-- PosApp.ClientB461.csproj -->
<DefineConstants>CLIENT_B</DefineConstants>
```

## C# 7.3 Compatibility Notes

This project targets C# 7.3 (max supported by net461). Key differences from the net8.0 version:

| net8.0 (C# 12) | net461 (C# 7.3) |
|---|---|
| File-level namespaces | Block namespaces `namespace X { }` |
| Top-level statements | `class Program { static void Main() }` |
| `using var` | `using (var ...)` |
| `new()` target-typed | `new ClassName()` |
| Collection literals `[x]` | `new[] { x }` |
| `Enum.Parse<T>()` | `(T)Enum.Parse(typeof(T), s)` |

## Notes

This project only a showdown to let dev know that it's possible to use #IF for building. For more, please read 
[Preprosesor Directive](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/preprocessor-directives)