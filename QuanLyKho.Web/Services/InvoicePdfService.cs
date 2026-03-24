using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuanLyKho.Domain.Entities;

namespace QuanLyKho.Web.Services;

public static class InvoicePdfService
{
    public static byte[] GeneratePdfBytes(Invoice invoice, CompanySetting company, byte[]? logoBytes = null)
    {
        var document = BuildDocument(invoice, company, logoBytes);
        return document.GeneratePdf();
    }

    private static IDocument BuildDocument(Invoice invoice, CompanySetting company, byte[]? logoBytes)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.Header().Column(col =>
                {
                    if (logoBytes != null && logoBytes.Length > 0)
                    {
                        try
                        {
                            col.Item().Height(40).Width(120).Image(logoBytes);
                        }
                        catch { /* bỏ qua nếu ảnh lỗi */ }
                    }
                    col.Item().PaddingTop(4).Text("HÓA ĐƠN BÁN HÀNG").SemiBold().FontSize(20).AlignCenter();
                });
                page.Content().PaddingVertical(1, Unit.Centimetre).Column(column =>
                {
                    column.Item().Text($"Số HD: {invoice.SoHoaDon}").FontSize(14);
                    column.Item().Text($"Ngày: {invoice.NgayLap:dd/MM/yyyy HH:mm}").FontSize(12);
                    column.Item().Text($"Công ty: {company.TenCongTy}").FontSize(12);
                    column.Item().Text($"Mã số thuế: {company.MaSoThue}").FontSize(12);
                    column.Item().Text($"Khách hàng: {invoice.KhachHang} - {invoice.DienThoaiKH}").FontSize(12);
                    column.Item().PaddingTop(1, Unit.Centimetre).Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn(3);
                            c.ConstantColumn(60);
                            c.ConstantColumn(80);
                            c.ConstantColumn(80);
                        });
                        table.Header(h =>
                        {
                            h.Cell().Text("Tên SP").Bold();
                            h.Cell().Text("SL").AlignRight().Bold();
                            h.Cell().Text("Đơn giá").AlignRight().Bold();
                            h.Cell().Text("Thành tiền").AlignRight().Bold();
                        });
                        foreach (var item in invoice.ChiTiet)
                        {
                            table.Cell().Text(item.TenSanPham);
                            table.Cell().Text(item.SoLuong.ToString()).AlignRight();
                            table.Cell().Text(item.DonGia.ToString("N0")).AlignRight();
                            table.Cell().Text(item.ThanhTien.ToString("N0")).AlignRight();
                        }
                    });
                    column.Item().PaddingTop(1, Unit.Centimetre).AlignRight().Text($"Tổng trước CK: {invoice.TongTienTruocCK:N0} ₫").FontSize(12);
                    column.Item().AlignRight().Text($"Chiết khấu: {invoice.ChietKhau}%").FontSize(12);
                    column.Item().AlignRight().Text($"Tiền sau CK: {invoice.TongTienSauCK:N0} ₫").FontSize(12);
                    if (invoice.ThueGTGT > 0)
                        column.Item().AlignRight().Text($"Thuế GTGT ({invoice.ThueGTGT}%): {invoice.TienThueGTGT:N0} ₫").FontSize(12);
                    column.Item().AlignRight().Text($"Tổng thanh toán: {invoice.TongThanhToan:N0} ₫").FontSize(16).Bold();
                    column.Item().AlignRight().Text($"Đã thanh toán: {invoice.TienDaThu:N0} ₫").FontSize(12);
                    column.Item().AlignRight().Text($"Còn nợ: {invoice.ConNo:N0} ₫").FontSize(12).FontColor(Colors.Orange.Darken2);
                });
                page.Footer().AlignCenter().Text($"Cảm ơn quý khách! - {company.TenCongTy}").FontSize(10);
            });
        });
    }
}
