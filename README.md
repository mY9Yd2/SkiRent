## DbContext Scaffold

```sh
Scaffold-DbContext "Server=localhost;User=root;Password=;Database=SkiRent;Port=3306" "Pomelo.EntityFrameworkCore.MySql" -ContextDir Data -OutputDir Data/Models -DataAnnotations -NoOnconfiguring
```

vagy

```sh
dotnet ef dbcontext scaffold "Server=localhost;User=root;Password=;Database=SkiRent;Port=3306" "Pomelo.EntityFrameworkCore.MySql" --context-dir Data --output-dir Data/Models --data-annotations --no-onconfiguring --project .\src\SkiRent.Api\
```
