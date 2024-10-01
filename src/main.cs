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

public class main {
	public static void Main() {
		GameManager gm = new GameManager();
		gm.GameStart();
		
		return;
	}
}