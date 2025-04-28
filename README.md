# SkiRent vizsgaremek projekt

## Projektkövetelmények, csapaton belüli munkamegosztás és kezdeti projekt terv

A vizsgaremeket az utolsó tanévben kellett elkészíteni, minimum 2, maximum 3 fős csapatokban. Eredetileg egy Kahoot szerű alkalmazást terveztünk 3 fős csapatban, viszont sajnos váratlan csapattag változások történtek kétszer is, így azt a projektet félretettük és Februárban kezdtünk hozzá a SkiRent (síkölcsönző) projekthez, aminek a leadási határideje április 27 éjfél volt (~3 hónap).

A Képzési és kimeneti követelmények (KKK) szerint a projektet 3 részre kellett bontani: frontend, backend és egy asztali alkalmazás.
Két csapattaggal indult el a fejlesztés és tervezés, az alábbi módon felosztva:

- Frontend: Varga Gábor
- Backend: Kovács József Miklós
- Desktop: Kovács József Miklós

Alkalmanként történtek módosítások a saját területeinken kívül is, ha azt a csapattag szükségesnek ítélte.

## Adatbázisfájlok és dokumentációk elérhetősége

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
