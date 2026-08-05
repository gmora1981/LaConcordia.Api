using Identity.Api.DTO;

namespace Identity.Api.Interfaces
{
    public interface IGenerarMulta
    {
        // Multas registradas para un socio (para historial y para el hub de cobranza).
        List<GenerarMultaDTO> GetMultasPorSocio(string cidentidad);

        // El Idmulta se calcula en el servidor (siguiente consecutivo para ese socio,
        // ya que la clave primaria es compuesta Idmulta+Cidentidad).
        GenerarMultaDTO InsertGenerarMulta(GenerarMultaDTO nueva);
    }
}
