using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using WebBanDoTheThao.Models;

namespace WebBanDoTheThao.Controllers
{
    public class AdminController : Controller
    {
        QLBanDoTheThaoDataContext db = new QLBanDoTheThaoDataContext();
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SanPham(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            //return View(db.SANPHAMs.ToList());
            return View(db.SANPHAMs.ToList().OrderBy(n => n.MaSP).ToPagedList(pageNumber, pageSize));
        }
        public ActionResult HangSanXuat(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            return View(db.HANGSANXUATs.OrderBy(n => n.MaHSX).ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            //Gán các giá trị người dùng nhập liệu cho các biến
            var tendn = collection["UserAdmin"];
            var matkhau = collection["PassAdmin"];
            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Phải nhập tên đăng nhập !!";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "Phải nhập mật khẩu !!";
            }
            else
            {
                //Gán giá trị đối tượng được tạo mới (ad)
                ADMIN ad = db.ADMINs.SingleOrDefault(n => n.UserAdmin == tendn && n.PassAdmin == matkhau);
                if (ad != null)
                {
                    //ViewBag.ThongBao = "Chúc mừng đăng nhập thành công";
                    Session["TaiKhoanAdmin"] = ad;
                    return RedirectToAction("Index", "Admin");
                }
                else
                    ViewBag.ThongBao = "Tên đang nhập hoặc mật khẩu không đúng";
            }
            return View();
        }

        [HttpGet]
        public ActionResult ThemMoiSanPham()
        {
            //Đưa dữ liệu vào dropdownList
            //Lấy ds từ table chủ đề, sắp xếp tăng dần theo tên chủ đề, chọn lấy giá trị Mã loại, hiển thị loại sản phẩm
            ViewBag.MaLoai = new SelectList(db.LOAISANPHAMs.ToList().OrderBy(n => n.TenLoai), "MaLoai", "TenLoai");
            ViewBag.MaHSX = new SelectList(db.HANGSANXUATs.ToList().OrderBy(n => n.TenHSX), "MaHSX", "TenHSX");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemMoiSanPham(SANPHAM sanpham, HttpPostedFileBase fileupload)
        {
            ViewBag.MaLoai = new SelectList(db.LOAISANPHAMs.ToList().OrderBy(n => n.TenLoai), "MaLoai", "TenLoai");
            ViewBag.MaHSX = new SelectList(db.HANGSANXUATs.ToList().OrderBy(n => n.TenHSX), "MaHSX", "TenHSX");
            //Kiểm tra đường dẫn file
            if (fileupload == null)
            {
                ViewBag.ThongBao = "Vui lòng chọn ảnh bìa";
                return View();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    //Lưu tên file, lưu ý bổ sung thư viện using System.IO;
                    var fileName = Path.GetFileName(fileupload.FileName);
                    //Lưu đường dẫn của file
                    var path = Path.Combine(Server.MapPath("~/HinhAnh"), fileName);
                    //Kiểm tra hình ảnh tồn tại chưa ?
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.ThongBao = "Hình ảnh đã tồn tại";
                    }
                    else
                    {
                        //Lưu hình ảnh vào đường dẫn
                        fileupload.SaveAs(path);
                    }
                    sanpham.AnhBia = fileName;
                    //Lưu vào CSDL
                    db.SANPHAMs.InsertOnSubmit(sanpham);
                    db.SubmitChanges();
                }
                return RedirectToAction("SanPham");
            }
        }

        //Hiển thị sản phẩm
        public ActionResult ChiTietSanPham(int id)
        {
            //Lấy ra đối tượng sản phẩm theo mã
            SANPHAM sanpham = db.SANPHAMs.SingleOrDefault(n => n.MaSP == id);
            ViewBag.MaSP = sanpham.MaSP;
            if (sanpham == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sanpham);
        }

        //Xóa sản phẩm
        [HttpGet]
        public ActionResult XoaSanPham(int id)
        {
            //Lấy ra đối tượng sản phẩm cần xóa theo mã
            SANPHAM sanpham = db.SANPHAMs.SingleOrDefault(n => n.MaSP == id);
            ViewBag.MaSP = sanpham.MaSP;
            if (sanpham == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sanpham);
        }

