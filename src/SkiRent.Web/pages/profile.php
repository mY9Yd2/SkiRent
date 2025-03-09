<!DOCTYPE html>
<html lang="hu">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Profilom - SkiRent</title>
    <link rel="stylesheet" href="../css/bootstrap.min.css">
    <link rel="stylesheet" href="../css/style.css">
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
                    <li class="nav-item"><a class="nav-link menu-item text-warning active" href="profile.php">Profilom</a></li>
                    <li class="nav-item"><a class="nav-link menu-item" href="rentals.php">Foglalásaim</a></li>
                    <li class="nav-item"><a class="nav-link text-danger menu-item" href="logout.php" id="logout-link">Kijelentkezés</a></li>
                </ul>
            </div>
        </div>
    </nav>

    <!-- Profilkártya -->
    <div class="container mt-5 d-flex justify-content-center">
        <div class="card bg-light text-dark shadow-lg p-4 profile-card">
            <!-- Profilkép középen -->
            <div class="profile-picture-container">
                <img src="../assets/pictures/profile-default.png" alt="Profilkép" id="profile-picture">
            </div>

            <h3 class="fw-bold text-warning mt-3" id="user-name">Betöltés...</h3>
            <p class="text-muted" id="user-email">Betöltés...</p>
            <hr>
            <h5 class="fw-bold">Felhasználói adatok</h5>
            <p><strong>Név:</strong> <span id="display-name">Betöltés...</span></p>
            <p><strong>E-mail:</strong> <span id="display-email">Betöltés...</span></p>
        </div>
    </div>

    <!-- Bootstrap JS és egyéni script -->
    <script src="../js/bootstrap.bundle.min.js"></script>
    <script src="../js/script.js"></script>
    <script>
        // Ellenőrizzük, hogy a felhasználó be van-e jelentkezve
        document.addEventListener("DOMContentLoaded", function () {
            if (!sessionStorage.getItem("accessToken")) {
                window.location.href = "login.php"; // Ha nincs token, visszairányítás
            }

            // Dinamikus adatbetöltés (később API-ból)
            document.getElementById("user-name").textContent = "Teszt Felhasználó";
            document.getElementById("user-email").textContent = "teszt@example.com";
        });
    </script>
</body>
</html>
