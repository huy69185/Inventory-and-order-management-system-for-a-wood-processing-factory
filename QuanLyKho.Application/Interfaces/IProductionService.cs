using System.Threading.Tasks;

namespace QuanLyKho.Application.Interfaces;

public interface IProductionService
{
    /// <summary>
    /// Ghi nhận tiêu hao nguyên liệu cho 1 dòng đơn đặt hàng:
    /// giảm tồn kho nguyên liệu, tăng số lượng đã sản xuất của dòng đơn và ghi lịch sử kho.
    /// </summary>
    Task ProduceForOrderAsync(int salesOrderItemId, int nguyenLieuId, int soLuongNguyenLieu, int soLuongHoanThanh, decimal chiPhiKhac, string? ghiChu = null);
}

