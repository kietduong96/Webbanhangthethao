using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBanDoTheThao.Models
{
    public class GioHang
    {
        //Tạo đối tượng data chứa dữ liệu từ model database Bán đồ thể thao đã tạo
        QLBanDoTheThaoDataContext data = new QLBanDoTheThaoDataContext();
        
        public int iMaSP { set; get; }
        public string sTenSP { set; get; }
        public string sAnhBia { set; get; }
        public Double dDonGia { set; get; }
        public int iSoLuong { set; get; }
        public Double dThanhTien
        {
            get { return iSoLuong * dDonGia; }
        }
        
        //Khởi tạo giỏ hàng theo MaSP được truyền vào với số lượng mặc định là 1
        public GioHang(int MaSP)
        {
            iMaSP = MaSP;
            SANPHAM sp = data.SANPHAMs.Single(n => n.MaSP == iMaSP);
            sTenSP = sp.TenSP;
            sAnhBia = sp.AnhBia;
            dDonGia = double.Parse(sp.GiaBan.ToString());
            iSoLuong = 1;
        }




    }
}