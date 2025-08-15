using Identity.Api.DTO;
using Identity.Api.Interfaces;
using Identity.Api.Paginado;
using Identity.Api.Reporteria;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modelo.laconcordia.Modelo.Database;
using QuestPDF.Infrastructure;

namespace Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FichapersonaController : Controller
    {
        private readonly DbAa5796GmoraContext _context;

        private readonly IFichapersona _fichapersona;
        public FichapersonaController(IFichapersona fichapersona)
        {
            _fichapersona = fichapersona;
        }
        [HttpGet("GetFichaPersonalInfoAll")]
        public IActionResult GetAll()
        {
            var lista = _fichapersona.GetFichaPersonalInfoAll();
            return Ok(lista);
        }
        [HttpGet("GetFichaPersonalById/{cedula}")]
        public IActionResult GetById(string cedula)
        {
            var item = _fichapersona.GetFichaPersonalById(cedula);
            if (item == null)
                return NotFound("Ficha personal no encontrada.");
            return Ok(item);
        }

        [HttpPost("InsertFichaPersonal")]
        public IActionResult Create([FromBody] FichapersonalDTO nueva)
        {
            try
            {
                _fichapersona.InsertFichaPersonal(nueva);
                return Ok("Ficha personal creada correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al crear: " + ex.Message);
            }
        }

        [HttpPut("UpdateFichaPersonal")]
        public IActionResult Update([FromBody] FichapersonalDTO actualizada)
        {
            try
            {
                _fichapersona.UpdateFichaPersonal(actualizada);
                return Ok("Ficha personal actualizada correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al actualizar: " + ex.Message);
            }
        }

        [HttpDelete("DeleteFichaPersonalById/{cedula}")]
        public IActionResult Delete(string cedula)
        {
            try
            {
                _fichapersona.DeleteFichaPersonalById(cedula);
                return Ok("Ficha personal eliminada correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al eliminar: " + ex.Message);
            }
        }

        [HttpGet("GetFichaPersonalPaginados")]
        public async Task<IActionResult> GetFichaPersonalPaginados(
            int pagina = 1,
            int pageSize = PaginadorHelper.NumeroDeDatosPorPagina,
            string? filtro = null,
            string? estado = null)
        {
            try
            {
                var resultado = await _fichapersona.GetFichaPersonalPaginados(pagina, pageSize, filtro, estado);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        //[HttpPost("ExportarFichaCompleta")]
        //public IActionResult ExportarFichaCompleta([FromBody] ExportFichaDTO exportData)
        //{
        //    if (string.IsNullOrWhiteSpace(exportData?.Ficha?.Cedula))
        //        return BadRequest("No se recibió la cédula para exportar.");

        //    QuestPDF.Settings.License = LicenseType.Community;

        //    // 🔹 Usar DbContext temporal para no afectar el resto
        //    using var context = new DbAa5796GmoraContext();

        //    // 🔹 Buscar ficha personal con relaciones
        //    var ficha = context.Fichapersonals
        //        .Include(f => f.FknacionalidadNavigation)
        //        .Include(f => f.FkestadocivilNavigation)
        //        .Include(f => f.FkcargoNavigation)
        //        .Include(f => f.FktipolicenciaNavigation)
        //        .Include(f => f.FkniveleducacionNavigation)
        //        .FirstOrDefault(f => f.Cedula == exportData.Ficha.Cedula);

        //    if (ficha == null)
        //        return NotFound("No se encontró la ficha personal.");

        //    // 🔹 Buscar unidad si existe
        //    Unidad unidad = null;
        //    if (!string.IsNullOrEmpty(ficha.Fkunidad))
        //        unidad = context.Unidads.FirstOrDefault(u => u.Unidad1 == ficha.Fkunidad);

        //    // 🔹 Buscar dueño del puesto si existe
        //    Duenopuesto duenopuesto = null;
        //    if (!string.IsNullOrEmpty(ficha.Fkdpuesto))
        //        duenopuesto = context.Duenopuestos.FirstOrDefault(d => d.Cedula == ficha.Fkdpuesto);

        //    // 🔹 Beneficiarios
        //    var beneficiarios = context.Segurovida
        //        .Where(b => b.CiBeneficiario == ficha.Cedula)
        //        .Include(b => b.PkparentescoNavigation)
        //        .AsEnumerable()
        //        .Select(b => new SegurovidumDTO
        //        {
        //            Nombres = b.Nombres,
        //            Apellidos = b.Apellidos,
        //            Pkparentesco = b.Pkparentesco,
        //            ParentescoDescripcion = b.PkparentescoNavigation?.Parentesco1
        //        })
        //        .ToList();

        //    // 🔹 Armar DTO completo
        //    var exportDataCompleto = new ExportFichaDTO
        //    {
        //        Ficha = new FichapersonalDTO
        //        {
        //            Cedula = ficha.Cedula,
        //            Nombre = ficha.Nombre,
        //            Apellidos = ficha.Apellidos,
        //            TipoLicenciaDescripcion = ficha.FktipolicenciaNavigation?.Tipolicencia,
        //            Fechanacimiento = ficha.Fechanacimiento,
        //            Fechaingreso = ficha.Fechaingreso,
        //            NacionalidadDescripcion = ficha.FknacionalidadNavigation?.Nacionalidad1,
        //            EstadoCivilDescripcion = ficha.FkestadocivilNavigation?.Descripcion,
        //            NivelEducacionDescripcion = ficha.FkniveleducacionNavigation?.Descripcion,
        //            Telefono = ficha.Telefono,
        //            Celular = ficha.Celular,
        //            Correo = ficha.Correo,
        //            CargoDescripcion = ficha.FkcargoNavigation?.Cargo1,
        //            Domicilio = ficha.Domicilio,
        //            Referencia = ficha.Referencia,
        //            Cuotaf = ficha.Cuotaf,
        //            Fkunidad = ficha.Fkunidad,
        //            Fkdpuesto = ficha.Fkdpuesto
        //        },
        //        Unidad = unidad != null ? new UnidadDTO
        //        {
        //            Unidad1 = unidad.Unidad1,
        //            Placa = unidad.Placa,
        //            Marca = unidad.Marca,
        //            Modelo = unidad.Modelo,
        //            Anio = unidad.Anio,
        //            Color = unidad.Color,
        //            Estado = unidad.Estado
        //        } : null,
        //        Duenopuesto = duenopuesto != null ? new DuenopuestoDTO
        //        {
        //            Cedula = duenopuesto.Cedula,
        //            Nombres = duenopuesto.Nombres,
        //            Apellidos = duenopuesto.Apellidos
        //        } : null,
        //        Beneficiarios = beneficiarios
        //    };

        //    var pdfBytes = FichaPersonalPdfGenerator.GenerarPdf(exportDataCompleto);

        //    if (pdfBytes == null || pdfBytes.Length == 0)
        //        return StatusCode(StatusCodes.Status500InternalServerError, "No se pudo generar el PDF.");

        //    return File(pdfBytes, "application/pdf", "FichaCompleta.pdf");
        //}
        [HttpPost("ExportarFichaCompleta")]
        public IActionResult ExportarFichaCompleta([FromBody] ExportFichaDTO exportData)
        {
            if (string.IsNullOrWhiteSpace(exportData?.Ficha?.Cedula))
                return BadRequest("No se recibió la cédula para exportar.");

            QuestPDF.Settings.License = LicenseType.Community;

            // 🔹 Usar DbContext temporal para no afectar el resto
            using var context = new DbAa5796GmoraContext();

            // 🔹 Buscar ficha personal con relaciones
            var ficha = context.Fichapersonals
                .Include(f => f.FknacionalidadNavigation)
                .Include(f => f.FkestadocivilNavigation)
                .Include(f => f.FkcargoNavigation)
                .Include(f => f.FktipolicenciaNavigation)
                .Include(f => f.FkniveleducacionNavigation)
                .FirstOrDefault(f => f.Cedula == exportData.Ficha.Cedula);

            if (ficha == null)
                return NotFound("No se encontró la ficha personal.");

            // 🔹 Buscar unidad si existe
            Unidad unidad = null;
            if (!string.IsNullOrEmpty(ficha.Fkunidad))
                unidad = context.Unidads.FirstOrDefault(u => u.Unidad1 == ficha.Fkunidad);

            // 🔹 Buscar dueño del puesto si existe
            Duenopuesto duenopuesto = null;
            if (!string.IsNullOrEmpty(ficha.Fkdpuesto))
                duenopuesto = context.Duenopuestos.FirstOrDefault(d => d.Cedula == ficha.Fkdpuesto);

            // 🔹 Beneficiarios y ayudas
            var beneficiarios = context.Segurovida
                .Where(b => b.CiAfiliado == ficha.Cedula)
                .Where(b => b.CiAfiliado == ficha.Cedula)
                .Include(b => b.PkparentescoNavigation)
                .AsEnumerable()
                .Select(b => new SegurovidumDTO
                {
                    CiBeneficiario = b.CiBeneficiario,
                    Nombres = b.Nombres,
                    Apellidos = b.Apellidos,
                    Pkparentesco = b.Pkparentesco,
                    ParentescoDescripcion = b.PkparentescoNavigation?.Parentesco1,
                    CiAfiliado = b.CiAfiliado,
                    Telefono = b.Telefono,
                    Tipo = b.Tipo // 1 = BENEFICIARIO, 2 = AYUDA
                })
                .ToList();

            // 🔹 Armar DTO completo para el PDF
            var exportDataCompleto = new ExportFichaDTO
            {
                Ficha = new FichapersonalDTO
                {
                    Cedula = ficha.Cedula,
                    Nombre = ficha.Nombre,
                    Apellidos = ficha.Apellidos,
                    TipoLicenciaDescripcion = ficha.FktipolicenciaNavigation?.Tipolicencia,
                    Fechanacimiento = ficha.Fechanacimiento,
                    Fechaingreso = ficha.Fechaingreso,
                    NacionalidadDescripcion = ficha.FknacionalidadNavigation?.Nacionalidad1,
                    EstadoCivilDescripcion = ficha.FkestadocivilNavigation?.Descripcion,
                    NivelEducacionDescripcion = ficha.FkniveleducacionNavigation?.Descripcion,
                    Telefono = ficha.Telefono,
                    Celular = ficha.Celular,
                    Correo = ficha.Correo,
                    CargoDescripcion = ficha.FkcargoNavigation?.Cargo1,
                    Domicilio = ficha.Domicilio,
                    Referencia = ficha.Referencia,
                    Cuotaf = ficha.Cuotaf,
                    Fkunidad = ficha.Fkunidad,
                    Fkdpuesto = ficha.Fkdpuesto
                },
                Unidad = unidad != null ? new UnidadDTO
                {
                    Unidad1 = unidad.Unidad1,
                    Placa = unidad.Placa,
                    Marca = unidad.Marca,
                    Modelo = unidad.Modelo,
                    Anio = unidad.Anio,
                    Color = unidad.Color,
                    Estado = unidad.Estado
                } : null,
                Duenopuesto = duenopuesto != null ? new DuenopuestoDTO
                {
                    Cedula = duenopuesto.Cedula,
                    Nombres = duenopuesto.Nombres,
                    Apellidos = duenopuesto.Apellidos
                } : null,
                Beneficiarios = beneficiarios
            };

            // 🔹 Generar PDF
            var pdfBytes = FichaPersonalPdfGenerator.GenerarPdf(exportDataCompleto);

            if (pdfBytes == null || pdfBytes.Length == 0)
                return StatusCode(StatusCodes.Status500InternalServerError, "No se pudo generar el PDF.");

            // 🔹 Retornar archivo
            return File(pdfBytes, "application/pdf", "FichaCompleta.pdf");
        }






    }
}
