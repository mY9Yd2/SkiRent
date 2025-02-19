const API_BASE_URL = "http://localhost:5101/api";       // API alap URL

document.addEventListener("DOMContentLoaded", function () {
    const loginForm = document.getElementById("login-form");
    
    const emailInput = document.getElementById("email");

    // E-mail formátum ellenőrzés, piros szegély ha rossz
    emailInput.addEventListener("input", function () {
        const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;      // Email minta
        if (emailPattern.test(emailInput.value)) {
            emailInput.style.border = "1px solid #ced4da";      // Eredeti szegély visszaállítása
        } else {
            emailInput.style.border = "2px solid red";              // Piros szegély, ha nem megfelelő
        }
    });



    if (loginForm) {
        loginForm.addEventListener("submit", async function (event) {
            event.preventDefault();                                 // Megakadályozza az alapértelmezett form elküldést

            const email = document.getElementById("email").value;
            const password = document.getElementById("password").value;

            console.log("Bejelentkezési adatok: ", { email, password }); // Debug

            try {
                const response = await fetch(`${API_BASE_URL}/auth/sign-in?useTokens=true`, {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    credentials: "include",                                  // A süti miatt kell
                    body: JSON.stringify({ email, password })
                });

                console.log("API válasz státusz:", response.status);        // Debug

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




