

CREATE TABLE Vehicle (
    id INT IDENTITY(1, 1) PRIMARY KEY,
    make VARCHAR(255) NOT NULL,
    model VARCHAR(255) NOT NULL,
    year INT NOT NULL,
    mileage INT NOT NULL,
    vin VARCHAR(255) NOT NULL,
    user_id INT NOT NULL,
    FOREIGN KEY (user_id) REFERENCES Users(id)
);

CREATE TABLE Maintenance (
    id INT IDENTITY(1, 1) PRIMARY KEY,
    vehicle_id INT NOT NULL,
    maintenance_date DATETIME NOT NULL,
    maintenance_type VARCHAR(255) NOT NULL,
    maintenance_description VARCHAR(255) NOT NULL,
    maintenance_cost DECIMAL(10, 2) NOT NULL,
    next_maintenance_date DATETIME NOT NULL,
    next_maintenance_type VARCHAR(255) NOT NULL,
    FOREIGN KEY (vehicle_id) REFERENCES Vehicle(id)
);

CREATE TABLE Users (
    id INT IDENTITY(1, 1) PRIMARY KEY,
    username VARCHAR(255) NOT NULL,
    password VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL,
    first_name VARCHAR(255) NOT NULL,
    last_name VARCHAR(255) NOT NULL,
    phone_number VARCHAR(255) NOT NULL,
    address VARCHAR(255) NOT NULL,
);


CREATE TABLE Breakdown (
    id INT IDENTITY(1, 1) PRIMARY KEY,
    vehicle_id INT NOT NULL,
    breakdown_type VARCHAR(255) NOT NULL,
    repair_cost DECIMAL(10, 2) NOT NULL,
    description VARCHAR(255) NOT NULL,
    can_fix BIT NOT NULL,
    FOREIGN KEY (vehicle_id) REFERENCES Vehicle(id)
);

ALTER TABLE Breakdown
ADD installed_parts VARCHAR(255) NULL;

CREATE PROCEDURE sp_InsertUser(
    @in_username VARCHAR(255),
    @in_password VARCHAR(255),
    @in_email VARCHAR(255),
    @in_first_name VARCHAR(255),
    @in_last_name VARCHAR(255),
    @in_phone_number VARCHAR(255),
    @in_address VARCHAR(255)
)
AS
BEGIN
DECLARE @RowCount int = 0

SET @RowCount = (SELECT COUNT(1) FROM dbo.Users WHERE username = @in_username)
	BEGIN TRY
		BEGIN TRAN
		IF (@RowCount = 0)
			BEGIN
				INSERT INTO Users (username, password, email, first_name, last_name, phone_number, address)
				VALUES (@in_username, @in_password, @in_email, @in_first_name, @in_last_name, @in_phone_number, @in_address);
			END
		COMMIT TRAN
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN
		SELECT ERROR_MESSAGE()
	END CATCH    
END;


CREATE PROCEDURE sp_InsertVehicle
(
    @make VARCHAR(255),
    @model VARCHAR(255),
    @year INT,
    @mileage INT,
    @vin VARCHAR(255),
    @user_id INT
)
AS
BEGIN
    BEGIN TRY
        IF NOT EXISTS(SELECT * FROM Vehicle WHERE vin = @vin)
        BEGIN
            INSERT INTO Vehicle (make, model, year, mileage, vin, user_id)
            VALUES (@make, @model, @year, @mileage, @vin, @user_id);
        END
        ELSE
        BEGIN
            THROW 51000, 'A vehicle with this VIN already exists.', 1;
        END
    END TRY
    BEGIN CATCH
        PRINT 'Error: ' + ERROR_MESSAGE();
    END CATCH
END

