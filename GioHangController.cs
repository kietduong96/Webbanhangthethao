using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanDoTheThao.Models;


namespace WebBanDoTheThao.Controllers
{
    public class GioHangController : Controller
    {
        //Tạo đối tượng database chứa dữ liệu từ model data Bán đồ thể thao đã tạo.
        QLBanDoTheThaoDataContext data = new QLBanDoTheThaoDataContext();

        // Lấy giỏ hàng
        public List<GioHang> LayGioHang()
        {
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang == null)
            {
                //Nếu giỏ hàng chưa tồn tại thì khỏi tạo listGioHang
                lstGioHang = new List<GioHang>();
                Session["GioHang"] = lstGioHang;
            }
            return lstGioHang;
        }

        //Thêm hàng vào giỏ
        public ActionResult ThemGioHang(int iMaSP, string strURL)
        {
            //Lấy ra Session giỏ hàng
            List<GioHang> lstGioHang = LayGioHang();
            //Kiểm tra sản phẩm này tồn tại trong Session["GioHang"] chưa ?
            GioHang sanpham = lstGioHang.Find(n => n.iMaSP == iMaSP);
            if (sanpham == null)
            {
                sanpham = new GioHang(iMaSP);
                lstGioHang.Add(sanpham);
                return Redirect(strURL);
            }
            else
            {
                sanpham.iSoLuong++;
                return Redirect(strURL);
            }
        }

        //Phương thức tính tổng số lượng
        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                iTongSoLuong = lstGioHang.Sum(n => n.iSoLuong);
            }
            return iTongSoLuong;
        }

        //Tính tổng tiền
        private double TongTien()
        {
            double iTongTien = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                iTongTien = lstGioHang.Sum(n => n.dThanhTien);
            }
            return iTongTien;
        }

        //Xây dựng trang Giỏ Hàng
        public ActionResult GioHang()
        {
            List<GioHang> lstGioHang = LayGioHang();
            if (lstGioHang.Count == 0)
            {
                return RedirectToAction("Index", "SportStore");
            }
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(lstGioHang);
        }

        //Tạo Partial View để hiển thị thông tin giỏ hàng
        public ActionResult GioHangPartial()
        {
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return PartialView();
        }

        //Xóa giỏ hàng
        public ActionResult XoaGioHang(int iMasp)
        {
            //Lấy giỏ hàng từ Session
            List<GioHang> lstGioHang = LayGioHang();
            //Kiểm tra sản phẩm đã có trong Session["GioHang"]
            GioHang sanpham = lstGioHang.SingleOrDefault(n => n.iMaSP == iMasp);
            //Nếu đã tồn tại thì cho sửa số lượng
            if (sanpham != null)
            {
                lstGioHang.RemoveAll(n => n.iMaSP == iMasp);
                return RedirectToAction("GioHang");
            }
            if (lstGioHang.Count == 0)
            {
                return RedirectToAction("Index", "SportStore");
            }
            return RedirectToAction("GioHang");
        }

        //Cập nhật giỏ hàng
        public ActionResult CapNhatGioHang(int iMasp, FormCollection f)
        {
            //Lấy giỏ hàng từ Session
            List<GioHang> lstGioHang = LayGioHang();
            //Kiểm tra sản phẩm đã có trong Session["GioHang"] chưa?
            GioHang sanpham = lstGioHang.SingleOrDefault(n => n.iMaSP == iMasp);
            //Nếu tồn tại thì cho sửa số lượng
            if (sanpham != null)
            {
                sanpham.iSoLuong = int.Parse(f["txtSoLuong"].ToString());
            }
            return RedirectToAction("GioHang");
        }

        //Xóa tất cả giỏ hàng
        public ActionResult XoaTatCaGioHang()
        {
            //Lấy giỏ hàng từ Session
            List<GioHang> lstGioHang = LayGioHang();
            lstGioHang.Clear();
            return RedirectToAction("Index", "SportStore");
        }

        //Hiển thị View ĐẶT HÀNG để cập nhật các thông tin cho đơn hàng
        [HttpGet]
        public ActionResult DatHang()
        {
            //Kiểm tra đăng nhập
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {
                return RedirectToAction("DangNhap", "NguoiDung");
            }
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "SportStore");
            }

            //Lấy giỏ hàng từ Session
            List<GioHang> lstGioHang = LayGioHang();
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(lstGioHang);

        }

        //Phương thức đặt hàng DatHang(FormCollection)
        public ActionResult DatHang(FormCollection collection)
        {
            //Thêm đơn hàng
            DONHANG dh = new DONHANG();
            KHACHHANG kh = (KHACHHANG)Session["TaiKhoan"];
            List<GioHang> gh = LayGioHang();
            dh.MaKH = kh.MaKH;
            dh.NgayDat = DateTime.Now;
            var NgayGiao = String.Format("{0:MM/dd/yyyy}", collection["NgayGiao"]);
            dh.NgayGiao = DateTime.Parse(NgayGiao);
            dh.TinhTrangGiaoHang = false;
            dh.DaThanhToan = false;
            data.DONHANGs.InsertOnSubmit(dh);
            data.SubmitChanges();
            //Thêm chi tiết đơn hàng
            foreach(var item in gh)
            {
                CHITIETDONHANG ctdh = new CHITIETDONHANG();
                ctdh.MaDonHang = dh.MaDonHang;
                ctdh.MaSP = item.iMaSP;
                ctdh.SoLuong = item.iSoLuong;
                ctdh.DonGia = (decimal)item.dDonGia;
                data.CHITIETDONHANGs.InsertOnSubmit(ctdh);
            }
            data.SubmitChanges();
            Session["GioHang"] = null;
            return RedirectToAction("XacNhanDonHang","GioHang");
        }
        
        //Phương thức XacNhanDonHang
        public ActionResult XacNhanDonHang()
        {
            return View();
        }
    }
}