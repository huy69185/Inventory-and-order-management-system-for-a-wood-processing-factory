namespace QuanLyKho.Domain.Entities;

public class Supplier
{
    public int Id { get; set; }
    public string MaNhaCungCap { get; set; } = "";
    public string TenNhaCungCap { get; set; } = "";
    public string DienThoai { get; set; } = "";
    public string DiaChi { get; set; } = "";
    public string Email { get; set; } = "";
    public string GhiChu { get; set; } = "";
}