CREATE PROCEDURE sp_InsertMaintenance
(
    @vehicle_id INT,
    @maintenance_date DATETIME,
    @maintenance_type VARCHAR(255),
    @maintenance_description VARCHAR(255),
    @maintenance_cost DECIMAL(10, 2),
    @next_maintenance_date DATETIME,
    @next_maintenance_type VARCHAR(255)
)
AS
BEGIN
    BEGIN TRY
        IF NOT EXISTS(SELECT * FROM Maintenance WHERE vehicle_id = @vehicle_id AND maintenance_type = @maintenance_type AND maintenance_date = @maintenance_date)
        BEGIN
            INSERT INTO Maintenance (vehicle_id, maintenance_date, maintenance_type, maintenance_description, maintenance_cost, next_maintenance_date, next_maintenance_type)
            VALUES (@vehicle_id, @maintenance_date, @maintenance_type, @maintenance_description, @maintenance_cost, @next_maintenance_date, @next_maintenance_type);
        END
        ELSE
        BEGIN
            THROW 51000, 'A maintenance record with this type and date for this vehicle already exists.', 1;
        END
    END TRY
    BEGIN CATCH
        PRINT 'Error: ' + ERROR_MESSAGE();
    END CATCH
END

CREATE PROCEDURE sp_InsertBreakdown
(
    @vehicle_id INT,
    @breakdown_date DATETIME,
    @breakdown_type VARCHAR(255),
    @repair_cost DECIMAL(10, 2),
    @description VARCHAR(255),
    @past_breakdowns VARCHAR(255),
    @installed_parts VARCHAR(255)
)
AS
BEGIN
    BEGIN TRY
        IF NOT EXISTS(SELECT * FROM Breakdown WHERE vehicle_id = @vehicle_id AND breakdown_type = @breakdown_type AND breakdown_date = @breakdown_date)
        BEGIN
            INSERT INTO Breakdown (vehicle_id, breakdown_date, breakdown_type, repair_cost, description, past_breakdowns, installed_parts)
            VALUES (@vehicle_id, @breakdown_date, @breakdown_type, @repair_cost, @description, @past_breakdowns, @installed_parts);
        END
        ELSE
        BEGIN
            THROW 51000, 'A breakdown record with this type for this vehicle already exists.', 1;
        END
    END TRY
    BEGIN CATCH
        PRINT 'Error: ' + ERROR_MESSAGE();
    END CATCH
END



CREATE PROCEDURE sp_GetAllUsers
AS
BEGIN
    DECLARE @RowCount int = 0

SET @RowCount = (SELECT COUNT(1) FROM dbo.Users)
	BEGIN TRY
		BEGIN TRAN
		IF (@RowCount = 0)
			BEGIN
				SELECT id, username, email, first_name, last_name, phone_number, address FROM Users;
			END
		COMMIT TRAN
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN
		SELECT ERROR_MESSAGE()
	END CATCH    
END;


CREATE PROCEDURE sp_GetVehiclesByUserId
    @user_id INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        SELECT id, make, model, year, mileage, vin
        FROM Vehicle
        WHERE user_id = @user_id;
    END TRY
    BEGIN CATCH
        -- Handle the error
        PRINT 'Error occurred: ' + ERROR_MESSAGE();
    END CATCH;
END



CREATE PROCEDURE sp_GetMaintenanceByUserId
    @user_id INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF EXISTS(SELECT 1 FROM [Users] WHERE id = @user_id)
        BEGIN
            SELECT id, vehicle_id, maintenance_date, maintenance_type, maintenance_description, maintenance_cost, next_maintenance_date, next_maintenance_type
            FROM Maintenance
            WHERE vehicle_id IN (SELECT id FROM Vehicle WHERE user_id = @user_id)
            ORDER BY maintenance_date DESC;
        END
        ELSE
        BEGIN
            SELECT 'User not found' AS Error;
        END
    END TRY
    BEGIN CATCH
        SELECT ERROR_MESSAGE() AS Error;
    END CATCH;
END



CREATE PROCEDURE sp_GetBreakdownByUserId
    @userId INT
