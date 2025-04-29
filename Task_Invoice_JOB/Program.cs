using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Task_Invoice_JOB.Rutinas;
using Task_Invoice_JOB.Colecciones;
using Task_Invoice_JOB.Rutinas;

class Program
{
    static async Task Main(string[] args)
    {
        // Crear una instancia de MainRutinas
        MainRutinas rutinas = new MainRutinas();

        try
        {
            ///****************** Paso 1 Obtencion de Clientes Configurados *****************************            
            var ClientesConfigurados = await rutinas.ClientesConfiguradosAsync();

            ///****************** Paso 2  Obtener Comprobantes Timbrados del dia por cliente Configurado ************************
            var facturasCliente = await rutinas.ComprobantesTimbradosDelDiaPorClienteySociedadAsync(ClientesConfigurados);

            ///****************** Paso 3 Buscar Documentos en Carpeta de Facturas y envia mensaje de los que no se encuentran ************************
            var facturasConPathArchivos = await rutinas.BusquedaArchivosXMLyPDF(facturasCliente);

            ///******* Paso 4  Considerar leer CFDI *****************************
            var facturasDataXML = await rutinas.DatosCFDIByXML(facturasConPathArchivos);

            ///******* Paso 5  Almacenar en Base de Datos ************************
            await rutinas.AlamcenarComprobantes(facturasDataXML);


            //--------------------------------------- Envío de Comprobantes -----------------------------------------------------------

            ///****************** Paso 6 Obtencion Comprobantes Pendientes de Envio por Cliente *****************************            
            var ClientesyCmprobantesPendientesPorEnviar = await rutinas.ComprobantesPorCliente(ClientesConfigurados);

            ///****************** Paso 7 Envío de Comprobantes *****************************            
            await rutinas.EnviarComprobantePorCliente(ClientesyCmprobantesPendientesPorEnviar);







        }
        catch (Exception ex)
        {
            Console.WriteLine("Ocurrió un error:");
            Console.WriteLine(ex.Message);
        }
    }
}
