using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;

namespace Utility
{
	class MyUtils
	{
		public static Decimal Sqrt(Decimal value)
		{
			if(value < 0.0M) return -1.0M;
			Decimal crit = 1.0M;
			Decimal times = (value > 1.0M) ? 16.0M : 0.0625M;
			
			while(!(crit<=value&&value<crit*times || crit>=value&&value>crit*times)) crit *= times;
			
			int cnt = 100;// The larger cnt is, the higher the precision is.
			while(cnt-- > 0) crit = 0.5M * (crit + value/crit);
			
			return crit;
		}
	}
	
	class CelestialObject
	{
		
	}
}

namespace Celestial
{
	
}