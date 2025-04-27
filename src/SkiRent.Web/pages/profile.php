<!DOCTYPE html>
<html lang="hu">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Profilom - SkiRent</title>

    <!-- Bootstrap CSS -->
    <link href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" rel="stylesheet">
    <!-- Egyéni stílus -->
    <link rel="stylesheet" href="../style/style.css">
    <link rel="stylesheet" href="../style/profile.css">
</head>
<body>

    <!-- Navigációs sáv -->
    <nav class="navbar navbar-expand-lg navbar-dark bg-dark px-4" id="navbar">
        <a class="navbar-brand fw-bold text-warning" href="#">SkiRent</a>
        <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarContent"
                aria-controls="navbarContent" aria-expanded="false" aria-label="Menü váltása">
            <span class="navbar-toggler-icon"></span>
        </button>

        <div class="collapse navbar-collapse justify-content-end" id="navbarContent">
            <ul class="navbar-nav" id="navbar-menu">
                <!-- Dinamikus menüpontok JS-ből töltődnek be -->
            </ul>
        </div>
    </nav>

    <!-- Háttérszekció (Parallax) -->
    <div class="profile-hero-section d-flex align-items-center justify-content-center">
        <div id="profile-content" class="text-center">
            <!-- Ide kerülnek a profil adatok JS-ből -->
        </div>
    </div>

    <!-- Bootstrap JS + jQuery -->
    <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.5.2/dist/umd/popper.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>

    <!-- Saját JS - Main JS -->
    <script src="../js/script.js"></script>
    <!-- Saját JS - Login JS -->
    <script src="../js/login.js"></script>
    <!-- Saját JS - Profile JS -->
    <script src="../js/profile.js"></script>
</body>
</html>

