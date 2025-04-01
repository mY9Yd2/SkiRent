# SkiRent vizsgaremek projekt

Az adatbázishoz szükséges SQL-fájlok a [src/SkiRent.Database](./src/SkiRent.Database/) mappában találhatók.  
A szoftver dokumentációja és a fejlesztés során készült diagramok, ábrák stb. amit felhasználtunk a fejlesztés és dokumentáció során megtalálható a [docs/](./docs/) mappában.

## Alkalmazás futtatása, röviden

Az alkalmazás 4 fő részből áll:

- Api (backend)
- Web (frontend)
- FakePay (fizetéshez egy demo, példa)
- Desktop

A Visual Studionál ezért szükségeltetik a "Multiple startup projects" beállítása, hogy mindegyik szolgáltatás elinduljon.  
A frontendet (SkiRent.Web) tartalmát vagy a mappát magát pedig a XAMPP-on belül a htdocs/ mappában kell elhelyezni.

## Tesztek

Egy probléma miatt az integrációs tesztek hibát jeleznek, ha az *Images* mappa nem létezik.

A hiba megoldható az alkalmazás egyszeri futtatásával, vagy az alábbi PowerShell-paranccsal:

```powershell
mkdir "$env:TEMP\Images"
```

## DbContext Scaffold (nem szükséges)

Az alábbi parancsokkal az adatbázis séma alapján újragenerálhatók az Entity Framework Core modellek és a DbContext.

```sh
Scaffold-DbContext "Server=localhost;User=root;Password=;Database=SkiRent;Port=3306" "Pomelo.EntityFrameworkCore.MySql" -ContextDir Data -OutputDir Data/Models -DataAnnotations -NoOnconfiguring
```

vagy

```sh
dotnet ef dbcontext scaffold "Server=localhost;User=root;Password=;Database=SkiRent;Port=3306" "Pomelo.EntityFrameworkCore.MySql" --context-dir Data --output-dir Data/Models --data-annotations --no-onconfiguring --project .\src\SkiRent.Api\
```
