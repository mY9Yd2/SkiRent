# SkiRent vizsgaremek projekt

## Projektkövetelmények, csapaton belüli munkamegosztás és kezdeti projekt terv

A vizsgaremeket az utolsó tanévben kellett elkészíteni, minimum 2, maximum 3 fős csapatokban. Eredetileg egy Kahoot szerű alkalmazást terveztünk 3 fős csapatban, viszont sajnos váratlan csapattag változások történtek kétszer is, így azt a projektet félretettük és Februárban kezdtünk hozzá a SkiRent (síkölcsönző) projekthez, aminek a leadási határideje április 27 éjfél volt (~3 hónap).

A Képzési és kimeneti követelmények (KKK) szerint a projektet 3 részre kellett bontani: frontend, backend és egy asztali alkalmazás.
Két csapattaggal indult el a fejlesztés és tervezés, az alábbi módon felosztva:

- Frontend: Varga Gábor
- Backend: Kovács József Miklós
- Desktop: Kovács József Miklós

Ritkán történtek módosítások a saját területeinken kívül is, ha azt a csapattag szükségesnek ítélte.

## Mappákról röviden

src/ mappa tartalma:

- SkiRent.Api: Backend REST API
- SkiRent.Database: Az adatbázishoz tartozó SQL fájlok
- SkiRent.Desktop: Az asztali alkalmazás
- SkiRent.FakePay: Egy demo fizetési felület. Stripe, Simplepay, Barion stb. helyett
- SkiRent.Shared: Class library. DTOk, validátorok stb., amelyek több projekt között megosztásra került
- SkiRent.Web: Az alkalmazás frontend része

tests/ mappa tartalma:

Integrációs és unit teszteket tartalmaz a backend és a desktophoz.  
Emellett, az egyik tanárunk által adott tesztelési jegyzőkönyv mintát felhasználva és a követelményeknek eleget téve, néhány manuális tesztjegyzőkönyv is megtalálható benne.

docs/ mappa tartalma:

Minden egyéb dolog, ami a KKK megkövetelt: diagram, wireframe, dokumentáció, telepítő, képek amik a dokumentumokban felhasználásra került stb.

## Adatbázisfájlok és dokumentációk elérhetősége

Az adatbázishoz szükséges SQL-fájlok a [src/SkiRent.Database](./src/SkiRent.Database/) mappában találhatók.  
A szoftver dokumentációja és a fejlesztés során készült diagramok, ábrák stb. amit felhasználtunk a fejlesztés és dokumentáció során megtalálható a [docs/](./docs/) mappában.

## Alkalmazás futtatása

Az alkalmazás 4 fő részből áll:

- Api (backend)
- Web (frontend)
- FakePay (fizetéshez egy demo, példa)
- Desktop

A Visual Studionál ezért szükségeltetik a "Multiple startup projects" beállítása, hogy mindegyik szolgáltatás elinduljon.  
A frontendet (SkiRent.Web) mappástól a XAMPP-on belül a htdocs/ mappában kell elhelyezni.

A termék képeket a [src/SkiRent.Web/assets/pictures/Images](./src/SkiRent.Web/assets/pictures/Images/) mappában található. Ezeket az *Images* mappába kell elhelyezni.
Ha az API-nál nincs beállítva hova tegye a fájlokat (DataDirectoryPath), akkor alapértelmezetten a TEMP mappában fog létrehozni egy *Images* mappát.

Pontokba szedve a lépések:

1) Repository klónozása vagy letöltése
2) 'data' mappa létrehozása a SkiRent-main/ mappán belül
3) src\SkiRent.Web\assets\pictures\Images mappát átmásolni a data/ mappába (15 darab JPG képpel együtt)
4) src\SkiRent.Web-et átmásolni a htdocs mappába (SkiRent.Web mappástól)
5) XAMPP elindítása: Apache, MySQL
6) src\SkiRent.Database mappában található schema.sql és seed.sql lefuttatása/importálása, ebben a sorrendben
7) SkiRent.sln megnyitása Visual Studioval
8) SkiRent.Api/appsettings.json-ban a 'DataDirectoryPath' módosítása pl.:

```json
"DataDirectoryPath": "C:\\Users\\vboxuser\\Downloads\\SkiRent-main\\data"
```

10) Configura Startup Projects... -> Multiple startup projects: Api, Desktop, FakePay-et Start-ra rakni. Apply és OK
11) Böngészőben: localhost/SkiRent.Web/

Összesen ennyi lépést igényel a projekt elindítása.

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
