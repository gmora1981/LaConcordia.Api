
using FluentFTP;
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
using System.Net;


//se usar FluentFTP par aenvio ft

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


        // 📌 Subir imagen del chofer
        [HttpPost("SubirImagenChoferDocumentos")]
        public IActionResult SubirImagenChofer([FromForm] IFormFile? archivo, [FromForm] string cedula, [FromForm] string tipoDocumento)
        {
            string host = "win8104.site4now.net";
            string user = "lconcordiadoc";
            string pass = "Geo100100.";
            string basePath = "/documentos";
            string urlBase = "https://lconcordia.compugtech.com/documentos";

            var log = new List<string>();

            try
            {
                if (archivo != null && archivo.Length > 0)
                {
                    var extension = Path.GetExtension(archivo.FileName);
                    var nombreArchivo = $"{cedula}-{tipoDocumento}{extension}";
                    var rutaRemota = $"{basePath}/{nombreArchivo}";

                    using (var client = new FtpClient(host, new NetworkCredential(user, pass)))
                    {
                        client.Connect();
                        log.Add("Conexión FTP establecida.");

                        // 🔄 Borrar únicamente el archivo viejo del mismo tipo
                        foreach (var item in client.GetListing(basePath))
                        {
                            if (item.Type == FtpObjectType.File && item.Name.StartsWith($"{cedula}-{tipoDocumento}"))
                            {
                                client.DeleteFile(item.FullName);
                                log.Add($"Archivo viejo eliminado: {item.Name}");
                            }
                        }

                        // 📤 Subir nuevo archivo
                        using (var stream = archivo.OpenReadStream())
                        {
                            client.UploadStream(stream, rutaRemota, FtpRemoteExists.Overwrite, true);
                            log.Add($"Archivo subido: {nombreArchivo}");
                        }
                    }

                    string urlPublica = $"{urlBase}/{nombreArchivo}";
                    return Ok(new { mensaje = "Imagen subida correctamente ✅", url = urlPublica, log });
                }

                return BadRequest("No se envió ningún archivo.");
            }
            catch (Exception ex)
            {
                log.Add($"Error: {ex.Message}");
                return StatusCode(500, new { mensaje = "Error al subir la imagen", log });
            }
        }

        // 📌 Subir imagen de Matrícula
        [HttpPost("SubirImagenMatricula")]
        public IActionResult SubirImagenMatricula([FromForm] IFormFile? archivo, [FromForm] string cedula)
        {
            string host = "win8104.site4now.net";
            string user = "lconcordiadoc";
            string pass = "Geo100100.";
            string basePath = "/matricula";
            string urlBase = "https://lconcordia.compugtech.com/matricula";
            var log = new List<string>();

            try
            {
                if (archivo != null && archivo.Length > 0)
                {
                    var extension = Path.GetExtension(archivo.FileName);
                    var nombreArchivo = $"{cedula}{extension}";
                    var rutaRemota = $"{basePath}/{nombreArchivo}";

                    using (var client = new FtpClient(host, new NetworkCredential(user, pass)))
                    {
                        client.Connect();
                        log.Add("Conexión FTP establecida.");

                        if (!client.DirectoryExists(basePath)) client.CreateDirectory(basePath);

                        foreach (var item in client.GetListing(basePath))
                            if (item.Type == FtpObjectType.File && item.Name.StartsWith(cedula))
                            {
                                client.DeleteFile(item.FullName);
                                log.Add($"Archivo viejo eliminado: {item.Name}");
                            }

                        using (var stream = archivo.OpenReadStream())
                            client.UploadStream(stream, rutaRemota, FtpRemoteExists.Overwrite, true);

                        log.Add($"Archivo subido: {nombreArchivo}");
                    }

                    string urlPublica = $"{urlBase}/{nombreArchivo}";
                    return Ok(new { mensaje = "Imagen de matrícula subida correctamente ✅", url = urlPublica, log });
                }

                return BadRequest("No se envió ningún archivo.");
            }
            catch (Exception ex)
            {
                log.Add($"Error: {ex.Message}");
                return StatusCode(500, new { mensaje = "Error al subir la imagen de matrícula", log });
            }
        }

        // 📌 Subir imagen de Vehículo
        [HttpPost("SubirImagenVehiculo")]
        public IActionResult SubirImagenVehiculo([FromForm] IFormFile? archivo, [FromForm] string cedula)
        {
            string host = "win8104.site4now.net";
            string user = "lconcordiadoc";
            string pass = "Geo100100.";
            string basePath = "/vehiculo";
            string urlBase = "https://lconcordia.compugtech.com/vehiculo";
            var log = new List<string>();

            try
            {
                if (archivo != null && archivo.Length > 0)
                {
                    var extension = Path.GetExtension(archivo.FileName);
                    var nombreArchivo = $"{cedula}{extension}";
                    var rutaRemota = $"{basePath}/{nombreArchivo}";

                    using (var client = new FtpClient(host, new NetworkCredential(user, pass)))
                    {
                        client.Connect();
                        log.Add("Conexión FTP establecida.");

                        if (!client.DirectoryExists(basePath)) client.CreateDirectory(basePath);

                        foreach (var item in client.GetListing(basePath))
                            if (item.Type == FtpObjectType.File && item.Name.StartsWith(cedula))
                            {
                                client.DeleteFile(item.FullName);
                                log.Add($"Archivo viejo eliminado: {item.Name}");
                            }

                        using (var stream = archivo.OpenReadStream())
                            client.UploadStream(stream, rutaRemota, FtpRemoteExists.Overwrite, true);

                        log.Add($"Archivo subido: {nombreArchivo}");
                    }

                    string urlPublica = $"{urlBase}/{nombreArchivo}";
                    return Ok(new { mensaje = "Imagen de vehículo subida correctamente ✅", url = urlPublica, log });
                }

                return BadRequest("No se envió ningún archivo.");
            }
            catch (Exception ex)
            {
                log.Add($"Error: {ex.Message}");
                return StatusCode(500, new { mensaje = "Error al subir la imagen de vehículo", log });
            }
        }

        // 📌 Subir imagen de Licencia
        [HttpPost("SubirImagenLicencia")]
        public IActionResult SubirImagenLicencia([FromForm] IFormFile? archivo, [FromForm] string cedula)
        {
            string host = "win8104.site4now.net";
            string user = "lconcordiadoc";
            string pass = "Geo100100.";
            string basePath = "/licencia";
            string urlBase = "https://lconcordia.compugtech.com/licencia";
            var log = new List<string>();

            try
            {
                if (archivo != null && archivo.Length > 0)
                {
                    var extension = Path.GetExtension(archivo.FileName);
                    var nombreArchivo = $"{cedula}{extension}";
                    var rutaRemota = $"{basePath}/{nombreArchivo}";

                    using (var client = new FtpClient(host, new NetworkCredential(user, pass)))
                    {
                        client.Connect();
                        log.Add("Conexión FTP establecida.");

                        if (!client.DirectoryExists(basePath)) client.CreateDirectory(basePath);

                        foreach (var item in client.GetListing(basePath))
                            if (item.Type == FtpObjectType.File && item.Name.StartsWith(cedula))
                            {
                                client.DeleteFile(item.FullName);
                                log.Add($"Archivo viejo eliminado: {item.Name}");
                            }

                        using (var stream = archivo.OpenReadStream())
                            client.UploadStream(stream, rutaRemota, FtpRemoteExists.Overwrite, true);

                        log.Add($"Archivo subido: {nombreArchivo}");
                    }

                    string urlPublica = $"{urlBase}/{nombreArchivo}";
                    return Ok(new { mensaje = "Imagen de licencia subida correctamente ✅", url = urlPublica, log });
                }

                return BadRequest("No se envió ningún archivo.");
            }
            catch (Exception ex)
            {
                log.Add($"Error: {ex.Message}");
                return StatusCode(500, new { mensaje = "Error al subir la imagen de licencia", log });
            }
        }












        // 📌 Buscar imagen por cédula en FTP
        [HttpGet("BuscarImagenesChofer/{cedula}")]
        public IActionResult BuscarImagenesChofer(string cedula)
        {
            string host = "win8104.site4now.net";
            string user = "lconcordiadoc";
            string pass = "Geo100100.";

            var carpetas = new Dictionary<string, string>
    {
        { "Documento", "/documentos" },
        { "Licencia", "/Licencia" },
        { "Matricula", "/Matricula" },
        { "Vehiculo", "/Vehiculo" }
    };

            bool frontal = false;
            bool trasera = false;
            bool licencia = false;
            bool matricula = false;
            bool vehiculo = false;

            using var client = new FtpClient(host, new NetworkCredential(user, pass));
            try
            {
                client.Connect();
                if (!client.IsConnected)
                    return StatusCode(403, new { mensaje = "Error de autenticación FTP" });

                foreach (var item in carpetas)
                {
                    var carpetaNombre = item.Key;
                    var carpetaRuta = item.Value;

                    var archivos = client.GetListing(carpetaRuta)
                                         .Where(f => f.Type == FtpObjectType.File &&
                                                     f.Name.StartsWith(cedula, StringComparison.OrdinalIgnoreCase))
                                         .ToList();

                    if (carpetaNombre == "Documento")
                    {
                        frontal = archivos.Any(a => a.Name.Contains("FRONTAL", StringComparison.OrdinalIgnoreCase));
                        trasera = archivos.Any(a => a.Name.Contains("TRASERA", StringComparison.OrdinalIgnoreCase));
                    }
                    else if (carpetaNombre == "Licencia")
                    {
                        licencia = archivos.Any();
                    }
                    else if (carpetaNombre == "Matricula")
                    {
                        matricula = archivos.Any();
                    }
                    else if (carpetaNombre == "Vehiculo")
                    {
                        vehiculo = archivos.Any();
                    }
                }

                return Ok(new
                {
                    Frontal = frontal,
                    Trasera = trasera,
                    Licencia = licencia,
                    Matricula = matricula,
                    Vehiculo = vehiculo
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error al buscar imágenes: {ex.Message}" });
            }
        }







        // 📌 Eliminar imagen por cédula en FTP
        [HttpDelete("EliminarImagenChofer/{cedula}")]
        public IActionResult EliminarImagenChofer(string cedula)
        {
            string host = "win8104.site4now.net";
            string user = "lconcordiadoc";
            string pass = "Geo100100.";
            string basePath = "/documentos";

            var log = new List<string>();

            try
            {
                using var client = new FtpClient(host, new NetworkCredential(user, pass));
                client.Connect();
                log.Add("Conexión FTP establecida.");

                if (!client.DirectoryExists(basePath))
                    return Ok(new { mensaje = "No hay carpeta '/documentos' en FTP.", log });

                var archivos = client.GetListing(basePath)
                    .Where(f => f.Name.StartsWith(cedula))
                    .Select(f => f.FullName)
                    .ToList();

                if (!archivos.Any())
                    return Ok(new { mensaje = "No se encontró ninguna imagen para esta cédula.", log });

                foreach (var file in archivos)
                {
                    client.DeleteFile(file);
                    log.Add($"Archivo eliminado: {file}");
                }

                return Ok(new { mensaje = "Imagen eliminada correctamente ✅", log });
            }
            catch (Exception ex)
            {
                log.Add($"Error: {ex.Message}");
                return StatusCode(500, new { mensaje = "Error al eliminar imagen", log });
            }
        }




        // 📌 Método de debug para verificar existencia y permisos de la imagen

        [HttpGet("ProbarFtp")]
        public IActionResult ProbarFtp()
        {
            string host = "win8104.site4now.net";
            string user = "lconcordiadoc";
            string pass = "Geo100100.";
            string basePath = "/documentos";
            string publicBaseUrl = "https://lconcordia.compugtech.com/documentos";

            var log = new List<string>();
            FtpClient client = null;

            try
            {
                client = new FtpClient(host, new NetworkCredential(user, pass));
                log.Add("Cliente FTP creado.");

                client.Connect();
                log.Add("Conexión al servidor FTP establecida.");

                if (!client.IsConnected)
                {
                    log.Add("Autenticación fallida.");
                    return StatusCode(403, new { mensaje = "Error de autenticación FTP", log });
                }
                log.Add("Autenticación exitosa.");

                var raiz = client.GetListing("/");
                log.Add("Contenido raíz del FTP:");
                foreach (var item in raiz)
                {
                    log.Add($"- {item.Type}: {item.FullName}");
                }

                if (!client.DirectoryExists(basePath))
                {
                    log.Add($"Carpeta '{basePath}' no existe. Creándola...");
                    client.CreateDirectory(basePath);
                    log.Add($"Carpeta '{basePath}' creada correctamente.");
                }
                else
                {
                    log.Add($"Carpeta '{basePath}' encontrada.");
                }

                var archivos = client.GetListing(basePath);

                // Construir URLs públicas
                var urlsPublicas = archivos
                    .Where(a => a.Type == FtpObjectType.File)
                    .Take(5)
                    .Select(a => $"{publicBaseUrl}/{a.Name}");

                return Ok(new
                {
                    mensaje = "Conexión FTP exitosa ✅",
                    cantidadArchivos = archivos.Length,
                    ejemplos = urlsPublicas,
                    log
                });
            }
            catch (FluentFTP.Exceptions.FtpCommandException ftpEx)
            {
                log.Add($"Error FTP: Código {ftpEx.CompletionCode}, Mensaje: {ftpEx.Message}");
                return StatusCode(403, new { mensaje = "Error de autenticación FTP", log });
            }
            catch (Exception ex)
            {
                log.Add($"Error inesperado: {ex.Message}");
                return StatusCode(500, new { mensaje = "Error inesperado", log });
            }
            finally
            {
                client?.Dispose();
            }
        }



        [HttpGet("DescargarFoto/{fileName}")]
        public IActionResult DescargarFoto(string fileName)
        {
            string host = "win8104.site4now.net";
            string user = "lconcordiadoc";
            string pass = "Geo100100.";
            string basePath = "/documentos";

            using var client = new FtpClient(host, new NetworkCredential(user, pass));
            try
            {
                client.Connect();

                if (!client.IsConnected)
                    return StatusCode(403, new { mensaje = "Error de autenticación FTP" });

                var remotePath = $"{basePath}/{fileName}";

                if (!client.FileExists(remotePath))
                    return NotFound(new { mensaje = $"Archivo {fileName} no existe en FTP" });

                using var ms = new MemoryStream();
                client.DownloadStream(ms, remotePath);
                ms.Position = 0;

                // Detectar content type simple (puedes mejorar con un helper si tienes más tipos)
                var extension = Path.GetExtension(fileName).ToLowerInvariant();
                string contentType = extension switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    ".gif" => "image/gif",
                    _ => "application/octet-stream"
                };

                return File(ms.ToArray(), contentType, fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error al descargar archivo: {ex.Message}" });
            }
        }




    }
}
