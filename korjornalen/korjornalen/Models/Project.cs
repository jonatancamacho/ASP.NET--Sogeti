using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace korjornalen.Models
{
    public class Project
    {
        public Project()
        {
            Active = true;
        }

        public int Id { get; set; }
        public string ProjectNumber { get; set; }
        public string Name { get; set; }
        public virtual ICollection<ApplicationUser> AssociatedUsers { get; set; }
        public bool Active { get; set; }

        public string FullInfo {
            get { 
                return string.Format("{0} - {1}", this.Name, this.ProjectNumber);
            }
        }
    }
}