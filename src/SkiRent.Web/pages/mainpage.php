<!DOCTYPE html>
<html lang="hu">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>SkiRent - Főoldal</title>

    <!-- Bootstrap CSS -->
    <link rel="stylesheet" href="../css/bootstrap.min.css">

    <!-- Egyéni CSS -->
    <link rel="stylesheet" href="../style/style.css">
    <link rel="stylesheet" href="../style/mainpage.css">
</head>

<body>

    <!-- Navbar -->
    <nav class="navbar navbar-expand-lg navbar-dark px-4">
        <a class="navbar-brand fw-bold text-warning" href="#">SkiRent</a>
        <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarContent"
            aria-controls="navbarContent" aria-expanded="false" aria-label="Menü">
            <span class="navbar-toggler-icon"></span>
        </button>

        <div class="collapse navbar-collapse justify-content-end" id="navbarContent">
            <ul class="navbar-nav" id="navbar-menu">
                <!-- Dinamikus menüpontok JS-ből -->
            </ul>
        </div>
    </nav>

    <!-- Hero szekció -->
    <section class="hero-section">
        <div class="overlay"></div>
        <div class="hero-content">
            <h1>Üdvözlünk a SkiRent világában!</h1>
            <p>Fedezd fel a legjobb sífelszereléseket, és indulj a kalandra!</p>
            <a href="products.php" class="btn btn-warning btn-lg mt-4">Eszközök böngészése</a>
        </div>
    </section>

    <!-- Így működik szekció -->
    <section class="how-it-works py-5">
        <div class="container">
            <h2 class="text-center mb-5 text-dark">Így működik</h2>
            <div class="row text-center">
                <div class="col-md-4 mb-4">
                    <i class="bi bi-search how-it-works-icon mb-3"></i>
                    <h4 class="text-warning">1. Böngészd</h4>
                    <p>Válaszd ki a számodra megfelelő sífelszerelést kínálatunkból egyszerűen.</p>
                </div>
                <div class="col-md-4 mb-4">
                    <i class="bi bi-cart-check how-it-works-icon mb-3"></i>
                    <h4 class="text-warning">2. Bérelj</h4>
                    <p>Foglalj online gyorsan és kényelmesen, pár kattintással.</p>
                </div>
                <div class="col-md-4 mb-4">
                    <i class="bi bi-emoji-sunglasses how-it-works-icon mb-3"></i>
                    <h4 class="text-warning">3. Élvezd!</h4>
                    <p>Vedd át felszerelésed, és élvezd a felejthetetlen havas kalandot!</p>
                </div>
            </div>
        </div>
    </section>



    <!-- Vélemények szekció -->
    <section class="testimonials-section py-5">
        <div class="container">
            <h2 class="text-center mb-5 text-light">Vásárlóink véleményei</h2>
            <div class="row">
                <!-- 1. vélemény -->
                <div class="col-md-4 mb-4">
                    <div class="testimonial card p-4 h-100">
                        <h5 class="text-warning">Kovács Anna</h5>
                        <div class="stars mb-2">
                            <i class="bi bi-star-fill text-warning"></i>
                            <i class="bi bi-star-fill text-warning"></i>
                            <i class="bi bi-star-fill text-warning"></i>
                            <i class="bi bi-star-fill text-warning"></i>
                            <i class="bi bi-star text-warning"></i>
                        </div>
                        <p>"Gyors, egyszerű bérlés, kiváló minőségű felszerelések! Csak ajánlani tudom."</p>
                    </div>
                </div>
                <!-- 2. vélemény -->
                <div class="col-md-4 mb-4">
                    <div class="testimonial card p-4 h-100">
                        <h5 class="text-warning">Nagy Péter</h5>
                        <div class="stars mb-2">
                            <i class="bi bi-star-fill text-warning"></i>
                            <i class="bi bi-star-fill text-warning"></i>
                            <i class="bi bi-star-fill text-warning"></i>
                            <i class="bi bi-star-fill text-warning"></i>
                            <i class="bi bi-star-fill text-warning"></i>
                        </div>
                        <p>"Nagyon profi csapat, segítőkészek voltak mindenben. A foglalás is pofonegyszerű."</p>
                    </div>
                </div>
                <!-- 3. vélemény -->
                <div class="col-md-4 mb-4">
                    <div class="testimonial card p-4 h-100">
                        <h5 class="text-warning">Tóth Eszter</h5>
                        <div class="stars mb-2">
                            <i class="bi bi-star-fill text-warning"></i>
                            <i class="bi bi-star-fill text-warning"></i>
                            <i class="bi bi-star-fill text-warning"></i>
                            <i class="bi bi-star-half text-warning"></i>
                            <i class="bi bi-star text-warning"></i>
                        </div>
                        <p>"Nagyon elégedett voltam, bár a választék lehetne kicsit bővebb. De amit béreltem, az kifogástalan volt."</p>
                    </div>
                </div>
            </div>
        </div>
    </section>


    <!-- Bootstrap JS -->
    <script src="../js/jquery.slim.min.js"></script>
    <link rel="stylesheet" href="../font/bootstrap-icons.min.css">
    <script src="../js/bootstrap.bundle.min.js"></script>

    <!-- Saját JS -->
    <script src="../js/script.js"></script>

</body>
</html>
