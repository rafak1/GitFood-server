Aby serwer sensownie działał należy mnieć pobrane najnowsze .NET SDK


WAŻNE
W plikach appsettings.json oraz appsetting.Development.json należy mieć ustawioną krotkę WebApiDatabase na loklizacje poza repo (dokładniej na folder powyżej repo), tam będzie znajdowała się lokalna baza. GIT IGNORE NIE DZIAŁA NA PLIKU .db (przynajmniej mi, jak ktoś to umie naprawić to zapszam) dlatego trzymamy go poza repo i poprostu podnosimy sie odpowienio po tym, pullu mastera.

Postawienie lub update bazy do najnowszych migracji
```
dotnet ef database update
```


Dodanie nowej migracji (zmiany na sql)
```
dotnet ef migrations add MIGRATIONNAME
```