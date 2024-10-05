using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Drawing;
using SnakeGame;
using Celestial;

public class Person
{
	public int Age {
		get {
			return age;
		}
		set {
			if(value >= 0) age = value;
		}
	}
	public string Name {
		get {
			return name;
		}
		set {
			Regex regex = new Regex(@"^[가-힣a-zA-Z\s]+$");
			if(regex.IsMatch(value)) name = value;
			else name = "NONAME";
		}
	}
	private string job = "inoccupation";
	
	public Guid Id { get; set; } = Guid.NewGuid();
	
	// public string Job
	// {
	// 	get => job;
	// 	set => job = value;
	// }

	
	public void SayHi() => Console.WriteLine("Hello!");
	
	private int age;
	private string name;
	
	public Person(string name = "", int age = 0)
	{
		this.name = name;
		this.age = age;
	}
	
	public override string ToString()
	{
		return name + ", " + age + " years old. id: " + Id;
	}
	
	public string Message { get; private set; } = "Readonly property";
	
}

public class main {
	public static void Main() {
		// GroupBy
		
		var data = new { Id = 1, Name = "AnonymousType"};
		Console.WriteLine($"{data.Id}, {data.Name}");
		Console.WriteLine($"{data.GetType()}");
		
		var anonys = new[] {
			new { Name = "qwer", Age = 20 },
			new { Name = "asdf", Age = 21 },
			new { Name = "zxcv", Age = 22 }
		};
		
		foreach(var item in anonys) Console.WriteLine($"{item.Name}, {item.Age}");
		
		// Duck Typing
		var duck = new { Id = 1, Name = "Quack" };
		Console.WriteLine($"{duck.Id}, {duck.Name}");
		// duck = new { Id = 3.14, Wing = "Double wing" }; // error - duck typing
		
		
		return;
	}
	
	public static Func<int,bool> isEven = x=>x%2==0;
	public static Action<string> greet = name =>{ var msg = $"Hello {name}"; Console.WriteLine(msg); };
	
	public static void ZipTest()
	{
		int[] numbers = {1,2,3};
		string[] words = {"one","two"};
		var numsAndWords = numbers.Zip(words, (first, second) => first+"-"+second);
		numsAndWords.ToList().ForEach(n => Console.Write($"{n}, "));
		Console.WriteLine();
	}
	
	public static void ForEachTest()
	{
		var numbers = new List<int>() { 1,2,43,54,6,7,9 };
		numbers.Where(n => n%2==0).ToList().ForEach(n => Console.Write($"{n}, "));
		Console.WriteLine();
	}
	
	public static void SelectTest()
	{
		int[] numbers = {5,-3,1,2,3,4,5,6};
		var squares = numbers.Select(n => n*n);
		var realNum = numbers.Select(n => (double)n+0.25d);
		foreach(var item in squares) Console.WriteLine(item);
		foreach(var item in realNum) Console.WriteLine(item);
		
		var names = new List<string> { "qwer", "vsad", "qwefew", "rehgr"};
		var nameObjects = names.Select(n => new {Namee = n+"e"});
		
		foreach(var name in nameObjects) Console.WriteLine(name);
		Console.WriteLine("*****");
		foreach(var name in nameObjects) Console.WriteLine(name.Namee);
		
	}
	
	public static void QueryTest()
	{
		var numbers = Enumerable.Range(-10,20);
		var newNumbers = numbers.Where(n => n%2==0).OrderByDescending(n => n*n);
		foreach(var item in newNumbers) Console.WriteLine(item);
		Console.WriteLine("-----");
		var nums = from n in numbers where n%2==0 orderby n descending select n*n;
		foreach(var item in nums) Console.WriteLine(item);
		Console.WriteLine("-----");
		
		var evens = from n in numbers where n%2==0 select n;
		foreach(var item in evens) Console.WriteLine(item);
		Console.WriteLine($"Sum: {evens.Sum()}");
		Console.WriteLine($"Avg: {evens.Average()}");
		Console.WriteLine("-----");
		
		Console.WriteLine(evens);
		
		Person[] people = {
			new Person("Anne", 18),
			new Person("Bob", 15),
			new Person("Eve", 20),
			new Person("Argenne", 17),
			new Person("grtBob", 10),
			new Person("Evefd", 23)
		};
		
		var sortedPeopleName = from p in people where p.Age>15 orderby p.Age descending select p.Name;
		foreach(var item in sortedPeopleName) Console.WriteLine(item);
		
	}
	
	public static void SingleFirstTest()
	{
		List<string> colors = new List<string>{"Red", "Green", "Blue"};
		string red = colors.Single(c => c=="Red");
		Console.WriteLine(red);
		string yellow = colors.SingleOrDefault(c => c=="Yellow");
		Console.WriteLine(yellow);
		
		string[] name = {"asdf", "qewr", "vds", "vdsV", "wgewge"};
		string v = name.First(n => n.ToUpper().Contains("V"));
		Console.WriteLine(v);
		string z = name.FirstOrDefault(n => n.ToUpper().Contains("Z"));
		Console.WriteLine(z);
	}
	
