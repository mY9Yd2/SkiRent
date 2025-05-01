USE `SkiRent`;

INSERT INTO `Users` (`Email`, `PasswordHash`, `UserRole`) VALUES
-- https://github.com/dotnet/aspnetcore/blob/main/src/Identity/Extensions.Core/src/PasswordHasher.cs
('admin@example.com', 'AQAAAAIAAYagAAAAENew/SuQlJ+phKSg6bhiGnQiJe3TBYDfUvY7KSaaN4T8wDKvETd7mGv+ZSO11lB0oQ==', 'Admin'), -- Password: Admin1234
('teszt@example.com', 'AQAAAAIAAYagAAAAENGMc5UQKipvQgKP1Cbfz8c3d9AZkeh8PcLji9xrprohYkRb+7ysAsFspsN2LhegRA==', 'Customer'); -- Password: Teszt1234

INSERT INTO `EquipmentCategories` (`Id`, `Name`) VALUES
(1, 'Síléc'),
(2, 'Snowboard'),
(3, 'Bakancs'),
(4, 'Síbot');

INSERT INTO `EquipmentImages` (`Id`, `DisplayName`, `CreatedAt`, `UpdatedAt`) VALUES
-- Sílécek
('2c3bb9fa-edca-4522-9326-a70ac3b53325', 'Salomon X Drive Focus Rocker 130cm carving síléc', '2025-04-27', '2025-04-27'),
('79950ac6-ebfa-4f04-a85e-e5d0cb36a4d3', 'Atomic Race 8 140cm carving síléc', '2025-04-27', '2025-04-27'),
('011ade01-971a-46ef-867b-187b6b6031a2', 'Elan ExarPro 140cm carving síléc', '2025-04-27', '2025-04-27'),
-- ('0d09c95e-60eb-4728-b265-502790d792b4', 'Atomic Redster RX Rocker 156cm carving síléc Grip Walk', '2025-04-27', '2025-04-27'),
-- Snowboardok
('339a9ff0-fbd9-43a2-880e-19317fd712ba', 'Férfi snowboard allmountain/freeride - All Road 500', '2025-04-27', '2025-04-27'),
('a3688083-d85f-4152-be50-71caa6456a2a', 'Női freestyle és all mountain snowboard Endzone 500', '2025-04-27', '2025-04-27'),
('67fa5ff6-7bdd-4442-bb8c-b600c7657d7a', 'Női all mountain és freestyle snowboard - Dreamscape SNB100', '2025-04-27', '2025-04-27'),
('1c362460-628c-42d4-984d-bb286a790d3c', 'Férfi snowboard all mountain és freestyle síeléshez - Endzone 900 PRO', '2025-04-27', '2025-04-27'),
-- Bakancsok
('7695fc8e-59de-4ca7-b409-32687043780c', 'Nordica GP TJ 31 - 32 -es gyerek síbakancs', '2025-04-27', '2025-04-27'),
('2e31f44d-8d02-4622-9508-cf553ab30dda', 'TecnoPro T50.3 31-32 -es gyerek síbakancs', '2025-04-27', '2025-04-27'),
('7e3a0f52-fb54-4efd-85ba-c6dec3804b83', 'Roces Idea Up 36 - 40 -es méretű síbakancs, állítható', '2025-04-27', '2025-04-27'),
('ee5dc68b-d6e5-4785-bf33-b602cd87fb34', 'Lange Starlet 60 37 - 38 -as síbakancs', '2025-04-27', '2025-04-27'),
-- Síbotok
('70fe23e5-83c7-4237-80dd-8fd0ef4c8ba0', 'Síbot pályára, Boost 100 safety, fekete', '2025-04-27', '2025-04-27'),
('bcaf9653-b79c-45f7-a9a1-a773a71d1140', 'Síbot, pályára - Boost 500 Safety', '2025-04-27', '2025-04-27'),
('a79e42a3-d804-4a19-b969-61659a7db61b', 'Gyerek síbot, állítható, levehető csuklópánttal - Safety 500', '2025-04-27', '2025-04-27'),
('228609f5-9d51-4518-9134-05afdae95f0f', 'Fizan Action Pro síbot', '2025-04-27', '2025-04-27');

