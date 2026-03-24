using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKho.Domain.Entities;

/// <summary>Hóa đơn mua hàng (đầu vào) - dùng cho kê khai thuế GTGT.</summary>
[Table("PurchaseInvoices")]
public class PurchaseInvoice
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string SoHoaDon { get; set; } = "";
    public DateTime NgayHoaDon { get; set; } = DateTime.Now;
    public string NhaCungCap { get; set; } = "";
    /// <summary>Tiền hàng chưa thuế (giá trị hàng hóa).</summary>
    public decimal TienHangChuaThue { get; set; } = 0;
    /// <summary>Thuế suất GTGT (%): 0, 5, 8, 10.</summary>
    public decimal ThueGTGT { get; set; } = 0;
    /// <summary>Tiền thuế GTGT đầu vào.</summary>
    public decimal TienThueGTGT { get; set; } = 0;
    public string GhiChu { get; set; } = "";
}
