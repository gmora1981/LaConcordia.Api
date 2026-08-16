using Identity.Api.DTO;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace Identity.Api.Reporteria
{
    // Replica "REPORTE DE INGRESO Y SALIDA" del escritorio: el mismo reporte se genera tanto
    // desde FrmReporteOperadora (RptOperadorasIngresoSalidaUni, filtrado por operadora/monitora)
    // como desde FrmReporteUnidad (RptUnidadesIngresoSalida, filtrado por unidad). Aqui se
    // unifican ambos filtros (independientes entre si) en un solo reporte.
    public static class ReporteIngresoSalidaPdfGenerator
    {
        public static byte[] GenerarPdf(List<ControlUnidadMovimientoDTO> movimientos, DateTime desde, DateTime hasta, string? monitora, string? unidad, string usuario)
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

                    page.Header().Column(headerCol =>
                    {
                        headerCol.Item().Row(row =>
                        {
                            row.ConstantColumn(70).AlignMiddle().Column(col =>
                            {
                                if (logoImage != null)
                                    col.Item().Image(logoImage);
                            });

                            row.RelativeColumn().AlignMiddle().PaddingLeft(10).Column(col =>
                            {
                                col.Item().AlignCenter().Text("SERVICIO DE TRANSPORTE “LA CONCORDIA”")
                                    .SemiBold().FontSize(14).FontColor(Colors.Black);

                                col.Item().AlignCenter().Text("Servicio de Transporte Exclusivo Puerta a Puerta")
                                    .SemiBold().FontSize(12).FontColor(Colors.Black);

                                col.Item().AlignCenter().Text("Tlf: 2606425 Claro: 0994227299 Movistar: 0987117307")
                                    .SemiBold().FontSize(10).FontColor(Colors.Black);
                            });

                            row.ConstantColumn(160).Border(1).BorderColor(Colors.Black).Padding(8).Column(col =>
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

                        headerCol.Item().PaddingTop(8).AlignCenter().Text("REPORTE DE INGRESO Y SALIDA").Bold().FontSize(14);

                        headerCol.Item().PaddingTop(8).Row(row =>
                        {
                            row.AutoItem().Text(t =>
                            {
                                t.Span("DESDE   ").Italic().Bold();
                                t.Span(desde.ToString("d/M/yyyy")).Italic();
                            });
                            row.RelativeItem();
                            row.AutoItem().Text(t =>
                            {
                                t.Span("HASTA   ").Italic().Bold();
                                t.Span(hasta.ToString("d/M/yyyy")).Italic();
                            });
                        });

                        headerCol.Item().PaddingTop(6).Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("MONITORA").Italic().Bold().Underline();
                                c.Item().Text(string.IsNullOrEmpty(monitora) ? "Todas" : monitora);
                            });
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("UNIDAD").Italic().Bold().Underline();
                                c.Item().Text(string.IsNullOrEmpty(unidad) ? "Todas" : unidad);
                            });
                        });

                        headerCol.Item().PaddingTop(4).LineHorizontal(1).LineColor(Colors.Black);
                    });

                    page.Content().PaddingTop(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2); // fecha y hora
                            columns.RelativeColumn(1); // turno
                            columns.RelativeColumn(1); // estado
                            columns.RelativeColumn(1); // unidad
                            columns.RelativeColumn(2); // monitora
                            columns.RelativeColumn(3); // conductor
                        });

                        table.Header(h =>
                        {
                            h.Cell().Text("FECHA Y HORA").Bold();
                            h.Cell().Text("TURNO").Bold();
                            h.Cell().Text("ESTADO").Bold();
                            h.Cell().Text("unidad");
                            h.Cell().Text("monitora");
                            h.Cell().Text("conductor");
                        });

                        foreach (var m in movimientos)
                        {
                            table.Cell().Text(m.Fecharegistro.ToString("dd/MM/yyyy HH:mm"));
                            table.Cell().Text(m.Turno);
                            table.Cell().Text(m.Estado);
                            table.Cell().Text(m.Unidad);
                            table.Cell().Text(m.Monitora);
                            table.Cell().Text(m.Conductor);
                        }

                        if (movimientos.Count == 0)
                        {
                            table.Cell().ColumnSpan(6).PaddingTop(10).Text("No hay movimientos registrados para el rango, la operadora y la unidad seleccionados.")
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
