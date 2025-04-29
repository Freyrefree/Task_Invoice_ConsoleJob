using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Task_Invoice_JOB.API;
using Task_Invoice_JOB.Colecciones;

namespace Task_Invoice_JOB.Rutinas
{
    public class MainRutinas
    {
        public string pathFacturas = "\\\\192.168.1.3\\facturas_4.0\\";
        private string[] emailsNotificarenTeamsError = { 
            //"sergio.valencia@mcplasticos.com",
            "diego.gutierrez@mcplasticos.com" 
        };

        public string correoEmisor = "facturacion@mcplasticos.com";
        public string pswCorreoEmisor = "N$s#d4@275&";


        


        public async Task<List<Cliente>> ClientesConfiguradosAsync()
        {
            // Instanciar la clase API_Get_Data
            var api = new API_Get_Data();

            // Llamar a la función GetClientesConfiguradosAsync y obtener los resultados
            List<Cliente> clientes = await api.GetClientesConfiguradosAsync();

            // Retornar la lista de clientes si hay resultados
            if (clientes.Count > 0)
            {
                return clientes;
            }

            // Si no hay resultados, retorna una lista vacía
            return new List<Cliente>();
        }



        public async Task<List<facturasTimbradas>> ComprobantesTimbradosDelDiaPorClienteySociedadAsync(List<Cliente> Clientes)
        {
            // Crear las variables fechaInicio y fechaFin con formato YYYY-MM-DD
            string fechaInicio = DateTime.Now.ToString("yyyy-MM-dd");
            string fechaFin = DateTime.Now.ToString("yyyy-MM-dd");

            // PARA PRUEBAS
            //fechaInicio = "2025-01-13";
            //fechaFin = "";

            // Instanciar la clase API_Get_Data
            var api = new API_Get_Data();

            // Clases de documento a considerar
            string claseDocumentoDM = "DM"; // Facturas producto terminado Ingreso
            string claseDocumentoBC = "BC"; // Comprobantes de Pago

            // Lista para almacenar todas las facturas
            List<facturasTimbradas> todasLasFacturas = new List<facturasTimbradas>();

            // Procesar cada cliente de forma secuencial
            foreach (var cliente in Clientes)
            {
                Console.WriteLine($"Procesando cliente {cliente.ClaveCliente}");

                try
                {
                    // Obtener facturas para clase DM
                    var facturasDM = await api.GetFacturasPorCliente(
                        cliente.Sociedad,
                        fechaInicio,
                        fechaFin,
                        cliente.ClaveCliente.ToString(),
                        claseDocumentoDM
                    );

                    // Si hay facturas DM, agregarlas a la lista general
                    if (facturasDM != null)
                    {
                        todasLasFacturas.AddRange(facturasDM);
                    }

                    // Obtener facturas para clase BC
                    var facturasBC = await api.GetFacturasPorCliente(
                        cliente.Sociedad,
                        fechaInicio,
                        fechaFin,
                        cliente.ClaveCliente.ToString(),
                        claseDocumentoBC
                    );

                    // Si hay facturas BC, agregarlas a la lista general
                    if (facturasBC != null)
                    {
                        todasLasFacturas.AddRange(facturasBC);
                    }
                }
                catch (Exception ex)
                {
                    // Manejar errores de manera específica por cliente
                    Console.WriteLine($"Error procesando cliente {cliente.ClaveCliente}: {ex.Message}");
                }
            }

            return todasLasFacturas;
        }


