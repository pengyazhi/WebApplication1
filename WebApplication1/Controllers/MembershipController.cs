using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class MembershipController : Controller
    {

        private MembershipDB db; // 宣告全域變數
        public void ConnectDB()
        {
            db = new MembershipDB(); // 初始化全域變數
            db.ConnectToDatabase();
        }
        public ActionResult JoinMembership()
        {
            return View();
        }
        [HttpPost]
        public JsonResult CheckIfEmailExist(string email)
        {
            // 檢查 DB 是否存在該筆資料*/
            bool emailExist = db.IsEmailExist(email);
            return Json(emailExist);

        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            string MembershipExist = db.IsMembershipExist(email, password);
            if (MembershipExist != null)
            {
                Session["Username"] = MembershipExist;
                Session["Email"] = email;
                Session["Password"] = password;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["ErrorMessage"] = "E-mail或密碼錯誤。請重新輸入。";
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult JoinStatus(string name, string email, string password)
        {
            bool IsEmailSaved = db.IsEmailExist(email);
            if (IsEmailSaved)
            {
                TempData["EmailExist"] = "此Email已經註冊";
                return RedirectToAction("JoinMembership", "Membership");
            }
            else
            {
                // 資料不存在，儲存資料並返回成功的 View
                db.SaveMembership(name, email, password);
                ViewBag.Message = "success";
                return View();
            }
        }
        public void UpdateMembership(string name, string email, string password)
        {
            db.IfMembershipChanged(name, email, password);
        }
    }
}