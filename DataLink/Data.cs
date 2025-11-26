using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLink
{
    public class Data
    {
        public Geometry geometry { get; set; }
        public Properties Properties { get; set; } = new Properties();
        public Loads Loads { get; set; } = new Loads();
        public Criteria Criteria { get; set; } = new Criteria();
        public Results Results { get; set; } = new Results();

        public Data()
        {
                geometry = new Geometry();
        }
    }
}
