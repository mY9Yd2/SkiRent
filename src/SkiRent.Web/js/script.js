//Dinamikus menü kezelése.
document.addEventListener("DOMContentLoaded", () => {
  const navbarMenu = document.getElementById("navbar-menu");
  if (!navbarMenu) return;

  const currentPage = window.location.pathname.split("/").pop();

  function getNavItem(label, href, extraClass = "", id = "") {
    return `
            <li class="nav-item">
                <a class="nav-link ${extraClass}" href="${href}" ${
      id ? `id="${id}"` : ""
    }>${label}</a>
            </li>`;
  }

  fetch("http://localhost:5101/api/auth/me", {
    method: "GET",
    credentials: "include",
  })
    .then((response) => {
      if (!response.ok) throw new Error("Not logged in");
      return response.json();
    })
    .then((user) => {
      console.log("Bejelentkezve:", user.email);
      window.loggedInUserEmail = user.email; // globális változóként mentem el, hogy fel tudjam használni a foglalásoknál

      navbarMenu.innerHTML = `
            ${getNavItem(
              "Eszközök",
              "products.php",
              currentPage === "products.php" ? "active text-warning" : ""
            )}
            ${getNavItem(
              "Profilom",
              "profile.php",
              currentPage === "profile.php" ? "active text-warning" : ""
            )}
            ${getNavItem(
              "Foglalásaim",
              "rentals.php",
              currentPage === "rentals.php" ? "active text-warning" : ""
            )}
            <li class="nav-item">
                <a class="nav-link ${
                  currentPage === "cart.php" ? "active text-warning" : ""
                } position-relative" href="cart.php">
                    🛒
                    <span id="cart-count" class="badge bg-danger position-absolute top-0 start-100 translate-middle rounded-pill d-none">0</span>
                </a>
            </li>
            ${getNavItem("Kijelentkezés", "#", "text-danger", "logout-link")}
        `;

      // Korábban ellenőrzés céljára szolgált:
      //console.log("Menü HTML:", navbarMenu.innerHTML);

      // Itt jön a timeoutos logout eseményfigyelő
      setTimeout(() => {
        const logoutLink = document.getElementById("logout-link");
        if (logoutLink) {
          logoutLink.addEventListener("click", (e) => {
            e.preventDefault();
            fetch("http://localhost:5101/api/auth/sign-out", {
              method: "POST",
              credentials: "include",
              headers: {
                "Content-Type": "application/json"
              },
              body: JSON.stringify({
                empty: ""
              })
            })
              .then(() => {
                console.log("Kijelentkezett");
                sessionStorage.clear();
                window.location.href =
                  window.location.origin + "/SkiRent.Web/index.php";
              })
              .catch((err) => {
                console.error("Kijelentkezés hiba:", err);
              });
          });
        }
      }, 0); // Ez biztosítja, hogy az elem ténylegesen létrejöjjön DOM-ban

      updateCartCount();
    })
    .catch(() => {
      console.log("Nem bejelentkezett állapot");

      // index.php-n NEM jelenik meg a "vissza a kezdőoldalra"
      const path = window.location.pathname;
      const currentPage = path.split("/").pop();
      const isInPages = path.includes("/pages/");
      const showHome = !(currentPage === "index.php" || currentPage === "");

      navbarMenu.innerHTML = `
            ${getNavItem(
              "Eszközök",
              isInPages ? "products.php" : "pages/products.php"
            )}
            ${
              showHome
                ? getNavItem(
                    "Vissza a kezdőoldalra",
                    isInPages ? "../index.php" : "index.php"
                  )
                : ""
            }
            ${getNavItem(
              "Bejelentkezés",
              isInPages ? "login.php" : "pages/login.php"
            )}
            ${getNavItem(
              "Regisztráció",
              isInPages ? "reg.php" : "pages/reg.php"
            )}
        `;
    });

  updateCartCount();
});

