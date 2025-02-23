const API_BASE_URL = "http://localhost:5101/api"; // API alap URL

document.addEventListener("DOMContentLoaded", function () {
    const loginForm = document.getElementById("login-form");
    const emailInput = document.getElementById("email");
    const passwordInput = document.getElementById("password");

    // E-mail formátum ellenőrzés, piros szegély ha rossz
    emailInput.addEventListener("input", function () {
        const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/; // Email minta
        if (emailPattern.test(emailInput.value.trim())) {
            emailInput.style.border = "1px solid #ced4da"; // Eredeti szegély visszaállítása
        } else {
            emailInput.style.border = "2px solid red"; // Piros szegély, ha nem megfelelő
        }
    });

    if (loginForm) {
        loginForm.addEventListener("submit", async function (event) {
            event.preventDefault(); // Megakadályozza az alapértelmezett form elküldést

            // Ellenőrzi az input mezőket
            if (!validateLoginForm()) {
                return; // Ha hibás, akkor itt megáll
            }

            const email = emailInput.value.trim();
            const password = passwordInput.value.trim();

            console.log("Bejelentkezési adatok: ", { email, password }); // Debug

            try {
                const response = await fetch(`${API_BASE_URL}/auth/sign-in?useTokens=true`, {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    credentials: "include", // A süti miatt kell
                    body: JSON.stringify({ email, password })
                });

                console.log("API válasz státusz:", response.status); // Debug

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

                console.log("API válasz:", data);

                if (!data || !data.accessToken) {
                    throw new Error("Bejelentkezés sikeres, de nincs visszakapott token!");
                }

                // Token mentése a sessionStorage-ba
                sessionStorage.setItem("accessToken", data.accessToken);
                sessionStorage.setItem("refreshToken", data.refreshToken);

                // Átirányítás a főoldalra
                window.location.href = "mainpage.php";

            } catch (error) {
                console.error("Hiba történt:", error); // Debug
                alert(error.message);
            }
        });
    }
});

// Függvény az üres mezők ellenőrzésére
function validateLoginForm() {
    const emailInput = document.getElementById("email");
    const passwordInput = document.getElementById("password");

    let isValid = true;

    // Ha az e-mail üres, akkor piros szegélyt kap
    if (emailInput.value.trim() === "") {
        emailInput.style.border = "2px solid red";
        isValid = false;
    } else {
        emailInput.style.border = "1px solid #ced4da"; // Visszaállítja az eredetit, ha jó
    }

    // Ha a jelszó üres, akkor piros szegélyt kap
    if (passwordInput.value.trim() === "") {
        passwordInput.style.border = "2px solid red";
        isValid = false;
    } else {
        passwordInput.style.border = "1px solid #ced4da"; // Visszaállítja az eredetit, ha jó
    }

    // Ha bármi hiba van, megjeleníti az üzenetet
    if (!isValid) {
        alert("Az email cím és jelszó megadása kötelező!");
    }

    return isValid; // Ha true, akkor mehet tovább a bejelentkezés
}



document.addEventListener("DOMContentLoaded", function () {
    const registerForm = document.getElementById("register-form");

    if (registerForm) {
        registerForm.addEventListener("submit", function (event) {
            event.preventDefault();

            const emailInput = document.getElementById("email");
            const passwordInput = document.getElementById("password");
            const confirmPasswordInput = document.getElementById("confirm-password");
            const passwordError = document.getElementById("password-error");

            let hasError = false; // Hibát jelző változó

            // Ellenőrzi az üres mezőket és piros szegélyt adunk
            if (!emailInput.value.trim()) {
                emailInput.style.border = "2px solid red";
                hasError = true;
            } else {
                emailInput.style.border = "1px solid #ced4da"; // Visszaállítás
            }

            if (!passwordInput.value.trim()) {
                passwordInput.style.border = "2px solid red";
                hasError = true;
            } else {
                passwordInput.style.border = "1px solid #ced4da";
            }

            if (!confirmPasswordInput.value.trim()) {
                confirmPasswordInput.style.border = "2px solid red";
                hasError = true;
            } else {
                confirmPasswordInput.style.border = "1px solid #ced4da";
            }

            // Ha volt üres mező, akkor megállíta a küldést és figyelmezteti a felhasználót
            if (hasError) {
                alert("Az összes mező kitöltése kötelező!");
                return;
            }

            // Ha a két jelszó nem egyezik
            if (passwordInput.value !== confirmPasswordInput.value) {
                passwordError.textContent = "A két jelszó nem egyezik!";
                passwordError.style.color = "red";
                return;
            } else {
                passwordError.textContent = "";
            }

            // Ha minden rendben, küldi az API-nak a regisztrációt
            registerUser(emailInput.value, passwordInput.value);
        });
    }
});

// Regisztrációs API hívás
function registerUser(email, password) {
    fetch("http://localhost:5101/api/users", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            email: email,
            password: password
        })
    })
    .then(response => response.json())
    .then(data => {
        console.log("Sikeres regisztráció:", data);
        alert("Sikeres regisztráció! Most már bejelentkezhetsz.");
        window.location.href = "login.php";
    })
    .catch(error => {
        console.error("Hiba történt:", error);
        alert("Hiba történt a regisztráció során!");
    });
}

