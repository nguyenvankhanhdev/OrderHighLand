﻿namespace OrderHighLand.Models
{
	public class Cart
	{
		public int Id { get; set; }
		public int A_Id { get; set; }
		public int Quantity { get; set; }
		public int Provar_Id { get; set; }
		public float TotalPrice { get; set; }
		public List<int> Topping_Id { get; set; }
	}
}
