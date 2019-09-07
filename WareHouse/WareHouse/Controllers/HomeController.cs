using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WareHouse.Models;
using System.Security.Cryptography;
using System.Text;
using WareHouse.Service;

namespace WareHouse.Controllers
{
	public class HomeController : Controller
	{
		WareHouseEntities context = new WareHouseEntities();

		public ActionResult Index()
		{
			ViewBag.UserName = Session["UserName"];
			return View();
		}

		[HttpPost]
		public ActionResult LogOut()
		{
			Session.Abandon();
			return RedirectToAction("Login");
		}

		[HttpGet]
		public ActionResult Login()
		{
			LoginModel model = new LoginModel();
			return View(model);
		}

		[HttpPost]
		public ActionResult Login(LoginModel model)
		{
			if (model == null) return Json(new { Error = true, Message = "Fill the login info." }, JsonRequestBehavior.AllowGet);

			using (var context = new WareHouseEntities())
			{
				var user = context.Users.FirstOrDefault();
				if (user == null) return Json(new { Error = true, Message = "Incorrect username or password." }, JsonRequestBehavior.AllowGet);

				string inputHash = "";
				bool correctPassword;
				using (MD5 md5Hash = MD5.Create())
				{
					inputHash = GetMd5Hash(md5Hash, model.Password);

					if (VerifyMd5Hash(md5Hash, inputHash, user.Password))
					{
						correctPassword = true;
					}
					else
					{
						correctPassword = false;
					}
				}

				if (!correctPassword) return Json(new { Error = true, Message = "Incorrect username or password." });

				Session["UserName"] = user.UserName;
				Session["UserID"] = user.UserID;

				return RedirectToAction("Index", "Home");
			}
		}

		static bool VerifyMd5Hash(MD5 md5Hash, string inputHash, string dbHash)
		{
			// Create a StringComparer an compare the hashes.
			StringComparer comparer = StringComparer.OrdinalIgnoreCase;

			if (0 == comparer.Compare(inputHash, dbHash))
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		static string GetMd5Hash(MD5 md5Hash, string input)
		{
			// Convert the input string to a byte array and compute the hash.
			byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

			// Create a new Stringbuilder to collect the bytes
			// and create a string.
			StringBuilder sBuilder = new StringBuilder();

			// Loop through each byte of the hashed data 
			// and format each one as a hexadecimal string.
			for (int i = 0; i < data.Length; i++)
			{
				sBuilder.Append(data[i].ToString("x2"));
			}

			return sBuilder.ToString();
		}

		[HttpGet]
		public ActionResult SendProduct()
		{
			return View();
		}

		[HttpPost]
		public ActionResult SendProduct(ProductQuantityModel model)
		{
			if (Session["UserID"] == null) RedirectToAction("Login");
			if (model == null) return Json(new { Error = true, Message = "Quantity cannot be empty." }, JsonRequestBehavior.AllowGet);

			using (var context = new WareHouseEntities())
			{
				var userID = Session["UserID"];
				var user = context.Users.Find(userID);

				if(user != null)
				{
					user.Quantity += model.Quantity;
					context.SaveChanges();
				}
			}

			return View();
		}
	}
}