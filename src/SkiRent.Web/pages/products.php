<!DOCTYPE html>
<html lang="hu">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Eszközök - SkiRent</title>

    <!-- Bootstrap CSS -->
    <link href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" rel="stylesheet">

    <!-- Egyéni stílus -->
    <link rel="stylesheet" href="../style/style.css">
    <link rel="stylesheet" href="../style/products.css">
</head>
<body>

    <!-- Navigációs sáv -->
    <nav class="navbar navbar-expand-lg navbar-dark bg-dark px-4" id="navbar">
        <a class="navbar-brand fw-bold text-warning" href="#">SkiRent</a>
        <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarNav"
                aria-controls="navbarNav" aria-expanded="false" aria-label="Menü váltása">
            <span class="navbar-toggler-icon"></span>
        </button>
    <div class="collapse navbar-collapse justify-content-end" id="navbarNav">
        <ul class="navbar-nav" id="navbar-menu">
                <!-- Menüpontok JS-ből töltődnek be -->
            </ul>
        </div>
    </nav>

    <!-- Tartalom -->
    <!-- Fő parallax szekció -->
    <section class="product-hero-section">
        <div class="container mt-5">
            <h2 class="text-center text-warning">Elérhető sífelszerelések</h2>
            <p class="text-center text-muted">Jelentkezz be a foglaláshoz!</p>

            <div class="row" id="product-list">
                <!-- Ide jönnek a kártyák -->
            </div>
        </div>
    </section>


    <!-- Kosárba tett termék megerősítés modal -->
    <div class="modal fade" id="cartModal" tabindex="-1" aria-labelledby="cartModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
        <div class="modal-header">
            <h5 class="modal-title" id="cartModalLabel">Kosár frissítve</h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Bezárás"></button>
        </div>
        <div class="modal-body" id="cart-modal-body">
            <!-- Ide jön a dinamikus szöveg -->
        </div>
        </div>
    </div>
    </div>

    <!-- Bootstrap JS + jQuery -->
    <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.5.2/dist/umd/popper.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>

    <!-- Saját JS -->
    <script src="../js/script.js"></script>

    <!-- Saját JS - Main JS -->
    <script src="../js/login.js"></script>


</body>

</html>

