using Identity.Api.DTO;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace Identity.Api.Reporteria
{
    public static class ControlUnidadPdfGenerator
    {
        public static byte[] GenerarPdf(List<UnidadServicioDTO> fueraDeServicio, List<UnidadServicioDTO> enServicio, string? turno, string? monitora)
        {
            var doc = Document.Create(container =>
            {
                var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "favicon.jpeg");
                byte[]? logoImage = null;
                if (File.Exists(logoPath))
                {
                    logoImage = File.ReadAllBytes(logoPath);
                }

                container.Page(page =>
                {
                    page.Margin(20);
                    page.Size(PageSizes.A4);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Column(header =>
                    {
                        header.Item().Height(80).Row(row =>
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

                        header.Item().PaddingTop(5).Row(row =>
                        {
                            row.RelativeColumn().Text("CONTROL DE UNIDADES").Bold().FontSize(13);
                        });

                        header.Item().PaddingTop(2).Row(row =>
                        {
                            row.RelativeColumn().Text($"Monitora: {monitora}").FontSize(9);
                            row.RelativeColumn().AlignRight().Text($"Turno: {(string.IsNullOrEmpty(turno) ? "-- Todos --" : turno)}").FontSize(9);
                        });
                    });

                    page.Content().PaddingTop(10).Column(content =>
                    {
                        content.Item().Text($"Fuera de Servicio ({fueraDeServicio.Count})").Bold().FontSize(11);
                        content.Item().PaddingBottom(10).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2); // Unidad
                                columns.RelativeColumn(4); // Nombres
                                columns.RelativeColumn(4); // Apellidos
                            });

                            table.Header(h =>
                            {
                                h.Cell().Text("Unidad").Bold();
                                h.Cell().Text("Nombres").Bold();
                                h.Cell().Text("Apellidos").Bold();
                            });

                            foreach (var f in fueraDeServicio)
                            {
                                table.Cell().Text(f.Fkunidad);
                                table.Cell().Text(f.Nombre);
                                table.Cell().Text(f.Apellidos);
                            }
                        });

                        content.Item().Text($"En Servicio ({enServicio.Count})").Bold().FontSize(11);
                        content.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2); // Unidad
                                columns.RelativeColumn(4); // Nombres
                                columns.RelativeColumn(4); // Apellidos
                            });

                            table.Header(h =>
                            {
                                h.Cell().Text("Unidad").Bold();
                                h.Cell().Text("Nombres").Bold();
                                h.Cell().Text("Apellidos").Bold();
                            });

                            foreach (var f in enServicio)
                            {
                                table.Cell().Text(f.Fkunidad);
                                table.Cell().Text(f.Nombre);
                                table.Cell().Text(f.Apellidos);
                            }
                        });
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
