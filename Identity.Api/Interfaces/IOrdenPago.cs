using Identity.Api.DTO;

namespace Identity.Api.Interfaces
{
    public interface IOrdenPago
    {
        // Pedidos facturados a una empresa (Ruc no vacio) que aun no tienen voucher generado.
        List<PedidoVoucherDTO> GetPedidosPendientesVoucher(string? ruc = null);

        // Texto de direccion (calle + numero + referencia) mas cercano a esas coordenadas para ese celular.
        string? GetDireccionTexto(string celular, decimal lat, decimal lng);

        decimal GetSaldoCajaActual();

        // Guarda de forma atomica: OrdenPago + actualizacion del Pedido (voucher) + auditoria + egreso de caja.
        // Valida que el saldo final de caja no quede negativo antes de escribir nada.
        OrdenPagoResultadoDTO GenerarOrdenPago(GenerarOrdenPagoRequestDTO request, string usuario);

        // Facturacion: listado de vouchers ya generados para una empresa, opcionalmente
        // acotado hasta una fecha (incluye el dia completo).
        List<OrdenPagoResumenDTO> GetOrdenPagoPorEmpresa(string ruc, DateTime? hasta = null);

        // "Modificar Datos": corrige Precio/Recorrido/Empleado sin generar voucher.
        void ActualizarDatosPedido(ActualizarDatosPedidoRequestDTO request, string usuario);

        // PDF del listado de vouchers de una empresa, igual al reporte RptEmpresaVoucher del escritorio.
        byte[] ExportarFacturacionPdf(string ruc, string razonSocial, DateTime? hasta, string usuario);

        // "Reporte de Voucher por Pagar": pedidos con empresa asignada aun sin voucher
        // generado, filtrados por unidad (opcional) y rango de fechas.
        List<ReporteVoucherPagarDTO> GetVouchersPendientesPorUnidad(string? unidad, DateTime desde, DateTime hasta);
        byte[] ExportarReporteVoucherPagarPdf(string? unidad, DateTime desde, DateTime hasta, string usuario);

        // Dashboard "Vouchers Emitidos".
        ResumenVoucherDTO GetResumenVouchers(DateTime desde, DateTime hasta, string? ruc = null);
    }
}
