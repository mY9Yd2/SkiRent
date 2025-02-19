<!DOCTYPE html>
<html lang="hu">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Bejelentkezés</title>
    <link rel="stylesheet" href="../css/bootstrap.min.css">
    <link rel="stylesheet" href="../css/style.css">
</head>

<body>
<div class="container d-flex flex-column justify-content-center align-items-center vh-100">
        <!-- SkiRent Márkanév -->
        <h1 class="text-warning fw-bold mb-4" id="login-brand">SkiRent</h1>

        <div class="card p-4 shadow-lg login-card">
            <h3 class="text-center mb-3">Bejelentkezés</h3>
            <form id="login-form">
                <div class="mb-3">
                    <label for="email" class="form-label">E-mail cím</label>
                    <input type="email" class="form-control" id="email" required>
                </div>
                <div class="mb-3">
                    <label for="password" class="form-label">Jelszó</label>
                    <input type="password" class="form-control" id="password" required>
                </div>
                <button type="submit" class="btn btn-warning w-100">Bejelentkezés</button>
            </form>
            <div class="text-center mt-3">
                <small>Még nincs fiókod? <a href="register.php" class="text-warning fw-bold">Regisztrálj itt</a></small>
            </div>
            <div class="text-center mt-2">
                <a href="../index.php"  class="btn btn-outline-light btn-sm">Vissza a főoldalra</a>
            </div>
        </div>
    </div>


    <!-- Bootstrap JS -->
    <script src="../js/bootstrap.bundle.min.js"></script>
    <!-- script.js betöltése -->
    <script src="../js/script.js"></script>
</body>

</html>