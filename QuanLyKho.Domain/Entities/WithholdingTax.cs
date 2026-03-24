using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKho.Domain.Entities;

/// <summary>Khấu trừ thuế TNCN (khi chi trả cho cá nhân) - theo quy định 2026.</summary>
[Table("WithholdingTaxes")]
public class WithholdingTax
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public DateTime NgayChi { get; set; } = DateTime.Now;
    /// <summary>Tên cá nhân nhận (người nộp thuế TNCN).</summary>
    public string NguoiNhan { get; set; } = "";
    public string MaSoThue { get; set; } = "";
    /// <summary>Thu nhập chịu thuế (trước khi khấu trừ).</summary>
    public decimal ThuNhapChiuThue { get; set; } = 0;
    /// <summary>Thuế TNCN đã khấu trừ (số tiền nộp thay cho cá nhân).</summary>
    public decimal ThueKhauTru { get; set; } = 0;
    public string GhiChu { get; set; } = "";
}