//Termékek kezelése. - dinamikusan.
document.addEventListener("DOMContentLoaded", () => {
  const productList = document.getElementById("product-list");

  if (!productList) return;

  let isLoggedIn = false;

  fetch("http://localhost:5101/api/auth/me", {
    method: "GET",
    credentials: "include",
  })
    .then((res) => {
      if (!res.ok) throw new Error("Not logged in");
      isLoggedIn = true;
    })
    .catch(() => {
      isLoggedIn = false;
    })
    .finally(() => {
      fetch("http://localhost:5101/api/equipments")
        .then((res) => res.json())
        .then((data) => {
          console.log("BACKEND VÁLASZ:", data);
          productList.innerHTML = "";

          data.forEach((item) => {
            const col = document.createElement("div");
            let imageUrl;

            // Statikus képek hozzárendelése a terméknév alapján
            const staticImages = {
              "Nordica GP TJ 31 - 32 -es gyerek síbakancs":
                "../assets/pictures/nordica.jpg",
              "TecnoPro T50.3 31-32 -es gyerek síbakancs":
                "../assets/pictures/TecnoProT50.jpg",
              "Roces Idea Up 36 - 40 -es méretű síbakancs, állítható":
                "../assets/pictures/roces.jpg",
              "Lange Starlet 60 37 - 38 -as síbakancs":
                "../assets/pictures/lange.jpg",
              "Síbot pályára, Boost 100 safety, fekete":
                "../assets/pictures/sibotSafety100.jpg",
              "Síbot, pályára - Boost 500 Safety":
                "../assets/pictures/boost500.jpg",
              "Gyerek síbot, állítható, levehető csuklópánttal - Safety 500":
                "../assets/pictures/Safety500.jpg",
              "Fizan Action Pro síbot": "../assets/pictures/Fizan1.jpg",
              "Salomon X Drive Focus Rocker 130cm carving síléc":
                "../assets/pictures/SalomonXDrive.jpg",
              "Atomic Race 8 140cm carving síléc":
                "../assets/pictures/AtomicRace8.jpg",
              "Elan ExarPro 140cm carving síléc":
                "../assets/pictures/ElanExarPro.jpg",
              "Atomic Redster RX Rocker 156cm carving síléc Grip Walk":
                "../assets/pictures/boost500.jpg",
              "Férfi snowboard allmountain/freeride - All Road 500":
                "../assets/pictures/AllRoad.jpg",
              "Női freestyle és all mountain snowboard Endzone 500":
                "../assets/pictures/Endzone500.jpg",
              "Női all mountain és freestyle snowboard - Dreamscape SNB100":
                "../assets/pictures/SNB100.jpg",
              "Férfi snowboard all mountain és freestyle síeléshez - Endzone 900 PRO":
                "../assets/pictures/Endzone.jpg",
            };

            // Ha van ismert név, azt használom
            if (staticImages[item.name]) {
              imageUrl = staticImages[item.name];
            } else {
              imageUrl = "../assets/pictures/SkiRent_sieloLogo.jpg"; // alapértelmezett kép
            }
            col.className = "col-12 col-md-4 mb-4";
            col.innerHTML = `
                    <div class="card h-100 bg-light shadow-sm p-2 d-flex flex-column" data-id="${
                      item.id
                    }">
                        <img src="${imageUrl}" class="card-img-top custom-img" alt="${
              item.name
            }">
                        <div class="card-body d-flex flex-column">
                        <div class="card-content mb-3">
                            <h5 class="card-title text-warning">${
                              item.name
                            }</h5>
                            <p class="card-text description">${
                              item.description || "Nincs leírás."
                            }</p>
                            <p class="card-text price fw-bold">Ár: ${
                              item.pricePerDay
                            } Ft/nap</p>
                            ${
                              isLoggedIn
                                ? `
                                <p class="card-text stock text-success">
                                <strong>Készleten: <span class="stock-quantity">${item.availableQuantity} db</span></strong>
                                </p>
                                <div class="mb-3 d-flex align-items-center mb-3">
                                <label for="quantity-${item.id}" class="card-label me-2">Mennyiség:</label>
                                <input type="number" class="form-control form-control-sm quantity-input"
                                    id="quantity-${item.id}" min="1" max="${item.availableQuantity}" value="1" style="width: 70px;">
                                </div>
                                `
                                : ""
                            }
                        </div>
                        ${
                          isLoggedIn
                            ? `<button class="btn btn-warning w-100 add-to-cart-button">Kosárba</button>`
                            : ""
                        }
                        </div>
                    </div>
                    `;

            productList.appendChild(col);
          });

          // Mennyiség gombok eseménykezelői
          document.querySelectorAll(".quantity-increase").forEach((btn) => {
            btn.addEventListener("click", () => {
              const input = btn.previousElementSibling;
              const max = parseInt(input.max);
              if (parseInt(input.value) < max)
                input.value = parseInt(input.value) + 1;
            });
          });

          document.querySelectorAll(".quantity-decrease").forEach((btn) => {
            btn.addEventListener("click", () => {
              const input = btn.nextElementSibling;
              if (parseInt(input.value) > 1)
                input.value = parseInt(input.value) - 1;
            });
          });
        })
        .catch((err) => {
          console.error("Termék betöltés hiba:", err);
          productList.innerHTML = `<p class="text-danger">Nem sikerült betölteni a termékeket.</p>`;
        });
    });
});

/*==============================
    KOSÁR - kezelése
    ============================*/

