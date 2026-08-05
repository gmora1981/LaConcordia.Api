using Identity.Api.DTO;

namespace Identity.Api.Interfaces
{
    public interface IGenerarPlanAyuda
    {
        // Beneficiarios activos registrados para un afiliado (SeguroVida.Estado = 'a').
        List<BeneficiarioDTO> GetBeneficiariosPorAfiliado(string ciAfiliado);

        // True si ya se genero un plan de ayuda para ese beneficiario (evita duplicar el cobro colectivo).
        bool YaFueGenerado(string beneficiario);

        // Cobra "Valor" a todos los socios activos a favor de "Beneficiario", y marca
        // el registro de SeguroVida correspondiente como usado (Estado = 'i').
        GenerarPlanResultadoDTO GenerarPlanAyuda(GenerarPlanAyudaRequestDTO request);
    }
}
