using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKho.Domain.Entities;

[Table("Invoices")]
public class Invoice
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string SoHoaDon { get; set; } = "";
    public DateTime NgayLap { get; set; } = DateTime.Now;
    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public string KhachHang { get; set; } = "";
    public string DienThoaiKH { get; set; } = "";
    /// <summary>Tiền khách đã thanh toán. Còn nợ = TongThanhToan - TienDaThu</summary>
    public decimal TienDaThu { get; set; } = 0;
    public decimal ChietKhau { get; set; } = 0;
    /// <summary>Thuế GTGT (%): 0, 5, 8, 10...</summary>
    public decimal ThueGTGT { get; set; } = 0;
    public decimal TongTienTruocCK { get; set; } = 0;
    public decimal TongTienSauCK { get; set; } = 0;
    /// <summary>Tổng thanh toán = TongTienSauCK + TienThueGTGT (tính toán, không lưu DB)</summary>
    [NotMapped] public decimal TongThanhToan => TongTienSauCK + TienThueGTGT;
    [NotMapped] public decimal TienThueGTGT => TongTienSauCK * ThueGTGT / 100;
    [NotMapped] public decimal ConNo => TongThanhToan - TienDaThu;

    public List<InvoiceDetail> ChiTiet { get; set; } = new();
}