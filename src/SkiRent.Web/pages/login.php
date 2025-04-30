<!DOCTYPE html>
<html lang="hu">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Bejelentkezés - SkiRent</title>
    <!-- Bootstrap CSS -->
    <link rel="stylesheet" href="../css/bootstrap.min.css">
    <link rel="stylesheet" href="../font/bootstrap-icons.min.css">
    <!-- Egyéni CSS -->
    <link rel="stylesheet" href="../style/style.css">
    <link rel="stylesheet" href="../style/login.css">
</head>

<body class="login-body">

    <!-- Parallax háttér -->
    <div class="parallax">
        <div class="container d-flex flex-column justify-content-center align-items-center vh-100" id="login-container">

            <div class="card p-4 shadow-lg login-card" id="login-card">
            <!-- SkiRent márkanév -->
            <h1 class="text-warning fw-bold mb-4 text-center" id="brand-logo">SkiRent</h1>

                <h3 class="text-center mb-3" id="login-title">Bejelentkezés</h3>

                <div id="alert-placeholder" class="w-100"></div>
                <!-- Bejelentkezési űrlap -->
                <form id="login-form">
                    <div class="mb-3">
                        <label for="email" class="form-label" id="email-label">E-mail cím</label>
                        <input type="text" class="form-control" id="email">
                        <div class="invalid-feedback">Hibás e-mail vagy nem létezik.</div>
                    </div>
                    <div class="mb-3">
                        <label for="password" class="form-label" id="password-label">Jelszó</label>
                        <input type="password" class="form-control" id="password">
                        <div class="invalid-feedback">Hibás jelszó.</div>
                    </div>
                    <button type="submit" class="btn btn-warning w-100" id="login-submit">Bejelentkezés</button>
                </form>

                <div class="text-center mt-3" id="register-link-container">
                    <small>Még nincs fiókod?
                        <a href="reg.php" class="auth-link" id="register-link">Regisztrálj itt</a>
                    </small>
                </div>

                <div class="text-center mt-2" id="back-home-button">
                    <a href="../index.php" class="btn btn-warning btn-sm">Vissza a kezdőoldalra</a>
                </div>
            </div>
        </div>
    </div>

    <!-- Bootstrap JS + jQuery -->
    <script src="../js/jquery.slim.min.js"></script>
    <script src="../js/bootstrap.bundle.min.js"></script>

    <!-- Saját JS - Main JS -->
    <script src="../js/script.js"></script>

    <!-- Saját JS -->
    <script src="../js/login.js"></script>


    <!-- Bootstrap Modal sikeres/sikertelen bejelentkezéshez -->
    <div class="modal fade" id="feedbackModal" tabindex="-1" aria-labelledby="feedbackModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content text-center">
            <div class="modal-header">
                <h5 class="modal-title" id="feedbackModalLabel">Üzenet</h5>
            </div>
            <div class="modal-body" id="feedbackModalBody">
                <!-- Ide jön az üzenet dinamikusan -->
            </div>
            <div class="modal-footer justify-content-center">
                <button type="button" class="btn btn-warning" data-bs-dismiss="modal">OK</button>
            </div>
            </div>
        </div>
    </div>

</body>
</html>
