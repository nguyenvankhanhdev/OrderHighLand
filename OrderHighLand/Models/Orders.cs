namespace OrderHighLand.Models
{
	public class Orders
	{
		public int ID { get; set; }
		public DateTime ORDER_DATE { get; set; }
		public string ORDER_STATUS { get; set; }
		public float ORDER_TOTAL_PRICE { get; set; }
		public int A_ID { get; set; }
	}
}
