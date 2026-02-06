using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace VendorManager
{
    public static class DatabaseInitializer
    {
        public static void InitializeDatabase()
        {
            try
            {
                string connectionString = System.Configuration.ConfigurationManager
                    .ConnectionStrings["VendorManagerConnection"].ConnectionString;

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string createDbSql = @"
                    IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'VendorManagerDB')
                    BEGIN
                        CREATE DATABASE VendorManagerDB;
                        PRINT 'База данных VendorManagerDB создана.';
                    END";

                    using (var command = new SqlCommand(createDbSql, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    connection.ChangeDatabase("VendorManagerDB");

                    string createVendorsTableSql = @"
                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'gmc_Vendors')
                    BEGIN
                        CREATE TABLE [gmc_Vendors] (
                            [id] INT IDENTITY(1,1) PRIMARY KEY,
                            [macs] NVARCHAR(8) NOT NULL UNIQUE,
                            [brand] NVARCHAR(255) NOT NULL,
                            CONSTRAINT CHK_macs_Format CHECK ([macs] LIKE '[0-9A-F][0-9A-F]:[0-9A-F][0-9A-F]:[0-9A-F][0-9A-F]')
                        );
                        
                        CREATE INDEX IX_gmc_Vendors_macs ON [gmc_Vendors]([macs]);
                        
                        INSERT INTO [gmc_Vendors] ([macs], [brand]) VALUES
                        ('02:42:BD', 'Broadcom Inc.'),
                        ('00:1A:11', 'Samsung Electronics'),
                        ('00:26:AB', 'Apple Inc.'),
                        ('00:50:56', 'VMware Inc.'),
                        ('08:00:27', 'Oracle VirtualBox');
                    END";

                    using (var command = new SqlCommand(createVendorsTableSql, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    string createIPMacTableSql = @"
                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'gmc_IPMAC')
                    BEGIN
                        CREATE TABLE [gmc_IPMAC] (
                            [Id] INT IDENTITY(1,1) PRIMARY KEY,
                            [Mac] NVARCHAR(17) NOT NULL,
                            [ip_cur] NVARCHAR(20) NULL,
                            [Inbase] BIT NULL,
                            [last_dateupdate] DATETIME NOT NULL DEFAULT GETDATE(),
                            CONSTRAINT CHK_Mac_Format CHECK ([Mac] LIKE '[0-9A-F][0-9A-F]:[0-9A-F][0-9A-F]:[0-9A-F][0-9A-F]:[0-9A-F][0-9A-F]:[0-9A-F][0-9A-F]:[0-9A-F][0-9A-F]')
                        );
                        
                        CREATE INDEX IX_gmc_IPMAC_Mac ON [gmc_IPMAC]([Mac]);
                        
                        INSERT INTO [gmc_IPMAC] ([Mac], [ip_cur], [Inbase]) VALUES
                        ('02:42:BD:01:02:03', '192.168.1.100', 1),
                        ('00:1A:11:AA:BB:CC', '192.168.1.101', 0),
                        ('00:26:AB:DD:EE:FF', '192.168.1.102', 1),
                        ('00:50:56:11:22:33', '192.168.1.103', NULL),
                        ('08:00:27:44:55:66', '192.168.1.104', 1);
                    END";

                    using (var command = new SqlCommand(createIPMacTableSql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации базы данных:\n{ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}