Aby serwer sensownie działał należy mnieć pobrane .NET SDK 8.x:
https://dotnet.microsoft.com/en-us/download/dotnet

przygotowanie podstawowych nugetow:
```
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Design
```

Ważne, żeby po pobraniu najnowsze mastera pusić update bazydanych bo .db jest ignorowany

Postawienie lub update bazy do najnowszych migracji
```
dotnet ef database update
```


Dodanie nowej migracji (zmiany na sql)
```
dotnet ef migrations add MIGRATIONNAME
```
