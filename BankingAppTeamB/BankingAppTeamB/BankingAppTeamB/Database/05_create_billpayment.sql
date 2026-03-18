IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'BillPayment')
BEGIN
    CREATE TABLE BillPayment (
        Id                  INT             NOT NULL IDENTITY(1,1),
        UserId              INT             NOT NULL,
        SourceAccountId     INT             NOT NULL,
        BillerId            INT             NOT NULL,
        TransactionId       INT             NULL,
        BillerReference     NVARCHAR(200)   NOT NULL,
        Amount              DECIMAL(18,2)   NOT NULL,
        Fee                 DECIMAL(18,2)   NOT NULL    DEFAULT 0,
        ReceiptNumber       NVARCHAR(100)   NOT NULL,
        Status              NVARCHAR(50)    NOT NULL,
        CreatedAt           DATETIME2       NOT NULL    DEFAULT GETUTCDATE(),
        CONSTRAINT PK_BillPayment               PRIMARY KEY (Id),
        CONSTRAINT FK_BillPayment_Biller        FOREIGN KEY (BillerId)       REFERENCES Biller(Id),
        CONSTRAINT FK_BillPayment_Transaction   FOREIGN KEY (TransactionId)  REFERENCES Transactions(Id),
        CONSTRAINT CK_BillPayment_Amount        CHECK       (Amount > 0),
        CONSTRAINT CK_BillPayment_Fee           CHECK       (Fee >= 0),
        CONSTRAINT CK_BillPayment_Status        CHECK       (Status IN ('Pending', 'Completed', 'Failed'))
    );
END