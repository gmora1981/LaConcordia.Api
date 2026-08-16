using Identity.Api.DTO;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace Identity.Api.Reporteria
{
    // Replica "REPORTE DE SOLICITUD DE CARRERA" (FrmReporteOperadora, modo Detalle de Pedido ->
    // RptOperadoraDetallePedidoUni) del sistema de escritorio: pedidos de un usuario/operadora
    // dentro de un rango de fechas, con las calles de origen/destino ya resueltas.
    public static class ReporteSolicitudCarreraPdfGenerator
    {
        public static byte[] GenerarPdf(List<PedidoOperadoraDTO> pedidos, string? usuario, DateTime desde, DateTime hasta, string usuarioLogueado)
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
                                    t.Span(usuarioLogueado);
                                });
                            });
                        });

                        headerCol.Item().PaddingTop(8).AlignCenter().Text("REPORTE DE SOLICITUD DE CARRERA").Bold().FontSize(14);

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

                        headerCol.Item().PaddingTop(6).Text("USUARIO").Italic().Bold().Underline();
                        headerCol.Item().Text(usuario ?? "");

                        headerCol.Item().PaddingTop(4).LineHorizontal(1).LineColor(Colors.Black);
                    });

                    page.Content().PaddingTop(10).Column(col =>
                    {
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2); // fecha y hora
                                columns.RelativeColumn(3); // calle de origen
                                columns.RelativeColumn(3); // calle destino
                                columns.RelativeColumn(1); // unidad
                                columns.RelativeColumn(1); // precio
                            });

                            table.Header(h =>
                            {
                                h.Cell().Text("FECHA Y HORA").Bold().Italic().Underline();
                                h.Cell().AlignCenter().Text("CALLE DE ORIGEN").Bold().Italic().Underline();
                                h.Cell().AlignCenter().Text("CALLE DESTINO").Bold().Italic().Underline();
                                h.Cell().Text("UNIDAD").Bold().Italic().Underline();
                                h.Cell().AlignRight().Text("PRECIO").Bold().Italic().Underline();
                            });

                            foreach (var p in pedidos)
                            {
                                table.Cell().Text(p.Fecharegistro.ToString("d/M/yyyy H:mm:ss"));
                                table.Cell().Text(p.CalleOrigen);
                                table.Cell().Text(p.CalleDestino);
                                table.Cell().Text(p.Unidad);
                                table.Cell().AlignRight().Text(p.Precio?.ToString("0.00"));
                            }

                            if (pedidos.Count == 0)
                            {
                                table.Cell().ColumnSpan(5).PaddingTop(10).Text("No hay pedidos registrados para el rango y la operadora seleccionados.")
                                    .Italic().FontColor(Colors.Grey.Darken1);
                            }
                        });

                        col.Item().PaddingTop(10).Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("Total de Registros").Italic().Underline();
                                c.Item().Text(pedidos.Count.ToString());
                            });
                            row.RelativeItem().AlignRight().Column(c =>
                            {
                                c.Item().AlignRight().Text("Total").Italic().Underline();
                                c.Item().AlignRight().Text(pedidos.Sum(x => x.Precio ?? 0).ToString("0.00"));
                            });
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
