using Identity.Api.DTO;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.DataRepository
{
    public class GenerarPlanChoqueRepository
    {
        private readonly DbAa5796GmoraContext _context;

        public GenerarPlanChoqueRepository()
        {
            _context = new DbAa5796GmoraContext();
        }

        public bool YaFueGenerado(string unidad)
        {
            using var context = new DbAa5796GmoraContext();
            return context.Generarplanchoques.Any(p => p.Unidad == unidad);
        }

        public GenerarPlanResultadoDTO GenerarPlanChoque(GenerarPlanChoqueRequestDTO request)
        {
            if (YaFueGenerado(request.Unidad))
                throw new InvalidOperationException("Ya se generó el plan de choque para esta unidad.");

            var activos = _context.Fichapersonals
                .Where(f => f.Estado == "a")
                .Select(f => f.Cedula)
                .ToList();

            var resultado = new GenerarPlanResultadoDTO();
            var fecha = DateOnly.FromDateTime(DateTime.Now);

            foreach (var cedula in activos)
            {
                _context.Generarplanchoques.Add(new Generarplanchoque
                {
                    Unidad = request.Unidad,
                    Cidentidad = cedula,
                    Fecha = fecha,
                    Observacion = request.Observacion,
                    Valor = request.Valor,
                    Abono = 0m
                });
                resultado.CedulasGeneradas.Add(cedula);
            }

            _context.SaveChanges();
            resultado.TotalGenerados = resultado.CedulasGeneradas.Count;
            return resultado;
        }
    }
}
