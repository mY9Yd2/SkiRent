CREATE DATABASE `SkiRent`
CHARACTER SET = 'utf8mb4'
COLLATE = 'utf8mb4_unicode_ci';

USE `SkiRent`;

CREATE TABLE `Users` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `Email` VARCHAR(100) NOT NULL UNIQUE,
    `PasswordHash` VARCHAR(255) NOT NULL,
    `UserRole` ENUM('Admin', 'Customer') NOT NULL DEFAULT 'Customer'
);

CREATE TABLE `EquipmentCategories` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `Name` VARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE `Equipments` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `Name` VARCHAR(100) NOT NULL,
    `Description` TEXT,
    `CategoryId` INT NOT NULL,
    `PricePerDay` DECIMAL(10, 2) NOT NULL,
    `AvailableQuantity` INT NOT NULL,
    FOREIGN KEY (`CategoryId`) REFERENCES `EquipmentCategories` (`Id`)
    ON DELETE RESTRICT
);

CREATE TABLE `EquipmentImages` (
    `Id` CHAR(36) PRIMARY KEY,
    `EquipmentId` INT NOT NULL,
    `DisplayName` VARCHAR(255) NOT NULL,
    FOREIGN KEY (`EquipmentId`) REFERENCES `Equipments` (`Id`)
    ON DELETE RESTRICT,
    UNIQUE (`EquipmentId`, `DisplayName`)
);

ALTER TABLE `Equipments`
ADD COLUMN `MainImageId` CHAR(36) NULL,
ADD FOREIGN KEY (`MainImageId`) REFERENCES `EquipmentImages` (`Id`)
ON DELETE SET NULL;

CREATE TABLE `Bookings` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `UserId` INT NOT NULL,
    `StartDate` DATE NOT NULL,
    `EndDate` DATE NOT NULL,
    `TotalPrice` DECIMAL(10, 2) NOT NULL,
    `PaymentId` CHAR(36) NOT NULL UNIQUE,
    `Status` ENUM(
        'Pending', 'Paid', 'Cancelled', 'Returned'
    ) NOT NULL DEFAULT 'Pending',
    `CreatedAt` TIMESTAMP DEFAULT UTC_TIMESTAMP(),
    FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`)
    ON DELETE RESTRICT
);

CREATE TABLE `BookingItems` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `BookingId` INT NOT NULL,
    `EquipmentId` INT NOT NULL,
    `NameAtBooking` VARCHAR(100) NOT NULL,
    `PriceAtBooking` DECIMAL(10, 2) NOT NULL,
    `Quantity` INT NOT NULL,
    FOREIGN KEY (`BookingId`) REFERENCES `Bookings` (`Id`)
    ON DELETE CASCADE,
    UNIQUE (`BookingId`, `EquipmentId`)
);

CREATE TABLE `Invoices` (
    `Id` CHAR(36) PRIMARY KEY,
    `UserId` INT NULL,
    `BookingId` INT NULL UNIQUE,
    `CreatedAt` TIMESTAMP DEFAULT UTC_TIMESTAMP(),
    FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`)
    ON DELETE SET NULL,
    FOREIGN KEY (`BookingId`) REFERENCES `Bookings` (`Id`)
    ON DELETE SET NULL
);