AS
BEGIN
    BEGIN TRY
        SELECT b.id, b.breakdown_date, b.breakdown_type, b.repair_cost, b.description, b.past_breakdowns, b.installed_parts
        FROM Breakdown b
        INNER JOIN Vehicle vehicle ON b.vehicle_id = vehicle.id
        WHERE vehicle.user_id = @userId;
    END TRY
    BEGIN CATCH
        PRINT 'Error retrieving breakdown details.';
    END CATCH
END



CREATE PROCEDURE sp_UpdateUser(
    @id INT,
    @username VARCHAR(255),
    @password VARCHAR(255),
    @email VARCHAR(255),
    @first_name VARCHAR(255),
    @last_name VARCHAR(255),
    @phone_number VARCHAR(255),
    @address VARCHAR(255)
)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @count INT;
    SELECT @count = COUNT(*) FROM Users WHERE id <> @id AND username = @username;
    
    IF @count = 0
    BEGIN
        UPDATE Users SET 
            username = @username, 
            password = @password, 
            email = @email, 
            first_name = @first_name, 
            last_name = @last_name, 
            phone_number = @phone_number, 
            address = @address 
        WHERE id = @id;
        
        SELECT 'User details updated successfully.' AS message;
    END
    ELSE
    BEGIN
        SELECT 'User with the same username already exists.' AS message;
    END
END



CREATE PROCEDURE sp_UpdateVehicle (
    @id INT,
    @make VARCHAR(255),
    @model VARCHAR(255),
    @year INT,
    @mileage INT,
    @vin VARCHAR(255),
    @user_id INT
)
AS
BEGIN
    SET NOCOUNT ON;
    
    -- check if the VIN is already in use
    IF EXISTS (SELECT * FROM Vehicle WHERE vin = @vin AND id <> @id)
    BEGIN
        RAISERROR('A vehicle with this VIN already exists in the system.', 16, 1)
        RETURN
    END
    
    -- update the record
    UPDATE Vehicle SET 
        make = @make,
        model = @model,
        year = @year,
        mileage = @mileage,
        vin = @vin,
        user_id = @user_id
    WHERE id = @id
    
    -- return the updated record
    SELECT id, make, model, year, mileage, vin, user_id FROM Vehicle WHERE id = @id
END



CREATE PROCEDURE sp_UpdateBreakdown
(
    @breakdown_id INT,
    @vehicle_id INT,
    @breakdown_date DATETIME,
    @breakdown_type VARCHAR(255),
    @repair_cost DECIMAL(10, 2),
    @description VARCHAR(255),
    @past_breakdowns VARCHAR(255),
    @installed_parts VARCHAR(255)
)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Check if the breakdown record already exists
        IF NOT EXISTS(SELECT 1 FROM Breakdown WHERE id = @breakdown_id)
        BEGIN
            RAISERROR('The breakdown record with id %d does not exist.', 16, 1, @breakdown_id);
            RETURN;
        END

        -- Update the breakdown record
        UPDATE Breakdown
        SET breakdown_date = @breakdown_date,
		    breakdown_type = @breakdown_type,
            repair_cost = @repair_cost,
            description = @description,
            past_breakdowns = @past_breakdowns,
            installed_parts = @installed_parts
        WHERE id = @breakdown_id;
    END TRY
    BEGIN CATCH
        PRINT 'Error: ' + ERROR_MESSAGE();
    END CATCH;
END


CREATE PROCEDURE [dbo].[sp_UpdateMaintenance] 
	@id int,
	@vehicle_id int,
	@maintenance_date datetime,
	@maintenance_type varchar(255),
	@maintenance_description varchar(255),
	@maintenance_cost decimal(10, 2),
	@next_maintenance_date datetime,
	@next_maintenance_type varchar(255)
