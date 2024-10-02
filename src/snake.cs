using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;

namespace SnakeGame
{
	enum Direction
	{
		left, right, up, down
	}
	
	class Snake
	{
		public LinkedList<Tuple<int,int>> Body { get { return body; } }
		public Direction TailDir { get { return tailDir; } }
		
		private LinkedList<Tuple<int,int>> body;
		private Direction headDir;
		private Direction tailDir;
		
		public Snake(int i=0, int j=0)
		{
			body = new LinkedList<Tuple<int,int>>();
			body.AddFirst(new Tuple<int,int>(i,j));
			headDir = tailDir = Direction.up;
		}
		
		public void Turn(ConsoleKeyInfo input)
		{
			switch(input.Key)
			{
				case ConsoleKey.LeftArrow:
					headDir = Direction.left;
					break;
				case ConsoleKey.RightArrow:
					headDir = Direction.right;
					break;
				case ConsoleKey.UpArrow:
					headDir = Direction.up;
					break;
				case ConsoleKey.DownArrow:
					headDir = Direction.down;
					break;
			}
		}
		
		public void Move()
		{
			int i = body.First.Value.Item1;
			int j = body.First.Value.Item2;
			switch(headDir)
			{
				case Direction.left:
					body.AddFirst(new Tuple<int,int>(i,j-1));
					break;
				case Direction.right:
					body.AddFirst(new Tuple<int,int>(i,j+1));
					break;
				case Direction.up:
					body.AddFirst(new Tuple<int,int>(i-1,j));
					break;
				case Direction.down:
					body.AddFirst(new Tuple<int,int>(i+1,j));
					break;
			}
			
			int iBefore = body.Last.Value.Item1;
			int jBefore = body.Last.Value.Item2;
			body.RemoveLast();
			int iAfter = body.Last.Value.Item1;
			int jAfter = body.Last.Value.Item2;
			
			switch(2*(iAfter-iBefore) + (jAfter-jBefore))
			{
				case -2:// -1 0
					tailDir = Direction.down;
					break;
				case 2:// 1 0
					tailDir = Direction.up;
					break;
				case -1:// 0 -1
					tailDir = Direction.right;
					break;
				case 1:// 0 1
					tailDir = Direction.left;
					break;
			}
		}
		
		public void Feed()
		{
			int i = body.Last.Value.Item1;
			int j = body.Last.Value.Item2;
			switch(tailDir)
			{
				case Direction.left:
					body.AddLast(new Tuple<int,int>(i,j-1));
					break;
				case Direction.right:
					body.AddLast(new Tuple<int,int>(i,j+1));
					break;
				case Direction.up:
					body.AddLast(new Tuple<int,int>(i-1,j));
					break;
				case Direction.down:
					body.AddLast(new Tuple<int,int>(i+1,j));
					break;
			}
		}
		
		public Tuple<int,int> GetHead() { return body.First.Value; }
		public Tuple<int,int> GetTail() { return body.Last.Value; }
		public Tuple<int,int> GetNeck() { return body.First.Next.Value; }
		
		public override string ToString()
		{
			StringBuilder res = new StringBuilder("");
			foreach(var item in body)
			{
				res.Append(item);
				res.Append(", ");
			}
			return res.ToString();
		}
	}
	
	enum Area
	{
		reachable,
		unreachable,
		occupied,
		feed
	}
	
	class GameManager
	{
		private const int UPDATE_TICK = 5;
		private int MOVE_TICK = 100;
		private int TIMER_TICK = 100;
		
		private const int MAP_SIZE_R = 30;
		private const int MAP_SIZE_C = 60;
		
		private Area[,] map;
		private Snake snake;
		
		private int feedX, feedY;
		
		private bool updated;
		
		private int highscore;
		
		private char shape = '@';
		
		public GameManager()
		{
			Console.CursorVisible = false;
			Console.Clear();
			Console.SetWindowSize(MAP_SIZE_C, MAP_SIZE_R+4);
			Console.SetCursorPosition(0,0);
			Console.BackgroundColor = ConsoleColor.Black;
			
			map = new Area[MAP_SIZE_R, MAP_SIZE_C];
			snake = new Snake(map.GetLength(0)/2, map.GetLength(1)/2);
			for(int i=0;i<map.GetLength(0);i++) for(int j=0;j<map.GetLength(0);j++) map[i,j] = Area.reachable;
			// map[10,10] = Area.unreachable;
			
			CreateNewFeed();
			updated = true;
			highscore = FileSystem.LoadScore();
		}
		
