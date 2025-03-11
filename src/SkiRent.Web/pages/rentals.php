<!DOCTYPE html>
<html lang="hu">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Foglalásaim - SkiRent</title>
    <link rel="stylesheet" href="../css/bootstrap.min.css">
    <link rel="stylesheet" href="../css/style.css">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css">
</head>

<body class="bg-dark text-light">
    <!-- Felső menü -->
    <nav class="navbar navbar-expand-lg navbar-dark bg-dark shadow">
        <div class="container">
            <a class="navbar-brand text-warning fw-bold fs-3" href="mainpage.php" id="brand-logo">SkiRent</a>
            <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
                <span class="navbar-toggler-icon"></span>
            </button>
            <div class="collapse navbar-collapse" id="navbarNav">
                <ul class="navbar-nav ms-auto">
                    <li class="nav-item"><a class="nav-link menu-item" href="products.php">Eszközök</a></li>
                    <li class="nav-item"><a class="nav-link menu-item" href="profile.php">Profilom</a></li>
                    <li class="nav-item"><a class="nav-link menu-item text-warning active" href="rentals.php">Foglalásaim</a></li>

                    <!-- Kosár ikon beillesztése -->
                    <li class="nav-item">
                        <a class="nav-link text-light menu-item position-relative" href="cart.php">
                            <i class="fas fa-shopping-cart"></i> <!-- Kosár ikon -->
                            <span id="cart-count" class="badge bg-danger position-absolute top-0 start-100 translate-middle rounded-pill d-none">0</span>
                        </a>
                    </li>

                    <li class="nav-item"><a class="nav-link text-danger menu-item" href="logout.php" id="logout-link">Kijelentkezés</a></li>
                </ul>
            </div>
        </div>
    </nav>

    <!-- Tartalom -->
    <div class="container mt-5 text-left bg-light text-dark p-4 rounded shadow">
        <h1 class="fw-bold text-warning">Foglalásaim</h1>
        <p>Itt láthatod a korábbi és aktuális bérléseidet.</p>
        
        <!-- Foglalási adatok (Később API-ból tölthető be) -->
        <div class="mt-4">
            <h3>Jelenlegi foglalásaid</h3>
            <p><strong>Betöltés...</strong></p>
        </div>
    </div>

    <!-- Bootstrap JS és egyéni script -->
    <script src="../js/bootstrap.bundle.min.js"></script>
    <script src="../js/script.js"></script>
    
    <script>
        document.addEventListener("DOMContentLoaded", function () {
            if (!sessionStorage.getItem("accessToken")) {
                window.location.href = "login.php"; // Ha nincs token, visszairányítás
            }

            // Kosár számláló frissítése az oldal betöltésekor
            updateCartCount();
        });
    </script>
</body>
</html>