        public async Task<List<facturasTimbradas>> BusquedaArchivosXMLyPDF(List<facturasTimbradas> Facturas)
        {
            var api = new API_Get_Data();

            // Validar conexión a la carpeta compartida
            if (!Directory.Exists(pathFacturas))
            {
                Console.WriteLine($"No se pudo conectar a la carpeta compartida: {pathFacturas}");
                return Facturas; // Retornar la lista original en caso de falla
            }

            // Recorrer la lista de facturas
            foreach (var factura in Facturas)
            {
                string folio = factura.Folio;

                if (string.IsNullOrEmpty(folio))
                {
                    Console.WriteLine("Folio vacío o nulo, se omite esta factura.");
                    continue;
                }

                try
                {
                    // Buscar archivos excluyendo la carpeta ComercioExterior y sus subcarpetas
                    var archivos = Directory.EnumerateFiles(pathFacturas, $"*{folio}*", SearchOption.AllDirectories)
                        .Where(file => !file.Contains("\\ComercioExterior\\"))
                        .ToList();

                    // Filtrar y asignar archivos XML
                    var archivoXML = archivos.FirstOrDefault(file => Path.GetExtension(file).Equals(".xml", StringComparison.OrdinalIgnoreCase));
                    if (archivoXML != null)
                    {
                        factura.PathXml = archivoXML;
                        Console.WriteLine($"Archivo XML encontrado: {archivoXML}");
                    }

                    // Filtrar y asignar archivos PDF
                    var archivoPDF = archivos.FirstOrDefault(file => Path.GetExtension(file).Equals(".pdf", StringComparison.OrdinalIgnoreCase));
                    if (archivoPDF != null)
                    {
                        factura.PathPDF = archivoPDF;
                        Console.WriteLine($"Archivo PDF encontrado: {archivoPDF}");
                    }

                    // Si no se encontró el archivo XML, enviar mensaje
                    if (factura.PathXml == null)
                    {
                        foreach (var email in emailsNotificarenTeamsError)
                        {
                            string mensaje = $"No se encontró el archivo XML para el folio: {folio}\n" +
                                             $"Cliente: {factura.Cliente}\n" +
                                             $"Sociedad: {factura.Sociedad}\n" +
                                             $"Fecha: {factura.Fecha}";
                            bool apiResult = await api.EnviarMensaje(email, mensaje);

                            if (apiResult)
                            {
                                Console.WriteLine($"Mensaje enviado a {email} sobre la ausencia de archivo XML para el folio: {folio}");
                            }
                            else
                            {
                                Console.WriteLine($"Error al enviar mensaje a {email} sobre la ausencia de archivo XML para el folio: {folio}");
                            }
                        }
                    }

                    // Si no se encontró el archivo PDF, enviar mensaje
                    if (factura.PathPDF == null)
                    {
                        foreach (var email in emailsNotificarenTeamsError)
                        {
                            string mensaje = $"No se encontró el archivo PDF para el folio: {folio}\n" +
                                             $"Cliente: {factura.Cliente}\n" +
                                             $"Sociedad: {factura.Sociedad}\n" +
                                             $"Fecha: {factura.Fecha}";
                            bool apiResult = await api.EnviarMensaje(email, mensaje);

                            if (apiResult)
                            {
                                Console.WriteLine($"Mensaje enviado a {email} sobre la ausencia de archivo PDF para el folio: {folio}");
                            }
                            else
                            {
                                Console.WriteLine($"Error al enviar mensaje a {email} sobre la ausencia de archivo PDF para el folio: {folio}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error buscando archivos para el folio {folio}: {ex.Message}");
                }
            }

            // Retornar la lista de facturas actualizada
            return Facturas;
        }

        public async Task<List<facturasTimbradas>> DatosCFDIByXML(List<facturasTimbradas> Facturas)
        {
            var api = new API_Get_Data();

            // Validar conexión a la carpeta compartida
            if (!Directory.Exists(pathFacturas))
            {
                Console.WriteLine($"No se pudo conectar a la carpeta compartida: {pathFacturas}");
                return Facturas; // Retornar la lista original en caso de falla
            }

            // Eliminar facturas con PathXml vacío o nulo
            Facturas.RemoveAll(f => string.IsNullOrEmpty(f.PathXml));

            // Recorrer la lista de facturas
            foreach (var factura in Facturas)
            {
                string pathXml = factura.PathXml;

                if (factura.ClaseDocumento == "BC")
                {

                    //CFDI data = await api.DatosCFDI(pathXml);

                    //if (data != null)
                    //{
                        string dataSS = "";
                    //}
                }



                try
                {
                    // Consumir la API para obtener los datos CFDI
                    CFDI data = await api.DatosCFDI(pathXml);

                    if (data != null)
                    {
                        // Actualizar los datos de la factura si los datos CFDI son válidos
                        factura.VersionCFDIXml = data.Version;
                        factura.SerieXml = data.Serie;
                        factura.FolioXml = data.Folio.ToString();
                        factura.FechaTimbradoXml = data.Complemento.TimbreFiscalDigital.FechaTimbrado;
                        factura.FormaPagoXml = data.FormaPago;
                        factura.CondicionesDePagoXml = data.CondicionesDePago;

                        // Conversión explícita a double para asegurar formato decimal
                        factura.TipoCambioXml = Convert.ToDecimal(data.TipoCambio);
                        factura.MontoFacturaXml = Convert.ToDecimal(data.Total);

                        factura.TipoComprobanteXml = data.TipoDeComprobante;
                        factura.MetodoPagoXml = data.MetodoPago;
                        factura.RFCEmisorXml = data.Emisor.Rfc;
                        factura.RFCReceptorXml = data.Receptor.Rfc;
                        factura.RsEmisorXml = data.Emisor.Nombre;
                        factura.RsReceptorXml = data.Receptor.Nombre;
                        factura.UUIDXml = data.Complemento.TimbreFiscalDigital.Uuid;

                        // ***** Datos de Pago *****
                        //if (data.Pagos?.PagosDetalles != null && data.Pagos.PagosDetalles.Count > 0)
                        //{
                        //    var detallePago = data.Pagos.PagosDetalles[0]; // Tomar el primer detalle de pago

                        //    factura.FechaPago = detallePago.FechaPago;
                        //    factura.FormaDePagoP = detallePago.FormaDePagoP;
                        //    factura.MonedaP = detallePago.MonedaP;
                        //    factura.TipoCambioP = Convert.ToDecimal(detallePago.TipoCambioP);
                        //    factura.Monto = Convert.ToDecimal(detallePago.Monto);
                        //    factura.NumOperacion = detallePago.NumOperacion;
                        //    factura.RfcEmisorCtaOrd = detallePago.RfcEmisorCtaOrd;
                        //    factura.NomBancoOrdExt = detallePago.NomBancoOrdExt;
                        //    factura.RfcEmisorCtaBen = detallePago.RfcEmisorCtaBen;
                        //    factura.CtaBeneficiario = detallePago.CtaBeneficiario;




                        //}
                        if (data.Pagos?.PagosDetalles != null && data.Pagos.PagosDetalles.Count > 0)
                        {
                            var detallePago = data.Pagos.PagosDetalles[0]; // Tomar el primer detalle de pago

                            // Asignar valores a las propiedades de la factura
                            factura.FechaPago = detallePago.FechaPago;
                            factura.FormaDePagoP = detallePago.FormaDePagoP;
                            factura.MonedaP = detallePago.MonedaP;
                            factura.TipoCambioP = Convert.ToDecimal(detallePago.TipoCambioP);
                            factura.Monto = Convert.ToDecimal(detallePago.Monto);
                            factura.NumOperacion = detallePago.NumOperacion;
                            factura.RfcEmisorCtaOrd = detallePago.RfcEmisorCtaOrd;
                            factura.NomBancoOrdExt = detallePago.NomBancoOrdExt;
                            factura.RfcEmisorCtaBen = detallePago.RfcEmisorCtaBen;
                            factura.CtaBeneficiario = detallePago.CtaBeneficiario;

                            // Verificar si existen documentos relacionados en el detalle de pago
                            if (detallePago.DocumentosRelacionados != null && detallePago.DocumentosRelacionados.Count > 0)
                            {
                                // Iterar sobre los documentos relacionados y agregarlos a la lista de DoctosRelacionados de la factura
                                foreach (var docRelacionado in detallePago.DocumentosRelacionados)
                                {
                                    var documento = new DoctoRelacionado
                                    {
                                        IdDocumento = docRelacionado.IdDocumento,
                                        Folio = docRelacionado.Folio,
                                        MonedaDR = docRelacionado.MonedaDR,
                                        EquivalenciaDR = docRelacionado.EquivalenciaDR,
                                        NumParcialidad = docRelacionado.NumParcialidad,
                                        ImpSaldoAnt = docRelacionado.ImpSaldoAnt,
                                        ImpPagado = docRelacionado.ImpPagado,
                                        ImpSaldoInsoluto = docRelacionado.ImpSaldoInsoluto,
                                        ObjetoImpDR = docRelacionado.ObjetoImpDR,
                                    };

                                    // Agregar el documento relacionado a la lista de la factura
                                    factura.DoctosRelacionados.Add(documento);
                                }
                            }
                        }


                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al procesar el archivo {pathXml}: {ex.Message}");
                }
            }

            // Retornar la lista de facturas actualizada
            return Facturas;
        }

        public async Task AlamcenarComprobantes(List<facturasTimbradas> Facturas)
        {
            try
            {
                // Opciones para la serialización
                var opciones = new JsonSerializerOptions
                {
                    WriteIndented = true, // Formato legible
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, // Ignorar valores nulos
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase // Convertir nombres de propiedades a camelCase
                };

                // Convertir la lista de facturas a JSON
                string jsonFacturas = JsonSerializer.Serialize(Facturas, opciones);

                // Instanciar la clase API_Get_Data y enviar el JSON
                var api = new API_Get_Data();
                await api.AlmacenamientoComprobante(jsonFacturas);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocurrió un error: {ex.Message}");
            }
        }


        //public async Task<List<Cliente>> ComprobantesPorCliente(List<Cliente> ClientesConfigurados)
        //{
        //    // Lista para almacenar los clientes actualizados con sus comprobantes pendientes
        //    var clientesConComprobantes = new List<Cliente>();

        //    // Instanciar la clase API_Get_Data
        //    var api = new API_Get_Data();

        //    try
        //    {
        //        foreach (var datoCliente in ClientesConfigurados)
        //        {
        //            string sociedad = datoCliente.Sociedad;
        //            string claveCliente = datoCliente.ClaveCliente.ToString();

        //            // Llamar a la API para obtener los comprobantes pendientes por sociedad y cliente
        //            var comprobantesPendientes = await api.ComprobantesPendientesDeEnvioBySociedadyCliente(sociedad, claveCliente);

        //            if (comprobantesPendientes != null && comprobantesPendientes.Detalle.Any())
        //            {
        //                // Agregar los comprobantes obtenidos a la propiedad ComprobantesPendientes del cliente actual
        //                datoCliente.ComprobantesPendientes = comprobantesPendientes.Detalle;

        //                // Formatear la respuesta para impresión
        //                Console.WriteLine("=====================================");
        //                Console.WriteLine($"Cliente: {datoCliente.Nombre} (Clave Cliente: {datoCliente.ClaveCliente}, Sociedad: {datoCliente.Sociedad})");
        //                Console.WriteLine($"Comprobantes Pendientes: {datoCliente.ComprobantesPendientes.Count}");
        //                Console.WriteLine("-------------------------------------");

        //                //Aqui el comporbante será validado por el estatus, si el comporbante es cancelado no se enviará



        //                // Agregar el cliente actualizado a la lista de clientes con comprobantes
        //                clientesConComprobantes.Add(datoCliente);
        //            }
        //            else
        //            {
        //                // Si no hay comprobantes pendientes
        //                Console.WriteLine("=====================================");
        //                Console.WriteLine($"Cliente: {datoCliente.Nombre} (Clave: {datoCliente.ClaveCliente}, Sociedad: {datoCliente.Sociedad})");
        //                Console.WriteLine("No hay comprobantes pendientes.");
        //                Console.WriteLine("-------------------------------------");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error al obtener comprobantes por cliente: {ex.Message}");
        //    }

        //    // Retornar la lista de clientes con sus comprobantes pendientes asociados
        //    return clientesConComprobantes;
        //}

        public async Task<List<Cliente>> ComprobantesPorCliente(List<Cliente> ClientesConfigurados)
        {
            var clientesConComprobantes = new List<Cliente>();
            var api = new API_Get_Data();

            try
            {
                foreach (var datoCliente in ClientesConfigurados)
                {
                    string sociedad = datoCliente.Sociedad;
                    string claveCliente = datoCliente.ClaveCliente.ToString();

                    var comprobantesPendientes = await api.ComprobantesPendientesDeEnvioBySociedadyCliente(sociedad, claveCliente);

                    if (comprobantesPendientes != null && comprobantesPendientes.Detalle.Any())
                    {
                        var comprobantesValidos = new List<Comprobante>();

                        foreach (var comprobante in comprobantesPendientes.Detalle)
                        {
                            var estatus = await api.EstatusSAPCFDI(comprobante.RfcEmisor, comprobante.RfcReceptor, comprobante.MontoFactura, comprobante.Uuid);

                            Console.WriteLine($"El comprobante: {comprobante.Serie} {comprobante.Folio} tiene un Estatus: {estatus.Estado}");


                            if (estatus != null && estatus.Estado == "Vigente")
                            {

                                var res  = await api.CambiarEstatusCFDI_SAT(comprobante.Id, estatus.Estado);
                                if (res.Exito)
                                {
                                    comprobantesValidos.Add(comprobante);
                                }
                                    
                            }
                            else
                            {
                                var res = await api.CambiarEstatusCFDI_SAT(comprobante.Id, estatus.Estado);
                                Console.WriteLine($"Comprobante {comprobante.Uuid} está cancelado y no será enviado.");
                            }
                        }

                        datoCliente.ComprobantesPendientes = comprobantesValidos;

                        if (comprobantesValidos.Any())
                        {
                            Console.WriteLine("=====================================");
                            Console.WriteLine($"Cliente: {datoCliente.Nombre} (Clave Cliente: {datoCliente.ClaveCliente}, Sociedad: {datoCliente.Sociedad})");
                            Console.WriteLine($"Comprobantes Pendientes: {datoCliente.ComprobantesPendientes.Count}");
                            Console.WriteLine("-------------------------------------");

                            clientesConComprobantes.Add(datoCliente);
                        }
                    }
                    else
                    {
                        Console.WriteLine("=====================================");
                        Console.WriteLine($"Cliente: {datoCliente.Nombre} (Clave: {datoCliente.ClaveCliente}, Sociedad: {datoCliente.Sociedad})");
                        Console.WriteLine("No hay comprobantes pendientes.");
                        Console.WriteLine("-------------------------------------");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener comprobantes por cliente: {ex.Message}");
            }

            return clientesConComprobantes;
        }



        public async Task EnviarComprobantePorCliente(List<Cliente> ClientesyComprobantes)
        {
            try
            {
                // Obtener el día actual en español con acentos
                var diaHoy = DateTime.Now.ToString("dddd", new System.Globalization.CultureInfo("es-ES"));
                var horaActual = DateTime.Now.TimeOfDay; // Hora actual como TimeSpan
                Console.WriteLine($"Hoy es: {diaHoy}, Hora Actual: {horaActual}");

                foreach (var datoCliente in ClientesyComprobantes)
                {
                    if (datoCliente.Scheduler != null && datoCliente.Scheduler.Dias != null && datoCliente.Scheduler.Dias.Any())
                    {
                        Console.WriteLine($"\nCliente: {datoCliente.Nombre}, Verificando días configurados...");

                        // Buscar el día configurado que coincide con el día de hoy
                        var diaConfigurado = datoCliente.Scheduler.Dias
                            .FirstOrDefault(d => string.Equals(d.Nombre, diaHoy, StringComparison.OrdinalIgnoreCase));

                        if (diaConfigurado != null)
                        {
                            Console.WriteLine($"Día encontrado: {diaConfigurado.Nombre}");

                            // Convertir HoraInicio y HoraFin a TimeSpan
                            //if (TimeSpan.TryParse(diaConfigurado.HoraInicio, out TimeSpan horaInicio) &&
                            //    TimeSpan.TryParse(diaConfigurado.HoraFin, out TimeSpan horaFin))
                            //{
                            // Calcular hora fin ajustada (+59m 59s 999ms)
                            TimeSpan horaInicio = diaConfigurado.HoraInicio;
                            TimeSpan horaFin = diaConfigurado.HoraFin;
                                //var horaFinAjustada = horaFin.Add(new TimeSpan(0, 59, 59)).Add(TimeSpan.FromMilliseconds(999));

                                Console.WriteLine($"Hora Inicio: {horaInicio}, Hora Fin Ajustada: {horaFin}");

                                // Verificar si la hora actual está dentro del rango
                                bool horaEnRango = horaActual >= horaInicio && horaActual <= horaFin;

                                Console.WriteLine($"¿La hora actual ({horaActual}) está en el rango?: {horaEnRango}");

                                if (horaEnRango)
                                {
                                    Console.WriteLine($"********* Enviando comprobantes del Cliente: {datoCliente.ClaveCliente} ************");

                                    // Llamar a la función EmailComprobante y recibir el resultado
                                    bool enviado = await EmailAndUpdateComprobante(datoCliente);

                                    if (enviado)
                                    {
                                        Console.WriteLine($"Comprobantes enviados correctamente para el cliente: {datoCliente.ClaveCliente}");

                                    }
                                    else
                                    {
                                        Console.WriteLine($"Error al enviar comprobantes para el cliente: {datoCliente.ClaveCliente}");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"La hora actual ({horaActual}) no está dentro del rango configurado.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Error: Las horas configuradas no son válidas.");
                            }
                        //}
                        //else
                        //{
                        //    Console.WriteLine($"El día de hoy ({diaHoy}) no coincide con los días configurados del cliente.");
                        //}
                    }
                    else
                    {
                        Console.WriteLine($"Cliente: {datoCliente.Nombre} no tiene días configurados.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al procesar comprobantes por cliente: {ex.Message}");
            }
        }




        public async Task<bool> EmailAndUpdateComprobante(Cliente DatosClienteyComprobante)
        {
            string correosConcatTo = "";
            string correosConcatCC = "";

            try
            {
                var api = new API_Get_Data();

                // Configuración del servidor SMTP
                var smtpClient = new SmtpClient("smtp.office365.com") // Cambiar a tu servidor SMTP
                {
                    Port = 587,
                    Credentials = new NetworkCredential(correoEmisor, pswCorreoEmisor),
                    EnableSsl = true
                };

                // Iterar por los comprobantes del cliente
                foreach (var comprobante in DatosClienteyComprobante.ComprobantesPendientes)
                {
                    correosConcatTo = "";
                    correosConcatCC = "";
                    try
                    {
                        // Crear mensaje de correo
                        var mailMessage = new MailMessage
                        {
                            From = new MailAddress(correoEmisor),
                            Subject = $"{comprobante.Serie} {comprobante.Folio}",
                            Body = $"Serie: {comprobante.Serie}\nFolio: {comprobante.Folio}",
                            IsBodyHtml = false
                        };

                        // Verificar correos de destino (TO y CC)
                        if (DatosClienteyComprobante.CorreosTO != null && DatosClienteyComprobante.CorreosTO.Any())
                        {
                            foreach (var correoTo in DatosClienteyComprobante.CorreosTO)
                            {
                                mailMessage.To.Add(correoTo.Email);
                                correosConcatTo += correoTo.Email + "|";
                                //mailMessage.To.Add("diego.gutierrez@mcplasticos.com");
                            }
                        }

                        if (DatosClienteyComprobante.CorreosCC != null && DatosClienteyComprobante.CorreosCC.Any())
                        {
                            foreach (var correoCc in DatosClienteyComprobante.CorreosCC)
                            {
                                mailMessage.CC.Add(correoCc.Email);
                                correosConcatCC += correoCc.Email + "|";
                                //mailMessage.CC.Add("diego.gutierrez@mcplasticos.com");
                            }
                        }

                        // Adjuntar archivos según configuración en Archivos
                        if (DatosClienteyComprobante.Archivos != null)
                        {
                            if (DatosClienteyComprobante.Archivos.XML && !string.IsNullOrEmpty(comprobante.PathXML))
                            {
                                mailMessage.Attachments.Add(new Attachment(comprobante.PathXML));
                            }

                            if (DatosClienteyComprobante.Archivos.PDF && !string.IsNullOrEmpty(comprobante.PathPDF))
                            {
                                mailMessage.Attachments.Add(new Attachment(comprobante.PathPDF));
                            }
                        }

                        // Enviar el correo de forma asíncrona
                        await smtpClient.SendMailAsync(mailMessage);

                        Console.WriteLine($"Correo enviado con éxito para el comprobante: Serie {comprobante.Serie}, Folio {comprobante.Folio}");

                        // Validación adicional: Confirmar que el correo se envió antes de actualizar el estatus
                        if (mailMessage != null)
                        {
                            // Actualizar el estatus en la API
                            var resultadoApi = await api.CambiarEstatusEnvio(comprobante.Id,correosConcatTo,correosConcatCC);

                            if (resultadoApi.Exito)
                            {
                                Console.WriteLine($"Estatus actualizado correctamente para el comprobante con ID: {comprobante.Id}");
                            }
                            else
                            {
                                Console.WriteLine($"Error al actualizar el estatus para el comprobante con ID: {comprobante.Id}. Mensaje: {resultadoApi.Message}");
                                return false; // Detener si falla el almacenamiento
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Error: No se pudo confirmar el envío del correo para el comprobante con ID: {comprobante.Id}");
                            return false;
                        }
                    }
                    catch (SmtpException smtpEx)
                    {
                        Console.WriteLine($"Error SMTP al enviar el correo para el comprobante con ID {comprobante.Id}: {smtpEx.Message}");
                        return false; // Detener si falla el envío del correo
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al procesar el comprobante con ID: {comprobante.Id}. Detalle: {ex.Message}");
                        return false;
                    }
                }

                return true; // Retorna true si todos los comprobantes se enviaron y actualizaron correctamente
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general al procesar comprobantes: {ex.Message}");
                return false; // Retorna false si ocurrió un error general
            }
        }











    }
}