		public void GameStart()
		{
			InitBoard();
			
			int prev_m, prev_u;
			prev_m = prev_u = Environment.TickCount;
			
			int CURR_MOVE_TICK = MOVE_TICK;
			// The smaller it is, the harder the game is.
			
			while(true)
			{
				int curr = Environment.TickCount;
				if(curr - prev_u < UPDATE_TICK) continue;
				
				if(updated)
				{
					Draw();
					updated = false;
				}
				if(isOver()) break;
				
				if(Console.KeyAvailable)
				{
					ConsoleKeyInfo input = Console.ReadKey(true);
					snake.Turn(input);
					
					if(input.Key == ConsoleKey.Spacebar) CURR_MOVE_TICK = MOVE_TICK / 10;
					if(input.Key == ConsoleKey.Escape)
					{
						Console.SetCursorPosition(0,MAP_SIZE_R+2);
						Console.ForegroundColor = ConsoleColor.White;
						Console.WriteLine("PAUSE      ");
						while(!Console.KeyAvailable);
						
						Console.SetCursorPosition(0,MAP_SIZE_R+2);
						Console.ForegroundColor = ConsoleColor.White;
						Console.WriteLine("IN PROGRESS");
					}
				}
				else CURR_MOVE_TICK = MOVE_TICK;
				
				if(curr - prev_m >= CURR_MOVE_TICK)
				{
					snake.Move();
					if(isFeedTaken())
					{
						snake.Feed();
						Score();
						CreateNewFeed();
					}
					updated = true;
					prev_m = curr;
				}
			}
		}
		
		private void CreateNewFeed()
		{
			if(isFeedTaken()) map[feedX,feedY] = Area.occupied;
			// Color change's delt with in Draw/Update function
			
			Random random = new Random();
			feedX = random.Next(0,map.GetLength(0));
			feedY = random.Next(0,map.GetLength(1));
			// 생성 위치 - 단순 랜덤이 아니라, unreachable 제외한 곳에서 random
			
			
			map[feedX,feedY] = Area.feed;
		}
		
		private bool isFeedTaken()
		{
			Tuple<int,int> head = snake.GetHead();
			return head.Item1==feedX && head.Item2==feedY;
		}
		
		private bool isOver()
		{
			bool res = false;
			Tuple<int,int> head = snake.GetHead();
			
			res = res || head.Item1<0 || head.Item1>=map.GetLength(0) || head.Item2<0 || head.Item2>=map.GetLength(1);
			res = res || map[head.Item1, head.Item2]==Area.unreachable;
			
			if(res)
			{
				Console.SetCursorPosition(map.GetLength(1)/2-9, map.GetLength(0)/2);
				Console.ForegroundColor = ConsoleColor.Red;
				Console.BackgroundColor = ConsoleColor.White;
				Console.WriteLine("---GAME OVER---");
				Console.ForegroundColor = ConsoleColor.White;
				Console.BackgroundColor = ConsoleColor.Black;
			}
			
			return res;
		}
		
		private void InitBoard()
		{
			Console.SetCursorPosition(0,0);
			for(int i=0;i<map.GetLength(0);i++)
			{
				for(int j=0;j<map.GetLength(1);j++)
				{
					switch(map[i,j])
					{
						case Area.reachable:
							Console.ForegroundColor = ConsoleColor.Gray;
							break;
						case Area.unreachable:
							Console.ForegroundColor = ConsoleColor.Black;
							break;
						case Area.occupied:
							Console.ForegroundColor = ConsoleColor.Green;
							break;
						case Area.feed:
							Console.ForegroundColor = ConsoleColor.Red;
							break;
						
					}
					Console.Write(shape);
				}
				Console.WriteLine();
			}
			Score();
		}
		
