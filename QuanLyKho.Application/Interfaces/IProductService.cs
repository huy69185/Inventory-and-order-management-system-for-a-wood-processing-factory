using QuanLyKho.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuanLyKho.Application.Interfaces
{
    public interface IProductService
    {
        /// <summary>
        /// Lấy tất cả sản phẩm, hỗ trợ tìm kiếm theo tên hoặc mã
        /// </summary>
        Task<List<Product>> GetAllAsync(string? search = null);

        /// <summary>
        /// Lấy sản phẩm theo ID
        /// </summary>
        Task<Product?> GetByIdAsync(int id);

        /// <summary>
        /// Thêm sản phẩm mới
        /// </summary>
        Task AddAsync(Product product);

        /// <summary>
        /// Cập nhật sản phẩm
        /// </summary>
        Task UpdateAsync(Product product);

        /// <summary>
        /// Xóa sản phẩm theo ID
        /// </summary>
        Task DeleteAsync(int id);

        /// <summary>
        /// Lấy danh sách sản phẩm sắp hết hàng (tồn kho dưới ngưỡng)
        /// </summary>
        Task<List<Product>> GetLowStockAsync(int threshold = 10);

        /// <summary>
        /// Thực hiện nhập kho: tăng tồn kho, cập nhật giá nhập mới, lưu lịch sử giao dịch
        /// </summary>
        Task NhapKhoAsync(int productId, int soLuongNhap, decimal giaNhapMoi, string? ghiChu = null);

        /// <summary>
        /// Điều chỉnh tồn kho theo kiểm kê thực tế, tự động tạo giao dịch tăng/giảm.
        /// </summary>
        Task DieuChinhTonKhoAsync(int productId, int tonThucTe, string? lyDo = null);

        /// <summary>
        /// Xuất hủy hàng hư hỏng / mất mát, giảm tồn kho và lưu giao dịch xuất kho.
        /// </summary>
        Task XuatHuyAsync(int productId, int soLuong, string? lyDo = null);
    }
}