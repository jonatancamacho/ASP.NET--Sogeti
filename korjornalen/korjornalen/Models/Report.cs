using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace korjornalen.Models
{
	public class Report
	{
        public int Id { get; set; }
        public int OdometerStart { get; set; }
        public int? OdometerEnd { get; set; }
        public string FromLocation { get; set; }
        public string ToLocation { get; set; }
        public bool Debitable { get; set; }
        public DateTime Date { get; set; }
        public string Purpose { get; set; }
        public string Passengers { get; set; }
        
        public int CarId { get; set; }
        public string UserId { get; set; }
        public int ProjectId { get; set; }

        public virtual Car AssociatedCar { get; set; }
        public virtual ApplicationUser AssociatedUser { get; set; }
        public virtual Project AssociatedProject { get; set; }
	}
}