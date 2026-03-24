namespace QuanLyKho.Domain.Entities;

public class Customer
{
    public int Id { get; set; }
    public string MaKhachHang { get; set; } = "";
    public string TenKhachHang { get; set; } = "";
    public string DienThoai { get; set; } = "";
    public string DiaChi { get; set; } = "";
    public string Email { get; set; } = "";
    public string GhiChu { get; set; } = "";
}
