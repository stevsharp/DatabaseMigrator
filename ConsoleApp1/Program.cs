using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                MigrateDatabase();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.ReadLine();
        }

        private static void MigrateDatabase()
        {
            string filter = "ConsoleApp1.db.migrations";

            var connectionString = "Server=DESKTOP-FFHEA6P;Database=TourManagementDB;Trusted_Connection=True;";
            try
            {
                var cn = new SqlConnection(connectionString);
                var evolve = new Evolve.Evolve(cn, msg => Log(msg))
                {
                    EmbeddedResourceAssemblies = new[] { typeof(Program).Assembly },
                    EmbeddedResourceFilters = new[] { filter },
                    IsEraseDisabled = true,

                    Placeholders = new Dictionary<string, string>
                    {
                        ["${Employee}"] = "Employee"
                    }
                };

                evolve.Migrate();
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        private static void Log(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
