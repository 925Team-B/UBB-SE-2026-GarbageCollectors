IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Biller')
BEGIN
    CREATE TABLE Biller (
        Id          INT             NOT NULL IDENTITY(1,1),
        Name        NVARCHAR(200)   NOT NULL,
        Category    NVARCHAR(50)    NOT NULL,
        LogoUrl     NVARCHAR(500)   NULL,
        IsActive    BIT             NOT NULL    DEFAULT 1,
        CONSTRAINT PK_Biller                PRIMARY KEY (Id),
        CONSTRAINT UQ_Biller_Name           UNIQUE      (Name),
        CONSTRAINT CK_Biller_Category       CHECK       (Category IN ('Utilities', 'Telecom', 'Insurance', 'Rent', 'Government', 'Subscriptions', 'Other'))
    );
END