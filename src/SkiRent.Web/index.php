<!DOCTYPE html>
<html lang="hu">

<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>SkiRent</title>
    <!-- Bootstrap CSS -->
    <link rel="stylesheet" href="css/bootstrap.min.css">
    <!-- Saját stílusok (style.css) -->
    <link rel="stylesheet" href="style/style.css">
    <link rel="stylesheet" href="style/index.css">
    <!-- Fontawesome (ha szükséges ikonos menü) -->
    <link rel="stylesheet" href="css/all.min.css">
</head>

<body>

    <!-- Felső menü -->
    <nav class="navbar navbar-expand-lg navbar-dark px-4" id="navbar">
        <a class="navbar-brand fw-bold text-warning" href="#">SkiRent</a>
        <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav"
            aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
            <span class="navbar-toggler-icon"></span>
        </button>
        <div class="collapse navbar-collapse justify-content-end" id="navbarNav">
            <ul class="navbar-nav" id="navbar-menu">
                <!-- Menüpontokat a script.js tölti be -->
            </ul>
        </div>
    </nav>

    <div class="hero-section">
        <div class="overlay d-flex flex-column justify-content-center align-items-center text-center">
            <h1 class="text-white mb-3">Üdvözlünk a SkiRent weboldalán!</h1>
            <p class="text-light mb-4">Fedezd fel a legújabb sífelszereléseket és foglalj most!</p>
            <a href="pages/products.php" class="btn btn-warning btn-lg">Böngéssz a kínálatban</a>
        </div>
    </div>


    <section class="why-choose-us py-5">
        <div class="container">
            <h2 class="text-center mb-5">Miért válassz minket?</h2>
            <div class="row">
                <div class="col-md-4 text-center mb-4">
                    <i class="fas fa-skiing fa-3x text-warning mb-3"></i>
                    <h4>Kiváló felszerelések</h4>
                    <p>Csak a legjobb minőségű sífelszerelést kínáljuk, hogy élvezhesd a havas kalandokat.</p>
                </div>
                <div class="col-md-4 text-center mb-4">
                    <i class="fas fa-clock fa-3x text-warning mb-3"></i>
                    <h4>Gyors és egyszerű foglalás</h4>
                    <p>Pillanatok alatt lefoglalhatod a szükséges eszközöket online rendszerünkön keresztül.</p>
                </div>
                <div class="col-md-4 text-center mb-4">
                    <i class="fas fa-star fa-3x text-warning mb-3"></i>
                    <h4>Elégedett ügyfelek</h4>
                    <p>Ügyfeleink visszajelzései alapján büszkék vagyunk kiváló szolgáltatásainkra.</p>
                </div>
            </div>
        </div>
    </section>


    <!-- Felhasználói Vélemények -->
    <section class="testimonials-section py-5 bg-dark text-light">
        <div class="container">
            <h2 class="text-center mb-5">Felhasználói vélemények</h2>
            <div class="row">
                <div class="col-md-4 mb-4">
                    <div class="testimonial p-4 rounded custom-shadow bg-secondary">
                        <p>"Nagyszerű szolgáltatás, kiváló minőségű felszerelések. Minden síelésem előtt itt bérelek!"
                        </p>
                        <h5 class="mt-3">- Kovács Péter</h5>
                    </div>
                </div>
                <div class="col-md-4 mb-4">
                    <div class="testimonial p-4 rounded custom-shadow bg-secondary">
                        <p>"Gyors ügyintézés, barátságos csapat. Mindenkinek ajánlom a SkiRent-et!"</p>
                        <h5 class="mt-3">- Nagy Anna</h5>
                    </div>
                </div>
                <div class="col-md-4 mb-4">
                    <div class="testimonial p-4 rounded custom-shadow bg-secondary">
                        <p>"Nagyon meg voltam elégedve! Profi felszerelések, egyszerű bérlési folyamat."</p>
                        <h5 class="mt-3">- Tóth Gábor</h5>
                    </div>
                </div>
            </div>
        </div>
    </section>


    <div class="container" id="main-content">
        <!-- Regisztrációs felhívás -->
        <div class="text-center mt-5" id="register-prompt">
            <p>Nincs még fiókod?
                <a href="pages/reg.php" class="fw-bold text-primary" id="register-home-link">Regisztrálj!</a>
            </p>
        </div>
    </div>
    <!-- jQuery -->
    <script src="js/jquery.slim.min.js"></script>
    <!-- Bootstrap JS -->
    <script src="js/bootstrap.bundle.min.js"></script>

    <!-- Saját JS -->
    <script src="js/script.js"></script>
    <!-- Saját JS - Login JS -->
    <script src="js/login.js"></script>


</body>

</html>
