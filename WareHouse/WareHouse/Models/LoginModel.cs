using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WareHouse.Models
{
	public class LoginModel
	{
		[Required(ErrorMessage = "Required")]
		public string UserName { get; set; }
		[Required(ErrorMessage = "Required")]
		public string Password { get; set; }
	}
}