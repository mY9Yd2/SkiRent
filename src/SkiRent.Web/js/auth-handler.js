document.addEventListener("DOMContentLoaded", function () {
    const accessToken = sessionStorage.getItem("accessToken");

    // Menü elemek lekérése
    const profileLink = document.getElementById("profile-link");
    const rentalsLink = document.getElementById("rentals-link");

    if (!accessToken) {
        // Ha nincs token, akkor a Profilom és Foglalásaim login-ra mutasson
        profileLink.setAttribute("href", "pages/login.php");
        rentalsLink.setAttribute("href", "pages/login.php");
    } else {
        // Ha van token, akkor normál működés
        profileLink.setAttribute("href", "pages/profile.php");
        rentalsLink.setAttribute("href", "pages/rentals.php");
    }

    
});