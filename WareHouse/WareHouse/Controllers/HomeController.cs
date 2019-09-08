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
			if (Session["UserName"] == null)
				return RedirectToAction("Login");

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
				var user = context.Users.FirstOrDefault(x => x.UserName == model.UserName);
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

				return Json(new { Error = false, Message = "Logged in successfully." }, JsonRequestBehavior.AllowGet);
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
			if (Session["UserID"] == null)
				return RedirectToAction("Login");
			else
				return View();
		}

		[HttpPost]
		public ActionResult SendProduct(ProductQuantityModel model)
		{
			if (Session["UserID"] == null)
				return RedirectToAction("Login");

			if (model == null)
				return View();

			using (var context = new WareHouseEntities())
			{
				var userID = Session["UserID"];
				var user = context.Users.Find(userID);

				if(user != null)
				{
					user.Quantity += model.Quantity;
					context.SaveChanges();

					ViewBag.Message = "Quantity " + model.Quantity + " was added successfully. Total quantity now is: " + user.Quantity;
					return Json(new { Error = false, Message = "Quantity " + model.Quantity + " was added successfully. Total quantity now is: " + user.Quantity }, JsonRequestBehavior.AllowGet);
				}

				return Json(new { Error = true, Message = "Error in adding quantity." }, JsonRequestBehavior.AllowGet);
			}

		}

		[HttpPost]
		public ActionResult CreateUser()
		{
			NewUserModel model = new NewUserModel();

			return PartialView("CreateUser", model);
		}

		[HttpPost]
		public ActionResult SaveUser(NewUserModel model)
		{
			if (model == null) return Json(new { Error = true, Message = "Fill all the input fields." }, JsonRequestBehavior.AllowGet);

			if(string.IsNullOrWhiteSpace(model.UserName) || string.IsNullOrWhiteSpace(model.Password) || string.IsNullOrWhiteSpace(model.ConfirmPassword))
				return Json(new { Error = true, Message = "Fill all the input fields." }, JsonRequestBehavior.AllowGet);

			if(model.Password.Length < 6)
				return Json(new { Error = true, Message = "Password has to be at least 6 characters long." }, JsonRequestBehavior.AllowGet);

			if (model.Password != model.ConfirmPassword)
				return Json(new { Error = true, Message = "Confirm password does not match password." }, JsonRequestBehavior.AllowGet);

			string hash = "";
			using (MD5 md5Hash = MD5.Create())
			{
				hash = GetMd5Hash(md5Hash, model.Password);
			}

			if (hash == "") return Json(new { Error = true, Message = "Error in adding user." }, JsonRequestBehavior.AllowGet);

			//check if username is in use
			using (var context = new WareHouseEntities())
			{
				var users = context.Users;

				var userNameTaken = users.FirstOrDefault(x => x.UserName == model.UserName) != null;

				if (userNameTaken) return Json(new { Error = true, Message = "User name is taken." }, JsonRequestBehavior.AllowGet);

				User newUser = new User()
				{
					UserName = model.UserName,
					Password = hash,
					Quantity = 0
				};

				context.Users.Add(newUser);
				context.SaveChanges();

				return Json(new { Error = false, Message = "User created successfully." }, JsonRequestBehavior.AllowGet);
			}
		}
	}
}