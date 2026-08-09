using Identity.Api.DTO;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Identity.Api.Reporteria
{
    public static class BalancePdfGenerator
    {
        // Orden y nombre de presentación de cada forma de pago, igual al reporte de escritorio (RptBalance.rpt).
        private static readonly (string Clave, string Etiqueta)[] OrdenActivo = new[]
        {
            ("EFECTIVO", "EFECTIVO"),
            ("BANCO GUAYAQUIL", "BANCO GUAYAQUIL"),
            ("BANCO PICHINCHA", "BANCO PICHINCHA"),
            ("BANCO DEL PACIFICO", "BANCO DEL PACIFICO"),
            ("COMISION", "COMISIÓN"),
            ("CUENTAXCOBRAR", "CUENTA X COBRAR"),
            ("CUENTA X COBRAR", "CUENTA X COBRAR"),
            ("EXONERACION", "EXONERACION"),
        };

        public static byte[] GenerarPdf(BalanceResultadoDTO resultado, string usuario)
        {
            var itemsPorClave = resultado.Items.ToDictionary(x => x.FormaDePago, x => x);

            // Arma la lista de Activo respetando el orden del reporte de escritorio; agrega
            // cualquier forma de pago adicional que no esté contemplada en ese orden fijo.
            var activo = new List<BalanceItemDTO>();
            var claveYaAgregada = new HashSet<string>();
            foreach (var (clave, etiqueta) in OrdenActivo)
            {
                if (itemsPorClave.TryGetValue(clave, out var item) && claveYaAgregada.Add(clave))
                {
                    activo.Add(new BalanceItemDTO { FormaDePago = etiqueta, Valor = item.Valor, Registros = item.Registros });
                }
            }
            foreach (var item in resultado.Items)
            {
                if (!claveYaAgregada.Contains(item.FormaDePago))
                {
                    activo.Add(item);
                    claveYaAgregada.Add(item.FormaDePago);
                }
            }

            var totalActivo = resultado.Total;
            var exoneracion = itemsPorClave.TryGetValue("EXONERACION", out var exo) ? exo.Valor : 0m;
            var capital = totalActivo - exoneracion;
            var totalPasivo = exoneracion + capital;

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

                    page.Header().Row(row =>
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

                    page.Content().PaddingTop(15).Column(col =>
                    {
                        col.Item().AlignCenter().Text("BALANCE INICIAL").Bold().FontSize(16);

                        col.Item().PaddingTop(10).Row(row =>
                        {
                            row.AutoItem().Text(t =>
                            {
                                t.Span("DESDE   ").Italic().Bold();
                                t.Span(resultado.FechaDesde.ToString("d/M/yyyy")).Italic();
                            });
                            row.RelativeItem();
                            row.AutoItem().Text(t =>
                            {
                                t.Span("HASTA   ").Italic().Bold();
                                t.Span(resultado.FechaHasta.ToString("d/M/yyyy")).Italic();
                            });
                        });

                        col.Item().PaddingTop(5).LineHorizontal(1).LineColor(Colors.Black);

                        col.Item().PaddingTop(15).Row(row =>
                        {
                            row.RelativeItem().Border(1).BorderColor(Colors.Black).Padding(8).Column(activoCol =>
                            {
                                ColumnaBalance(activoCol, "ACTIVO", "ACTIVO CIRCULANTE", activo,
                                    "TOTAL ACTIVO", totalActivo);
                            });

                            row.ConstantItem(10);

                            row.RelativeItem().Border(1).BorderColor(Colors.Black).Padding(8).Column(pasivoCol =>
                            {
                                var pasivo = new List<BalanceItemDTO>
                                {
                                    new BalanceItemDTO { FormaDePago = "EXONERACION", Valor = exoneracion },
                                    new BalanceItemDTO { FormaDePago = "CAPITAL", Valor = capital },
                                };
                                ColumnaBalance(pasivoCol, "PASIVO", "PASIVO CIRCULANTE", pasivo,
                                    "TOTAL PASIVO", totalPasivo);
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

        private static void ColumnaBalance(ColumnDescriptor col, string titulo, string subtitulo,
            List<BalanceItemDTO> items, string tituloTotal, decimal total)
        {
            col.Item().AlignCenter().Text(titulo).Bold().FontSize(12);
            col.Item().PaddingTop(4).PaddingBottom(4).LineHorizontal(1).LineColor(Colors.Black);
            col.Item().Text(subtitulo).Italic().Bold();

            col.Item().PaddingTop(6).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(2);
                });

                foreach (var item in items)
                {
                    table.Cell().PaddingVertical(2).Text(item.FormaDePago);
                    table.Cell().PaddingVertical(2).AlignRight().Text(item.Valor.ToString("N2"));
                }
            });

            col.Item().PaddingTop(6).LineHorizontal(1).LineColor(Colors.Black);
            col.Item().PaddingTop(4).Row(row =>
            {
                row.RelativeItem(3).Text(tituloTotal).Italic().Bold();
                row.RelativeItem(2).AlignRight().Text(total.ToString("N2")).Italic().Bold();
            });
        }
    }
}
