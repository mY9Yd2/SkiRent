const API_BASE_URL = "http://localhost:5101/api"; // API alap URL

document.addEventListener("DOMContentLoaded", function () {
    const loginForm = document.getElementById("login-form");
    const emailInput = document.getElementById("email");
    const passwordInput = document.getElementById("password");

    if (emailInput) {  // Csak akkor dolgozzuk fel, ha létezik az oldalon
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

    // Csak a products.php oldalon hívjuk meg a termékek betöltését
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
    const productList = document.getElementById("product-list");

    productList.innerHTML = ""; // Töröljük az előző tartalmat

    products.forEach(product => {
        const productCard = `
            <div class="col-md-4">
                <div class="card text-dark bg-light mb-3 shadow">
                    <div class="card-body">
                        <h5 class="card-title text-warning">${product.Name}</h5>
                        <p class="card-text">${product.Description ? product.Description : "Nincs leírás"}</p>
                        <p class="card-text"><strong>Ár: ${product.PricePerDay} Ft/nap</strong></p>
                        <p class="card-text"><small>Elérhető: ${product.AvailableQuantity} db</small></p>
                        <a href="#" class="btn btn-warning">Bérlés</a>
                    </div>
                </div>
            </div>
        `;
        productList.innerHTML += productCard;
    });
}