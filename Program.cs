using System;
using System.Collections.Generic;
using System.Linq;

namespace AlgorithmComplexityTheory
{
	class Program
	{
		/// <summary>
		/// Вариант 13. A = {w: w содержит ровно три 0 или ровно три 1}
		/// </summary>
		static string HW1(string input)
		{
			List<int> states = new List<int>();//последовательность состояний детерминированного конечного автомата
			int q = 0;//текущее состояние ДКА
			string ans_no = "Не допустимое слово";
			string ans_yes = "Допустимое слово";
			string ans_exeption = "Неверная входная строка";
			string ans = "";
			Dictionary<int, int[]> delta = new Dictionary<int, int[]>()
			{
				[0]	 = new[] {	1,	2 },
				[1]	 = new[] {	3,	4 },
				[2]	 = new[] {	4,	5 },
				[3]	 = new[] {	6,	7 },
				[4]	 = new[] {	7,	8 },
				[5]	 = new[] {	8,	9 },
				[6]	 = new[] { 10, 11 },
				[7]	 = new[] { 11, 12 },
				[8]	 = new[] { 12, 13 },
				[9]	 = new[] { 13, 14 },
				[10] = new[] { 10, 15 },
				[11] = new[] { 15, 16 },
				[12] = new[] { 16, 17 },
				[13] = new[] { 17, 18 },
				[14] = new[] { 18, 14 },
				[15] = new[] { 15, 19 },
				[16] = new[] { 19, 20 },
				[17] = new[] { 20, 21 },
				[18] = new[] { 21, 18 },
				[19] = new[] { 19, 22 },
				[20] = new[] { 22, 23 },
				[21] = new[] { 23, 21 },
				[22] = new[] { 22, 24 },
				[23] = new[] { 24, 23 },
				[24] = new[] { 24, 24 },
			};
			int[] admissible_states = new int[] { 6, 9, 11, 13, 16, 17, 20, 22, 23 };
			states.Add(q);
			try
			{
				int a;
				foreach (char s in input)
				{
					if (!int.TryParse(s.ToString(), out a))//если символ не удалось преобразовать
						throw new Exception(ans_exeption);
					if (a != 0 && a != 1)//если символ не входит в алфавит
						throw new Exception(ans_exeption);
					q = delta[q][a];//состояние меняется по предыдущему состоянию и входному символу
					states.Add(q);//добавляем новое состояние в конец последовательности
				}
				ans = string.Join(" ", states) + '\n';
				if (admissible_states.Contains(q))
					ans += ans_yes;
				else
					ans += ans_no;
			}
			catch (Exception ex)
			{
				ans = ex.Message;
			}
			return ans;
		}

		static void Main(string[] args)
		{
			string interrupt_symbol = "q";
			string input;
			while (true)
			{
				input = Console.ReadLine();
				if (input == interrupt_symbol)//выход из цикла консоли по управляющему символу
					break;
				Console.WriteLine(HW1(input));
			}

			//Console.WriteLine("Press any key...");
		}
	}
}
