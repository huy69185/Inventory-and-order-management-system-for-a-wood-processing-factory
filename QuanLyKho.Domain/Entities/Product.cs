using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyKho.Domain.Entities
{
    public enum ProductType
    {
        ThanhPham = 0,
        NguyenLieu = 1
    }

    public class Product
    {
        public int Id { get; set; }
        public string MaSanPham { get; set; } = "";
        public string TenSanPham { get; set; } = "";
        public string DonVi { get; set; } = "Cái";
        public int SoLuongTon { get; set; } = 0;
        public decimal GiaNhap { get; set; } = 0;
        public decimal GiaBan { get; set; } = 0;
        /// <summary>Ngưỡng cảnh báo sắp hết hàng (tồn &lt; ngưỡng). Mặc định 10.</summary>
        public int SoLuongCanhBao { get; set; } = 10;
        public DateTime NgayCapNhat { get; set; } = DateTime.Now;

        /// <summary>Nhà cung cấp chính cho sản phẩm (thường áp dụng cho nguyên liệu).</summary>
        public int? SupplierId { get; set; }
        public Supplier? Supplier { get; set; }
    }
}