		private void Draw()
		{
			// Draw and Update map
			/*
			more efficient way than prev one.
			Not print all the map's elements, nut just changed ones.
			Set cursor to the positions of snake's head, tail, and feed
			then it takes only const time in printings, which isn't dependant on the size sq.
			
			Without this optimization, the printing out process takes pretty much time, which might lead to raggings.
			It's because the printing out process takes much more times than the other calculations.
			*/
			Tuple<int,int> tail = snake.GetTail();
			Console.ForegroundColor = ConsoleColor.Gray;
			switch(snake.TailDir)
			{
				case Direction.left:
					map[tail.Item1,tail.Item2-1] = Area.reachable;
					Console.SetCursorPosition(tail.Item2-1,tail.Item1);
					break;
				case Direction.right:
					map[tail.Item1,tail.Item2+1] = Area.reachable;
					Console.SetCursorPosition(tail.Item2+1,tail.Item1);
					break;
				case Direction.up:
					map[tail.Item1-1,tail.Item2] = Area.reachable;
					Console.SetCursorPosition(tail.Item2,tail.Item1-1);
					break;
				case Direction.down:
					map[tail.Item1+1,tail.Item2] = Area.reachable;
					Console.SetCursorPosition(tail.Item2,tail.Item1+1);
					break;
			}
			Console.Write(shape);
			
			/*
			Q. what if the position of newly created feed is same with the tail?
			A. The below line draws the feed after the passing way(path) handling above/
			*/
			Console.SetCursorPosition(feedY,feedX);
			Console.ForegroundColor = ConsoleColor.Red;
			Console.Write(shape);
			
			Tuple<int,int> head = snake.GetHead();
			// Below is for uncolored positions resulted from overlap btw head and the other body parts. - const time.
			foreach(var item in snake.Body)
			{
				if(item == head) continue;
				if(map[item.Item1,item.Item2]==Area.reachable || map[item.Item1,item.Item2]==Area.feed)
				{
					map[item.Item1,item.Item2] = Area.occupied;
					Console.SetCursorPosition(item.Item2,item.Item1);
					Console.ForegroundColor = ConsoleColor.Green;
					Console.Write(shape);
				}
			}
			
			// Below is for highlighing the head with color blue.
			if(head.Item1>=0 && head.Item1<map.GetLength(0) && head.Item2>=0 && head.Item2<map.GetLength(1))
			{
				Console.SetCursorPosition(head.Item2,head.Item1);
				Console.ForegroundColor = ConsoleColor.Blue;
				Console.Write(shape);
				if(snake.Body.Count <= 1) return;
				
				Tuple<int,int> neck = snake.GetNeck();
				Console.SetCursorPosition(neck.Item2, neck.Item1);
				Console.ForegroundColor = ConsoleColor.Green;
				Console.Write(shape);
			}
			// printing's conducted at head, neck, and tail only. - O(1) time in printing
			return;
		}
		
		private void Score()
		{
			int score = snake.Body.Count - 1;
			Console.SetCursorPosition(0, MAP_SIZE_R);
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine("Score: " + score);
			
			if(score%10==0 && MOVE_TICK>10)
			{
				MOVE_TICK -= 10;
				Console.Write("Level: " + (score/10+1));
			}
			if(highscore < score) FileSystem.SaveScore(score);
			
		}
	}
	
	class FileSystem
	{
		public static void SaveScore(int score)
		{
			StreamWriter writer = null;
			string BASE = score.ToString();
			
			try
			{
				writer = new StreamWriter(@"./Data.ini");
				writer.Write(BASE);
			}
			finally
			{
				if(writer != null) writer.Close();
			}
		}
		
		private static int StringToInt(string str)
		{
			int res = 0;
			foreach(char item in str) res = res*10 + item-'0';
			return res;
		}
		
		public static int LoadScore()
		{
			StreamReader reader = null;
			int score = 0;
			
			try
			{
				reader = new StreamReader(@"./Data.ini");
				string BASE = reader.ReadToEnd();
				score = StringToInt(BASE);
			}
			catch(FileNotFoundException)
			{
				
			}
			catch(FormatException)
			{
				
			}
			finally
			{
				if(reader != null) reader.Close();
			}
			return score;
		}
	}
}