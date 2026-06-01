# Run BuildForge PC Store

## Visual Studio for Mac

This project targets **.NET 7** to match the SDK bundled with Visual Studio for Mac on your machine.

1. Open **`BuildForgePcStore.sln`**
2. Set **BuildForgePcStore** as startup project
3. **Build → Rebuild All**
4. Press **F5**

Check SDK in Terminal:

```bash
dotnet --version
# Should show 7.0.x when building from VS for Mac
```

If you previously built with .NET 8 or 9, delete `buildforge.db` and the `bin` / `obj` folders, then run again.

## Test logins

| Role | Email | Password |
|------|-------|----------|
| Admin | admin@buildforge.my | Admin@123 |
| Customer | demo@buildforge.my | Customer@123 |

## Terminal (macOS)

```bash
cd BuildForgePcStore
dotnet restore
dotnet run
```

## Windows (Visual Studio 2022)

Install **.NET 7 SDK** or use VS 2022 17.4+ which includes it. Same solution opens and runs.

## Troubleshooting

| Error | Fix |
|-------|-----|
| NETSDK1045 (.NET 8 not supported) | Project is **net7.0** — pull latest, delete `bin`/`obj`, rebuild |
| Database errors | Delete `buildforge.db`, run app again |
| Port in use | Stop other debug sessions |