// Segéd függvény kosár frissítéshez
function updateCartCount() {
  const cartCount = document.getElementById("cart-count");
  const savedCount = parseInt(sessionStorage.getItem("cartCount")) || 0;
  if (cartCount) {
    if (savedCount > 0) {
      cartCount.textContent = savedCount;
      cartCount.classList.remove("d-none");
    } else {
      cartCount.classList.add("d-none");
    }
  }
}

// Kosárba gomb kattintás kezelése
document.addEventListener("click", function (e) {
  if (e.target.classList.contains("add-to-cart-button")) {
    const card = e.target.closest(".card");
    const input = card.querySelector(".quantity-input");
    const quantity = parseInt(input.value) || 1;

    // Először kiszedi az adatokat a kártyából
    const itemId = parseInt(card.getAttribute("data-id"));
    const itemName = card.querySelector(".card-title").textContent;
    const itemPrice = parseInt(
      card.querySelector(".card-text.fw-bold").textContent.replace(/\D/g, "")
    );

    // Kosár darabszám frissítése
    const cartCount = document.getElementById("cart-count");
    let currentCount = parseInt(sessionStorage.getItem("cartCount")) || 0;
    currentCount += quantity;
    sessionStorage.setItem("cartCount", currentCount);

    // Kosár tartalom mentése (cartItems)
    let cartItems = JSON.parse(sessionStorage.getItem("cartItems") || "[]");

    // Megnézi, van-e már ilyen termék
    const existingItem = cartItems.find((item) => item.id === itemId);
    if (existingItem) {
      existingItem.quantity += quantity;
    } else {
      cartItems.push({
        id: itemId,
        name: itemName,
        pricePerDay: itemPrice,
        quantity: quantity,
      });
    }

    sessionStorage.setItem("cartItems", JSON.stringify(cartItems));

    // Frissítés megjelenítése
    cartCount.textContent = currentCount;
    cartCount.classList.remove("d-none");

    // Felugró sikeres hozzáadás értesítés
    const alert = document.createElement("div");
    alert.className =
      "alert alert-success alert-dismissible fade show position-fixed";
    alert.style.top = "20px";
    alert.style.right = "20px";
    alert.style.zIndex = "1050";
    alert.innerHTML = `
            <strong>✅ Kosár frissítve!</strong> ${quantity} db termék hozzáadva.
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Bezárás"></button>
        `;

    document.body.appendChild(alert);

    //  Automatikus eltűnés 3 másodperc után
    setTimeout(() => {
      alert.classList.remove("show");
      alert.classList.add("hide");
      alert.addEventListener("transitionend", () => alert.remove());
    }, 3000);
  }
});

// Kosár tartalom megjelenítése cart.php oldalon
document.addEventListener("DOMContentLoaded", () => {
  // 1. DÁTUM validálás & mentés
  const startDateInput = document.getElementById("start-date");
  const endDateInput = document.getElementById("end-date");
  const checkoutBtn = document.getElementById("checkout-btn");

  const today = new Date();
  today.setHours(0, 0, 0, 0);

  const errorMsg = document.createElement("div");
  errorMsg.id = "date-error";
  errorMsg.className = "text-danger mt-2";
  errorMsg.style.display = "none";
  errorMsg.textContent =
    "Bérlés záró dátumának későbbinek kell lennie, mint a kezdő dátum!";
  endDateInput?.parentNode?.appendChild(errorMsg);

  function validateDates() {
    const startDate = new Date(startDateInput.value);
    const endDate = new Date(endDateInput.value);

    if (!startDateInput.value || !endDateInput.value) {
      checkoutBtn.disabled = true;
      startDateInput.classList.remove("is-invalid");
      endDateInput.classList.remove("is-invalid");
      errorMsg.style.display = "none";
      return;
    }

    if (startDate < today) {
      checkoutBtn.disabled = true;
      startDateInput.classList.add("is-invalid");
      endDateInput.classList.remove("is-invalid");
      errorMsg.textContent = "A kezdő dátum nem lehet korábbi, mint a mai nap!";
      errorMsg.style.display = "block";
      return;
    }

    if (endDate <= startDate) {
      checkoutBtn.disabled = true;
      startDateInput.classList.add("is-invalid");
      endDateInput.classList.add("is-invalid");
      errorMsg.textContent =
        "Bérlés záró dátumának későbbinek kell lennie, mint a kezdő dátum!";
      errorMsg.style.display = "block";
      return;
    }

    checkoutBtn.disabled = false;
    startDateInput.classList.remove("is-invalid");
    endDateInput.classList.remove("is-invalid");
    errorMsg.style.display = "none";
  }

  if (startDateInput && endDateInput && checkoutBtn) {
    startDateInput.addEventListener("input", validateDates);
    endDateInput.addEventListener("input", validateDates);

    checkoutBtn.addEventListener("click", () => {
      sessionStorage.setItem("startDate", startDateInput.value);
      sessionStorage.setItem("endDate", endDateInput.value);
      window.location.href = "checkout.php";
    });
  }

  // 2. KOSÁR tartalom megjelenítés
  const cartTableBody = document.querySelector("#cart-table tbody");
  if (!cartTableBody) return;

  const cartItems = JSON.parse(sessionStorage.getItem("cartItems") || "[]");
  const groupedItems = {};

  cartItems.forEach((item) => {
    if (groupedItems[item.id]) {
      groupedItems[item.id].quantity += item.quantity;
    } else {
      groupedItems[item.id] = { ...item };
    }
  });

  const rows = Object.values(groupedItems);

  if (rows.length === 0) {
    cartTableBody.innerHTML =
      "<tr><td colspan='6' class='text-center'>A kosár üres.</td></tr>";
    return;
  }

  rows.forEach((item, index) => {
    const totalPrice = item.quantity * item.pricePerDay;

    const row = document.createElement("tr");
    row.innerHTML = `
            <td>${index + 1}</td>
            <td>${item.name}</td>
            <td>${item.quantity}</td>
            <td>${item.pricePerDay} Ft</td>
            <td>${totalPrice} Ft</td>
            <td><button class="btn btn-sm btn-danger remove-item" data-id="${
              item.id
            }">Törlés</button></td>
        `;
    cartTableBody.appendChild(row);
  });

  // Összesített végösszeg kiszámítása
  const grandTotal = rows.reduce(
    (sum, item) => sum + item.quantity * item.pricePerDay,
    0
  );

  // Új sor a végösszegnek
  const totalRow = document.createElement("tr");
  totalRow.innerHTML = `
    <td colspan="4" class="text-end fw-bold">Végösszeg:</td>
    <td colspan="2" class="fw-bold text-success">${grandTotal.toLocaleString()} Ft</td>
`;
  cartTableBody.appendChild(totalRow);
  // 3. TÖRLÉS kezelése
  document.querySelectorAll(".remove-item").forEach((button) => {
    button.addEventListener("click", () => {
      handleItemDeletion(button);
    });
  });
});

