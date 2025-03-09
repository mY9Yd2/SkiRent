<!DOCTYPE html>
<html lang="hu">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>SkiRent - Kezdőlap</title>
    <link rel="stylesheet" href="css/bootstrap.min.css">
    <link rel="stylesheet" href="css/style.css">
</head>

<body>
    <!-- Felső menü -->
    <nav class="navbar navbar-expand-lg navbar-dark bg-dark shadow">
        <div class="container">
            <a class="navbar-brand text-warning fw-bold fs-3" href="index.php" id="brand-logo">SkiRent</a>
            <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
                <span class="navbar-toggler-icon"></span>
            </button>
            <div class="collapse navbar-collapse" id="navbarNav">
                <ul class="navbar-nav ms-auto">
                    <li class="nav-item"><a class="nav-link text-light menu-item" href="pages/products.php" id="equipment-link">Eszközök</a></li>
                    <li class="nav-item"><a class="nav-link text-light menu-item" href="pages/login.php" id="login-link">Bejelentkezés</a></li>
                </ul>
            </div>
        </div>
    </nav>

    <!-- ÚJ REGISZTRÁCIÓS LINK -->
    <div class="container mt-5 text-left" id="index-container">
        <h1 id="welcome-message" class="fw-bold">Üdvözlünk a SkiRent weboldalon!</h1>
        <p id="description-text" class="fs-4">Bérelj sífelszerelést gyorsan és egyszerűen.</p>
        <p class="mt-3">
            <a href="pages/register.php" class="text-primary text-decoration-none">Nincs még fiókod? Regisztrálj!</a>
        </p>
    </div>

    <script src="js/bootstrap.bundle.min.js"></script>
    <script src="js/auth-handler.js"></script> <!-- Új fájl az autentikációhoz -->
    
</body>
</html>