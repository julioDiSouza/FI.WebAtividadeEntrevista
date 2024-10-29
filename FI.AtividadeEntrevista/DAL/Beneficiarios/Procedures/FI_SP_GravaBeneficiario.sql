CREATE PROCEDURE FI_SP_GravaBeneficiario
    @Beneficiarios BENEFICIARIOSTableType READONLY 
AS
BEGIN
    -- Start a transaction
    BEGIN TRY
        BEGIN TRANSACTION;
        
        DELETE FROM BENEFICIARIOS WHERE IDCLIENTE = (SELECT TOP 1 IDCLIENTE FROM @Beneficiarios);
        
        INSERT INTO BENEFICIARIOS (CPF, NOME, IDCLIENTE)
        SELECT CPF, NOME, IDCLIENTE 
        FROM @Beneficiarios;

        -- Commit the transaction if everything is successful
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        -- Roll back the transaction if any error occurs
        ROLLBACK TRANSACTION;

        -- Optional: Rethrow the error to handle it in the calling application
        THROW;
    END CATCH
END;