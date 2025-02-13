CREATE DATABASE SkiRent
    CHARACTER SET = 'utf8mb4'
    COLLATE = 'utf8mb4_unicode_ci';

USE SkiRent;

CREATE TABLE Users (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Email VARCHAR(100) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    UserRole ENUM('admin', 'customer') NOT NULL DEFAULT 'customer'
);
