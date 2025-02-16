<?php
session_start();

require_once "../config.php"; // Adatbáziskapcsolat betöltése


if ($_SERVER["REQUEST_METHOD"] == "POST") {
    $email = $_POST["email"];
    $password = $_POST["password"];


    // Felhasználó lekérdezése
    $stmt = $pdo->prepare("SELECT Id, Email, PasswordHash FROM Users WHERE Email = :email");
    $stmt->execute(["email" => $email]);
    $user = $stmt->fetch(PDO::FETCH_ASSOC);

    if ($user && password_verify($password, $user["PasswordHash"])) {
        $_SESSION["user_id"] = $user["Id"];
        $_SESSION["email"] = $user["Email"];
        header("Location: ../pages/mainpage.php"); // Sikeres bejelentkezés
        
        exit();

    } else {
        echo "<script>alert('Hibás e-mail vagy jelszó!'); window.location.href='../pages/login.php';</script>";
    }
}



?>