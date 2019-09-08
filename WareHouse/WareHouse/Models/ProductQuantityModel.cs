using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WareHouse.Models
{
	public class ProductQuantityModel
	{
		[Required(ErrorMessage = "Required")]
		[RegularExpression("^[1-9]$", ErrorMessage = "Quantity must be integer greater than 0.")]
		public int Quantity { get; set; }
	}
}