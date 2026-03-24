using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKho.Domain.Entities;

public enum SalesOrderStatus
{
    Moi = 0,
    DangSanXuat = 1,
    HoanThanh = 2,
    DaHuy = 3
}

[Table("SalesOrders")]
public class SalesOrder
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string SoDonHang { get; set; } = "";
    public DateTime NgayDat { get; set; } = DateTime.Now;

    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public string TenKhachHang { get; set; } = "";
    public string DienThoai { get; set; } = "";
    public string DiaChiGiaoHang { get; set; } = "";

    /// <summary>Tiền khách đã thanh toán trước cho đơn hàng.</summary>
    public decimal TienDaTraTruoc { get; set; } = 0;

    public SalesOrderStatus TrangThai { get; set; } = SalesOrderStatus.Moi;
    public string GhiChu { get; set; } = "";

    public List<SalesOrderItem> ChiTiet { get; set; } = new();

    [NotMapped]
    public decimal TongTien => ChiTiet?.Sum(i => i.ThanhTien) ?? 0;

    [NotMapped]
    public decimal ConLai => TongTien - TienDaTraTruoc;
}

[Table("SalesOrderItems")]
public class SalesOrderItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int SalesOrderId { get; set; }
    public SalesOrder SalesOrder { get; set; } = null!;

    /// <summary>Mô tả sản phẩm ván ép / hạng mục (kích thước, loại gỗ...).</summary>
    public string MoTaSanPham { get; set; } = "";
    public int SoLuong { get; set; } = 0;
    public string DonVi { get; set; } = "";
    public decimal DonGia { get; set; } = 0;
    public decimal ThanhTien => SoLuong * DonGia;

    /// <summary>Số lượng đã sản xuất (phục vụ theo dõi tiến độ).</summary>
    public int SoLuongDaSanXuat { get; set; } = 0;
}

