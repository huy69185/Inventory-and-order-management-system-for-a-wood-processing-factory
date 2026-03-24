using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyKho.Domain.Entities
{
    public enum TransactionType { NhapKho, XuatKho }

    public class StockTransaction
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public TransactionType LoaiGiaoDich { get; set; }
        public int SoLuong { get; set; }
        public decimal Gia { get; set; }
        public string GhiChu { get; set; } = "";
        public DateTime NgayGiaoDich { get; set; } = DateTime.Now;
    }
}
