# PowerShell script to update cards table with new columns
$serverName = "localhost\SQLEXPRESS"
$databaseName = "BFASDatabase"

$connectionString = "Server=$serverName;Database=$databaseName;Integrated Security=True;TrustServerCertificate=True;"

Write-Host "Connecting to SQL Server..." -ForegroundColor Cyan

try {
    $connection = New-Object System.Data.SqlClient.SqlConnection
    $connection.ConnectionString = $connectionString
    $connection.Open()

    Write-Host "Connected successfully!" -ForegroundColor Green

    # SQL to add new columns to cards table
    $sql = @"
    USE BFASDatabase;

    -- Check if balance column exists, if not add it
    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'cards' AND COLUMN_NAME = 'balance')
    BEGIN
        ALTER TABLE cards ADD balance DECIMAL(18,2) NOT NULL DEFAULT 0;
        PRINT 'Added balance column to cards table';
    END
    ELSE
    BEGIN
        PRINT 'balance column already exists';
    END

    -- Check if is_primary column exists, if not add it
    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'cards' AND COLUMN_NAME = 'is_primary')
    BEGIN
        ALTER TABLE cards ADD is_primary BIT NOT NULL DEFAULT 0;
        PRINT 'Added is_primary column to cards table';
    END
    ELSE
    BEGIN
        PRINT 'is_primary column already exists';
    END

    -- Check if updated_at column exists, if not add it
    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'cards' AND COLUMN_NAME = 'updated_at')
    BEGIN
        ALTER TABLE cards ADD updated_at DATETIME NOT NULL DEFAULT GETDATE();
        PRINT 'Added updated_at column to cards table';
    END
    ELSE
    BEGIN
        PRINT 'updated_at column already exists';
    END

    -- Set the first card of each account as primary if no primary exists
    UPDATE c1
    SET c1.is_primary = 1
    FROM cards c1
    INNER JOIN (
        SELECT account_id, MIN(card_id) as first_card_id
        FROM cards
        WHERE card_status = 'Active'
        GROUP BY account_id
    ) c2 ON c1.account_id = c2.account_id AND c1.card_id = c2.first_card_id
    WHERE NOT EXISTS (
        SELECT 1 FROM cards c3 
        WHERE c3.account_id = c1.account_id AND c3.is_primary = 1
    );

    PRINT 'Updated schema successfully!';
    PRINT 'Cards table now has balance, is_primary, and updated_at columns';
"@

    $command = $connection.CreateCommand()
    $command.CommandText = $sql
    $result = $command.ExecuteNonQuery()

    Write-Host "`nDatabase updated successfully!" -ForegroundColor Green
    Write-Host "New columns added to cards table:" -ForegroundColor Yellow
    Write-Host "  - balance (DECIMAL(18,2))" -ForegroundColor White
    Write-Host "  - is_primary (BIT)" -ForegroundColor White
    Write-Host "  - updated_at (DATETIME)" -ForegroundColor White

    $connection.Close()
}
catch {
    Write-Host "Error: $_" -ForegroundColor Red
    if ($connection.State -eq 'Open') {
        $connection.Close()
    }
}

Write-Host "`nPress any key to continue..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
