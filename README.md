Aby serwer sensownie działał należy mnieć pobrane najnowsze .NET SDK


WAŻNE
W plikach appsettings.json oraz appsetting.Development.json należy mieć ustawioną krotkę WebApiDatabase na loklizacje poza repo, gdzie chesz żeby się znajdowała lokalna baza. GIT IGNORE NIE DZIAŁA NA PLIKU .db dlatego trzymamy go poza repo i poprostu podnosimy sie odpowienio po tym, pullu mastera (w związku z tym appsettings leci do .gitignore), oraz zeby pierwszy raz odpalić serwer należy odpalić komendę poniżej.

Postawienie lub update bazy do najnowszych migracji
```
dotnet ef database update
```


Dodanie nowej migracji (zmiany na sql)
```
dotnet ef migrations add MIGRATIONNAME
```