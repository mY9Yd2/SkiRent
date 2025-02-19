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
    <div class="container d-flex justify-content-center align-items-center vh-100">
        <div class="card p-4 shadow-lg" style="width: 350px;">
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
                <button type="submit" class="btn btn-primary w-100">Bejelentkezés</button>
            </form>
            <div class="text-center mt-3">
                <small>Még nincs fiókod? <a href="#">Regisztrálj itt</a></small>
            </div>
        </div>
    </div>


    <!-- Bootstrap JS -->
    <script src="../js/bootstrap.bundle.min.js"></script>
    <!-- script.js betöltése -->
    <script src="../js/script.js"></script>
</body>

</html>