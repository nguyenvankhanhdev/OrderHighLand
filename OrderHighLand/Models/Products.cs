<<<<<<< HEAD
﻿namespace OrderHighLand.Models
{
    public class Products
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Image { get; set; }
        public int Cate_Id { get; set; } 
    }
=======
﻿using System.ComponentModel.DataAnnotations.Schema;

namespace OrderHighLand.Models
{
	public class Products
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Slug { get; set; }
		public string Image { get; set; }
		public int Cate_Id { get; set; }
	}
>>>>>>> 73520e085f0bbfa6526db0cfe5bc8fc6a60db6e6
}
