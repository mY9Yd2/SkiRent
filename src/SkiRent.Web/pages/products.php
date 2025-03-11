<?php
session_start();
$isLoggedIn = isset($_SESSION['user_id']); // Ellenőrzi, hogy be van-e jelentkezve a felhasználó
?>

<!DOCTYPE html>
<html lang="hu">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Eszközök - SkiRent</title>
    <link rel="stylesheet" href="../css/bootstrap.min.css">
    <link rel="stylesheet" href="../css/style.css">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css">
</head>

<body>
    <!-- Felső menü -->
    <nav class="navbar navbar-expand-lg navbar-dark bg-dark">
        <div class="container">
            <a class="navbar-brand text-warning fw-bold fs-3" href="products.php" id="brand-logo">SkiRent</a>
            <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
                <span class="navbar-toggler-icon"></span>
            </button>
            <div class="collapse navbar-collapse" id="navbarNav">
                <ul class="navbar-nav ms-auto">
                    <li class="nav-item"><a class="nav-link menu-item" href="products.php">Eszközök</a></li>
                    <?php if (!$isLoggedIn): ?>
                        <li class="nav-item"><a class="nav-link menu-item" href="../index.php">Vissza a kezdőoldalra</a></li>
                        <li class="nav-item"><a class="nav-link menu-item" href="login.php">Bejelentkezés</a></li>
                    <?php else: ?>
                        <li class="nav-item"><a class="nav-link menu-item" href="profile.php">Profilom</a></li>
                        <li class="nav-item"><a class="nav-link menu-item" href="rentals.php">Foglalásaim</a></li>
                        
                        <!-- Kosár ikon beillesztése -->
                        <li class="nav-item">
                            <a class="nav-link text-light menu-item position-relative" href="cart.php">
                                <i class="fas fa-shopping-cart"></i> <!-- Kosár ikon -->
                                <span id="cart-count" class="badge bg-danger position-absolute top-0 start-100 translate-middle rounded-pill d-none">0</span>
                            </a>
                        </li>

                        <li class="nav-item"><a class="nav-link text-danger menu-item" href="logout.php">Kijelentkezés</a></li>
                    <?php endif; ?>
                </ul>
            </div>
        </div>
    </nav>

    <!-- Tartalom -->
    <div class="container mt-5" id="products-container">
        <h1 id="welcome-message" class="text-warning">Bérelhető Eszközök</h1>
        <p class="text">Válogass a különböző sífelszerelések közül!</p>

        <!-- Itt jelennek meg majd az eszközök listázva -->
        <div class="row" id="product-list">
            <p class="text-muted">Hamarosan feltöltésre kerülnek...</p>
        </div>
    </div>

    <script src="../js/bootstrap.bundle.min.js"></script>
    <script src="../js/script.js"></script>

    <script>
        document.addEventListener("DOMContentLoaded", function () {
            const isLoggedIn = <?php echo json_encode($isLoggedIn); ?>;
            if (!isLoggedIn) {
                document.querySelectorAll(".quantity-info").forEach(el => el.style.display = "none");
            }
        });
    </script>

</body>

</html>