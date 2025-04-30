document.addEventListener("DOMContentLoaded", () => {
  const startInput = document.getElementById("startDate");
  const endInput = document.getElementById("endDate");

  const startDate = sessionStorage.getItem("startDate");
  const endDate = sessionStorage.getItem("endDate");

  if (startDate) startInput.value = startDate;
  if (endDate) endInput.value = endDate;

  const items = JSON.parse(sessionStorage.getItem("cartItems") || "[]");

  const nameInput = document.getElementById("FullName");
  const phoneInput = document.getElementById("PhoneNumber");
  const mobilePhoneInput = document.getElementById("MobilePhoneNumber");
  const countryInput = document.getElementById("country");
  const zipInput = document.getElementById("postalCode");
  const cityInput = document.getElementById("city");
  const addressInput = document.getElementById("streetAddress");
  const submitBtn = document.querySelector("button[type='submit']");

  const touchedFields = new Set();

  function formatPhoneNumber(value) {
    let cleaned = value.replace(/\D/g, "");
    if (cleaned.startsWith("36")) cleaned = "+" + cleaned;
    if (!cleaned.startsWith("+36")) return value;

    if (cleaned.length === 11) {
      return cleaned.replace(/(\+36)(\d{1})(\d{3})(\d{3})/, "$1 $2 $3 $4");
    } else if (cleaned.length === 12) {
      return cleaned.replace(/(\+36)(\d{2})(\d{3})(\d{4})/, "$1 $2 $3 $4");
    } else {
      return value;
    }
  }

  function validateFullName(name) {
    const parts = name.trim().split(" ");
    if (parts.length !== 2) return false;
    return parts.every(
      (part) => part.length >= 2 && /^[A-ZÁÉÍÓÖŐÚÜŰ]/.test(part)
    );
  }

  function validatePhone(phone) {
    const cleaned = phone.replace(/\s/g, "");
    return /^\+36\d{8,9}$/.test(cleaned);
  }

  function validatePostalCode(zip) {
    return /^\d{4}$/.test(zip);
  }

  function validateText(text, minLength = 4) {
    return text.trim().length >= minLength;
  }

  function checkFormCompletion() {
    const fullNameValid = validateFullName(nameInput.value);
    const phoneValid = validatePhone(phoneInput.value);
    const mobilePhoneValid = validatePhone(mobilePhoneInput.value);
    const countryValid = validateText(countryInput.value);
    const postalValid = validatePostalCode(zipInput.value);
    const cityValid = validateText(cityInput.value);
    const addressValid = validateText(addressInput.value);
    const datesValid = startInput.value.trim() && endInput.value.trim();

    setValidationState(nameInput, fullNameValid);
    setValidationState(phoneInput, phoneValid);
    setValidationState(mobilePhoneInput, mobilePhoneValid);
    setValidationState(countryInput, countryValid);
    setValidationState(zipInput, postalValid);
    setValidationState(cityInput, cityValid);
    setValidationState(addressInput, addressValid);

    submitBtn.disabled = !(
      fullNameValid &&
      phoneValid &&
      mobilePhoneValid &&
      countryValid &&
      postalValid &&
      cityValid &&
      addressValid &&
      datesValid
    );
  }

  function setValidationState(input, isValid) {
    if (touchedFields.has(input.id)) {
      input.classList.toggle("is-invalid", !isValid);
    } else {
      input.classList.remove("is-invalid");
    }
  }

  // Telefonszám mező formázása blur eseményre (vezetékes)
  phoneInput.addEventListener("blur", () => {
    let raw = phoneInput.value.replace(/\D/g, ""); // Csak számok
    if (!raw.startsWith("36")) {
      raw = "36" + raw;
    }

    let formatted = "+36 ";

    if (raw.length >= 5) formatted += raw.slice(2, 4) + " ";
    if (raw.length >= 8) formatted += raw.slice(4, 7) + " ";
    if (raw.length > 7) formatted += raw.slice(7);

    phoneInput.value = formatted.trim();
  });

  mobilePhoneInput.addEventListener("blur", () => {
    mobilePhoneInput.value = formatPhoneNumber(mobilePhoneInput.value);
    touchedFields.add(mobilePhoneInput.id);
    checkFormCompletion();
  });

  // Minden inputra input + blur figyelés
  [
    nameInput,
    phoneInput,
    mobilePhoneInput,
    countryInput,
    zipInput,
    cityInput,
    addressInput,
  ].forEach((input) => {
    input.addEventListener("input", () => {
      touchedFields.add(input.id);
      checkFormCompletion();
    });
    input.addEventListener("blur", () => {
      touchedFields.add(input.id);
      checkFormCompletion();
    });
  });

  checkFormCompletion();

  const form = document.getElementById("checkout-form");

  form.addEventListener("submit", function (e) {
    e.preventDefault();

    const bookingData = {
      PersonalDetails: {
        FullName: nameInput.value.trim(),
        PhoneNumber: phoneInput.value.trim(),
        MobilePhoneNumber: mobilePhoneInput.value.trim(),
        Address: {
          Country: countryInput.value.trim(),
          PostalCode: zipInput.value.trim(),
          City: cityInput.value.trim(),
          StreetAddress: addressInput.value.trim(),
        },
      },
      Equipments: items.map((item) => ({
        EquipmentId: parseInt(item.id),
        Quantity: parseInt(item.quantity),
      })),
      StartDate: startInput.value,
      EndDate: endInput.value,
      SuccessUrl: window.location.origin + "/SkiRent.Web/pages/success.php",
      CancelUrl: window.location.origin + "/SkiRent.Web/pages/cancel.php",
    };

    sessionStorage.setItem(
      "lastBookingDetails",
      JSON.stringify(bookingData.PersonalDetails)
    );

    fetch("http://localhost:5101/api/bookings", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      credentials: "include",
      body: JSON.stringify(bookingData),
    })
      .then((response) => {
        if (!response.ok) {
          throw new Error("Foglalás sikertelen.");
        }
        return response.json();
      })
      .then((data) => {
        console.log("Foglalás sikeres:", data);
        if (data.paymentUrl) {
          window.location.href = data.paymentUrl;
        } else {
          alert("Sikeres foglalás, de nincs fizetési link.");
        }
      })
      .catch((error) => {
        console.error("Hiba:", error);
        alert("❌ Hiba történt a foglalás során: " + error.message);
      });

    console.log(
      " Foglalási JSON amit küldünk:",
      JSON.stringify(bookingData, null, 2)
    );
  });
});
