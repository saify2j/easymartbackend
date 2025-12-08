-- Utility Queries

-- Get columns of the table as CSV

-- MSSQL
SELECT STRING_AGG(COLUMN_NAME, ', ') AS Columns
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = 'dbo'
  AND TABLE_NAME = 'Products';

-- PostgreSQL
SELECT STRING_AGG(column_name, ', ') AS columns
FROM information_schema.columns
WHERE table_schema = 'public'
  AND table_name = 'products'


-- Prepare a dummy insert statement 

-- MSSQL
SELECT 
    'INSERT INTO ' + @schema + '.' + @table + ' (' +
    STRING_AGG(COLUMN_NAME, ', ') +
    ') VALUES (' +
    STRING_AGG('NULL', ', ') +
    ');' AS InsertStatement
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = @schema
  AND TABLE_NAME = @table;


-- PostgreSQL
SELECT 
    'INSERT INTO ' || table_schema || '.' || table_name || ' (' ||
    STRING_AGG(column_name, ', ') ||
    ') VALUES (' ||
    STRING_AGG('NULL', ', ') ||
    ');'
FROM information_schema.columns
WHERE table_schema = 'public'
  AND table_name = 'products'
GROUP BY table_schema, table_name;