        [HttpPost, ActionName("XoaSanPham")]
        public ActionResult XacNhanXoa(int id)
        {
            //Lấy ra đối tượng sản phẩm cần xóa theo mã
            SANPHAM sanpham = db.SANPHAMs.SingleOrDefault(n => n.MaSP == id);
            ViewBag.MaSP = sanpham.MaSP;
            if (sanpham == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.SANPHAMs.DeleteOnSubmit(sanpham);
            db.SubmitChanges();
            return RedirectToAction("SanPham");
        }

        //Chỉnh sửa sản phẩm
        [HttpGet]
        public ActionResult SuaSanPham(int id)
        {
            //Lấy ra đối tượng sản phẩm theo mã
            SANPHAM sanpham = db.SANPHAMs.SingleOrDefault(n => n.MaSP == id);
            if (sanpham == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            //Đưa dữ liệu vào dropdownList
            //Lấy ds từ table loại sản phẩm, sắp xếp tăng dần theo tên loại, chọn lấy giá trị MaLoai, hiển thị tên Tên Loại
            ViewBag.MaLoai = new SelectList(db.LOAISANPHAMs.ToList().OrderBy(n => n.TenLoai), "MaLoai", "TenLoai", sanpham.MaLoai);
            ViewBag.MaHSX = new SelectList(db.HANGSANXUATs.ToList().OrderBy(n => n.TenHSX), "MaHSX", "TenHSX", sanpham.MaHSX);
            return View(sanpham);
        }

        [HttpPost, ActionName("SuaSanPham")]
        [ValidateInput(false)]
        public ActionResult SuaSanPham(int id, HttpPostedFileBase fileupload)
        {
            SANPHAM sanpham = db.SANPHAMs.SingleOrDefault(n => n.MaSP == id);
            ViewBag.MaSP = sanpham.MaSP;
            if (fileupload == null)
            {
                UpdateModel(sanpham);
                db.SubmitChanges();
                return RedirectToAction("SanPham");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var fileName = Path.GetFileName(fileupload.FileName);
                    var path = Path.Combine(Server.MapPath("~/HinhAnh"), fileName);
                    if (System.IO.File.Exists(path))
                    {
                        ViewData["Loi1"] = " Hình đã tồn tại ";
                    }
                    else
                    {
                        fileupload.SaveAs(path);
                    }
                    sanpham.AnhBia = fileName;
                    UpdateModel(sanpham);
                    db.SubmitChanges();
                }
                return RedirectToAction("SanPham");
            }
        }

        // Thêm mới Hãng Sản Xuất
        [HttpGet]
        public ActionResult ThemMoiHSX()
        {
            //Đưa dữ liệu vào dropdownList
            //Lấy ds từ table chủ đề, sắp xếp tăng dần theo tên chủ đề, chọn lấy giá trị Mã loại, hiển thị loại sản phẩm
            ViewBag.MaLoai = new SelectList(db.LOAISANPHAMs.ToList().OrderBy(n => n.TenLoai), "MaLoai", "TenLoai");
            ViewBag.MaHSX = new SelectList(db.HANGSANXUATs.ToList().OrderBy(n => n.TenHSX), "MaHSX", "TenHSX");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemmoiHSX(HANGSANXUAT HSX)
        {
            //Dua du lieu vao dropdownload
            ViewBag.MaHSX = new SelectList(db.HANGSANXUATs.ToList().OrderBy(n => n.TenHSX), "TenHSX", "DiaChi", "DienThoai");
            db.HANGSANXUATs.InsertOnSubmit(HSX);
            db.SubmitChanges();
            return RedirectToAction("HangSanXuat");
        }


        //Xóa HÃNG SẢN XUẤT
        [HttpGet]
        public ActionResult XoaHSX(int id)
        {
            //Lay ra doi tuong can xoa theo ma
            HANGSANXUAT HSX = db.HANGSANXUATs.SingleOrDefault(n => n.MaHSX == id);
            ViewBag.MaHSX = HSX.MaHSX;
            if (HSX == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(HSX);
        }
        [HttpPost, ActionName("XoaHSX")]
        public ActionResult XacNhanXoaHSX(int id)
        {
            //Lay ra doi tuong SẢN PHẨM can xoa theo ma
            HANGSANXUAT HSX = db.HANGSANXUATs.SingleOrDefault(n => n.MaHSX == id);
            ViewBag.MaHSX = HSX.MaHSX;
            if (HSX == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.HANGSANXUATs.DeleteOnSubmit(HSX);
            db.SubmitChanges();
            return RedirectToAction("HangSanXuat");
        }
    }
}