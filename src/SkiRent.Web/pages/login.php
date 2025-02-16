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
        <div class="card p-4 shadow-lg" id="login-card" style="width: 350px;">

            <h3 class="text-center mb-3" id="login-title">Bejelentkezés</h3>

            <form action="../api/auth.php" method="POST" id="login-form">
                <div class="mb-3">
                    <label for="email" class="form-label">E-mail cím</label>
                    <input type="email" class="form-control" id="email" name="email" required>
                </div>

                <div class="mb-3">
                    <label for="password" class="form-label">Jelszó</label>
                    <input type="password" class="form-control" id="password" name="password" required>
                </div>

                <div class="form-check mb-3">
                    <input type="checkbox" class="form-check-input" id="remember" name="remember">
                    <label class="form-check-label" for="remember">Maradjak bejelentkezve</label>
                </div>

                <button type="submit" class="btn btn-primary w-100" id="login-button">Bejelentkezés</button>
            </form>

            <div class="text-center mt-3" id="register-link">
                <small>Még nincs fiókod? <a href="register.php" id="register-link-a">Regisztráció</a></small>
            </div>

        </div>
    </div>


    
    <script src="../js/bootstrap.bundle.min.js"></script>

</body>

</html>