<!DOCTYPE html>
<html lang="hu">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Eszközök - SkiRent</title>
    <link rel="stylesheet" href="../css/bootstrap.min.css">
    <link rel="stylesheet" href="../css/style.css">
</head>

<body>
    <!-- Felső menü -->
    <nav class="navbar navbar-expand-lg navbar-dark bg-dark">
        <div class="container">
            <a class="navbar-brand text-warning fw-bold fs-3" href="../index.php" id="brand-logo">SkiRent</a>
            <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
                <span class="navbar-toggler-icon"></span>
            </button>
            <div class="collapse navbar-collapse" id="navbarNav">
                <ul class="navbar-nav ms-auto">
                    <li class="nav-item"><a class="nav-link text-light menu-item" href="../index.php">Kezdőlap</a></li>
                    <li class="nav-item"><a class="nav-link text-light menu-item" href="products.php">Eszközök</a></li>
                    <li class="nav-item"><a class="nav-link text-light menu-item" href="login.php?redirect=profile.php">Profilom</a></li>
                    <li class="nav-item"><a class="nav-link text-light menu-item" href="login.php?redirect=rentals.php">Foglalásaim</a></li>
                    <li class="nav-item"><a class="nav-link text-light menu-item" href="login.php">Bejelentkezés</a></li>
                </ul>
            </div>
        </div>
    </nav>

    <!-- Tartalom -->
    <div class="container mt-5" id="products-container">
        <h1 id="welcome-message" class="fw-bold text-warning text-center">Bérelhető Eszközök</h1>
        <p id="description-text" class="fs-4 text-center">Válogass a különböző sífelszerelések közül!</p>
        
        <!-- Itt jelennek meg az eszközök listázva -->
        <div class="row g-4" id="product-list">
            <p class="text-muted text-center" id="loading-text">Hamarosan feltöltésre kerülnek...</p>
        </div>
    </div>

    <script src="../js/bootstrap.bundle.min.js"></script>
    <script src="../js/script.js"></script>

</body>
</html>
