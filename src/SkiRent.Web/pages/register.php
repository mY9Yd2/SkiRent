<?php
session_start();
?>

<!DOCTYPE html>
<html lang="hu">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Regisztráció</title>
    <link rel="stylesheet" href="../css/bootstrap.min.css">
    <link rel="stylesheet" href="../css/style.css">
</head>

<body class="register-body">
    <div class="container d-flex flex-column justify-content-center align-items-center vh-100">
        <!-- SkiRent Márkanév -->
        <h1 class="text-warning fw-bold mb-4" id="brand-logo">SkiRent</h1>

        <div class="card p-4 shadow-lg register-card">
            <h3 class="text-center mb-3">Regisztráció</h3>
            <form id="register-form">
                <div class="mb-3">
                    <label for="email" class="form-label">E-mail cím</label>
                    <input type="email" class="form-control" id="email">
                </div>
                <div class="mb-3">
                    <label for="password" class="form-label">Jelszó</label>
                    <input type="password" class="form-control" id="password">
                </div>
                <div class="mb-3">
                    <label for="confirm-password" class="form-label">Jelszó ismét</label>
                    <input type="password" class="form-control" id="confirm-password">
                    <small id="password-error" class="error-text"></small>
                </div>
                <button type="submit" class="btn btn-warning w-100">Regisztráció</button>
            </form>

            <div class="text-center mt-3">
                <p class="login-link">Már van fiókod? <a href="login.php" class="auth-link">Jelentkezz be!</a></p>
            </div>
            <div class="text-center mt-2">
                <a href="../index.php" class="btn btn-warning btn-sm">Vissza a főoldalra</a>
            </div>
        </div>
    </div>

    <script src="../js/bootstrap.bundle.min.js"></script>
    <script src="../js/script.js"></script>
    
</body>
</html>