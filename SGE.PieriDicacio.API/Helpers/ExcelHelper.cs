using ClosedXML.Excel;
using SGE.PieriDicacio.API.DTOs.Reporte;

namespace SGE.PieriDicacio.API.Helpers
{
    public static class ExcelHelper
    {
        public static ExportResultDto CrearLibro(string titulo, List<object[]> filas, string hoja)
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add(hoja);

            ws.Cell(1, 1).Value = titulo;
            ws.Cell(1, 1).Style.Font.SetBold();
            ws.Cell(1, 1).Style.Font.FontSize = 14;
            ws.Range(1, 1, 1, filas[0].Length).Merge();
            ws.Range(1, 1, 1, filas[0].Length).Style.Fill.BackgroundColor = XLColor.FromHtml("#7B1114");
            ws.Range(1, 1, 1, filas[0].Length).Style.Font.FontColor = XLColor.White;
            ws.Range(1, 1, 1, filas[0].Length).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            var filaEncabezado = 3;
            for (int c = 0; c < filas[0].Length; c++)
            {
                ws.Cell(filaEncabezado, c + 1).Value = filas[0][c]?.ToString() ?? "";
                ws.Cell(filaEncabezado, c + 1).Style.Font.SetBold();
                ws.Cell(filaEncabezado, c + 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#D96B10");
                ws.Cell(filaEncabezado, c + 1).Style.Font.FontColor = XLColor.White;
                ws.Cell(filaEncabezado, c + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            for (int i = 1; i < filas.Count; i++)
            {
                for (int c = 0; c < filas[i].Length; c++)
                {
                    ws.Cell(filaEncabezado + i, c + 1).Value = XLCellValue.FromObject(filas[i][c]);
                }
            }

            ws.Range(filaEncabezado + 1, 1, filaEncabezado + filas.Count - 1, filas[0].Length).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Range(filaEncabezado + 1, 1, filaEncabezado + filas.Count - 1, filas[0].Length).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            ws.Range(filaEncabezado + 1, 1, filaEncabezado + filas.Count - 1, filas[0].Length).Style.Border.InsideBorderColor = XLColor.LightGray;

            ws.SheetView.FreezeRows(filaEncabezado);
            ws.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            var contenido = ms.ToArray();

            return new ExportResultDto
            {
                NombreArchivo = $"{hoja}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                Contenido = contenido
            };
        }
    }
}
