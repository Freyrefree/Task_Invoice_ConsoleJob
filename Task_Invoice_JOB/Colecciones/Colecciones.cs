using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Task_Invoice_JOB.Contexto;

namespace Task_Invoice_JOB.Colecciones
{
    public class Colecciones
    {



    }


    // ******************************* Data Facturas SAP **************************

    public class facturasTimbradas
    {
        public string? Sociedad { get; set; }
        public string? Ejercicio { get; set; }
        public string? Periodo { get; set; }
        public string? Fecha { get; set; }
        public string? ClaseDocumento { get; set; }
        public string? Referencia { get; set; }
        public string? NumeroDocumento { get; set; }
        public string? Folio { get; set; }
        public string? Moneda { get; set; }
        public decimal TipoCambio { get; set; }
        public string? EstatusCFDIMonitor { get; set; }
        public string? UUIDMonitor { get; set; }
        public string? Cliente { get; set; }
        public string? PathXml { get; set; }
        public string? PathPDF { get; set; }

        // los siguientes elementos se usan en el proceso de lectura de XML
        public string? VersionCFDIXml { get; set; }
        public string? SerieXml { get; set; }
        public string? FolioXml { get; set; }
        public DateTime? FechaTimbradoXml { get; set; }
        public int FormaPagoXml { get; set; }
        public string? CondicionesDePagoXml { get; set; }
        public decimal TipoCambioXml { get; set; }
        public decimal MontoFacturaXml { get; set; }
        public string? TipoComprobanteXml { get; set; }
        public string? MetodoPagoXml { get; set; }
        public string? RFCEmisorXml { get; set; }
        public string? RFCReceptorXml { get; set; }
        public string? RsEmisorXml { get; set; }
        public string? RsReceptorXml { get; set; }
        public string? UUIDXml { get; set; }
        public string? EstatusCFDI { get; set; }
        public int? EstatusEnvio { get; set; }

        // los siguientes elementos se usan PARA PAGOS

        public DateTime? FechaPago { get; set; }
        public string? FormaDePagoP { get; set; }
        public string? MonedaP { get; set; }
        public decimal? TipoCambioP { get; set; }
        public decimal? Monto { get; set; }
        public string? NumOperacion { get; set; }
        public string? RfcEmisorCtaOrd { get; set; }
        public string? NomBancoOrdExt { get; set; }
        public string? RfcEmisorCtaBen { get; set; }
        public string? CtaBeneficiario { get; set; }

        // Documentos relacionados
       
        public List<DoctoRelacionado>? DoctosRelacionados { get; set; } = new List<DoctoRelacionado>();

    }

    //  ******************************* Data Comprobantes Task Invoice ***********************

    public class ResponseComprobante
    {
        public string Message { get; set; }
        public List<Comprobante> Detalle { get; set; }
    }

    public class Comprobante
    {
        public int Id { get; set; }
        public string SociedadSAP { get; set; }
        public string ClaveClienteSAP { get; set; }
        public DateTime FechaSAP { get; set; }
        public int PeriodoSAP { get; set; }
        public string ReferenciaSAP { get; set; }
        public string FolioSAP { get; set; }
        public string MonedaSAP { get; set; }
        public decimal TipoCambioSAP { get; set; }
        public string UuidSAP { get; set; }
        public string ClaseDocumentoSAP { get; set; }
        public string PathXML { get; set; }
        public string PathPDF { get; set; }
        public string VersionCFDI { get; set; }
        public string Serie { get; set; }
        public string Folio { get; set; }
        public DateTime FechaTimbrado { get; set; }
        public int FormaPago { get; set; }
        public string CondicionesPago { get; set; }
        public decimal TipoCambio { get; set; }
        public decimal MontoFactura { get; set; }
        public string TipoComprobante { get; set; }
        public string MetodoPago { get; set; }
        public string RfcEmisor { get; set; }
        public string RfcReceptor { get; set; }
        public string RsEmisor { get; set; }
        public string RsReceptor { get; set; }
        public string Uuid { get; set; }
        public string EstatusCFDI { get; set; }
        public int EstatusEnvio { get; set; }
        public DateTime FechaConsultaEstatus { get; set; }
    }


    // *******************************  Respuesta de Almacenamiento *******************

    public class ResponseDto
    {
        public string Message { get; set; }
        public List<ComprobanteDetalle> Detalle { get; set; }
    }

    public class ComprobanteDetalle
    {
        public string Folio { get; set; }
        public string UUID { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }


    //********************  Datos XML ************************************************

    public class CFDI
    {
        public string? Version { get; set; }
        public string? Serie { get; set; }
        public long Folio { get; set; }
        public int FormaPago { get; set; }
        public string? CondicionesDePago { get; set; }
        public string? Moneda { get; set; }
        public string TipoCambio { get; set; }
        public string Total { get; set; }
        public string? TipoDeComprobante { get; set; }
        public string? MetodoPago { get; set; }

