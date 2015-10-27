using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace korjornalen.Models
{
	public class Car
	{
        public Car()
        {
            Active = true;
        }
        
        public int Id { get; set; }
        public string RegistrationNumber { get; set; }
        public bool Active { get; set; }
	}
}