using System;
using System.Collections.Generic;
using System.Linq;

namespace AlgorithmComplexityTheory
{
	class Program1
	{
		static string HW1(string input)
		{
			int q = 0;//текущее состояние ДКА
			string ans_no = "\nНе допустимое слово";
			string ans_yes = "\nДопустимое слово";
			string ans_exeption = "\nНеверная входная строка";
			string ans = "";
			Dictionary<int, int[]> delta = new Dictionary<int, int[]>()
			{
				[0] = new[] { 1, 2 },
				[1] = new[] { 3, 4 },
				[2] = new[] { 4, 5 },
				[3] = new[] { 6, 7 },
				[4] = new[] { 7, 8 },
				[5] = new[] { 8, 9 },
				[6] = new[] { 10, 11 },
				[7] = new[] { 11, 12 },
				[8] = new[] { 12, 13 },
				[9] = new[] { 13, 14 },
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
			Console.Write(q.ToString() + ' ');
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
					Console.Write(q.ToString() + ' ');
				}
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

		static string HW2(string input)
		{
			int q = 0;//текущее состояние НКА
			string ans_no = "\nНе допустимое слово";
			string ans_yes = "\nДопустимое слово";
			string ans_exeption = "\nНеверная входная строка";
			string ans = "";
			string e = "";
			string H = "#";
			string O = "0";
			string I = "1";
			Dictionary<string, Dictionary <string, (string, int)>> SetDictItem((string, string, string, int)[] edge)
			{
				var d = new Dictionary<string, Dictionary<string, (string, int)>> { };
				foreach ((string, string, string, int) t in edge)
				{
					string wi_input = t.Item1;
					string stack_top = t.Item2;
					string stack_push = t.Item3;
					int q_next = t.Item4;
					d[wi_input] = new Dictionary<string, (string, int)> { [stack_top] = (stack_push, q_next) };
				}
				return d;
			}
			Dictionary<int, Dictionary<string, Dictionary <string, (string, int)>>> delta = new Dictionary<int, Dictionary<string, Dictionary<string, (string, int)>>>()
			{
				[0] = SetDictItem(new[] { (e, e, H, 1) }),
				[1] = SetDictItem(new[] { (O, e, O, 2), (I, e, I, 3) }),
				[2] = SetDictItem(new[] { (O, e, O, 2), (I, O, e, 6) }),
				[3] = SetDictItem(new[] { (I, I, I, 3), (O, I, e, 4) }),
				[4] = SetDictItem(new[] { (O, H, H, 4), (I, H, H, 5) }),
				[5] = SetDictItem(new[] { (e, e, e, 5) }),
				[6] = SetDictItem(new[] { (I, O, O, 6), (O, O, O, 7), (e, H, H, 8) }),
				[7] = SetDictItem(new[] { (O, O, O, 7), (I, O, e, 6) }),
				[8] = SetDictItem(new[] { (O, H, H, 4), (I, e, I, 3) })
			};
			int[] admissible_states = new int[] { 0, 3, 4, 8 };
			//для создания разных веток
			List<Stack<string>> stack_list = new List<Stack<string>>() { new Stack<string>() { } };
			List<int> q_list = new List<int>() { 0 };
			void WriteState(int b)
			{
				Console.Write($"{b}=={q_list[b]}:{string.Join("_", stack_list[b].Select(x => x.ToString()).ToArray())}\n");
			}
			void next_branch(int branch, string a)
			{
				var q = q_list[branch];
				var stack = stack_list[branch];
				string stack_top = stack.Count != 0 ? stack.Peek() : "abracadabra";
				string stack_push;
				Stack<string> sb;
				int qb;
				void cur_branch(Stack<string> sb)
				{
					sb.Push(stack_push);
					q_list[branch] = qb;
					stack_list[branch] = sb;
				}
				void new_branch(Stack<string> sb)
				{
					sb.Push(stack_push);
					q_list.Add(qb);
					stack_list.Add(sb);
				}
				//эпсилон - это либо пустая строка, либо любой символ из алфавита
				//состояние и стек меняются по предыдущему состоянию, входному символу, символу на стеке
				var dqa = delta[q].ContainsKey(a) ? delta[q][a] : null;
				bool avs = dqa != null ? dqa.ContainsKey(stack_top) : false;
				bool ave = dqa != null ? dqa.ContainsKey(e) : false;
				var dqe = delta[q].ContainsKey(e) ? delta[q][e] : null;
				bool evs = dqe != null ? dqe.ContainsKey(stack_top) : false;
				bool eve = dqe != null ? dqe.ContainsKey(e) : false;
				if (avs)
				{
					sb = stack;
					(stack_push, qb) = dqa[sb.Pop()];
					cur_branch(sb);
					if (ave)
					{
						sb = stack;
						(stack_push, qb) = dqa[e];
						new_branch(sb);
					}
					if (evs)
					{
						sb = stack;
						(stack_push, qb) = dqe[sb.Pop()];
						new_branch(sb);
					}
					if (eve)
					{
						sb = stack;
						(stack_push, qb) = dqe[e];
						new_branch(sb);
					}
				}
				else if (ave)
				{
					sb = stack;
					(stack_push, qb) = dqa[e];
					cur_branch(sb);
					if (avs)
					{
						sb = stack;
						(stack_push, qb) = dqa[sb.Pop()];
						new_branch(sb);
					}
					if (evs)
					{
						sb = stack;
						(stack_push, qb) = dqe[sb.Pop()];
						new_branch(sb);
					}
					if (eve)
					{
						sb = stack;
						(stack_push, qb) = dqe[e];
						new_branch(sb);
					}
				}
				else if (evs)
				{
					sb = stack;
					(stack_push, qb) = dqe[sb.Pop()];
					cur_branch(sb);
					if (avs)
					{
						sb = stack;
						(stack_push, qb) = dqa[sb.Pop()];
						new_branch(sb);
					}
					if (eve)
					{
						sb = stack;
						(stack_push, qb) = dqe[e];
						new_branch(sb);
					}
					if (eve)
					{
						sb = stack;
						(stack_push, qb) = dqe[e];
						new_branch(sb);
					}
				}
				else if (eve)
				{
					sb = stack;
					(stack_push, qb) = dqe[e];
					cur_branch(sb);
					if (avs)
					{
						sb = stack;
						(stack_push, qb) = dqa[sb.Pop()];
						new_branch(sb);
					}
					if (ave)
					{
						sb = stack;
						(stack_push, qb) = dqa[e];
						new_branch(sb);
					}
					if (evs)
					{
						sb = stack;
						(stack_push, qb) = dqe[sb.Pop()];
						new_branch(sb);
					}
				}
				/*
				//if (delta[q].ContainsKey(a) && !delta[q].ContainsKey(e))
				//{
				//	var dqa = delta[q][a];
				//	bool v1 = dqa.ContainsKey(stack_top);
				//	bool v2 = dqa.ContainsKey(e);
				//	if (v1 && !v2)
				//	{
				//		sb = stack;
				//		(stack_push, qb) = dqa[sb.Pop()];
				//		cur_branch(sb);
				//	}
				//	else if (!v1 && v2)
				//	{
				//		sb = stack;
				//		(stack_push, qb) = dqa[e];
				//		cur_branch(sb);
				//	}
				//	else if(v1 && v2)
				//	{
				//		sb = stack;
				//		(stack_push, qb) = dqa[sb.Pop()];
				//		cur_branch(sb);
				//		sb = stack;
				//		(stack_push, qb) = dqa[e];
				//		new_branch(sb);
				//	}
				//}
				//else if(!delta[q].ContainsKey(a) && delta[q].ContainsKey(e))
				//{
				//	var dqe = delta[q][e];
				//	bool v1 = dqe.ContainsKey(stack_top);
				//	bool v2 = dqe.ContainsKey(e);
				//	if (v1 && !v2)
				//	{
				//		sb = stack;
				//		(stack_push, qb) = dqe[sb.Pop()];
				//		cur_branch(sb);
				//	}
				//	else if (!v1 && v2)
				//	{
				//		sb = stack;
				//		(stack_push, qb) = dqe[e];
				//		cur_branch(sb);
				//	}
				//	else if (v1 && v2)
				//	{
				//		sb = stack;
				//		(stack_push, qb) = dqe[sb.Pop()];
				//		cur_branch(sb);
				//		sb = stack;
				//		(stack_push, qb) = dqe[e];
				//		new_branch(sb);
				//	}
				//}
				//else if (delta[q].ContainsKey(a) && delta[q].ContainsKey(e))
				//{

				//}
				*/
			}
			try
			{
				//Console.Write(q.ToString() + ' ');
				WriteState(0);
				int i = 0;
				while (i < input.Length)
				{
					string a = input[i].ToString();
					if (a != O && a != I)//если символ не входит в алфавит
						throw new Exception(ans_exeption);
					int b = 0;
					while (b < q_list.Count)
					{
						next_branch(b, a);
						WriteState(b);
						b++;
					}
					i++;
				}
				for (int b = 0; b < q_list.Count; b++)
				{
					if (admissible_states.Contains(q_list[b]))
						ans += ans_yes;
					else
						ans += ans_no;
				}
			}
			catch (Exception ex)
			{
				ans = ex.Message;
			}
			return ans;
		}

		/// <summary>
		/// Запуск цикла считывания строк с консоли. Каждая строка обрабатывается функцией, передаваемой в параметрах
		/// </summary>
		/// <param name="invite"></param>
		/// <param name="f"></param>
		static void ConsoleInputCycle(string invite, Func<string, string> f)
		{
			string interrupt_symbol = "q";
			Console.WriteLine("Для выхода введите " + interrupt_symbol);
			string input;

			Console.WriteLine(invite);
			while (true)
			{
				input = Console.ReadLine();
				if (input == interrupt_symbol)//выход из цикла консоли по управляющему символу
					break;
				Console.WriteLine(f(input));
			}
		}

		static void Main(string[] args)
		{
			//ConsoleInputCycle("Вариант 13. A = { w: w содержит ровно три 0 или ровно три 1 }", HW1);
			ConsoleInputCycle("Вариант 13. B = {w: w содержит подстроку 01 столько же раз, сколько в начале w расположено 0}", HW2);

			//Console.WriteLine("Press any key...");
		}
	}
}
