using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDS
{
   abstract public class MDSabstract
    {
         
        abstract public String iterate(int n);
        abstract public String iterate(int iter, int threshold, int timeout);
        abstract public double getStress();
        abstract public double getNormalizedStress();
      
    }
}
