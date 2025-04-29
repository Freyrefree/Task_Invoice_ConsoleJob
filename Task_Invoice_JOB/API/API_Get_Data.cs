using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Task_Invoice_JOB.Colecciones;

namespace Task_Invoice_JOB.API
{
    public class API_Get_Data
    {
        public string APITaskInvoiceBasePath = "http://192.168.1.21:9001";
        //public string APITaskInvoiceBasePath = "https://localhost:7060";

        public string ApiUtilServer14 = "https://g-mc.mx:8105";
        //public string ApiUtilServer14 = "https://localhost:44332";
        

        public string ApiUtilBasePath = "http://192.168.1.21:9000";        
        public string ApiCFDI = "http://192.168.1.21:8001";
        public async Task<List<Cliente>> GetClientesConfiguradosAsync()
        {
            // Inicializar la lista de clientes
            List<Cliente> clientes = new List<Cliente>();

            try
            {
                // Crear cliente HTTP
                using (var client = new HttpClient())
                {
                    // Configuración del cliente HTTP
                    client.BaseAddress = new Uri(APITaskInvoiceBasePath);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Realizar la solicitud GET
                    HttpResponseMessage response = await client.GetAsync("/ClientesConfigurados");

                    // Verificar si la respuesta es exitosa
                    if (response.IsSuccessStatusCode)
                    {
                        // Leer el contenido de la respuesta como string
                        string responseData = await response.Content.ReadAsStringAsync();

                        // Deserializar la respuesta JSON en una lista de Cliente
                        clientes = JsonSerializer.Deserialize<List<Cliente>>(responseData, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true // Ignorar mayúsculas/minúsculas en nombres de propiedades
                        }) ?? new List<Cliente>();
                    }
                    else
                    {
                        Console.WriteLine($"Error al consumir la API: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores
                Console.WriteLine($"Ocurrió un error: {ex.Message}");
            }

            return clientes;
        }

        public async Task<List<facturasTimbradas>> GetFacturasPorCliente(
            string sociedad,
            string fechaInicio,
            string fechaFin,
            string claveCliente,
            string claseDocumento)
        {
            // Inicializar la lista de facturas
            List<facturasTimbradas> facturas = new List<facturasTimbradas>();

            try
            {
                // Crear cliente HTTP
                using (var client = new HttpClient())
                {
                    // Configuración del cliente HTTP
                    client.BaseAddress = new Uri(ApiUtilBasePath);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Crear el contenido del formulario
                    var formData = new FormUrlEncodedContent(new[]
                    {
                new KeyValuePair<string, string>("sociedad", sociedad),
                new KeyValuePair<string, string>("fechaInicio", fechaInicio),
                new KeyValuePair<string, string>("fechaFin", fechaFin),
                new KeyValuePair<string, string>("claveCliente", claveCliente),
                new KeyValuePair<string, string>("claseDocumento", claseDocumento)
            });

                    // Realizar la solicitud POST
                    HttpResponseMessage response = await client.PostAsync("/api/FacturasGMC", formData);

                    // Verificar si la respuesta es exitosa
                    if (response.IsSuccessStatusCode)
                    {
                        // Leer el contenido de la respuesta como string
                        string responseData = await response.Content.ReadAsStringAsync();

                        // Deserializar la respuesta a una lista de facturas
                        facturas = JsonSerializer.Deserialize<List<facturasTimbradas>>(responseData, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true // Ignorar mayúsculas/minúsculas en nombres de propiedades
                        }) ?? new List<facturasTimbradas>();
                    }
                    else
                    {
                        Console.WriteLine($"Error al consumir la API: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores
                Console.WriteLine($"Ocurrió un error: {ex.Message}");
            }

            return facturas;
        }


        public async Task<bool> EnviarMensaje(string correoDestino, string mensaje)
        {
            // Inicializar el resultado como falso
            bool resultado = false;

            try
            {
                // Crear cliente HTTP
                using (var client = new HttpClient())
                {
                    // Configuración del cliente HTTP
                    client.BaseAddress = new Uri(ApiUtilBasePath);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Crear el contenido del formulario
                    var formData = new FormUrlEncodedContent(new[]
                    {
                new KeyValuePair<string, string>("api", "TI"),
                new KeyValuePair<string, string>("emailDestino", correoDestino),
                new KeyValuePair<string, string>("mensaje", mensaje)
            });

                    // Realizar la solicitud POST
                    HttpResponseMessage response = await client.PostAsync("/api/MicrosoftTeams_EnviarMensaje", formData);

                    // Verificar si la respuesta es exitosa
                    if (response.IsSuccessStatusCode)
                    {
                        // Leer el contenido de la respuesta como string
                        string responseData = await response.Content.ReadAsStringAsync();

                        // Deserializar la respuesta a un booleano
                        resultado = JsonSerializer.Deserialize<bool>(responseData);
                    }
                    else
                    {
                        Console.WriteLine($"Error al consumir la API: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores
                Console.WriteLine($"Ocurrió un error: {ex.Message}");
            }

            return resultado;
        }


        public async Task<CFDI> DatosCFDI(string pathXML)
        {
            // Inicializar el objeto CFDI como null
            CFDI cfdi = null;

            try
            {
                // Crear cliente HTTP
                using (var client = new HttpClient())
                {
                    // Configuración del cliente HTTP
                    client.BaseAddress = new Uri(ApiCFDI);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Crear el contenido del formulario
                    var formData = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("file", pathXML),
                    });

                    // Realizar la solicitud POST
                    HttpResponseMessage response = await client.PostAsync("/Xml", formData);

                    // Verificar si la respuesta es exitosa
                    if (response.IsSuccessStatusCode)
                    {
                        // Leer el contenido de la respuesta como string
                        string responseData = await response.Content.ReadAsStringAsync();

                        // Deserializar la respuesta al tipo CFDI
                        cfdi = JsonSerializer.Deserialize<CFDI>(responseData, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true // Opcional, si las propiedades de JSON no coinciden exactamente con las de CFDI
                        });
                    }
                    else
                    {
                        Console.WriteLine($"Error al consumir la API: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores
                Console.WriteLine($"Ocurrió un error: {ex.Message}");
            }

            return cfdi;
        }


        public async Task AlmacenamientoComprobante(string jsonFacturas)
        {
            try
            {
                // Imprimir el JSON generado
                Console.WriteLine("JSON Generado:");
                Console.WriteLine(jsonFacturas);

                // Configurar y enviar la solicitud HTTP
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APITaskInvoiceBasePath); // Cambia a tu URL de la API
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Crear el contenido para el POST
                    var content = new StringContent(jsonFacturas, Encoding.UTF8, "application/json");

                    // Enviar la solicitud POST
                    HttpResponseMessage response = await client.PostAsync("/GuardarComprobante", content);

                    if (response.IsSuccessStatusCode)
                    {
                        // Leer el contenido de la respuesta
                        string responseContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine("Respuesta del servidor:");
                        Console.WriteLine(responseContent);

                        // Opcional: Deserializar la respuesta para procesar los mensajes
                        var resultado = JsonSerializer.Deserialize<ResponseDto>(responseContent, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        if (resultado != null)
                        {
                            Console.WriteLine($"Mensaje: {resultado.Message}");
                            foreach (var detalle in resultado.Detalle)
                            {
                                Console.WriteLine($"Folio: {detalle.Folio}, UUID: {detalle.UUID}, Status: {detalle.Status}, Mensaje: {detalle.Message}");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Error al almacenar comprobantes: {response.StatusCode}");
                        string errorContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Detalle del error: {errorContent}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar los datos a la API: {ex.Message}");
            }
        }

        public async Task<ResponseComprobante> ComprobantesPendientesDeEnvioBySociedadyCliente(string sociedad, string cliente)
        {
            try
            {
                // Configurar y enviar la solicitud HTTP
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APITaskInvoiceBasePath); // Cambia a tu URL de la API
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Crear el contenido para el POST con sociedad y cliente
                    var formData = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("sociedad", sociedad),
                        new KeyValuePair<string, string>("claveCliente", cliente)
                    });

                    // Enviar la solicitud POST
                    HttpResponseMessage response = await client.PostAsync("/InvoiceToSendByClientAndEnterprice", formData);

                    if (response.IsSuccessStatusCode)
                    {
                        // Leer el contenido de la respuesta
                        string responseContent = await response.Content.ReadAsStringAsync();
                        //Console.WriteLine("Respuesta del servidor  de comprobantes Pendientes:");
                        //Console.WriteLine(responseContent);

                        // Deserializar la respuesta al tipo ResponseComprobante
                        var resultado = JsonSerializer.Deserialize<ResponseComprobante>(responseContent, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        return resultado ?? new ResponseComprobante
                        {
                            Message = "No se recibieron datos de la API.",
                            Detalle = new List<Comprobante>()
                        };
                    }
                    else
                    {
                        // Manejo de errores HTTP
                        string errorContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Error al consultar comprobantes: {response.StatusCode}");
                        Console.WriteLine($"Detalle del error: {errorContent}");

                        return new ResponseComprobante
                        {
                            Message = $"Error al consultar comprobantes: {response.StatusCode}",
                            Detalle = new List<Comprobante>()
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores generales
                Console.WriteLine($"Error al enviar los datos a la API: {ex.Message}");

                return new ResponseComprobante
                {
                    Message = $"Error al enviar los datos: {ex.Message}",
                    Detalle = new List<Comprobante>()
                };
            }
        }

        public async Task<ConsultaCFDIResponse> EstatusSAPCFDI(string rfcEmisor, string rfcReceptor, decimal total, string uuid)
        {
            try
            {
                // Configurar y enviar la solicitud HTTP
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(ApiUtilServer14); // Cambia a tu URL de la API
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Crear el contenido para el POST con sociedad y cliente
                    var formData = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("rfcEmisor", rfcEmisor),
                        new KeyValuePair<string, string>("rfcReceptor", rfcReceptor),
                        new KeyValuePair<string, string>("total", total.ToString("F2")), // Convertir decimal a string
                        new KeyValuePair<string, string>("uuid", uuid)
                    });

                    // Enviar la solicitud POST
                    HttpResponseMessage response = await client.PostAsync("/api/EstatusCFDI", formData);

                    if (response.IsSuccessStatusCode)
                    {
                        // Leer el contenido de la respuesta
                        string responseContent = await response.Content.ReadAsStringAsync();

                        // Deserializar la respuesta al tipo ResponseComprobante
                        var resultado = JsonSerializer.Deserialize<ConsultaCFDIResponse>(responseContent, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        return resultado;
                    }
                    else
                    {
                        // Manejo de errores HTTP
                        string errorContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Error al consultar comprobantes: {response.StatusCode}");
                        Console.WriteLine($"Detalle del error: {errorContent}");
                        throw new Exception($"HTTP Error: {response.StatusCode}, Details: {errorContent}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores generales
                Console.WriteLine($"Error al enviar los datos a la API: {ex.Message}");
                throw;
            }
        }


        public async Task<EstatusResponse> CambiarEstatusEnvio(int id,string correosTo, string correosCC)
        {
            try
            {
                // Configurar y enviar la solicitud HTTP
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APITaskInvoiceBasePath); // Cambia a tu URL de la API
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Crear el contenido para el POST con el parámetro ID
                    var formData = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("id", id.ToString()),
                        new KeyValuePair<string, string>("correosTO" ,correosTo.ToString()),
                        new KeyValuePair<string, string>("correosCC" , correosCC.ToString())
                    });

                    // Enviar la solicitud POST
                    HttpResponseMessage response = await client.PostAsync("/CambiarEstatusComprobanteEnviado", formData);

                    if (response.IsSuccessStatusCode)
                    {
                        // Leer el contenido de la respuesta
                        string responseContent = await response.Content.ReadAsStringAsync();

                        // Deserializar la respuesta
                        using (var jsonDoc = JsonDocument.Parse(responseContent))
                        {
                            var root = jsonDoc.RootElement;

                            // Verificar si contiene la clave "detalle" y si tiene la estructura esperada
                            if (root.TryGetProperty("detalle", out JsonElement detalleElement))
                            {
                                bool estatus = false;

                                // Obtener el valor de "estatus" si existe en "detalle"
                                if (detalleElement.TryGetProperty("estatus", out JsonElement estatusElement) && estatusElement.ValueKind == JsonValueKind.True || estatusElement.ValueKind == JsonValueKind.False)
                                {
                                    estatus = estatusElement.GetBoolean();
                                }

                                // Retornar respuesta formateada
                                return new EstatusResponse
                                {
                                    Exito = estatus,
                                    Message = root.GetProperty("message").GetString() ?? "Respuesta recibida pero sin mensaje."
                                };
                            }
                            else if (root.TryGetProperty("message", out JsonElement messageElement))
                            {
                                // Caso alternativo: la respuesta contiene solo un mensaje y un ID
                                return new EstatusResponse
                                {
                                    Exito = false,
                                    Message = messageElement.GetString() ?? "No se encontró el comprobante."
                                };
                            }
                        }

                        // Caso de respuesta inesperada
                        return new EstatusResponse
                        {
                            Exito = false,
                            Message = "La respuesta de la API no contiene la información esperada."
                        };
                    }
                    else
                    {
                        // Manejo de errores HTTP
                        string errorContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Error al consultar la API: {response.StatusCode}");
                        Console.WriteLine($"Detalle del error: {errorContent}");

                        return new EstatusResponse
                        {
                            Exito = false,
                            Message = $"Error al actualizar el estatus del comprobante. Código: {response.StatusCode}"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores generales
                Console.WriteLine($"Error al enviar los datos a la API: {ex.Message}");

                return new EstatusResponse
                {
                    Exito = false,
                    Message = $"Error al enviar los datos: {ex.Message}"
                };
            }
        }

        public async Task<EstatusResponse> CambiarEstatusCFDI_SAT(int id, string estatusSAT)
        {
            try
            {
                // Configurar y enviar la solicitud HTTP
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APITaskInvoiceBasePath); // Cambia a tu URL de la API
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Crear el contenido para el POST con el parámetro ID
                    var formData = new FormUrlEncodedContent(new[]
                    {
                new KeyValuePair<string, string>("id", id.ToString()),
                new KeyValuePair<string, string>("estatusSAT", estatusSAT)
            });

                    // Enviar la solicitud POST
                    HttpResponseMessage response = await client.PostAsync("/CambiarEstatusCFDI_SAT", formData);

                    if (response.IsSuccessStatusCode)
                    {
                        // Leer el contenido de la respuesta
                        string responseContent = await response.Content.ReadAsStringAsync();

                        // Deserializar la respuesta
                        using (var jsonDoc = JsonDocument.Parse(responseContent))
                        {
                            var root = jsonDoc.RootElement;

                            // Verificar si contiene la clave "detalle" y si tiene la estructura esperada
                            if (root.TryGetProperty("detalle", out JsonElement detalleElement))
                            {
                                bool estatus = false;

                                // Obtener el valor de "estatus" si existe en "detalle"
                                if (detalleElement.TryGetProperty("estatus", out JsonElement estatusElement) && estatusElement.ValueKind == JsonValueKind.True || estatusElement.ValueKind == JsonValueKind.False)
                                {
                                    estatus = estatusElement.GetBoolean();
                                }

                                // Retornar respuesta formateada
                                return new EstatusResponse
                                {
                                    Exito = estatus,
                                    Message = root.GetProperty("message").GetString() ?? "Respuesta recibida pero sin mensaje."
                                };
                            }
                            else if (root.TryGetProperty("message", out JsonElement messageElement))
                            {
                                // Caso alternativo: la respuesta contiene solo un mensaje y un ID
                                return new EstatusResponse
                                {
                                    Exito = false,
                                    Message = messageElement.GetString() ?? "No se encontró el comprobante."
                                };
                            }
                        }

                        // Caso de respuesta inesperada
                        return new EstatusResponse
                        {
                            Exito = false,
                            Message = "La respuesta de la API no contiene la información esperada."
                        };
                    }
                    else
                    {
                        // Manejo de errores HTTP
                        string errorContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Error al consultar la API: {response.StatusCode}");
                        Console.WriteLine($"Detalle del error: {errorContent}");

                        return new EstatusResponse
                        {
                            Exito = false,
                            Message = $"Error al actualizar el estatus del comprobante. Código: {response.StatusCode}"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores generales
                Console.WriteLine($"Error al enviar los datos a la API: {ex.Message}");

                return new EstatusResponse
                {
                    Exito = false,
                    Message = $"Error al enviar los datos: {ex.Message}"
                };
            }
        }




    }
}
