using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Invoice_JOB.Contexto
{
    public class ContextoTaskInvoice : DbContext
    {

        public ContextoTaskInvoice() : base("Data Source=192.168.1.21;Initial Catalog=Tesoreria;User ID=sa;Password=3Xk7#9aQ")
        {
            // Validar la conexión al servidor de base de datos

            Database.SetInitializer<ContextoTaskInvoice>(null);
            if (Database.Exists())
            {
                Console.WriteLine("Conexión exitosa al servidor de base de datos.");
            }
            else
            {
                Console.WriteLine("Error al conectar al servidor de base de datos.");
            }
        }

        public DbSet<Task_Invoice_ConfiguracionCliente> Task_Invoice_ConfiguracionCliente { get; set; }
        public DbSet<Task_Invoice_Comprobante> Task_Invoice_Comprobante { get; set; }
        public DbSet<Task_Invoice_Correos> Task_Invoice_Correos { get; set; }
        public DbSet<Task_Invoice_Dias> Task_Invoice_Dias { get; set; }
        public DbSet<Task_Invoice_Horas> Task_Invoice_Horas { get; set; }
        public DbSet<Task_Invoice_Scheduler> Task_Invoice_Scheduler { get; set; }


        




    }

    [Table("Task_Invoice_ConfiguracionCliente", Schema = "dbo")]
    public class Task_Invoice_ConfiguracionCliente
    {

        public int Id { get; set; }
        public int? ClaveCliente { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public int? Estatus { get; set; }
        public string UsuarioRegistro { get; set; }
        public string Sociedad { get; set; }
        public int? Pdf { get; set; }
        public int? Xml { get; set; }

    }

    [Table("Task_Invoice_Comprobante", Schema = "dbo")]
    public class Task_Invoice_Comprobante
    {
        public int Id { get; set; }
        public string ClaveCliente { get; set; }
        public string Serie { get; set; }
        public string Folio { get; set; }
        public string Uuid { get; set; }
        public string RsEmisor { get; set; }
        public string RsReceptor { get; set; }
        public DateTime? FechaTimbrado { get; set; }
        public string TipoComprobante { get; set; }
        public string VersionCFDI { get; set; }
        public string EstatusCFDI { get; set; }
        public int? EstatusEnvio { get; set; }
        public string Sociedad { get; set; }
        public decimal? MontoFactura { get; set; }

    }

    [Table("Task_Invoice_Correos", Schema = "dbo")]
    public class Task_Invoice_Correos
    {
        public int? Id { get; set; }
        public int? IdConfiguracion { get; set; }
        public string TipoCorreo { get; set; }
        public string Contacto { get; set; }
        public string Correo { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public string UsuarioRegistro { get; set; }


    }

    [Table("Task_Invoice_Dias", Schema = "dbo")]
    public class Task_Invoice_Dias
    {
        public int Id { get; set; }
        public int? NumeroDia { get; set; }
        public string Nombre { get; set; }
        public int? Estatus { get; set; }
        public DateTime? FechaRegistro { get; set; }

    }

    [Table("Task_Invoice_Horas", Schema = "dbo")]
    public class Task_Invoice_Horas
    {
        public int Id { get; set; }
        public TimeSpan? Hora { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public int? Estatus { get; set; }


    }

    [Table("Task_Invoice_Scheduler", Schema = "dbo")]
    public class Task_Invoice_Scheduler
    {
        public int Id { get; set; }
        public int? IdConfiguracion { get; set; }
        public int? IdDia { get; set; }
        public int? IdHoraInicio { get; set; }
        public int? IdHoraFin { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public string UsuarioRegistro { get; set; }

    }



}