	public static void ContainsTest()
	{
		string[] colors = {"Red", "Green", "Blue"};
		IEnumerable<string> sortedColors = colors.OrderBy(name => name);
		var sortedColorsDesc = colors.OrderByDescending(name => name);
		foreach(var color in sortedColors) Console.WriteLine(color);
		Console.WriteLine("------");
		foreach(var color in sortedColorsDesc) Console.WriteLine(color);
		Console.WriteLine("------");
		List<string> names = new List<string>() {
			"asdf", "qdf", "vdfgr", "rgrthtr", "vcd", "arefgr", "df"
		};
		var results = names.Where(n => n.Length > 3).OrderBy(name => name);
		foreach(var item in results) Console.WriteLine(item);
		Console.WriteLine("------");
		var datas = Enumerable.Range(1,20);
		var res = datas.Where(d => d%2==0).OrderByDescending(d => d);
		foreach(var item in res) Console.WriteLine(item);
		Console.WriteLine("------");
		var newNames = names.Where(n => n.ToUpper().Contains("D"));
		foreach(var item in newNames) Console.WriteLine(item);
		
	}
	
	public static void DistinctTest()
	{
		var data = Enumerable.Repeat(3,5);
		var result = data.Distinct();
		foreach(var item in result) Console.Write($"{item}, ");
		Console.WriteLine();
		
		int[] arr = {2,2,3,3,3};
		var dst = arr.Distinct();
		foreach(var item in dst) Console.Write($"{item}, ");
		Console.WriteLine();
	}
	
	public static void TakeSkip()
	{
		var data = Enumerable.Range(0,100); // 0~99
		foreach(var item in data.Take(5)) Console.Write($"{item}, ");
		Console.WriteLine();
		var dtake = data.Take(5);
		Console.WriteLine(dtake.GetType());
		
		var detake = data.Where(d => d%2==0).Take(5);
		foreach(var item in detake) Console.Write($"{item}, ");
		Console.WriteLine();
		Console.WriteLine(detake.GetType());
		
		foreach(var item in data.Skip(10).Take(5)) Console.Write($"{item}, ");
		Console.WriteLine();
	}
	
	public static void AnyAll()
	{
		bool[] completes = { true, true, true };
		Console.WriteLine(completes.All(c=> c==true));
		int[] numbers = {1,5,6,4,58,9,8,5,3};
		Console.WriteLine(numbers.Any(n => n%97==0));
		Console.WriteLine(numbers.Any(n => n==6));
		var dct = new Dictionary<int, string>() {
			{1, "st"},
			{2, "nd"},
			{3, "rd"},
			{4, "th"}
		};
		
		int a = 5;
		Console.WriteLine($"{nameof(dct)} has {a}? -> {dct.ContainsKey(a)}");
	}
	public static void whereTest2()
	{
		var numbers = new List<int> {1,2,3,4,5};
		int evenSum = numbers.Where(x => x%2==0).Sum();
		int oddSum = numbers.Where(x => x%2!=0).Sum();
		Console.WriteLine($"even numbers sum: {evenSum}");
		Console.WriteLine($"odd numbers sum: {oddSum}");
	
		Console.WriteLine(numbers.Where(x => x%2==0).GetType());
		
		int[] arr = {1,2,3,4,5,6,7};
		var nums = arr.Where(a => a%2==0 && a>3);
		foreach(var item in nums) Console.Write($"{item}, ");
		Console.WriteLine();
		
	}
	public static void whereTest()
	{
		int[] numbers = {1,2,3,4,5};
		IEnumerable<int> newNumbers = numbers.Where(number => number > 3);
		foreach(var item in newNumbers) Console.Write($"{item}, ");
		Console.WriteLine();
		List<int> newNumbersList = newNumbers.ToList();
		foreach(var item in newNumbersList) Console.Write($"{item}, ");
		Console.WriteLine();
		
	}
	
	public static void tmp()
	{
		int[] numbers = {1,2,3,5};
		int sum = numbers.Sum();
		Console.WriteLine(sum);
		int cnt = numbers.Count();
		Console.WriteLine(cnt);
		Console.WriteLine($"{nameof(numbers)}'s cnt: {cnt}");
		
		double average = numbers.Average();
		Console.WriteLine($"{nameof(numbers)}'s avg: {average}");
		
		var nums = new List<int>() { 1,2,3,4,56 };
		int max = numbers.Max();
		int min = numbers.Min();
		Console.WriteLine($"{nameof(nums)}'s max: {max}");
		Console.WriteLine($"{nameof(nums)}'s min: {min}");
		
		Console.WriteLine(main.isEven(4));
		Console.WriteLine(main.isEven(3));
		main.greet("Anne");
		
	}
}