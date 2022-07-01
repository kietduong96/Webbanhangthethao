using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanDoTheThao.Models;

using PagedList;
using PagedList.Mvc;

namespace WebBanDoTheThao.Controllers
{
    public class SportStoreController : Controller
    {
        //Tạo đối tượng data chứa dữ liệu từ model database bán đồ thể thao đã tạo
        QLBanDoTheThaoDataContext data = new QLBanDoTheThaoDataContext();

        //Hàm lấy n sản phẩm mới
        private List<SANPHAM> Layspmoi(int count)
        {
            //Sắp xếp sản phẩm theo ngày cập nhật, sau đó lấy top @count
            return data.SANPHAMs.OrderByDescending(a => a.NgayCapNhat).Take(count).ToList();
        }

        // GET: SportStore
        public ActionResult Index(int ? page)
        {
            //Tạo biến quy định số sản phẩm trên mỗi trang
            int pageSize = 6;
            //Tạo biến số trang
            int pageNum = (page ?? 1);

            //lấy top 5 sản phẩm mới bán chạy nhất
            var sanphammoi = Layspmoi(6);
            return View(sanphammoi.ToPagedList(pageNum, pageSize));
        }

        //Partial View của LoaiSanPham
        public ActionResult LoaiSanPham()
        {
            var loaisanpham = from lsp in data.LOAISANPHAMs select lsp;
            return PartialView(loaisanpham);
        }

        //Partial View của HangSanXuat
        public ActionResult HangSanXuat()
        {
            var hangsanxuat = from hsx in data.HANGSANXUATs select hsx;
            return PartialView(hangsanxuat);
        }

        //Partial View của SPTheoLoai
        public ActionResult SPTheoLoai(int id, int? page)
        {
            //Tạo biến quy định số sản phẩm trên mỗi trang
            int pageSize = 6;
            //Tạo biến số trang
            int pageNum = (page ?? 1);

            var sanpham = from sp in data.SANPHAMs where sp.MaLoai == id select sp;
            return View(sanpham.ToPagedList(pageNum, pageSize));
        }

        //Partial View của SP Theo HSX
        public ActionResult SPTheoHSX(int id, int? page)
        {
            //Tạo biến quy định số sản phẩm trên mỗi trang
            int pageSize = 6;
            //Tạo biến số trang
            int pageNum = (page ?? 1);

            var sanpham = from sp in data.SANPHAMs where sp.MaHSX == id select sp;
            return View(sanpham.ToPagedList(pageNum, pageSize));
        }

        //View trang chi tiết sản phẩm
        public ActionResult Details (int id)
        {
            var sanpham = from sp in data.SANPHAMs
                          where sp.MaSP == id
                          select sp;
            return View(sanpham.Single());
        }

    }
}