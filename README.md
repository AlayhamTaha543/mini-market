# mini-market

## MiniMarketApp

Offline WPF mini-market desktop app scaffolded with layered MVVM architecture:

- `Views -> ViewModels -> Services -> Repositories -> Data`
- SQLite via `Microsoft.Data.Sqlite` with `PRAGMA foreign_keys = ON` per connection
- Product barcode is indexed and non-unique
- Soft-delete enabled for Products and Users (`IsActive`)
- Snapshot pricing columns for `SaleItems` and `Losses`
- Global RTL UI and Arabic strings centralized in `MiniMarketApp/Resources/Strings.xaml`

### Build

```bash
dotnet build /home/runner/work/mini-market/mini-market/MiniMarketApp.slnx -c Release
```
