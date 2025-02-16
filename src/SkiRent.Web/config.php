<?php
$host = "localhost";    // Adatbázis szerver
$dbname = "SkiRent";    // Adatbázis neve
$username = "root";     // Adatbázis felhasználó (XAMPP esetén root)
$password = "";

try {
    $pdo = new PDO("mysql:host=$host;dbname=$dbname;charset=utf8mb4", $username, $password);
        $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
    } catch (PDOException $e) {
        die("Hiba az adatbázis kapcsolódásban: " . $e->getMessage());
    }

    
?>