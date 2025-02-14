USE SkiRent;

INSERT INTO Users (Email, PasswordHash, UserRole) VALUES
-- https://github.com/dotnet/aspnetcore/blob/main/src/Identity/Extensions.Core/src/PasswordHasher.cs
('admin@example.com', 'AQAAAAIAAYagAAAAENew/SuQlJ+phKSg6bhiGnQiJe3TBYDfUvY7KSaaN4T8wDKvETd7mGv+ZSO11lB0oQ==', 'admin'), -- Password: Admin1234
('teszt@example.com', 'AQAAAAIAAYagAAAAENGMc5UQKipvQgKP1Cbfz8c3d9AZkeh8PcLji9xrprohYkRb+7ysAsFspsN2LhegRA==', 'customer'); -- Password: Teszt1234

INSERT INTO EquipmentCategories (Id, Name) VALUES
(1, 'Síléc'),
(2, 'Snowboard'),
(3, 'Bakancs'),
(4, 'Síbot');

INSERT INTO Equipments (Name, Description, CategoryId, PricePerDay, AvailableQuantity) VALUES
-- Sílécek
('Salomon X Drive Focus Rocker 130cm carving síléc', NULL, 1, 1499, 5),
('Atomic Race 8 140cm carving síléc', 'Francia gyártású versenysíléc, profi síelők számára.', 1, 2499, 8),
('Elan ExarPro 140cm carving síléc', 'Kiváló minőségű síléc, gyors kanyarodáshoz.', 1, 2099, 3),
('Atomic Redster RX Rocker 156cm carving síléc Grip Walk', 'Német gyártású versenysíléc.', 1, 2300, 10),
-- Snowboardok
('Férfi snowboard allmountain/freeride - All Road 500', 'Snowboard deszka középhaladóknak a szép kanyarodáshoz, szűzhóra.', 2, 3499, 8),
('Női freestyle és all mountain snowboard Endzone 500', 'Játékos deszka parkba, pályára vagy freeride-hoz.', 2, 4499, 5),
('Női all mountain és freestyle snowboard - Dreamscape SNB100', NULL, 2, 2299, 10),
('Férfi snowboard all mountain és freestyle síeléshez - Endzone 900 PRO', 'Ezt a deszkát Enzo Valax, profi freestyle és big air világkupa versenyzővel közösen fejlesztettük ki a profi és igényes versenyzők számára.', 2, 5299, 3),
-- Bakancsok
('Nordica GP TJ 31 - 32 -es gyerek síbakancs', 'Norvég tervezésű síbakancs, kényelmes illeszkedéssel.', 3, 1300, 10),
('TecnoPro T50.3 31-32 -es gyerek síbakancs', NULL, 3, 1970, 7),
('Roces Idea Up 36 - 40 -es méretű síbakancs, állítható', NULL, 3, 1750, 4),
('Lange Starlet 60 37 - 38 -as síbakancs', 'Haladó síelőknek készült precíz bakancs.', 3, 2400, 3),
-- Síbotok
('Síbot pályára, Boost 100 safety, fekete', 'A beépített csuklópántok megkönnyítik és biztonságosabbá teszik a síelést esés esetén.', 4, 1290, 8),
('Síbot, pályára - Boost 500 Safety', NULL, 4, 1550, 6),
('Gyerek síbot, állítható, levehető csuklópánttal - Safety 500', 'Ez a síbot együtt nő gyermekeddel. Könnyű és kényelmes fogást biztosít, még kesztyűben is.', 4, 1150, 4),
('Fizan Action Pro síbot', NULL, 4, 1830, 9);