INSERT INTO `Equipments` (`Name`, `Description`, `CategoryId`, `MainImageId`, `PricePerDay`, `AvailableQuantity`) VALUES
-- Sílécek
('Salomon X Drive Focus Rocker 130cm carving síléc', NULL, 1, '2c3bb9fa-edca-4522-9326-a70ac3b53325', 1499, 5),
('Atomic Race 8 140cm carving síléc', 'Francia gyártású versenysíléc, profi síelők számára.', 1, '79950ac6-ebfa-4f04-a85e-e5d0cb36a4d3', 2499, 8),
('Elan ExarPro 140cm carving síléc', 'Kiváló minőségű síléc, gyors kanyarodáshoz.', 1, '011ade01-971a-46ef-867b-187b6b6031a2', 2099, 3),
('Atomic Redster RX Rocker 156cm carving síléc Grip Walk', 'Német gyártású versenysíléc.', 1, NULL/*'0d09c95e-60eb-4728-b265-502790d792b4'*/, 2300, 10),
-- Snowboardok
('Férfi snowboard allmountain/freeride - All Road 500', 'Snowboard deszka középhaladóknak a szép kanyarodáshoz, szűzhóra.', 2, '339a9ff0-fbd9-43a2-880e-19317fd712ba', 3499, 8),
('Női freestyle és all mountain snowboard Endzone 500', 'Játékos deszka parkba, pályára vagy freeride-hoz.', 2, 'a3688083-d85f-4152-be50-71caa6456a2a', 4499, 5),
('Női all mountain és freestyle snowboard - Dreamscape SNB100', NULL, 2, '67fa5ff6-7bdd-4442-bb8c-b600c7657d7a', 2299, 10),
('Férfi snowboard all mountain és freestyle síeléshez - Endzone 900 PRO', 'Ezt a deszkát Enzo Valax, profi freestyle és big air világkupa versenyzővel közösen fejlesztettük ki a profi és igényes versenyzők számára.', 2, '1c362460-628c-42d4-984d-bb286a790d3c', 5299, 3),
-- Bakancsok
('Nordica GP TJ 31 - 32 -es gyerek síbakancs', 'Norvég tervezésű síbakancs, kényelmes illeszkedéssel.', 3, '7695fc8e-59de-4ca7-b409-32687043780c', 1300, 10),
('TecnoPro T50.3 31-32 -es gyerek síbakancs', NULL, 3, '2e31f44d-8d02-4622-9508-cf553ab30dda', 1970, 7),
('Roces Idea Up 36 - 40 -es méretű síbakancs, állítható', NULL, 3, '7e3a0f52-fb54-4efd-85ba-c6dec3804b83', 1750, 4),
('Lange Starlet 60 37 - 38 -as síbakancs', 'Haladó síelőknek készült precíz bakancs.', 3, 'ee5dc68b-d6e5-4785-bf33-b602cd87fb34', 2400, 3),
-- Síbotok
('Síbot pályára, Boost 100 safety, fekete', 'A beépített csuklópántok megkönnyítik és biztonságosabbá teszik a síelést esés esetén.', 4, '70fe23e5-83c7-4237-80dd-8fd0ef4c8ba0', 1290, 8),
('Síbot, pályára - Boost 500 Safety', NULL, 4, 'bcaf9653-b79c-45f7-a9a1-a773a71d1140', 1550, 6),
('Gyerek síbot, állítható, levehető csuklópánttal - Safety 500', 'Ez a síbot együtt nő gyermekeddel. Könnyű és kényelmes fogást biztosít, még kesztyűben is.', 4, 'a79e42a3-d804-4a19-b969-61659a7db61b', 1150, 4),
('Fizan Action Pro síbot', NULL, 4, '228609f5-9d51-4518-9134-05afdae95f0f', 1830, 9);
