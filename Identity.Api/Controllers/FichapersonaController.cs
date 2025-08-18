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


        //ingresar y reemplazar imagen

        [HttpPost("SubirImagenChofer")]
        public async Task<IActionResult> SubirImagenChofer([FromForm] IFormFile archivo, [FromForm] string cedula)
        {
            try
            {
                if (archivo == null || archivo.Length == 0)
                    return BadRequest("No se ha proporcionado ninguna imagen.");

                var carpeta = Path.Combine("documentos");
                if (!Directory.Exists(carpeta))
                    Directory.CreateDirectory(carpeta);

                // Nombre de archivo = cedula + extensión original
                var extension = Path.GetExtension(archivo.FileName);
                var rutaArchivo = Path.Combine(carpeta, $"{cedula}{extension}");

                // 🔄 Si ya existe una imagen con esa cédula → eliminarla
                var archivosExistentes = Directory.GetFiles(carpeta, $"{cedula}.*");
                foreach (var file in archivosExistentes)
                {
                    System.IO.File.Delete(file);
                }

                // Guardar nueva imagen
                using (var stream = new FileStream(rutaArchivo, FileMode.Create))
                {
                    await archivo.CopyToAsync(stream);
                }

                return Ok(new { mensaje = "Imagen guardada correctamente", nombreArchivo = $"{cedula}{extension}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al guardar la imagen: {ex.Message}");
            }
        }

        //Buscar imagen por cédula
        [HttpGet("BuscarImagenChofer")]
        public IActionResult BuscarImagenChofer(string cedula)
        {
            try
            {
                var carpeta = Path.Combine("documentos");
                var archivos = Directory.GetFiles(carpeta, $"{cedula}.*");

                if (archivos.Length == 0)
                    return NotFound("No se encontró ninguna imagen para esta cédula.");

                var archivo = archivos.First();
                var extension = Path.GetExtension(archivo).ToLower();

                var contentType = extension switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    ".gif" => "image/gif",
                    _ => "application/octet-stream"
                };

                var bytes = System.IO.File.ReadAllBytes(archivo);
                return File(bytes, contentType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al buscar la imagen: {ex.Message}");
            }
        }

        //Eliminar imagen por cédula
        [HttpDelete("EliminarImagenChofer")]
        public IActionResult EliminarImagenChofer(string cedula)
        {
            try
            {
                var carpeta = Path.Combine("documentos");
                var archivos = Directory.GetFiles(carpeta, $"{cedula}.*");

                if (archivos.Length == 0)
                    return NotFound("No se encontró ninguna imagen para esta cédula.");

                foreach (var archivo in archivos)
                {
                    System.IO.File.Delete(archivo);
                }

                return Ok(new { mensaje = "Imagen eliminada correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar la imagen: {ex.Message}");
            }
        }

        //debug
        [HttpGet("VerificarImagen/{cedula}")]
        public IActionResult VerificarImagen(string cedula)
        {
            try
            {
                // Ruta base de las imágenes en tu servidor
                string carpeta = "/documentos"; // 👈 ajusta si fuera absoluta (ej: "C:/inetpub/wwwroot/documentos")

                // Buscamos por cédula (puede ser cualquier extensión: .jpg, .png, etc.)
                var archivo = Directory.GetFiles(carpeta, $"{cedula}.*").FirstOrDefault();

                if (archivo == null)
                {
                    return NotFound($"No se encontró ninguna imagen para la cédula {cedula}.");
                }

                // Intentamos leer el archivo para comprobar permisos
                using (var stream = System.IO.File.OpenRead(archivo))
                {
                    // Si se puede abrir, significa que hay permisos de lectura
                }

                return Ok($"Imagen encontrada y con permisos correctos: {Path.GetFileName(archivo)}");
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode(403, "No tienes permisos para acceder a la carpeta o al archivo.");
            }
            catch (DirectoryNotFoundException)
            {
                return NotFound("La carpeta no existe en el servidor.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error inesperado: {ex.Message}");
            }
        }

    }
}
