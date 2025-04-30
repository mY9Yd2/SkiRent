<!DOCTYPE html>
<html lang="hu">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Regisztráció - SkiRent</title>
    <!-- Bootstrap CSS -->
    <link rel="stylesheet" href="../css/bootstrap.min.css">
    <!-- Egyéni CSS -->
    <link rel="stylesheet" href="../style/style.css">
    <link rel="stylesheet" href="../style/reg.css">
</head>

<body class="login-body">

    <!-- Parallax háttér -->
    <div class="parallax">
        <div class="container d-flex flex-column justify-content-center align-items-center vh-100" id="register-container">
            <div class="card p-4 shadow-lg register-card" id="register-card">

                <!-- SkiRent logó -->
                <h1 class="text-warning fw-bold mb-4 text-center" id="brand-logo">SkiRent</h1>

                <h3 class="text-center mb-3" id="register-title">Regisztráció</h3>

                <!-- Alert helye -->
                <div id="alert-placeholder"></div>

                <!-- Regisztrációs űrlap -->
                <form id="register-form">
                    <div class="mb-3">
                        <label for="fullName" class="form-label" id="fullName-label">Teljes név</label>
                        <input type="text" class="form-control" id="fullName">
                        <div class="invalid-feedback">Adj meg egy nevet!</div>
                    </div>
                    <div class="mb-3">
                        <label for="email" class="form-label" id="email-label">E-mail cím</label>
                        <input type="email" class="form-control" id="email">
                        <div class="invalid-feedback">Hibás e-mail cím!</div>
                    </div>
                    <div class="mb-3">
                        <label for="password" class="form-label" id="password-label">Jelszó</label>
                        <input type="password" class="form-control" id="password">
                        <div class="invalid-feedback">Adj meg egy jelszót!</div>
                    </div>
                    <div class="mb-3">
                        <label for="confirmPassword" class="form-label" id="confirmPassword-label">Jelszó megerősítése</label>
                        <input type="password" class="form-control" id="confirmPassword">
                        <div class="invalid-feedback">A jelszavak nem egyeznek!</div>
                    </div>
                    <button type="submit" class="btn btn-warning w-100" id="register-submit">Regisztráció</button>
                </form>

                <div class="text-center mt-3" id="login-link-container">
                    <small>Van már fiókod?
                        <a href="login.php" class="auth-link" id="login-link">Jelentkezz be itt</a>
                    </small>
                </div>

                <div class="text-center mt-2" id="back-home-button">
                    <a href="../index.php" class="btn btn-warning btn-sm">Vissza a kezdőoldalra</a>
                </div>
            </div>
        </div>
    </div>

    <!-- Bootstrap JS + jQuery -->
    <script src="../js/jquery.min.js"></script>
    <script src="../js/bootstrap.bundle.min.js"></script>

    <!-- Saját JS -->
    <script src="../js/script.js"></script>
    <script src="../js/reg.js"></script>

</body>
</html>