// Kosár - tétel törlése gomb és modal ablak viselkedése.
function handleItemDeletion(button) {
  const id = parseInt(button.getAttribute("data-id"));

  const confirmModal = document.createElement("div");
  confirmModal.className = "modal fade";
  confirmModal.tabIndex = -1;
  confirmModal.innerHTML = `
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header bg-warning">
                    <h5 class="modal-title">Megerősítés</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Bezárás"></button>
                </div>
                <div class="modal-body">
                    <p>Biztosan törli a tételt és a hozzá tartozó mennyiséget a kosárból?</p>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Nem</button>
                    <button type="button" class="btn btn-danger" id="confirm-delete">Igen</button>
                </div>
            </div>
        </div>
    `;

  document.body.appendChild(confirmModal);
  const modalInstance = new bootstrap.Modal(confirmModal);
  modalInstance.show();

  const confirmBtn = confirmModal.querySelector("#confirm-delete");
  confirmBtn.addEventListener("click", () => {
    let cartItems = JSON.parse(sessionStorage.getItem("cartItems") || "[]");
    const updatedItems = cartItems.filter((item) => item.id !== id);
    sessionStorage.setItem("cartItems", JSON.stringify(updatedItems));

    // Kosár darabszám frissítés
    const totalQuantity = updatedItems.reduce(
      (sum, item) => sum + item.quantity,
      0
    );
    sessionStorage.setItem("cartCount", totalQuantity);

    // DOM-frissítés
    button.closest("tr").remove();
    const tableBody = document.querySelector("#cart-table tbody");
    if (tableBody.children.length === 0) {
      tableBody.innerHTML =
        "<tr><td colspan='6' class='text-center'>A kosár üres.</td></tr>";
    }

    // Értesítő
    const alert = document.createElement("div");
    alert.className =
      "alert alert-info alert-dismissible fade show position-fixed";
    alert.style.top = "20px";
    alert.style.right = "20px";
    alert.style.zIndex = "1050";
    alert.innerHTML = `
            <strong>ℹ️ A kosár tartalma frissült!</strong>
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Bezárás"></button>
        `;
    document.body.appendChild(alert);
    setTimeout(() => {
      alert.classList.remove("show");
      alert.classList.add("hide");
      alert.addEventListener("transitionend", () => alert.remove());
    }, 2000);

    modalInstance.hide();
    confirmModal.addEventListener("hidden.bs.modal", () => {
      confirmModal.remove();
    });
  });

  confirmModal.addEventListener("hidden.bs.modal", () => {
    confirmModal.remove();
  });
}
