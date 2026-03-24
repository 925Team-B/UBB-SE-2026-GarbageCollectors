
IF NOT EXISTS (
   SELECT * FROM sys.tables
        WHERE name = 'RateAlert'
)
BEGIN
    CREATE TABLE RateAlert (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        
        UserId INT NOT NULL,
        
        BaseCurrency VARCHAR(3) NOT NULL,
        TargetCurrency VARCHAR(3) NOT NULL,
                     
        TargetRate DECIMAL(18,6) NOT NULL,
        
        isTriggered BIT NOT NULL DEFAULT 0,
        
        isBuyAlert BIT NOT NULL,
    
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
    );
END
   