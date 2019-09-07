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
		Entities context = new Entities();

		public ActionResult Index()
		{
			return View();
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
			var dbUsers = context.Users;

			if (model == null) return Json(new { Error = true, Message = "Fill the login info." }, JsonRequestBehavior.AllowGet);

			var user = dbUsers.FirstOrDefault(x => x.UserName == model.UserName);

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

			return RedirectToAction("Index", "Home");
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

		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}
	}
}