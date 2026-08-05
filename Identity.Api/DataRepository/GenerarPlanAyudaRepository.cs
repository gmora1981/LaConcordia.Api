using Identity.Api.DTO;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.DataRepository
{
    public class GenerarPlanAyudaRepository
    {
        private readonly DbAa5796GmoraContext _context;

        public GenerarPlanAyudaRepository()
        {
            _context = new DbAa5796GmoraContext();
        }

        public List<BeneficiarioDTO> GetBeneficiariosPorAfiliado(string ciAfiliado)
        {
            using var context = new DbAa5796GmoraContext();

            return context.Segurovida
                .Where(b => b.CiAfiliado == ciAfiliado && b.Estado == "a")
                .Select(b => new BeneficiarioDTO
                {
                    CiBeneficiario = b.CiBeneficiario,
                    Pkparentesco = b.Pkparentesco,
                    Nombres = b.Nombres,
                    Apellidos = b.Apellidos,
                    CiAfiliado = b.CiAfiliado,
                    Telefono = b.Telefono,
                    Tipo = b.Tipo,
                    Estado = b.Estado
                })
                .ToList();
        }

        public bool YaFueGenerado(string beneficiario)
        {
            using var context = new DbAa5796GmoraContext();
            return context.Generarplanayuda.Any(p => p.Beneficiario == beneficiario);
        }

        public GenerarPlanResultadoDTO GenerarPlanAyuda(GenerarPlanAyudaRequestDTO request)
        {
            if (YaFueGenerado(request.Beneficiario))
                throw new InvalidOperationException("Ya se generó un plan de ayuda para este beneficiario.");

            var activos = _context.Fichapersonals
                .Where(f => f.Estado == "a")
                .Select(f => f.Cedula)
                .ToList();

            var resultado = new GenerarPlanResultadoDTO();
            var fecha = DateOnly.FromDateTime(DateTime.Now);

            foreach (var cedula in activos)
            {
                _context.Generarplanayuda.Add(new Generarplanayudum
                {
                    Beneficiario = request.Beneficiario,
                    Cidentidad = cedula,
                    Fecha = fecha,
                    Observacion = request.Observacion,
                    Valor = request.Valor,
                    Abono = 0m
                });
                resultado.CedulasGeneradas.Add(cedula);
            }

            // Marca el beneficiario como usado para que no se vuelva a generar ayuda para el mismo evento.
            var beneficiario = _context.Segurovida.FirstOrDefault(b =>
                b.CiBeneficiario == request.Beneficiario && b.CiAfiliado == request.CiAfiliado);
            if (beneficiario != null)
                beneficiario.Estado = "i";

            _context.SaveChanges();
            resultado.TotalGenerados = resultado.CedulasGeneradas.Count;
            return resultado;
        }
    }
}
