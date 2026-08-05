using Identity.Api.DTO;

namespace Identity.Api.Interfaces
{
    public interface IGenerarPlanChoque
    {
        // True si ya se genero un plan de choque para esa unidad (evita duplicar el cobro colectivo).
        bool YaFueGenerado(string unidad);

        // Cobra "Valor" a todos los socios activos a favor de la unidad accidentada.
        GenerarPlanResultadoDTO GenerarPlanChoque(GenerarPlanChoqueRequestDTO request);
    }
}
