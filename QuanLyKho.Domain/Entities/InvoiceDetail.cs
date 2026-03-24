using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyKho.Domain.Entities
{
    public class InvoiceDetail
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public Invoice Invoice { get; set; } = null!;
        public int ProductId { get; set; }
        public string TenSanPham { get; set; } = "";
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => SoLuong * DonGia;
    }
}
