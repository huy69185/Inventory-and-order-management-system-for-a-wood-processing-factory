namespace QuanLyKho.Web.Services;

/// <summary>
/// Cách tính thuế theo quy định Chi cục Thuế 2026.
/// Biểu thuế TNCN lũy tiến 5 bậc (áp dụng từ 01/7/2026).
/// </summary>
public static class TaxCalculator2026
{
    /// <summary>Giảm trừ gia cảnh bản thân (triệu/tháng).</summary>
    public const decimal GiamTruBanThan = 15.5m;

    /// <summary>Giảm trừ gia cảnh mỗi người phụ thuộc (triệu/tháng).</summary>
    public const decimal GiamTruNguoiPhuThuoc = 6.2m;

    // Biểu thuế lũy tiến từng phần 5 bậc (thu nhập tính thuế tháng, triệu đồng)
    // Bậc 1: Đến 10 tr -> 5%
    // Bậc 2: Trên 10 - 30 tr -> 10%
    // Bậc 3: Trên 30 - 60 tr -> 20%
    // Bậc 4: Trên 60 - 100 tr -> 30%
    // Bậc 5: Trên 100 tr -> 35%

    /// <summary>
    /// Tính thuế TNCN theo biểu thuế lũy tiến từng phần 5 bậc (2026).
    /// Thu nhập tính thuế = Thu nhập chịu thuế - Các khoản giảm trừ (đơn vị: đồng).
    /// </summary>
    public static decimal TinhThueTNCN(decimal thuNhapTinhThueDong, int soNguoiPhuThuoc = 0)
    {
        // Đã giả định thu nhập đưa vào là "thu nhập tính thuế" (sau giảm trừ).
        // Nếu cần tính từ thu nhập chịu thuế: thuNhapTinhThue = thuNhapChiuThue - GiamTruBanThan*1_000_000 - soNguoiPhuThuoc*GiamTruNguoiPhuThuoc*1_000_000
        if (thuNhapTinhThueDong <= 0) return 0;
        decimal thue = 0;
        decimal conLai = thuNhapTinhThueDong;
        // Bậc 1: 0 - 10tr
        decimal muc1 = Math.Min(conLai, 10_000_000m);
        thue += muc1 * 0.05m;
        conLai -= muc1;
        if (conLai <= 0) return thue;
        // Bậc 2: 10 - 30tr (20tr)
        decimal muc2 = Math.Min(conLai, 20_000_000m);
        thue += muc2 * 0.10m;
        conLai -= muc2;
        if (conLai <= 0) return thue;
        // Bậc 3: 30 - 60tr (30tr)
        decimal muc3 = Math.Min(conLai, 30_000_000m);
        thue += muc3 * 0.20m;
        conLai -= muc3;
        if (conLai <= 0) return thue;
        // Bậc 4: 60 - 100tr (40tr)
        decimal muc4 = Math.Min(conLai, 40_000_000m);
        thue += muc4 * 0.30m;
        conLai -= muc4;
        if (conLai <= 0) return thue;
        // Bậc 5: trên 100tr
        thue += conLai * 0.35m;
        return thue;
    }

    /// <summary>
    /// Tính thu nhập tính thuế từ thu nhập chịu thuế (đồng), sau giảm trừ gia cảnh 2026.
    /// </summary>
    public static decimal ThuNhapTinhThue(decimal thuNhapChiuThueDong, int soNguoiPhuThuoc = 0)
    {
        decimal giamTru = GiamTruBanThan * 1_000_000m + soNguoiPhuThuoc * GiamTruNguoiPhuThuoc * 1_000_000m;
        return Math.Max(0, thuNhapChiuThueDong - giamTru);
    }

    /// <summary>
    /// Thuế GTGT theo phương pháp trực tiếp trên doanh thu (Nghị định 123/2020).
    /// Thuế GTGT = Doanh thu × Tỷ lệ % (1%, 2%, 5% tùy ngành).
    /// </summary>
    public static decimal TinhThueGTGTTrucTiep(decimal doanhThu, decimal tyLePhanTram)
    {
        if (doanhThu <= 0 || tyLePhanTram <= 0) return 0;
        return doanhThu * tyLePhanTram / 100m;
    }

    /// <summary>
    /// Ước tính thuế TNCN phải nộp từ doanh thu kỳ (theo quy định 2026).
    /// Thu nhập ước tính = doanh thu kỳ × tỷ lệ %; quy về tháng, áp dụng giảm trừ và biểu thuế 5 bậc.
    /// </summary>
    public static decimal UocTinhThueTNCNTuDoanhThu(decimal doanhThuKy, decimal tyLeThuNhapPhanTram, int soThang, int soNguoiPhuThuoc = 0)
    {
        if (doanhThuKy <= 0 || soThang <= 0) return 0;
        decimal thuNhapUocTinhKy = doanhThuKy * tyLeThuNhapPhanTram / 100m;
        decimal thuNhapUocTinhThang = thuNhapUocTinhKy / soThang;
        decimal thuNhapTinhThueThang = ThuNhapTinhThue(thuNhapUocTinhThang, soNguoiPhuThuoc);
        decimal thueThang = TinhThueTNCN(thuNhapTinhThueThang, soNguoiPhuThuoc);
        return thueThang * soThang;
    }
}
