using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data.SqlClient;

namespace WebApplication10
{
    public class Startup
    {
        private readonly ILogger _logger;
        public IConfiguration Configuration { get; }
        public IHostingEnvironment Env { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment env, ILogger<Startup> logger)
        {
            Configuration = configuration;

            Env = env;
            _logger = logger;

            MigrateDatabase(); // Evolve
        }

        private void Log(string m)
        {
            Console.WriteLine(m);
        }

        private void MigrateDatabase()
        {
            string filter = "WebApplication10.db.migrations";

            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            try
            {
                var cn = new SqlConnection(connectionString);
                var evolve = new Evolve.Evolve(cn, msg => Log(msg))
                {
                    EmbeddedResourceAssemblies = new[] { typeof(Startup).Assembly },
                    EmbeddedResourceFilters = new[] { filter },
                    IsEraseDisabled = true,

                    Placeholders = new Dictionary<string, string>
                    {
                        ["${Employee}"] = "Employee"
                    }
                };

                //var cnx = new SqlConnection(connectionString);
                //var evolve = new Evolve.Evolve(cnx, msg => Log(msg))
                //{
                //    Locations = new[] { "db/migrations/" },
                //    IsEraseDisabled = true,
                //};

                evolve.Migrate();
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Database migration failed.", ex);
                throw;
            }

        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}


//IF NOT EXISTS(SELECT* FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'Employee')
//BEGIN
//        CREATE TABLE dbo.Employee(EmployeeID int
//        PRIMARY KEY CLUSTERED);
//END

//IF EXISTS(SELECT* FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'Employee')
//BEGIN
//    ALTER TABLE Employee

//    ADD EmpName nvarchar(100);
//END