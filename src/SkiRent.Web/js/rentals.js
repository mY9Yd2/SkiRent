document.addEventListener("DOMContentLoaded", () => {
  const bookingList = document.getElementById("booking-list");
  const tableBody = document.getElementById("booking-table-body");

  fetch("http://localhost:5101/api/bookings", {
    method: "GET",
    credentials: "include",
  })
    .then((res) => {
      if (!res.ok) throw new Error("Nem sikerült lekérni a foglalásokat");
      return res.json();
    })
    .then((bookings) => {
      console.log("Foglalások:", bookings);

      tableBody.innerHTML = "";

      if (bookings.length === 0) {
        tableBody.innerHTML = `<tr><td colspan="4">Nincs még egyetlen foglalás sem.</td></tr>`;
        return;
      }

      bookings.forEach((booking, index) => {
        const createdDate = new Date(booking.createdAt).toLocaleDateString(
          "hu-HU"
        );
        const startDate = new Date(booking.startDate).toLocaleDateString(
          "hu-HU"
        );
        const endDate = new Date(booking.endDate).toLocaleDateString("hu-HU");

        const row = document.createElement("tr");
        /*const bookingId = booking.id;
            const invoiceId = booking.paymentId; // fallback, amíg invoice objektum nincs*/
        row.innerHTML = `
                <td>${index + 1}</td>
                <td>${createdDate}</td>
                <td>${startDate} - ${endDate}</td>
                <td>
                    <a href="#" class="text-primary">Foglalás részletei</a><br>
                    <a href="#" class="text-secondary invoice-link">Számla megtekintése</a>
                </td>
            `;

        const detailsLink = row.querySelector(".text-primary");
        detailsLink.addEventListener("click", (e) => {
          e.preventDefault();

          document.getElementById("detail-booking-id").textContent = booking.id;
          document.getElementById("detail-booking-date").textContent = new Date(
            booking.createdAt
          ).toLocaleString("hu-HU");
          document.getElementById(
            "detail-interval"
          ).textContent = `${startDate} - ${endDate}`;
          document.getElementById("detail-status").textContent = booking.status;
          document.getElementById("detail-price").textContent =
            booking.totalPrice;

          $("#bookingDetailsModal").modal("show"); // Bootstrap modális megnyitása jQuery-vel
        });

        const invoiceLink = row.querySelector(".invoice-link");
        invoiceLink.addEventListener("click", (e) => {
          e.preventDefault();

          fetch(`http://localhost:5101/api/bookings/${booking.id}`, {
            method: "GET",
            credentials: "include",
          })
            .then((res) => {
              if (!res.ok)
                throw new Error("Nem sikerült lekérni a foglalás részleteit.");
              return res.json();
            })
            .then((fullBooking) => {
              console.log("Foglalás részletes adatai:", fullBooking);
              renderInvoiceModal(fullBooking);
            })
            .catch((err) => {
              console.error("❌ Hiba a részletes foglalás lekérésénél:", err);
              alert("Nem sikerült megjeleníteni a számlát.");
            });
        });
        tableBody.appendChild(row);
      });
    })
    .catch((err) => {
      console.error("Hiba a foglalások betöltésekor:", err);
      bookingList.innerHTML = `
            <div class="alert alert-danger text-center">
                Nem sikerült betölteni a foglalásokat.
            </div>
        `;
    });
});

function renderInvoiceModal(booking) {
  const personalDetails = booking.personalDetails ?? {};
  const html = `
        <div style="display: flex; align-items: center; justify-content: space-between;">
            <img src="../assets/pictures/SkiRent_sieloLogo.jpg" alt="SkiRent logó" style="height: 60px;">
            <h2 style="text-align: right; flex-grow: 1;">Számla</h2>
        </div>

        <div style="margin-top: 1rem;">
           <p><strong>Vevő neve:</strong> ${
             personalDetails.fullName ?? "(nincs megadva)"
           }</p>
            <p><strong>Cím:</strong> ${
              personalDetails.address?.postalCode ?? ""
            } ${personalDetails.address?.city ?? ""}, ${
    personalDetails.address?.streetAddress ?? ""
  }</p>
        </div>

        <hr>

        <p><strong>Számla azonosító:</strong> ${booking.paymentId}</p>
        <p><strong>Dátum:</strong> ${new Date(booking.createdAt).toLocaleString(
          "hu-HU"
        )}</p>
        <p><strong>Felhasználó e-mail:</strong> ${
          window.loggedInUserEmail ?? "(nem elérhető)"
        }</p>

        <hr>

        <p><strong>Foglalás időtartama:</strong> ${new Date(
          booking.startDate
        ).toLocaleDateString("hu-HU")} - ${new Date(
    booking.endDate
  ).toLocaleDateString("hu-HU")}</p>
        <p><strong>Összesen fizetve:</strong> ${booking.totalPrice} Ft</p>
        <p><strong>Állapot:</strong> ${booking.status}</p>
        <p><strong>Tételek száma:</strong> ${
          booking.items?.length ?? "nem elérhető"
        } db</p>
    `;

  const downloadButton = document.getElementById("download-button");
  if (booking.status !== "Cancelled" && booking.status !== "Pending") {
    downloadButton.addEventListener('click', () => {
      const link = document.createElement("a");
      link.href = `http://localhost:5101/api/invoices/${booking.paymentId}`;
      link.click();
    });
    downloadButton.hidden = false;
  } else {
    downloadButton.hidden = true;
  }

  document.getElementById("invoice-content").innerHTML = html;
  const modal = new bootstrap.Modal(document.getElementById("invoiceModal"));
  modal.show();
}
