// Bejelentkezés kezelése
document.addEventListener("DOMContentLoaded", () => {
  console.log("loginscript.js betöltve");

  const loginForm = document.getElementById("login-form");
  const emailInput = document.getElementById("email");
  const passwordInput = document.getElementById("password");

  if (loginForm) {
    loginForm.addEventListener("submit", function (e) {
      e.preventDefault();

      // Üzenet törlése Modal helyett, nincs már alertPlaceholder
      const email = emailInput.value.trim();
      const password = passwordInput.value.trim();
      let hasError = false;
      let errorMessage = "";

      // Jelszó formaellenőrzés ELTŰNIK innen
      // Csak ürességet nézem JS oldalon!

      if (!email) {
        emailInput.classList.add("is-invalid");
        hasError = true;
        errorMessage = "⚠️ Minden mező kitöltése kötelező!";
      }
      if (!password) {
        passwordInput.classList.add("is-invalid");
        hasError = true;
        errorMessage = "⚠️ Minden mező kitöltése kötelező!";
      }

      // Email formátum validálás (helyes email szintaktika)
      const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
      if (email && !emailPattern.test(email)) {
        emailInput.classList.add("is-invalid");
        hasError = true;
        errorMessage = "⚠️ Kérlek érvényes e-mail címet adj meg!";
      }

      if (hasError) {
        // Hiba esetén Bootstrap Modal üzenet
        $("#feedbackModalLabel").text("Hiba!");
        $("#feedbackModalBody").html(
          '<i class="bi bi-x-circle-fill text-danger" style="font-size: 2rem;"></i><br><br>' +
            errorMessage
        );
        $("#feedbackModal").modal("show");

        // Fókusz az első hibás mezőre
        if (emailInput.classList.contains("is-invalid")) {
          emailInput.focus();
        } else if (passwordInput.classList.contains("is-invalid")) {
          passwordInput.focus();
        }

        return;
      }

      // Ha minden valid, mehet a bejelentkezés
      fetch("http://localhost:5101/api/auth/sign-in", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        credentials: "include",
        body: JSON.stringify({ email, password }),
      })
        .then((response) => {
          if (response.ok) {
            // Sikeres bejelentkezés
            $("#feedbackModalLabel").text("Sikeres bejelentkezés!");
            $("#feedbackModalBody").html(
              '<i class="bi bi-check-circle-fill text-success" style="font-size: 2rem;"></i><br><br>✅ Sikeres bejelentkezés!'
            );
            $("#feedbackModal").modal("show");

            // Modal bezárása után átirányítás
            $("#feedbackModal")
              .off("hidden.bs.modal")
              .on("hidden.bs.modal", function () {
                window.location.href = "mainpage.php";
              });
          } else {
            return response.json().then((data) => {
              throw new Error(data.message || "Hibás bejelentkezési adatok.");
            });
          }
        })
        .catch((error) => {
          console.error("Hiba:", error);
          emailInput.classList.add("is-invalid");
          passwordInput.classList.add("is-invalid");

          $("#feedbackModalLabel").text("Hiba!");
          $("#feedbackModalBody").html(
            '<i class="bi bi-x-circle-fill text-danger" style="font-size: 2rem;"></i><br><br>❌ Bejelentkezés sikertelen: ' +
              error.message
          );
          $("#feedbackModal").modal("show");
        });
    });

    // Gépelés közben hibák törlése
    [emailInput, passwordInput].forEach((input) => {
      input.addEventListener("input", () => {
        input.classList.remove("is-invalid");
      });
    });
  }
});