        public Emisor? Emisor { get; set; }
        public Receptor? Receptor { get; set; }
        public Complemento? Complemento { get; set; }
        public Pagos? Pagos { get; set; }

    }

    public class Emisor
    {
        public string? Rfc { get; set; }
        public string? Nombre { get; set; }
    }

    public class Receptor
    {
        public string? Rfc { get; set; }
        public string? Nombre { get; set; }
        public string? UsoCFDI { get; set; }
    }

    public class Complemento
    {
        public TimbreFiscalDigital? TimbreFiscalDigital { get; set; }

    }

    public class TimbreFiscalDigital
    {
        public string? Uuid { get; set; }
        public DateTime FechaTimbrado { get; set; }
    }

    public class Pagos
    {
        public string Version { get; set; }

        public Totales Totales { get; set; }

        public List<Pago> PagosDetalles { get; set; } = new List<Pago>();
    }

    public class Totales
    {
        public string MontoTotalPagos { get; set; }

        public string TotalTrasladosBaseIVA0 { get; set; }

        public string TotalTrasladosImpuestoIVA0 { get; set; }
    }

    public class Pago
    {
        public DateTime FechaPago { get; set; }

        public string FormaDePagoP { get; set; }

        public string MonedaP { get; set; }

        public string TipoCambioP { get; set; }

        public string Monto { get; set; }

        public string NumOperacion { get; set; }

        public string RfcEmisorCtaOrd { get; set; }

        public string NomBancoOrdExt { get; set; }

        public string RfcEmisorCtaBen { get; set; }

        public string CtaBeneficiario { get; set; }

        public List<DoctoRelacionado> DocumentosRelacionados { get; set; } = new List<DoctoRelacionado>();

        public ImpuestosP ImpuestosP { get; set; }
    }

    public class DoctoRelacionado
    {
        public string IdDocumento { get; set; }
        public string Folio { get; set; }
        public string MonedaDR { get; set; }
        public string EquivalenciaDR { get; set; }
        public int NumParcialidad { get; set; }
        public string ImpSaldoAnt { get; set; }
        public string ImpPagado { get; set; }
        public string ImpSaldoInsoluto { get; set; }
        public string ObjetoImpDR { get; set; }
    }

    public class ImpuestosDR
    {
        public List<TrasladoDR> TrasladosDR { get; set; } = new List<TrasladoDR>();
    }

    public class TrasladoDR
    {
        public string BaseDR { get; set; }

        public string ImpuestoDR { get; set; }

        public string TipoFactorDR { get; set; }

        public string TasaOCuotaDR { get; set; }

        public string ImporteDR { get; set; }
    }

    public class ImpuestosP
    {
        public List<TrasladoP> TrasladosP { get; set; } = new List<TrasladoP>();
    }

    public class TrasladoP
    {
        public string BaseP { get; set; }

        public string ImpuestoP { get; set; }

        public string TipoFactorP { get; set; }

        public string TasaOCuotaP { get; set; }

        public string ImporteP { get; set; }
    }









    //************************  Clientes ****************************************

    public class Cliente
    {
        public int Id { get; set; }
        public int ClaveCliente { get; set; }
        public String? Sociedad { get; set; }
        public string? Nombre { get; set; }
        public Scheduler? Scheduler { get; set; }
        public Archivos? Archivos { get; set; }
        public List<Correo>? CorreosTO { get; set; }
        public List<Correo>? CorreosCC { get; set; }
        public List<Comprobante>? ComprobantesPendientes { get; set; }
    }

    public class Scheduler
    {
        public List<Dia>? Dias { get; set; }
    }

    public class Dia
    {
        public int Numero { get; set; }
        public string? Nombre { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
    }

    public class Archivos
    {
        public bool XML { get; set; }
        public bool PDF { get; set; }
    }

    public class Correo
    {
        public string? Email { get; set; }
        public string? NombreContacto { get; set; }
        public string? TipoCorreo { get; internal set; }
    }




    ////******************** Respuesta de actualizacion de estatus ***********************
    ///
    public class EstatusResponse
    {
        public bool Exito { get; set; } // Indica si la operación fue exitosa o no
        public string Message { get; set; } // Mensaje descriptivo
    }


    // ********************** Esttaus Facturas CFDI ****************************+
    public class ConsultaCFDIResponse
    {
        public string CodigoEstatus { get; set; }
        public string EsCancelable { get; set; }
        public string Estado { get; set; }
        public string EstatusCancelacion { get; set; }
        public string ValidacionEFOS { get; set; }
    }



}
