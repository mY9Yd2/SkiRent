// Kosár ürítése
sessionStorage.removeItem("cartItems");
sessionStorage.removeItem("startDate");
sessionStorage.removeItem("endDate");

// Ha külön mentett kosár darabszámot is:
sessionStorage.removeItem("cartCount");

console.log("✅ Kosár és foglalási adatok törölve a sessionStorage-ből.");
