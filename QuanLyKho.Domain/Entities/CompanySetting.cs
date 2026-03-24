using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyKho.Domain.Entities
{
    public class CompanySetting
    {
        public int Id { get; set; } = 1;
        public string TenCongTy { get; set; } = "";
        public string DiaChi { get; set; } = "";
        public string DienThoai { get; set; } = "";
        public string Email { get; set; } = "";
        public string MaSoThue { get; set; } = "";
        public string LogoPath { get; set; } = "";

        /// <summary>Tỷ lệ % thuế GTGT trực tiếp trên doanh thu (theo quy định: 1%, 2%, 5% tùy ngành). Mặc định 1.</summary>
        public decimal TyLeThueGTGTTrucTiep { get; set; } = 1;
        /// <summary>Thuế suất GTGT % mặc định khi lập hóa đơn (0, 5, 10).</summary>
        public decimal ThueGTGTMacDinh { get; set; } = 0;
        /// <summary>Tỷ lệ % doanh thu ước tính là thu nhập chịu thuế (để ước tính TNCN từ doanh thu). Mặc định 30.</summary>
        public decimal TyLeThuNhapUocTinh { get; set; } = 30;
    }
}
