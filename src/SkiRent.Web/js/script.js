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

    // Csak a profile.php oldalon hívom meg a felhasználói adatok betöltését
    if (document.getElementById("user-email")) {
        fetchUserProfile();
    }
});




// ** FELHASZNÁLÓI ADATOK LEKÉRÉSE A PROFILHOZ **
async function fetchUserProfile() {
    console.log("fetchUserProfile() meghívódott!");

    const accessToken = sessionStorage.getItem("accessToken");

    if (!accessToken) {
        console.error("Nincs token elmentve a sessionStorage-ben!");
        return;
    }

    try {
        const response = await fetch(`${API_BASE_URL}/auth/me`, {
            method: "GET",
            headers: {
                "Authorization": `Bearer ${accessToken}`,
                "Accept": "application/json"
            }
        });

        if (!response.ok) {
            throw new Error(`Hiba történt az API lekérdezésekor: ${response.status}`);
        }

        const userData = await response.json();
        console.log("API válasz:", userData);

        // **Ellenőrzi, hogy léteznek-e az elemek**
        const userNameElement = document.getElementById("user-name");
        const userEmailElement = document.getElementById("user-email");
        const displayNameElement = document.getElementById("display-name");
        const displayEmailElement = document.getElementById("display-email");

        if (userNameElement) {
            userNameElement.textContent = userData.email.split("@")[0]; // Pl. "admin@example.com" -> "admin"
            console.log("Név frissítve (user-name):", userNameElement.textContent);
        } else {
            console.error("Hiba: A 'user-name' elem nem található az oldalon!");
        }

        if (userEmailElement) {
            userEmailElement.textContent = userData.email || "Nincs e-mail cím";
            console.log("E-mail frissítve (user-email):", userEmailElement.textContent);
        } else {
            console.error("Hiba: A 'user-email' elem nem található az oldalon!");
        }

        if (displayNameElement) {
            displayNameElement.textContent = userData.email.split("@")[0]; // Pl. "admin@example.com" -> "admin"
            console.log("Név frissítve (display-name):", displayNameElement.textContent);
        } else {
            console.error("Hiba: A 'display-name' elem nem található az oldalon!");
        }

        if (displayEmailElement) {
            displayEmailElement.textContent = userData.email || "Nincs e-mail cím";
            console.log("E-mail frissítve (display-email):", displayEmailElement.textContent);
        } else {
            console.error("Hiba: A 'display-email' elem nem található az oldalon!");
        }

    } catch (error) {
        console.error("Hiba:", error);
    }
}

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

        // Termékek megjelenítése
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
    const isLoggedIn = sessionStorage.getItem("accessToken") !== null && sessionStorage.getItem("accessToken") !== "";
    console.log("Be van jelentkezve? ", isLoggedIn);

    products.forEach(product => {
        const productCard = `
            <div class="col-md-4">
                <div class="card text-dark bg-light mb-3 shadow" data-product-id="${product.id}">
                    <div class="card-body">
                        <h5 class="card-title text-warning">${product.name}</h5>
                        <p class="card-text">${product.description ? product.description : "Nincs leírás"}</p>
                        <p class="card-text"><strong>Ár: ${product.pricePerDay} Ft/nap</strong></p>
                        ${isLoggedIn ? `
                            <p class="card-text"><small>Elérhető: ${product.availableQuantity} db</small></p>
                            <div class="mb-2">
                                <label for="quantity-${product.id}" class="form-label">Mennyiség:</label>
                                <input type="number" min="1" max="${product.availableQuantity}" value="1" 
                                    class="form-control quantity-input" id="quantity-${product.id}" style="width: 80px;">
                            </div>
                        ` : ""}
                        <a href="#" class="btn btn-warning add-to-cart-button" 
                           style="display: ${isLoggedIn ? 'inline-block' : 'none'};">Kosárba</a>
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
                <!-- KOSÁR ICON -->
                <li class="nav-item">
                    <a class="nav-link text-light menu-item position-relative" href="cart.php">
                        <i class="fas fa-shopping-cart"></i> <!-- Kosár ikon -->
                        <span id="cart-count" class="badge bg-danger position-absolute top-0 start-100 translate-middle rounded-pill d-none">0</span>
                    </a>
                </li>

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



// Kosár tartalmának frissítése sessionStorage-ben
function updateCartCount() {
    let cartItems = JSON.parse(sessionStorage.getItem("cart") || "[]"); 
    let totalQuantity = 0;

    cartItems.forEach(item => {
        totalQuantity += item.quantity || 1; // Ha régi adat, akkor legalább 1-gyel számol
    });

    console.log("Frissített kosár mennyiség:", totalQuantity); 

    const cartCountBadge = document.getElementById("cart-count");
    if (!cartCountBadge) {
        console.warn("Kosár ikon nem található az oldalon.");
        return;
    }

    if (totalQuantity > 0) {
        cartCountBadge.textContent = totalQuantity;
        cartCountBadge.classList.remove("d-none");
    } else {
        cartCountBadge.classList.add("d-none");
    }

    toggleClearCartButton();
}



document.addEventListener("DOMContentLoaded", function () {
    updateCartCount(); // Kosár frissítése betöltéskor

    const productList = document.getElementById("product-list");
    if (!productList) {
        console.warn("Nincs product-list az oldalon, kihagyom a termékkezelést.");
        return; // Ha nincs product-list, akkor ne folytassa ezt a részt
    }

    productList.addEventListener("click", function (event) {
        if (event.target.classList.contains("add-to-cart-button")) {
            event.preventDefault();
    
            console.log("Kosárba gombra kattintottak!");
    
            const productCard = event.target.closest(".card");
            if (!productCard) {
                console.error("Nem található a termék kártyája!");
                return;
            }
    
            const productName = productCard.querySelector(".card-title")?.textContent.trim();
            const productPrice = productCard.querySelector(".card-text strong")?.textContent.trim();
            const quantityInput = productCard.querySelector(".quantity-input");
            let quantity = parseInt(quantityInput?.value) || 1; // Ha üres vagy hibás, alapból 1
            const selectedQuantity = parseInt(quantityInput?.value) || 1;
    
            if (!productName || !productPrice || quantity <= 0) {
                console.error("Hibás termékadatok vagy mennyiség!");
                return;
            }
    
            let cartItems = JSON.parse(sessionStorage.getItem("cart") || "[]");
    
            // Megnézi van-e már ilyen termék a kosárban
            const existingItem = cartItems.find(item => item.name === productName);

            if (existingItem) {
                existingItem.quantity += selectedQuantity; // Hozzáadja a megadott mennyiséget
            } else {
                cartItems.push({ name: productName, price: productPrice, quantity: selectedQuantity }); // Új tétel mennyiséggel
            }
            sessionStorage.setItem("cart", JSON.stringify(cartItems));
    
            updateCartCount();
    
            console.log(`${selectedQuantity} db '${productName}' hozzáadva a kosárhoz.`);
    
            // Reseteli a mennyiséget 1-re
            if (quantityInput) quantityInput.value = 1;
    
            const cartModalElement = document.getElementById("cartModal");
            if (cartModalElement) {
                const cartModal = new bootstrap.Modal(cartModalElement);
                cartModal.show();
            }
        }
    });
});




// Kosár tartalmának betöltése
function loadCart() {
    let cartItems = JSON.parse(sessionStorage.getItem("cart") || "[]");
    const cartTableBody = document.getElementById("cart-items");
    const cartTotal = document.getElementById("cart-total");

    if (!cartTableBody || !cartTotal) {
        console.warn("A kosár táblázata vagy az összegzés nem található!");
        return;
    }

    cartTableBody.innerHTML = ""; // Kosár tartalmának törlése frissítés előtt
    let total = 0;

    if (cartItems.length === 0) {
        cartTableBody.innerHTML = `<tr><td colspan="3" class="text-muted">A kosár üres.</td></tr>`;
    } else {
        cartItems.forEach((item, index) => {
            total += (parseInt(item.price.replace(/\D/g, "")) || 0) * item.quantity;
        
            let row = `
                <tr>
                    <td>${item.name}</td>
                    <td>${item.price}</td>
                    <td>
                        <button class="btn btn-sm btn-warning quantity-decrease">-</button>
                        <span class="item-quantity">${item.quantity}</span>
                        <button class="btn btn-sm btn-warning quantity-increase">+</button>
                    </td>
                    <td><button class="btn btn-sm delete-item">
                            <i class="fas fa-times"></i>
                        </button>
                    </td>
                </tr>
            `;
            cartTableBody.insertAdjacentHTML("beforeend", row);
        });

        // Eltávolítás gombok eseménykezelői
        document.querySelectorAll(".remove-item").forEach(button => {
            button.addEventListener("click", function () {
                let index = this.getAttribute("data-index");
                cartItems.splice(index, 1);
                sessionStorage.setItem("cart", JSON.stringify(cartItems));
                loadCart();
            });
        });
    }

    cartTotal.textContent = total.toLocaleString() + " Ft";

    toggleClearCartButton(); 
}


function toggleClearCartButton() {
    const clearCartButton = document.getElementById("clear-cart");
    const cartItems = JSON.parse(sessionStorage.getItem("cart") || "[]");

    if (clearCartButton) {
        clearCartButton.disabled = cartItems.length === 0;
    }
}

loadCart();
updateCartCount();



// ============================================
// Kosár tétel törlés megerősítése modalból
// ============================================
let deleteIndexToConfirm = null; // Ideiglenesen tárolja, melyik indexet akarjuk törölni

document.addEventListener("DOMContentLoaded", function () {
    const cartTableBody = document.getElementById("cart-items");
    const confirmDeleteModalElement = document.getElementById("confirmDeleteModal");

    if (cartTableBody && confirmDeleteModalElement) {
        const confirmDeleteModal = new bootstrap.Modal(confirmDeleteModalElement);
        const confirmDeleteBtn = document.getElementById("confirm-delete-btn");

        cartTableBody.addEventListener("click", function (event) {
            if (event.target.closest(".delete-item")) {
                deleteIndexToConfirm = Array.from(cartTableBody.querySelectorAll(".delete-item")).indexOf(event.target.closest(".delete-item"));
                confirmDeleteModal.show();
            }
        });

        confirmDeleteBtn.addEventListener("click", function () {
            if (deleteIndexToConfirm !== null) {
                let cartItems = JSON.parse(sessionStorage.getItem("cart") || "[]");
                if (deleteIndexToConfirm >= 0 && deleteIndexToConfirm < cartItems.length) {
                    cartItems.splice(deleteIndexToConfirm, 1);
                    sessionStorage.setItem("cart", JSON.stringify(cartItems));
                    loadCart();
                    updateCartCount();
                    console.log("Tétel sikeresen eltávolítva.");
                }
                deleteIndexToConfirm = null;
                confirmDeleteModal.hide();
            }
        });
    }
});


//Kosár teljes ürítésének megerősítés:
document.addEventListener("DOMContentLoaded", function () {
    const clearCartButton = document.getElementById("clear-cart");
    const confirmClearModalElement = document.getElementById("confirmClearModal");

    if (clearCartButton && confirmClearModalElement) {
        const confirmClearModal = new bootstrap.Modal(confirmClearModalElement);
        const confirmClearBtn = document.getElementById("confirm-clear-btn");

        clearCartButton.addEventListener("click", function () {
            confirmClearModal.show(); // Megerősítő modal megnyitása
        });

        confirmClearBtn.addEventListener("click", function () {
            sessionStorage.removeItem("cart");
            loadCart();
            updateCartCount();
            confirmClearModal.hide();
            console.log("Kosár sikeresen ürítve.");
        });
    }
});




document.addEventListener("DOMContentLoaded", function () {
    const cartTableBody = document.getElementById("cart-items");

    if (!cartTableBody) return; // Ha nincs kosár az oldalon, ne fusson le

    cartTableBody.addEventListener("click", function (event) {
        const cartItems = JSON.parse(sessionStorage.getItem("cart") || "[]");

        // Növelés
        if (event.target.classList.contains("quantity-increase")) {
            const rowIndex = Array.from(cartTableBody.querySelectorAll("tr")).indexOf(event.target.closest("tr"));
            cartItems[rowIndex].quantity++;
            sessionStorage.setItem("cart", JSON.stringify(cartItems));
            loadCart();
            updateCartCount();
        }

        // Csökkentés
        if (event.target.classList.contains("quantity-decrease")) {
            const rowIndex = Array.from(cartTableBody.querySelectorAll("tr")).indexOf(event.target.closest("tr"));
            if (cartItems[rowIndex].quantity > 1) {
                cartItems[rowIndex].quantity--;
                sessionStorage.setItem("cart", JSON.stringify(cartItems));
                loadCart();
                updateCartCount();
            }
        }
    });
});