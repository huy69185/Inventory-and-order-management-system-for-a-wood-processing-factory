using ClosedXML.Excel;
using QuanLyKho.Domain.Entities;

namespace QuanLyKho.Web.Services;

public static class ExcelExportService
{
    public static byte[] ExportTonKho(List<Product> products)
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Tồn kho");
        ws.Cell(1, 1).Value = "Mã SP";
        ws.Cell(1, 2).Value = "Tên sản phẩm";
        ws.Cell(1, 3).Value = "Đơn vị";
        ws.Cell(1, 4).Value = "Số lượng tồn";
        ws.Cell(1, 5).Value = "Giá nhập";
        ws.Cell(1, 6).Value = "Giá trị (ước)";
        ws.Range(1, 1, 1, 6).Style.Font.Bold = true;
        int row = 2;
        foreach (var p in products)
        {
            ws.Cell(row, 1).Value = p.MaSanPham;
            ws.Cell(row, 2).Value = p.TenSanPham;
            ws.Cell(row, 3).Value = p.DonVi;
            ws.Cell(row, 4).Value = p.SoLuongTon;
            ws.Cell(row, 5).Value = (double)p.GiaNhap;
            ws.Cell(row, 6).Value = (double)(p.SoLuongTon * p.GiaNhap);
            row++;
        }
        ws.Columns().AdjustToContents();
        using var stream = new MemoryStream();
        wb.SaveAs(stream, false);
        return stream.ToArray();
    }

    public static byte[] ExportDoanhThu(List<Invoice> invoices)
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Doanh thu");
        ws.Cell(1, 1).Value = "Số HĐ";
        ws.Cell(1, 2).Value = "Ngày";
        ws.Cell(1, 3).Value = "Khách hàng";
        ws.Cell(1, 4).Value = "Tổng thanh toán";
        ws.Cell(1, 5).Value = "Đã thu";
        ws.Cell(1, 6).Value = "Còn nợ";
        ws.Range(1, 1, 1, 6).Style.Font.Bold = true;
        int row = 2;
        foreach (var i in invoices)
        {
            ws.Cell(row, 1).Value = i.SoHoaDon;
            ws.Cell(row, 2).Value = i.NgayLap.ToString("dd/MM/yyyy");
            ws.Cell(row, 3).Value = i.KhachHang;
            ws.Cell(row, 4).Value = (double)i.TongThanhToan;
            ws.Cell(row, 5).Value = (double)i.TienDaThu;
            ws.Cell(row, 6).Value = (double)i.ConNo;
            row++;
        }
        ws.Columns().AdjustToContents();
        using var stream = new MemoryStream();
        wb.SaveAs(stream, false);
        return stream.ToArray();
    }

    public static byte[] ExportCongNo(List<Invoice> invoicesCoNo)
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Công nợ");
        ws.Cell(1, 1).Value = "Số HĐ";
        ws.Cell(1, 2).Value = "Ngày";
        ws.Cell(1, 3).Value = "Khách hàng";
        ws.Cell(1, 4).Value = "Điện thoại";
        ws.Cell(1, 5).Value = "Tổng HĐ";
        ws.Cell(1, 6).Value = "Đã thu";
        ws.Cell(1, 7).Value = "Còn nợ";
        ws.Range(1, 1, 1, 7).Style.Font.Bold = true;
        int row = 2;
        foreach (var i in invoicesCoNo)
        {
            ws.Cell(row, 1).Value = i.SoHoaDon;
            ws.Cell(row, 2).Value = i.NgayLap.ToString("dd/MM/yyyy");
            ws.Cell(row, 3).Value = i.KhachHang;
            ws.Cell(row, 4).Value = i.DienThoaiKH;
            ws.Cell(row, 5).Value = (double)i.TongThanhToan;
            ws.Cell(row, 6).Value = (double)i.TienDaThu;
            ws.Cell(row, 7).Value = (double)i.ConNo;
            row++;
        }
        ws.Columns().AdjustToContents();
        using var stream = new MemoryStream();
        wb.SaveAs(stream, false);
        return stream.ToArray();
    }

    public static byte[] ExportDoanhThu(List<SalesOrder> orders)
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Doanh thu");
        ws.Cell(1, 1).Value = "Số đơn";
        ws.Cell(1, 2).Value = "Ngày";
        ws.Cell(1, 3).Value = "Khách hàng";
        ws.Cell(1, 4).Value = "Tổng đơn";
        ws.Cell(1, 5).Value = "Đã trả";
        ws.Cell(1, 6).Value = "Còn lại";
        ws.Range(1, 1, 1, 6).Style.Font.Bold = true;
        int row = 2;
        foreach (var o in orders)
        {
            ws.Cell(row, 1).Value = o.SoDonHang;
            ws.Cell(row, 2).Value = o.NgayDat.ToString("dd/MM/yyyy");
            ws.Cell(row, 3).Value = o.TenKhachHang;
            ws.Cell(row, 4).Value = (double)o.TongTien;
            ws.Cell(row, 5).Value = (double)o.TienDaTraTruoc;
            ws.Cell(row, 6).Value = (double)o.ConLai;
            row++;
        }
        ws.Columns().AdjustToContents();
        using var stream = new MemoryStream();
        wb.SaveAs(stream, false);
        return stream.ToArray();
    }

    public static byte[] ExportCongNo(List<SalesOrder> ordersCoNo)
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Công nợ");
        ws.Cell(1, 1).Value = "Số đơn";
        ws.Cell(1, 2).Value = "Ngày";
        ws.Cell(1, 3).Value = "Khách hàng";
        ws.Cell(1, 4).Value = "Điện thoại";
        ws.Cell(1, 5).Value = "Tổng đơn";
        ws.Cell(1, 6).Value = "Đã trả";
        ws.Cell(1, 7).Value = "Còn lại";
        ws.Range(1, 1, 1, 7).Style.Font.Bold = true;
        int row = 2;
        foreach (var o in ordersCoNo)
        {
            ws.Cell(row, 1).Value = o.SoDonHang;
            ws.Cell(row, 2).Value = o.NgayDat.ToString("dd/MM/yyyy");
            ws.Cell(row, 3).Value = o.TenKhachHang;
            ws.Cell(row, 4).Value = o.DienThoai;
            ws.Cell(row, 5).Value = (double)o.TongTien;
            ws.Cell(row, 6).Value = (double)o.TienDaTraTruoc;
            ws.Cell(row, 7).Value = (double)o.ConLai;
            row++;
        }
        ws.Columns().AdjustToContents();
        using var stream = new MemoryStream();
        wb.SaveAs(stream, false);
        return stream.ToArray();
    }
}
