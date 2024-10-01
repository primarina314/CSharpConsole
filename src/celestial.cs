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
}

namespace Celestial
{
	class CelestialObject
	{
		private Decimal mass;
		private Decimal posX;
		private Decimal posY;
		private Decimal veloX;
		private Decimal veloY;
		private Decimal radius;
		private static Decimal elasticity = 0.9M; // must not be greater than 1.0
		
		public Decimal Mass { get { return mass; } }
		public Decimal PosX { get { return posX; } }
		public Decimal PosY { get { return posY; } }
		public Decimal VeloX { get { return veloX; } }
		public Decimal VeloY { get { return veloY; } }
		public Decimal Radius { get { return radius; } }
		
		public CelestialObject(Decimal posX=0M, Decimal posY=0M, Decimal veloX=0M, Decimal veloY=0M, Decimal mass = 0M, Decimal radius = 0M)
		{
			this.posX = posX;
			this.posY = posY;
			this.veloX = veloX;
			this.veloY = veloY;
			this.mass = mass;
			this.radius = radius;
		}
		
		public void Update(Decimal dt, Decimal accX = 0M, Decimal accY = 0M)
		{
			posX = posX + veloX*dt;
			posY = posY + veloY*dt;
			veloX = veloX + accX*dt;
			veloY = veloY + accY*dt;
		}
		
		public void CollisionWith(CelestialObject other)
		{
			Decimal initX = this.veloX, initY = this.veloY;
			
			this.veloX = (other.mass * elasticity * (other.veloX - this.veloX) + this.mass * this.veloX + other.mass * other.veloX) / (this.mass + other.mass);
			this.veloY = (other.mass * elasticity * (other.veloY - this.veloY) + this.mass * this.veloY + other.mass * other.veloY) / (this.mass + other.mass);
			
			other.veloX = (this.mass * elasticity * (initX - other.veloX) + other.mass * other.veloX + this.mass * initX) / (this.mass + other.mass);
			other.veloY = (this.mass * elasticity * (initY - other.veloY) + other.mass * other.veloY + this.mass * initY) / (this.mass + other.mass);
		}
		
		public override string ToString()
		{
			return "(" + posX + ", " + posY + ")";
		}
	}
	
	class Simulation
	{
		// This class will handle the simulation of celestial objects and their interactions.
		private Decimal gravityConstant = 1.0M;
		public Decimal GravityConstant { get { return gravityConstant; } }
		public Decimal dt { get; set; }
		
		private List<CelestialObject> objects;
		
		private bool[,] board;
		private List<Tuple<int,int>> prevPosition;
		
		private const Decimal fromX = -12M, toX = 12M, fromY = -12M, toY = 12M;
		private const int SCREEN_SIZE_X = 74, SCREEN_SIZE_Y = 37;
		
		private StringBuilder buffer = new StringBuilder();
		
		public Simulation(Decimal dt = 0.001M)
		{
			Console.CursorVisible = false;
			Console.SetWindowSize(SCREEN_SIZE_X, SCREEN_SIZE_Y + 1);
			
			objects = new List<CelestialObject>();
			this.dt = dt;
			board = new bool[SCREEN_SIZE_Y, SCREEN_SIZE_X];
			prevPosition = new List<Tuple<int,int>>();
		}
		
		public void Start()
		{
			Console.SetCursorPosition(0,0);
			for(int i=0;i<board.GetLength(0);i++)
			{
				for(int j=0;j<board.GetLength(1);j++)
					Console.Write(' ');
				Console.WriteLine();
			}
			
			int UPDATE_TICK = 0;
			int prev = Environment.TickCount;
			
			while(true)
			{
				int curr = Environment.TickCount;
				if(curr - prev >= UPDATE_TICK)
				{
					Update();
					Draw();
					prev = curr;
				}
			}
		}
		
		public void AddObject(CelestialObject co) { objects.Add(co); }
		
		public void Update()
		{
			Decimal distX - 0.0M, distY = 0.0M;
			Decimal dist3 = 0.0M;
			Decimal[] accX = new Decimal[objects.Count];
			Decimal[] accY = new Decimal[objects.Count];
			
			for(int i=0;i<objects.Count-1;i++)
			{
				for(int j=i+1;j<objects.Count;j++)
				{
					distX = objects[i].PosX - objects[j].PosX;
					distY = objects[i].PosY - objects[j].PosY;
					dist3 = (distX*distX + distY*distY) * MyUtils.Sqrt(distX*distX + distY*distY);
					
					accX[i] -= gravityConstant * objects[j].Mass * distX / dist3;
					accY[i] -= gravityConstant * objects[j].Mass * distY / dist3;
					
					accX[j] += gravityConstant * objects[i].Mass * distX / dist3;
					accY[j] += gravityConstant * objects[i].Mass * distY / dist3;
				}
			}
			
			for(int i=0;i<objects.Count;i++)
			{
				objects[i].Update(dt, accX[i], accY[i]);
			}
			
			// doesn't handle collisions among three or more objects.
			for(int i=0;i<objects.Count-1;i++)
			{
				for(int j=0;j<objects.Count;j++)
				{
					distX = objects[i].PosX - objects[j].PosX;
					distY = objects[i].PosY - objects[j].PosY;
					
					if(distX*distX+distY*distY <= (objects[i].Radius+objects[j].Radius)*(objects[i].Radius+objects[j].Radius))
						objects[i].CollisionWith(objects[j]);
				}
			}
		}
		
		public void Focus(int focus = -1)
		{
			// relative position
		}
		
		public override string ToString()
		{
			
		}
	}
	
}

