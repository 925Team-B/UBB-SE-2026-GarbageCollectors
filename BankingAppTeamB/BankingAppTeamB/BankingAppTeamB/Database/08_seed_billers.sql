IF NOT EXISTS (SELECT 1 FROM Biller WHERE Name = 'Enel Energie')
    INSERT INTO Biller (Name, Category) VALUES ('Enel Energie', 'Utilities');

IF NOT EXISTS (SELECT 1 FROM Biller WHERE Name = 'E.ON Energie Romania')
    INSERT INTO Biller (Name, Category) VALUES ('E.ON Energie Romania', 'Utilities');

IF NOT EXISTS (SELECT 1 FROM Biller WHERE Name = 'Orange Romania')
    INSERT INTO Biller (Name, Category) VALUES ('Orange Romania', 'Telecom');

IF NOT EXISTS (SELECT 1 FROM Biller WHERE Name = 'Vodafone Romania')
    INSERT INTO Biller (Name, Category) VALUES ('Vodafone Romania', 'Telecom');

IF NOT EXISTS (SELECT 1 FROM Biller WHERE Name = 'Digi Communications')
    INSERT INTO Biller (Name, Category) VALUES ('Digi Communications', 'Telecom');

IF NOT EXISTS (SELECT 1 FROM Biller WHERE Name = 'Allianz Romania')
    INSERT INTO Biller (Name, Category) VALUES ('Allianz Romania', 'Insurance');

IF NOT EXISTS (SELECT 1 FROM Biller WHERE Name = 'NN Asigurari')
    INSERT INTO Biller (Name, Category) VALUES ('NN Asigurari', 'Insurance');

IF NOT EXISTS (SELECT 1 FROM Biller WHERE Name = 'ANAF')
    INSERT INTO Biller (Name, Category) VALUES ('ANAF', 'Government');

IF NOT EXISTS (SELECT 1 FROM Biller WHERE Name = 'Primaria Cluj-Napoca')
    INSERT INTO Biller (Name, Category) VALUES ('Primaria Cluj-Napoca', 'Government');

IF NOT EXISTS (SELECT 1 FROM Biller WHERE Name = 'Netflix')
    INSERT INTO Biller (Name, Category) VALUES ('Netflix', 'Subscriptions');

IF NOT EXISTS (SELECT 1 FROM Biller WHERE Name = 'Spotify')
    INSERT INTO Biller (Name, Category) VALUES ('Spotify', 'Subscriptions');

IF NOT EXISTS (SELECT 1 FROM Biller WHERE Name = 'Administratie Bloc')
    INSERT INTO Biller (Name, Category) VALUES ('Administratie Bloc', 'Rent');