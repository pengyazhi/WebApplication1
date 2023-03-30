using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplication1.Models;


namespace WebApplication1.Controllers
{

    public class MembershipController : Controller
    {
        public class MembershipDto
        {
            public string name { get; set; }
            public string email { get; set; }
            public string password { get; set; }
            public string id { get; set; }
        }

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
            ConnectDB();
            // 檢查 DB 是否存在該筆資料*/
            bool emailExist = db.IsEmailExist(email);
            return Json(emailExist);

        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            ConnectDB();
            dynamic MembershipExist = db.IsMembershipExist(email, password);
            if (MembershipExist != null)
            {
                Session["Id"] = MembershipExist.Item2;
                Session["Username"] = MembershipExist.Item1;
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
        public ActionResult JoinStatus(string name, string email, string password,string id)
        {
            ConnectDB();
            bool IsEmailSaved = db.IsEmailExist(email);
            if (IsEmailSaved)
            {
                return RedirectToAction("JoinMembership", "Membership");
            }
            else
            {
                // 資料不存在，儲存資料並返回成功的 View
                db.SaveMembership(name, email, password);
                Login(email, password);
                
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult UpdateMembership(string name, string email, string password, string id)
        {
            
            ConnectDB();
            var updateData = db.IfMembershipChanged(name, email, password, id);
            if (updateData != null)
            {
                Logout();
                Login(email, password);
                return RedirectToAction("Index", "Home");

                /*Session["updateName"] = updateData.Item1;
                Session["updateEmail"] = updateData.Item2;
                Session["updatePassword"] = updateData.Item3;

                return Json(new {
                    status = "success",
                    data = new
                    {
                        name = Session["updateName"],
                        email = Session["updateEmail"],
                        password = Session["updatePassword"]
                    }
                });*/
            }
            else
            {
                return null;
                /*return Json(new
                {
                    status = "error",
                    message = "Failed to update membership."
                });*/
            }
        }
        
    }
}