using Modelo.laconcordia.Modelo.Database;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace Identity.Api.Reporteria
{
    public class DuenoDePuestoPdfGenerator
    {
        public static byte[] GenerarPdf(List<Duenopuesto> empresas)
        {
            var doc = Document.Create(container =>
            {
                // ruta del logo 
                var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "favicon.jpeg");
                byte[] logoImage = null;
                if (File.Exists(logoPath))
                {
                    logoImage = File.ReadAllBytes(logoPath);
                }
                //fin

                container.Page(page =>
                {
                    page.Margin(20);
                    page.Size(PageSizes.A4);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Row(row =>
                    {
                        if (logoImage != null)
                            row.RelativeColumn(1).Height(40).Image(logoImage);
                        row.RelativeColumn(9).Column(col =>
                        {
                            col.Item().AlignCenter().Text("Dueños de puesto")
                                .SemiBold().FontSize(14).FontColor(Colors.Blue.Medium);

                            col.Item().AlignRight().Text($"Fecha Emisión: {DateTime.Now:dd/MM/yyyy}")
                                .FontSize(7).FontColor(Colors.Grey.Darken1).Bold();

                            col.Item().AlignRight().Text($"Hora Emisión: {DateTime.Now: HH:mm:ss}")
                                .FontSize(7).FontColor(Colors.Grey.Darken1).Bold();

                            //if (!string.IsNullOrWhiteSpace(correo))
                            //{
                            //    col.Item().AlignRight().Text($"Generado por: {correo}")
                            //        .FontSize(7).FontColor(Colors.Grey.Darken1).Italic();
                            //}
                        });
                    });

                    //    page.Header().Column(col =>
                    //{
                    //    col.Item().AlignCenter().Text("Listado de Empresas").FontSize(14).Bold();
                    //    col.Item().AlignRight().Text($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm:ss}").FontSize(9);
                    //    col.Item().AlignRight().Text($"Usuario: {correo}").FontSize(9);
                    //});

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2); // Cedula
                            columns.RelativeColumn(3); // Nombre
                            columns.RelativeColumn(3); // Apellidos
                            columns.RelativeColumn(2); // Estado
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Cédula").Bold();
                            header.Cell().Text("Nombre").Bold();
                            header.Cell().Text("Apellidos").Bold();
                            header.Cell().Text("Estado").Bold();
                        });

                        foreach (var emp in empresas)
                        {
                            table.Cell().Text(emp.Cedula);
                            table.Cell().Text(emp.Nombres);
                            table.Cell().Text(emp.Apellidos);
                            table.Cell().Text(emp.Estado);
                        }
                    });

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
