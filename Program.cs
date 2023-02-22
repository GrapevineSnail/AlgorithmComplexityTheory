using System;
using System.Collections.Generic;

namespace AlgorithmComplexityTheory
{
	class Program
	{
		/// <summary>
		/// Вариант 13. A = {w: w содержит ровно три 0 или ровно три 1}
		/// </summary>
		static string HW1(string input)
		{
			List<int> states = new List<int>() { 0 };//последовательность состояний детерминированного конечного автомата
			HashSet<int> Q = new HashSet<int>() { 0, 1, 2 };//состояния
			HashSet<int> Sigma = new HashSet<int>() { 0, 1 };//алфавит
			for (int i = 0; i < input.Length; i++)
			{

			}
			return string.Join(" ", states);
		}

		static void Main(string[] args)
		{
			string s = Console.ReadLine();
			Console.WriteLine(HW1(s));

			//Console.WriteLine("Press any key...");
		}
	}
}
