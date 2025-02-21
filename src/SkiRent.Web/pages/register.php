<!DOCTYPE html>
<html lang="hu">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Regisztráció - SkiRent</title>
    <link rel="stylesheet" href="../css/bootstrap.min.css">
    <link rel="stylesheet" href="../css/style.css">
</head>

<body>    
    <div class="container d-flex flex-column justify-content-center align-items-center vh-100">
        <h1 class="text-warning fw-bold mb-4" id="brand-logo">SkiRent</h1>
        <div class="login-box">
            <h2>Regisztráció</h2>
            <form id="register-form">
                <div class="form-group">
                    <label for="email">E-mail cím</label>
                    <input type="email" id="email" class="form-control" required>
                </div>
                <div class="form-group">
                    <label for="password">Jelszó</label>
                    <input type="password" id="password" class="form-control" required>
                </div>
                <div class="form-group">
                    <label for="confirm-password">Jelszó ismét</label>
                    <input type="password" id="confirm-password" class="form-control" required>
                    <small id="password-error" class="error-text"></small>
                </div>
                <button type="submit" class="btn btn-warning btn-block">Regisztráció</button>
            </form>

            <p class="login-link">Már van fiókod? <a href="login.php">Jelentkezz be!</a></p>
            <a href="../index.php" class="btn btn-secondary btn-block">Vissza a főoldalra</a>
        </div>
    </div>

    <script src="../js/bootstrap.bundle.min.js"></script>
    <script src="../js/script.js"></script>
</body>
</html>