Aby serwer sensownie działał należy mnieć pobrane .NET SDK 8.x:
https://dotnet.microsoft.com/en-us/download/dotnet

Ważne, żeby po pobraniu najnowsze mastera pusić update bazydanych bo .db jest ignorowany

Postawienie lub update bazy do najnowszych migracji
```
dotnet ef database update
```


Dodanie nowej migracji (zmiany na sql)
```
dotnet ef migrations add MIGRATIONNAME
```

Uruchomienie serwera:
```
dotnet run
```


Dla frontendu i testów:

Został wystawiony swagger gdzie możecie testować endpointy oraz zobaczyć ich dokumentacje/schematy, znajduje się on pod linkiem:
```
<ServerAdress>/swagger/index.html
```

Przykładowo:
```
http://localhost:5250/swagger/index.html
```