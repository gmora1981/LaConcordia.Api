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
    }
}
