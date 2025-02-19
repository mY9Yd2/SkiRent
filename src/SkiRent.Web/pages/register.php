<!DOCTYPE html>
<html lang="hu">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Regisztráció</title>
    <link rel="stylesheet" href="../css/bootstrap.min.css">
    <link rel="stylesheet" href="../css/style.css">
</head>
<body>
    <div class="container d-flex justify-content-center align-items-center vh-100">
        <div class="card p-4 shadow-lg" style="width: 400px;">
            <h3 class="text-center mb-3">Regisztráció</h3>
            <form id="register-form">
                <div class="mb-3">
                    <label for="email" class="form-label">E-mail cím</label>
                    <input type="email" class="form-control" id="email" required>
                </div>
                <div class="mb-3">
                    <label for="username" class="form-label">Felhasználónév</label>
                    <input type="text" class="form-control" id="username" required>
                </div>
                <div class="mb-3">
                    <label for="password" class="form-label">Jelszó</label>
                    <input type="password" class="form-control" id="password" required>
                </div>
                <div class="mb-3">
                    <label for="confirm-password" class="form-label">Jelszó megerősítése</label>
                    <input type="password" class="form-control" id="confirm-password" required>
                </div>

                <button type="submit" class="btn btn-success w-100">Regisztráció</button>

            </form>

            <div class="text-center mt-3">
                <small>Van már fiókod? <a href="login.php">Jelentkezz be itt</a></small>
            </div>

            <div class="text-center mt-2">
                <a href="../index.php" class="btn btn-secondary btn-sm">Vissza a főoldalra</a>
            </div>

        </div>
    </div>


    <script src="../js/bootstrap.bundle.min.js"></script>

    <!-- script.js betöltése -->
    <script src="../js/script.js"></script>
</body>
</html>
