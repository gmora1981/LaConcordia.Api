using Identity.Api.DTO;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace Identity.Api.Reporteria
{
    public static class FichaPersonalPdfGenerator
    {
        public static byte[] GenerarPdf(ExportFichaDTO data)
        {
            var ficha = data.Ficha;
            var unidad = data.Unidad;
            var duenopuesto = data.Duenopuesto;
            var beneficiarios = data.Beneficiarios ?? new List<SegurovidumDTO>();

            var doc = Document.Create(container =>
            {
                // Logo
                var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "favicon.jpeg");
                byte[] logoImage = null;
                if (File.Exists(logoPath))
                    logoImage = File.ReadAllBytes(logoPath);

                container.Page(page =>
                {
                    page.Margin(20);
                    page.Size(PageSizes.A4);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    // ---- ENCABEZADO ----
                    page.Header().Height(80).Row(row =>
                    {
                        row.RelativeColumn(2).AlignMiddle().AlignLeft().Column(col =>
                        {
                            if (logoImage != null)
                                col.Item().Image(logoImage);
                        });

                        row.RelativeColumn(8).Column(col =>
                        {
                            col.Item().AlignCenter().Text("SERVICIO DE TRANSPORTE “LA CONCORDIA”")
                                .SemiBold().FontSize(14).FontColor(Colors.Black);

                            col.Item().AlignCenter().Text("Servicio de Transporte Exclusivo Puerta a Puerta")
                                .SemiBold().FontSize(12).FontColor(Colors.Black);

                            col.Item().AlignCenter().Text("Tlf: 2606425 Claro: 0994227299 Movistar: 0987117307")
                                .SemiBold().FontSize(10).FontColor(Colors.Black);
                        });

                        row.RelativeColumn(2).Column(col =>
                        {
                            col.Item().AlignRight().Text($"Fecha Emisión: {DateTime.Now:dd/MM/yyyy}")
                                .FontSize(7).FontColor(Colors.Grey.Darken1).Bold();

                            col.Item().AlignRight().Text($"Hora Emisión: {DateTime.Now:HH:mm:ss}")
                                .FontSize(7).FontColor(Colors.Grey.Darken1).Bold();
                        });
                    });

                    // ---- CUERPO ----
                    page.Content().Column(col =>
                    {
                        // Datos Ficha
                        col.Item()
                        .Border(2) // borde de 1px
                        .BorderColor(Colors.Grey.Darken1) // color del borde
                        .Background(Colors.Grey.Lighten3) // color de fondo para resaltar
                        .Padding(5) // espacio interno
                        .AlignCenter() // centrar el texto horizontalmente
                        .Text("Datos Personales")
                            .Bold()
                            .FontSize(14)
                            .FontColor(Colors.Black);
                        col.Item().Text($"Cédula: {ficha.Cedula}");
                        col.Item().Text($"Nombre: {ficha.Nombre} {ficha.Apellidos}");
                        col.Item().Text($"Tipo Licencia: {ficha.TipoLicenciaDescripcion}");
                        col.Item().Text($"Fecha Nacimiento: {ficha.Fechanacimiento}");
                        col.Item().Text($"Fecha Ingreso: {ficha.Fechaingreso}");
                        col.Item().Text($"Nacionalidad: {ficha.NacionalidadDescripcion}");
                        col.Item().Text($"Estado Civil: {ficha.EstadoCivilDescripcion}");
                        col.Item().Text($"Nivel Educación: {ficha.Fkniveleducacion}");
                        col.Item().Text($"Teléfono: {ficha.Telefono}");
                        col.Item().Text($"Celular: {ficha.Celular}");
                        col.Item().Text($"Correo: {ficha.Correo}");
                        col.Item().Text($"Cargo: {ficha.CargoDescripcion}");
                        col.Item().Text($"Dirección Actual: {ficha.Domicilio}");
                        col.Item().Text($"Referencia: {ficha.Referencia}");
                        col.Item().Text($"Cuota Semanal: {ficha.Cuotaf}");

                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

                        // Datos Unidad
                        col.Item()
                        .Border(2) // borde de 1px
                        .BorderColor(Colors.Grey.Darken1) // color del borde
                        .Background(Colors.Grey.Lighten3) // color de fondo para resaltar
                        .Padding(5) // espacio interno
                        .AlignCenter() // centrar el texto horizontalmente
                        .Text("Datos Unidad")
                            .Bold()
                            .FontSize(14)
                            .FontColor(Colors.Black);
                        col.Item().Text($"Unidad: {unidad.Unidad1}");
                        col.Item().Text($"Placa: {unidad.Placa}");
                        col.Item().Text($"Marca: {unidad.Marca}");
                        col.Item().Text($"Modelo: {unidad.Modelo}");
                        col.Item().Text($"Año: {unidad.Anio}");
                        col.Item().Text($"Color: {unidad.Color}");
                        //col.Item().Text($"Estado: {unidad.Estado}");

                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

                        // Datos Dueño Puesto
                        col.Item()
                        .Border(2) // borde de 1px
                        .BorderColor(Colors.Grey.Darken1) // color del borde
                        .Background(Colors.Grey.Lighten3) // color de fondo para resaltar
                        .Padding(5) // espacio interno
                        .AlignCenter() // centrar el texto horizontalmente
                        .Text("Datos Del Dueño Del Puesto")
                            .Bold()
                            .FontSize(14)
                            .FontColor(Colors.Black);
                        col.Item().Text($"Cédula: {duenopuesto.Cedula}");
                        col.Item().Text($"Nombres: {duenopuesto.Nombres}");
                        col.Item().Text($"Apellidos: {duenopuesto.Apellidos}");

                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

                        // Beneficiarios
                        col.Item()
                        .Border(2) // borde de 1px
                        .BorderColor(Colors.Grey.Darken1) // color del borde
                        .Background(Colors.Grey.Lighten3) // color de fondo para resaltar
                        .Padding(5) // espacio interno
                        .AlignCenter() // centrar el texto horizontalmente
                        .Text("Beneficiario")
                            .Bold()
                            .FontSize(14)
                            .FontColor(Colors.Black);
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(3);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Nombres").Bold();
                                header.Cell().Text("Apellidos").Bold();
                                header.Cell().Text("Parentesco").Bold();
                                header.Cell().Text("Tipo").Bold();
                            });

                            foreach (var b in beneficiarios)
                            {
                                table.Cell().Text(b.Nombres);
                                table.Cell().Text(b.Apellidos);
                                //table.Cell().Text(b.Pkparentesco);
                                table.Cell().Text(b.ParentescoDescripcion);
                                table.Cell().Text(b.Tipo == "1" ? "BENEFICIARIO" : b.Tipo == "2" ? "AYUDA" : "DESCONOCIDO");
                            }
                        });
                    });

                    // ---- PIE DE PÁGINA ----
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                        x.Span(" de ");
                        x.TotalPages();
                    });
                });
            });

            return doc.GeneratePdf();
        }
    }
}
