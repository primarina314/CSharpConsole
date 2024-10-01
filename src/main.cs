using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using SnakeGame;
using Celestial;

public class main {
	public static void Main() {
		Simulation simul = new Simulation(0.001M);
		
		simul.AddObject(new CelestialObject(0M, 0M, 0M, 0M, 1000M, 1M));
		simul.AddObject(new CelestialObject(10M, 0M, 0M, 10M, 10M, 1M));
		
		// public CelestialObject(Decimal posX=0M, Decimal posY=0M, Decimal veloX=0M, Decimal veloY=0M, Decimal mass = 0M, Decimal radius = 0M)
		
		simul.Start();
		
		return;
	}
}