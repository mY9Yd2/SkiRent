<!DOCTYPE html>
<html lang="hu">

<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Foglalás véglegesítése - SkiRent</title>
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css">
    <link rel="stylesheet" href="../style/style.css">
    <link rel="stylesheet" href="../style/checkout.css">
    <link rel="icon" type="image/png" href="/favicon.ico">
</head>

<body>

    <!-- Fejléc logóval -->
    <nav class="navbar navbar-dark bg-dark px-4 mb-4">
        <span class="navbar-brand fw-bold text-warning fs-2">🎿 SkiRent - Foglalás</span>
    </nav>

    <div class="container">
        <h3 class="text-center mb-4">Add meg a személyes adataid a foglaláshoz</h3>

        <!-- Foglalási űrlap -->
        <form id="checkout-form">
            <div class="row">
                <div class="form-group col-md-6">
                    <label for="FullName">Teljes név *</label>
                    <input type="text" class="form-control" id="FullName" placeholder="Pl.: Teszt Elek">
                </div>
                <div class="form-group col-md-6">
                    <label for="PhoneNumber">Telefonszám *</label>
                    <input type="tel" class="form-control" id="PhoneNumber" placeholder="+36 62 123 456">
                </div>
            </div>

            <div class="row">
                <div class="form-group col-md-6">
                    <label for="MobilePhoneNumber">Mobil szám *</label>
                    <input type="tel" class="form-control" id="MobilePhoneNumber" placeholder="+36 20 123 4567">
                </div>
                <div class="form-group col-md-6">
                    <label for="country">Ország *</label>
                    <input type="text" id="country" class="form-control" placeholder="Magyarország">
                </div>
            </div>

            <div class="row">
                <div class="form-group col-md-4">
                    <label for="postalCode">Irányítószám *</label>
                    <input type="text" class="form-control" id="postalCode" placeholder="Pl.: 1137">
                </div>
                <div class="form-group col-md-4">
                    <label for="city">Város *</label>
                    <input type="text" class="form-control" id="city" placeholder="Pl.: Budapest">
                </div>
                <div class="form-group col-md-4">
                    <label for="streetAddress">Cím *</label>
                    <input type="text" class="form-control" id="streetAddress" placeholder="Pl.: Fő utca 1.">
                </div>
            </div>

            <div class="row">
                <div class="form-group col-md-6">
                    <label for="startDate">Kölcsönzés kezdete</label>
                    <input type="text" class="form-control" id="startDate" readonly>
                </div>
                <div class="form-group col-md-6">
                    <label for="endDate">Kölcsönzés vége</label>
                    <input type="text" class="form-control" id="endDate" readonly>
                </div>
            </div>

            <div class="text-center mt-4">
                <button type="submit" class="btn btn-success btn-lg px-5" disabled>
                    Foglalás véglegesítése: Fizetés!
                </button>
            </div>
        </form>

    </div>

    <!-- Bootstrap & JS -->
    <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@4.5.2/dist/js/bootstrap.bundle.min.js"></script>

    <script src="../js/script.js"></script>
    <script src="../js/checkout.js"></script>


</body>

</html>

