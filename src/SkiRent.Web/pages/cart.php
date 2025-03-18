<!DOCTYPE html>
<html lang="hu">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Kosár - SkiRent</title>
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
                    <li class="nav-item"><a class="nav-link menu-item" href="rentals.php">Foglalásaim</a></li>
                    <li class="nav-item">
                        <a class="nav-link text-light menu-item position-relative" href="cart.php">
                            <i class="fas fa-shopping-cart"></i> <!-- Kosár ikon -->
                            <span id="cart-count" class="badge bg-danger position-absolute top-0 start-100 translate-middle rounded-pill d-none">0</span>
                        </a>
                    </li>
                    <li class="nav-item"><a class="nav-link text-danger menu-item" href="logout.php">Kijelentkezés</a></li>
                </ul>
            </div>
        </div>
    </nav>

    <!-- Kosár tartalom -->
    <div class="container mt-5">
        <div class="card bg-light text-dark shadow-lg p-4 cart-card">
            <h2 class="fw-bold text-warning">🛒 Kosár</h2>
            <p class="text-muted">Itt láthatod a kosárba helyezett termékeket.</p>

            <!-- Táblázat a kosár tartalmának -->
            <table class="table table-bordered text-center">
                <thead>
                    <tr>
                    <th>Termék</th>
                    <th>Ár</th>
                    <th>Mennyiség</th>
                    <th>Művelet</th>
                    </tr>
                </thead>
                <tbody id="cart-items">
                    <tr>
                        <td colspan="3" class="text-muted">A kosár üres.</td>
                    </tr>
                </tbody>
            </table>

            <!-- Összegzés és műveletek -->
            <div class="d-flex justify-content-between align-items-center mt-3">
                <h4>Összesen: <span class="text-warning fw-bold" id="cart-total">0 Ft</span> <span class="text-muted">/nap</span></h4>
                <button class="btn btn-danger fw-bold" id="clear-cart">Kosár ürítése</button>
            </div>
        </div>
    </div>


    <!-- Tétel törlése megerősítő modal -->
    <div class="modal fade" id="confirmDeleteModal" tabindex="-1" aria-labelledby="confirmDeleteModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content bg-light text-dark">
            <div class="modal-header">
                <h5 class="modal-title" id="confirmDeleteModalLabel">Tétel törlése</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Bezárás"></button>
            </div>
            <div class="modal-body">
                Biztosan eltávolítja ezt a tételt és a hozzátartozó mennyiséget a kosárból?
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Nem</button>
                <button type="button" class="btn btn-danger" id="confirm-delete-btn">Igen</button>
            </div>
            </div>
        </div>
    </div>

    <!-- Kosár ürítése megerősítő modal -->
    <div class="modal fade" id="confirmClearModal" tabindex="-1" aria-labelledby="confirmClearModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content bg-light text-dark">
                <div class="modal-header">
                    <h5 class="modal-title" id="confirmClearModalLabel">Kosár ürítése</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Bezárás"></button>
                </div>
                <div class="modal-body">
                    Biztosan törli a kosárba helyezett összes tételt?
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Nem</button>
                    <button type="button" class="btn btn-danger" id="confirm-clear-btn">Igen</button>
                </div>
            </div>
        </div>
    </div>

    <!-- Bootstrap JS és egyéni script -->
    <script src="../js/bootstrap.bundle.min.js"></script>
    <script src="../js/script.js"></script>

</body>
</html>