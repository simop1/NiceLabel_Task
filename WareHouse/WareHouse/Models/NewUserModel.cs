using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Security;

namespace WareHouse.Models
{
	public class NewUserModel
	{
		[Required(ErrorMessage = "Required")]
		public string UserName { get; set; }
		[Required(ErrorMessage = "Required")]
		[MembershipPassword(
			ErrorMessage = "Your password must be 6 characters long.",
			MinRequiredPasswordLength = 6
		)]
		[DataType(DataType.Password)]
		public string Password { get; set; }
		[Compare("Password", ErrorMessage = "Confirm password doesn't match.")]
		public string ConfirmPassword { get; set; }
	}
}