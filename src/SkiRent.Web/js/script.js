const API_BASE_URL = "http://localhost:5101/api"; // API alap URL

document.addEventListener("DOMContentLoaded", function () {
    const loginForm = document.getElementById("login-form");
    const emailInput = document.getElementById("email");
    const passwordInput = document.getElementById("password");

    if (emailInput) {  // Csak akkor dolgozza fel, ha létezik az oldalon
        emailInput.addEventListener("input", function () {
            const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (emailPattern.test(emailInput.value.trim())) {
                emailInput.style.border = "1px solid #ced4da";
            } else {
                emailInput.style.border = "2px solid red";
            }
        });
    }

    if (loginForm) {
        loginForm.addEventListener("submit", async function (event) {
            event.preventDefault();

            if (!validateLoginForm()) {
                return;
            }

            if (emailInput && passwordInput) {
                const email = emailInput.value.trim();
                const password = passwordInput.value.trim();

                try {
                    const response = await fetch(`${API_BASE_URL}/auth/sign-in?useTokens=true`, {
                        method: "POST",
                        headers: {
                            "Content-Type": "application/json"
                        },
                        credentials: "include",
                        body: JSON.stringify({ email, password })
                    });

                    if (!response.ok) {
                        throw new Error("Hibás bejelentkezési adatok vagy hiba az API-val!");
                    }

                    let data;
                    try {
                        data = await response.json();
                    } catch (error) {
                        console.warn("Hibás JSON válasz:", error);
                        data = null;
                    }

                    if (!data || !data.accessToken) {
                        throw new Error("Bejelentkezés sikeres, de nincs visszakapott token!");
                    }

                    sessionStorage.setItem("accessToken", data.accessToken);
                    sessionStorage.setItem("refreshToken", data.refreshToken);
                    window.location.href = "mainpage.php";

                } catch (error) {
                    console.error("Hiba történt:", error);
                    alert(error.message);
                }
            }
        });
    }

    // Csak a products.php oldalon hívom meg a termékek betöltését
    if (document.getElementById("product-list")) {
        fetchProducts();
    }
});

// Függvény az üres mezők ellenőrzésére
function validateLoginForm() {
    const emailInput = document.getElementById("email");
    const passwordInput = document.getElementById("password");

    let isValid = true;

    if (emailInput && emailInput.value.trim() === "") {
        emailInput.style.border = "2px solid red";
        isValid = false;
    } else if (emailInput) {
        emailInput.style.border = "1px solid #ced4da";
    }

    if (passwordInput && passwordInput.value.trim() === "") {
        passwordInput.style.border = "2px solid red";
        isValid = false;
    } else if (passwordInput) {
        passwordInput.style.border = "1px solid #ced4da";
    }

    if (!isValid) {
        alert("Az email cím és jelszó megadása kötelező!");
    }

    return isValid;
}

// Termékek lekérdezése API-ról
async function fetchProducts() {
    console.log("API-lekérdezés indult...");

    try {
        const response = await fetch(`${API_BASE_URL}/equipments`);
        
        console.log("API válasz státusz:", response.status);

        if (!response.ok) {
            throw new Error(`Hiba történt a termékek lekérése közben! Státusz: ${response.status}`);
        }

        const products = await response.json();
        console.log("Termékek adatai:", products);

        // TERMÉKEK MEGJELENÍTÉSE A KÉPERNYŐN
        displayProducts(products);

    } catch (error) {
        console.error("Hiba történt:", error);
    }
}

// ** TERMÉKEK MEGJELENÍTÉSE A KÉPERNYŐN **
function displayProducts(products) {
    console.log("displayProducts() meghívódott!");

    const productList = document.getElementById("product-list");
    productList.innerHTML = ""; // Törli az előző tartalmat

    // Ellenőrzi, hogy a felhasználó be van-e jelentkezve
    const isLoggedIn = sessionStorage.getItem("accessToken") !== null && sessionStorage.getItem("accessToken") !== "";    console.log("Be van jelentkezve? ", isLoggedIn);

    products.forEach(product => {
        const productCard = `
            <div class="col-md-4">
                <div class="card text-dark bg-light mb-3 shadow">
                    <div class="card-body">
                        <h5 class="card-title text-warning">${product.name}</h5>
                        <p class="card-text">${product.description ? product.description : "Nincs leírás"}</p>
                        <p class="card-text"><strong>Ár: ${product.pricePerDay} Ft/nap</strong></p>
                        ${isLoggedIn ? `<p class="card-text"><small>Elérhető: ${product.availableQuantity} db</small></p>` : ""}
                        <a href="#" class="btn btn-warning rent-button" 
                           style="display: ${isLoggedIn ? 'inline-block' : 'none'};">Bérlés</a>
                    </div>
                </div>
            </div>
        `;
        productList.insertAdjacentHTML("beforeend", productCard);
    });
}


document.addEventListener("DOMContentLoaded", function () {
    const isLoggedIn = sessionStorage.getItem("accessToken") !== null && sessionStorage.getItem("accessToken") !== "";

    console.log("Be van jelentkezve? ", isLoggedIn);

    // Az aktuális oldal neve
    const currentPage = window.location.pathname.split("/").pop().split("?")[0];

    // Navigációs menü dinamikus frissítése
    const navbar = document.querySelector("#navbarNav .navbar-nav");
    if (navbar) {
        navbar.innerHTML = `
            <li class="nav-item"><a class="nav-link menu-item ${currentPage === "products.php" ? "text-warning active" : "text-light"}" href="products.php">Eszközök</a></li>
            ${isLoggedIn ? `
                <li class="nav-item"><a class="nav-link menu-item ${currentPage === "profile.php" ? "text-warning active" : "text-light"}" href="profile.php">Profilom</a></li>
                <li class="nav-item"><a class="nav-link menu-item ${currentPage === "rentals.php" ? "text-warning active" : "text-light"}" href="rentals.php">Foglalásaim</a></li>
                <li class="nav-item"><a class="nav-link text-danger menu-item" href="logout.php">Kijelentkezés</a></li>
            ` : `
                <li class="nav-item"><a class="nav-link text-light menu-item" href="../index.php">Vissza a kezdőoldalra</a></li>
                <li class="nav-item"><a class="nav-link text-light menu-item" href="login.php">Bejelentkezés</a></li>
            `}
        `;
    }
});

document.addEventListener("DOMContentLoaded", function () {
    const logoutButton = document.querySelector(".nav-link.text-danger"); // Kijelentkezés gomb kiválasztása

    if (logoutButton) {
        logoutButton.addEventListener("click", function () {
            sessionStorage.clear(); // SessionStorage ürítése
        });
    }
});


document.addEventListener("DOMContentLoaded", function () {
    const currentPage = window.location.pathname.split("/").pop().split("?")[0];
    const menuItems = document.querySelectorAll(".nav-link.menu-item");

    menuItems.forEach(item => {
        const itemHref = item.getAttribute("href").split("?")[0];
        if (itemHref === currentPage) {
            item.classList.add("active-page"); // Új osztály hozzáadása
        } else {
            item.classList.remove("active-page");
        }
    });
});