AS
BEGIN
	IF NOT EXISTS (SELECT * FROM [dbo].[Maintenance] WHERE [id] = @id)
	BEGIN
		RAISERROR('Maintenance record not found', 16, 1)
		RETURN
	END
	
	UPDATE [dbo].[Maintenance] SET 
		[vehicle_id] = @vehicle_id,
		[maintenance_date] = @maintenance_date,
		[maintenance_type] = @maintenance_type,
		[maintenance_description] = @maintenance_description,
		[maintenance_cost] = @maintenance_cost,
		[next_maintenance_date] = @next_maintenance_date,
		[next_maintenance_type] = @next_maintenance_type
	WHERE [id] = @id
END


CREATE PROCEDURE sp_DeleteUser
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF EXISTS (SELECT id FROM Users WHERE id = @id)
        BEGIN
            DELETE FROM Users WHERE id = @id;
            SELECT 'User deleted successfully.';
        END
        ELSE
        BEGIN
            SELECT 'User does not exist.';
        END
    END TRY
    BEGIN CATCH
        SELECT 'Error occurred while deleting user.';
    END CATCH
END


CREATE PROCEDURE sp_DeleteVehicle
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF EXISTS (SELECT id FROM Vehicle WHERE id = @id)
        BEGIN
            DELETE FROM Vehicle WHERE id = @id;
            SELECT 'Vehicle deleted successfully.';
        END
        ELSE
        BEGIN
            SELECT 'Vehicle does not exist.';
        END
    END TRY
    BEGIN CATCH
        SELECT 'Error occurred while deleting vehicle.';
    END CATCH
END


CREATE PROCEDURE sp_DeleteMaintenance
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF EXISTS (SELECT id FROM Maintenance WHERE id = @id)
        BEGIN
            DELETE FROM Maintenance WHERE id = @id;
            SELECT 'Maintenance record deleted successfully.';
        END
        ELSE
        BEGIN
            SELECT 'Maintenance record does not exist.';
        END
    END TRY
    BEGIN CATCH
        SELECT 'Error occurred while deleting maintenance record.';
    END CATCH
END


CREATE PROCEDURE sp_DeleteBreakdown
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF EXISTS (SELECT id FROM Breakdown WHERE id = @id)
        BEGIN
            DELETE FROM Breakdown WHERE id = @id;
            SELECT 'Breakdown record deleted successfully.';
        END
        ELSE
        BEGIN
            SELECT 'Breakdown record does not exist.';
        END
    END TRY
    BEGIN CATCH
        SELECT 'Error occurred while deleting breakdown record.';
    END CATCH
END


CREATE PROCEDURE sp_DeleteUserRecords
    @userId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @vehicleCount INT, @maintenanceCount INT, @breakdownCount INT;
    
    -- Check if there are any records in Vehicle table belonging to the user id
    SELECT @vehicleCount = COUNT(*) FROM Vehicle WHERE user_id = @userId;
    
    -- Check if there are any records in Maintenance table belonging to the user id
    SELECT @maintenanceCount = COUNT(*) FROM Maintenance WHERE vehicle_id IN (SELECT id FROM Vehicle WHERE user_id = @userId);
    
    -- Check if there are any records in Breakdown table belonging to the user id
    SELECT @breakdownCount = COUNT(*) FROM Breakdown WHERE vehicle_id IN (SELECT id FROM Vehicle WHERE user_id = @userId);
    
    IF @vehicleCount > 0
    BEGIN
        DELETE FROM Vehicle WHERE user_id = @userId;
    END
    
    IF @maintenanceCount > 0
    BEGIN
        DELETE FROM Maintenance WHERE vehicle_id IN (SELECT id FROM Vehicle WHERE user_id = @userId);
    END
    
    IF @breakdownCount > 0
    BEGIN
        DELETE FROM Breakdown WHERE vehicle_id IN (SELECT id FROM Vehicle WHERE user_id = @userId);
    END
    
    SELECT CONCAT(@vehicleCount + @maintenanceCount + @breakdownCount, ' records deleted.') AS 'Message';
END
