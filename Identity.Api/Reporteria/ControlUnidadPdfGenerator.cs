using Identity.Api.DTO;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace Identity.Api.Reporteria
{
    public static class ControlUnidadPdfGenerator
    {
        public static byte[] GenerarPdf(List<ControlUnidadMovimientoDTO> movimientos, string usuario)
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
                    page.Size(PageSizes.A4.Landscape());
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Row(row =>
                    {
                        row.ConstantColumn(90).AlignMiddle().Column(col =>
                        {
                            if (logoImage != null)
                                col.Item().Image(logoImage);
                        });

                        row.RelativeColumn().AlignMiddle().PaddingLeft(10).Text("REPORTE PARA EL CONTROL DE INGRESO Y SALIDA DE VEHICULOS")
                            .Bold().FontSize(18).FontColor(Colors.Black);

                        row.ConstantColumn(180).Border(1).BorderColor(Colors.Black).Padding(8).Column(col =>
                        {
                            col.Item().Text(t =>
                            {
                                t.Span("Fecha Emisión: ").Bold();
                                t.Span(DateTime.Now.ToString("d/M/yyyy"));
                            });
                            col.Item().Text(t =>
                            {
                                t.Span("Hora Emisión: ").Bold();
                                t.Span(DateTime.Now.ToString("HH:mm:ss"));
                            });
                            col.Item().Text(t =>
                            {
                                t.Span("Usuario : ").Bold();
                                t.Span(usuario);
                            });
                        });
                    });

                    page.Content().PaddingTop(15).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2); // fecharegistro
                            columns.RelativeColumn(1); // turno
                            columns.RelativeColumn(1); // unidad
                            columns.RelativeColumn(2); // ciconducto
                            columns.RelativeColumn(3); // conductor
                            columns.RelativeColumn(1); // estado
                        });

                        table.Header(h =>
                        {
                            h.Cell().Text("fecharegistro").Bold();
                            h.Cell().Text("turno").Bold();
                            h.Cell().Text("unidad").Bold();
                            h.Cell().Text("ciconducto").Bold();
                            h.Cell().Text("conductor").Bold();
                            h.Cell().Text("estado").Bold();
                        });

                        foreach (var m in movimientos)
                        {
                            table.Cell().Text(m.Fecharegistro.ToString("yyyy-MM-dd HH:mm:ss"));
                            table.Cell().Text(m.Turno);
                            table.Cell().Text(m.Unidad);
                            table.Cell().Text(m.Ciconductor);
                            table.Cell().Text(m.Conductor);
                            table.Cell().Text(m.Estado);
                        }

                        if (movimientos.Count == 0)
                        {
                            table.Cell().ColumnSpan(6).PaddingTop(10).Text("No hay movimientos registrados para la fecha y el turno seleccionados.")
                                .Italic().FontColor(Colors.Grey.Darken1);
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
