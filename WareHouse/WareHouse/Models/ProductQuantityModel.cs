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
		public int Quantity { get; set; }
	}
}