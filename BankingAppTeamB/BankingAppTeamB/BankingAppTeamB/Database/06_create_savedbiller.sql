IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'SavedBiller')
BEGIN
    CREATE TABLE SavedBiller (
        Id                  INT             NOT NULL IDENTITY(1,1),
        UserId              INT             NOT NULL,
        BillerId            INT             NOT NULL,
        Nickname            NVARCHAR(200)   NULL,
        DefaultReference    NVARCHAR(200)   NULL,
        CreatedAt           DATETIME2       NOT NULL    DEFAULT GETUTCDATE(),
        CONSTRAINT PK_SavedBiller           PRIMARY KEY (Id),
        CONSTRAINT FK_SavedBiller_Biller    FOREIGN KEY (BillerId) REFERENCES Biller(Id)
    );
END