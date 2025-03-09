<?php
session_start();
session_unset(); // Minden session adat törlése
session_destroy(); // Session lezárása

// Visszairányítás a kezdőlapra
header("Location: ../index.php");
exit();


?>