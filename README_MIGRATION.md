# Migration Guide

Tai lieu nay huong dan reset database, tao migration moi, va cap nhat database cho backend `dtcproject`.

## 1. Di chuyen vao thu muc goc backend

```powershell
cd D:\Project_Sample\driving-training-centers-project-v1\repo-backend\dtcproject
```

## 2. Build truoc khi migrate

```powershell
dotnet build dtc.API\dtc.API.csproj
```

Neu build loi thi sua loi code truoc khi chay migration.

## 3. Tao migration moi

Khi model trong `SQLDBContext` thay doi, tao migration bang lenh:

```powershell
dotnet ef migrations add TenMigrationMoi --project dtc.Infrastructure\dtc.Infrastructure.csproj --startup-project dtc.API\dtc.API.csproj --context SQLDBContext --output-dir Persistence\SQLServer\Migrations
```

Vi du:

```powershell
dotnet ef migrations add UpdateCourseSeed --project dtc.Infrastructure\dtc.Infrastructure.csproj --startup-project dtc.API\dtc.API.csproj --context SQLDBContext --output-dir Persistence\SQLServer\Migrations
```

## 4. Cap nhat SQL Server database

```powershell
dotnet ef database update --project dtc.Infrastructure\dtc.Infrastructure.csproj --startup-project dtc.API\dtc.API.csproj --context SQLDBContext
```

Lenh nay se apply toan bo migration chua chay vao SQL Server.

## 5. Reset SQL Server database tu dau

Neu muon xoa DB cu va tao lai schema moi:

```powershell
dotnet ef database drop --force --project dtc.Infrastructure\dtc.Infrastructure.csproj --startup-project dtc.API\dtc.API.csproj --context SQLDBContext
dotnet ef database update --project dtc.Infrastructure\dtc.Infrastructure.csproj --startup-project dtc.API\dtc.API.csproj --context SQLDBContext
```

Luu y:
- Luon dung `--force` de khong bi kẹt o cau hoi `(y/N)`.
- Khong go `y` o PowerShell sau khi lenh da ket thuc, vi khi do `y` se bi hieu la mot command rieng.

## 6. MongoDB seed data

MongoDB khong dung EF migration trong du an nay.

Du lieu mau MongoDB duoc seed tu dong khi chay API:

```powershell
dotnet run --project dtc.API\dtc.API.csproj
```

Seeder Mongo hien tai nam o:
- `dtc.Infrastructure\Persistence\Seeding\MongoSeedData.cs`
- `dtc.Infrastructure\Persistence\Seeding\MongoDbSeeder.cs`

Neu muon reset MongoDB sach hoan toan, hay drop database `dtcproject` hoac xoa cac collection can thiet roi chay lai API.

## 7. Seed SQL hien tai

Seed SQL duoc khai bao tai:

- `dtc.Infrastructure\Persistence\Seeding\SqlSeedData.cs`
- `dtc.Infrastructure\Persistence\SQLServer\SQLDBContext.cs`

Moi bang SQL hien dang co 2 ban ghi mau.

## 8. Migration hien tai

Tinh den hien tai, database SQL dang dung cac migration:

- `20260227155551_InitialCreate`
- `20260326061140_FinalClerkAndCloudinarySync`
- `20260327183728_SeedAndContextSync`
- `20260327185204_FinalModelSync`

## 9. Khi gap loi PendingModelChangesWarning

Neu gap loi:

```text
The model for context 'SQLDBContext' has pending changes.
```

thi lam theo thu tu:

```powershell
dotnet build dtc.API\dtc.API.csproj
dotnet ef migrations add TenMigrationMoi --project dtc.Infrastructure\dtc.Infrastructure.csproj --startup-project dtc.API\dtc.API.csproj --context SQLDBContext --output-dir Persistence\SQLServer\Migrations
dotnet ef database update --project dtc.Infrastructure\dtc.Infrastructure.csproj --startup-project dtc.API\dtc.API.csproj --context SQLDBContext
```

## 10. Ghi chu

- `SQLDBContextFactory` da duoc them de ho tro `dotnet ef` on dinh hon.
- Neu thay doi entity/domain ma khong tao migration moi, `database update` se bi chan.
- Nen kiem tra file snapshot sau moi lan tao migration:
  `dtc.Infrastructure\Persistence\SQLServer\Migrations\SQLDBContextModelSnapshot.cs`
