// Profil kártya és adatok dinamikus betöltése és kezelése
document.addEventListener("DOMContentLoaded", () => {
  const profileContainer = document.getElementById("profile-content");
  if (!profileContainer) return;

  fetch("http://localhost:5101/api/auth/me", {
    method: "GET",
    credentials: "include",
  })
    .then((response) => {
      if (!response.ok) throw new Error("Nem bejelentkezett állapot");
      return response.json();
    })
    .then((user) => {
      console.log("Profiladatok:", user);

      profileContainer.innerHTML = `
            <div class="card shadow mx-auto">
                <div class="card-body text-center">
                    <img src="https://cdn-icons-png.flaticon.com/512/149/149071.png" alt="Profilkép" width="120" class="mb-4 rounded-circle shadow-sm">
                    <h4 class="card-title mb-3">${user.email}</h4>
                    <p class="text-muted mb-1">Felhasználói azonosító: <strong>${user.id}</strong></p>
                    <p class="text-muted mb-3">Szerepkör: <span class="badge bg-info text-dark">${user.role}</span></p>

                    <hr class="my-4" style="border-color: rgba(0,0,0,0.1);">

                    <p class="text-muted mb-1"><strong>Fiók létrehozva:</strong> 2025.04.27</p>
                    <p class="text-muted mb-4"><strong>Legutóbbi aktivitás:</strong> 2025.04.27</p>

                    <a href="rentals.php" class="btn btn-warning mt-2">Foglalásaim megtekintése</a>
                    <p class="mt-4 text-muted small">Köszönjük, hogy a SkiRent szolgáltatásait választottad! 🎿</p>
                </div>
            </div>
        `;
    })
    .catch((error) => {
      console.error("❌ Nem sikerült betölteni a profiladatokat:", error);
      profileContainer.innerHTML = `<p class="text-danger">Nem vagy bejelentkezve, vagy hiba történt.</p>`;
    });
});
