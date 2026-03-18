IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'RecurringPayment')
BEGIN
    CREATE TABLE RecurringPayment (
        Id                  INT             NOT NULL IDENTITY(1,1),
        UserId              INT             NOT NULL,
        BillerId            INT             NOT NULL,
        SourceAccountId     INT             NOT NULL,
        Amount              DECIMAL(18,2)   NOT NULL,
        IsPayInFull         BIT             NOT NULL    DEFAULT 0,
        Frequency           NVARCHAR(20)    NOT NULL,
        StartDate           DATETIME2       NOT NULL,
        EndDate             DATETIME2       NULL,
        NextExecutionDate   DATETIME2       NOT NULL,
        Status              NVARCHAR(20)    NOT NULL    DEFAULT 'Active',
        CreatedAt           DATETIME2       NOT NULL    DEFAULT GETUTCDATE(),
        CONSTRAINT PK_RecurringPayment          PRIMARY KEY (Id),
        CONSTRAINT FK_RecurringPayment_Biller   FOREIGN KEY (BillerId) REFERENCES Biller(Id),
        CONSTRAINT CK_RecurringPayment_Freq     CHECK       (Frequency IN ('Weekly', 'Monthly', 'Quarterly')),
        CONSTRAINT CK_RecurringPayment_Amount   CHECK       (Amount > 0),
        CONSTRAINT CK_RecurringPayment_Status   CHECK       (Status IN ('Active', 'Paused', 'Cancelled'))
    );
END