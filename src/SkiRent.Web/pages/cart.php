<!DOCTYPE html>
<html lang="hu">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Kosár - SkiRent</title>

    <!-- Bootstrap CSS -->
    <link rel="stylesheet" href="../css/bootstrap.min.css">
    <link rel="stylesheet" href="../style/style.css">
    <link rel="stylesheet" href="../style/cart.css">
</head>
<body>

    <!-- Fejléc logóval (opcionális) -->
    <nav class="navbar navbar-dark bg-dark px-4 mb-4">
        <span class="navbar-brand fw-bold text-warning fs-3">🛒 Kosár</span>
    </nav>

    <!-- Tartalom -->
    <div class="container mb-5">
        <h2 class="text-center mb-4">Kosár tartalma</h2>

        <!-- Táblázat a kosár elemeinek -->
        <div class="table-responsive">
            <table class="table table-bordered table-hover" id="cart-table">
                <thead>
                    <tr class="table-dark">
                        <th>#</th>
                        <th>Termék</th>
                        <th>Mennyiség</th>
                        <th>Ár / nap</th>
                        <th>Összeg</th>
                        <th>Műveletek</th>
                    </tr>
                </thead>
                <tbody>
                    <!-- JavaScript fogja feltölteni -->
                </tbody>
            </table>
        </div>

        <!-- Vissza a termékekhez gomb -->
        <div class="text-center">
            <a href="products.php" class="btn btn-outline-primary mt-3">Vásárlás folytatása</a>
        </div>


        <!-- Foglalási dátumok és gomb -->
        <div class="container mt-5">
            <h4 class="mb-3">Foglalási időszak kiválasztása</h4>
            <form autocomplete="off">
                <div class="row">
                    <div class="mb-3 col-md-6">
                        <label for="start-date">Kezdő dátum</label>
                        <input type="date" class="form-control" id="start-date" name="start-date">
                    </div>
                    <div class="mb-3 col-md-6">
                        <label for="end-date">Záró dátum</label>
                        <input type="date" class="form-control" id="end-date" name="end-date">
                    </div>
                </div>
                <button id="checkout-btn" class="btn btn-success mt-3" type="button" disabled>Foglalás és fizetés</button>
            </form>
        </div>


    </div>

    <!-- Bootstrap JS -->
    <script src="../js/jquery.slim.min.js"></script>
    <script src="../js/bootstrap.bundle.min.js"></script>
    <script src="../js/script.js"></script>
</body>
</html>
