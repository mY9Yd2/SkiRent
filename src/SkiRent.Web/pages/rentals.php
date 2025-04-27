<!DOCTYPE html>
<html lang="hu">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Foglalásaim - SkiRent</title>

    <!-- Bootstrap CSS -->
    <link href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" rel="stylesheet">
    <!-- Egyéni stílus -->
    <link rel="stylesheet" href="../style/style.css">
    <link rel="stylesheet" href="../style/rentals.css">
</head>
<body>

   <!-- Navigációs sáv -->
   <nav class="navbar navbar-expand-lg navbar-dark bg-dark px-4" id="navbar">
        <a class="navbar-brand fw-bold text-warning" href="#">SkiRent</a>
        <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarContent"
                aria-controls="navbarContent" aria-expanded="false" aria-label="Menü váltása">
            <span class="navbar-toggler-icon"></span>
        </button>

        <div class="collapse navbar-collapse justify-content-end" id="navbarContent">
            <ul class="navbar-nav" id="navbar-menu">
                <!-- Dinamikus menüpontok JS-ből töltődnek be -->
            </ul>
        </div>
    </nav>

   <!-- Parallax szekció -->
    <div class="rentals-hero-section">
        <div class="container mt-5" id="rentals-content">
            <h2 class="text-center text-warning mb-4">Foglalásaim</h2>
            <p class="text-center">Itt találod a korábbi és aktuális foglalásaidat.</p>

            <div class="row justify-content-center">
                <div class="col-md-10">
                    <div class="card shadow p-3">
                        <div id="booking-list">
                            <table class="table table-bordered text-center">
                                <thead class="thead-dark">
                                    <tr>
                                        <th>Sorszám</th>
                                        <th>Kelt.</th>
                                        <th>Intervallum</th>
                                        <th>Műveletek</th>
                                    </tr>
                                </thead>
                                <tbody id="booking-table-body">
                                    <!-- Ide töltődnek a foglalások -->
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>


    <!-- Foglalás részletei modal -->
    <div class="modal fade" id="bookingDetailsModal" tabindex="-1" role="dialog" aria-labelledby="bookingDetailsModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-dialog-centered modal-lg" role="document">
        <div class="modal-content">
        <div class="modal-header bg-info text-white">
            <h5 class="modal-title" id="bookingDetailsModalLabel">Foglalás részletei</h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Bezárás"></button>
        </div>
        <div class="modal-body">
            <p><strong>Foglalás azonosító:</strong> <span id="detail-booking-id"></span></p>
            <p><strong>Létrehozva:</strong> <span id="detail-booking-date"></span></p>
            <p><strong>Bérlés időtartama:</strong> <span id="detail-interval"></span></p>
            <p><strong>Státusz:</strong> <span id="detail-status"></span></p>
            <p><strong>Végösszeg:</strong> <span id="detail-price"></span> Ft</p>
        </div>
        <div class="modal-footer">
            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Bezárás</button>
        </div>
        </div>
    </div>
    </div>


    <!-- Számla megtekintése modal -->
    <div class="modal fade" id="invoiceModal" tabindex="-1" aria-labelledby="invoiceModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title" id="invoiceModalLabel">Számla részletei</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Bezárás"></button>
            </div>
            <div class="modal-body" id="invoice-content">
                <!-- Itt jelenik meg a számlaadat dinamikusan -->
            </div>
            <div class="modal-footer">
                <button class="btn btn-secondary" data-bs-dismiss="modal">Bezárás</button>
                <button class="btn btn-primary" onclick="window.print()">Nyomtatás</button>
            </div>
            </div>
        </div>
    </div>




    <!-- Bootstrap JS + jQuery -->
    <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>


    <!-- Saját JS -->
    <script src="../js/script.js"></script>
    <!-- Saját JS - Login JS -->
    <script src="../js/login.js"></script>
    <!-- Saját JS - rentals JS -->
    <script src="../js/rentals.js"></script>

</body>

</html>

