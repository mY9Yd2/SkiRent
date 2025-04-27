// Regisztráció kezelése
document.addEventListener("DOMContentLoaded", () => {
  console.log("regscript.js betöltve");

  const registerForm = document.getElementById("register-form");
  const fullNameInput = document.getElementById("fullName");
  const emailInput = document.getElementById("email");
  const passwordInput = document.getElementById("password");
  const confirmPasswordInput = document.getElementById("confirmPassword");

  if (registerForm) {
    registerForm.addEventListener("submit", function (e) {
      e.preventDefault();

      const alertPlaceholder = document.getElementById("alert-placeholder");
      alertPlaceholder.innerHTML = "";

      const fullName = fullNameInput.value.trim();
      const email = emailInput.value.trim();
      const password = passwordInput.value.trim();
      const confirmPassword = confirmPasswordInput.value.trim();

      let hasError = false;
      let errorMessage = "";

      // === Teljes név validáció ===
      const nameParts = fullName.split(" ");
      if (
        nameParts.length !== 2 ||
        nameParts[0].length < 2 ||
        nameParts[1].length < 2 ||
        nameParts[0][0] !== nameParts[0][0].toUpperCase() ||
        nameParts[1][0] !== nameParts[1][0].toUpperCase()
      ) {
        fullNameInput.classList.add("is-invalid");
        hasError = true;
        errorMessage =
          "A név két részből álljon, 1 szóközzel elválasztva, mindkét rész minimum 2 betűs, és nagy kezdőbetűvel kezdődjön.";
      }

      // === E-mail validáció ===
      const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
      if (!email || email.length < 4 || !emailPattern.test(email)) {
        emailInput.classList.add("is-invalid");
        if (!hasError) {
          hasError = true;
          errorMessage = "Érvényes e-mail címet adj meg!";
        }
      }

      // === Jelszó validáció ===
      const hasUpperCase = /[A-Z]/.test(password);
      if (!password || password.length < 3 || !hasUpperCase) {
        passwordInput.classList.add("is-invalid");
        if (!hasError) {
          hasError = true;
          errorMessage =
            "A jelszónak legalább 3 karakter hosszúnak kell lennie és tartalmaznia kell nagybetűt!";
        }
      }

      // === Jelszó megerősítés validáció ===
      if (!confirmPassword || password !== confirmPassword) {
        confirmPasswordInput.classList.add("is-invalid");
        if (!hasError) {
          hasError = true;
          errorMessage = "A jelszavak nem egyeznek!";
        }
      }

      if (hasError) {
        alertPlaceholder.innerHTML = `
          <div class="alert alert-danger" role="alert">
            ${errorMessage}
          </div>
        `;

        // Fókusz az első hibás mezőre
        if (fullNameInput.classList.contains("is-invalid")) {
          fullNameInput.focus();
        } else if (emailInput.classList.contains("is-invalid")) {
          emailInput.focus();
        } else if (passwordInput.classList.contains("is-invalid")) {
          passwordInput.focus();
        } else if (confirmPasswordInput.classList.contains("is-invalid")) {
          confirmPasswordInput.focus();
        }

        return;
      }

      // Ha minden valid, mehet a regisztráció
      fetch("http://localhost:5101/api/users", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          fullName: fullName,
          email: email,
          password: password,
        }),
      })
        .then((response) => {
          if (response.ok) {
            alert("✅ Sikeres regisztráció!");
            window.location.href = "login.php";
          } else {
            return response.json().then((data) => {
              throw new Error(
                data.message || "Hiba történt a regisztráció során."
              );
            });
          }
        })
        .catch((error) => {
          console.error("Hiba:", error);
          alert("❌ Sikertelen regisztráció: " + error.message);
        });
    });

    // Gépelés közben törli a hibákat
    [fullNameInput, emailInput, passwordInput, confirmPasswordInput].forEach(
      (input) => {
        input.addEventListener("input", () => {
          input.classList.remove("is-invalid");
          document.getElementById("alert-placeholder").innerHTML = "";
        });
      }
    );
  }
});
