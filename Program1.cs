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

		/// <summary>
		/// для HW2
		/// </summary>
		class State
		{
			public State(int Q, string Stack) { q = Q; stack = Stack; }
			public int q;
			public string stack;
			public string Peek()
			{
				return stack.Length != 0 ? stack.Last().ToString() : "";
			}
			public string Pop()
			{
				var ans = Peek();
				stack = stack.Length != 0 ? stack.Remove(stack.Length - 1) : "";
				return ans;
			}
			public void Push(string end)
			{
				stack += end;
			}
		}
		static string HW2(string input)
		{
			string ans_no = " Не допустимое слово\n";
			string ans_yes = " Допустимое слово\n";
			string ans_exeption = " Неверная входная строка\n";
			string ans = "";
			string e = "";
			string H = "#";
			string O = "0";
			string I = "1";
			//для создания разных веток, пары состояние-стек
			var branches_state = new List<State> { new State(0, "") };
			var can_branch_read_next_symbol = new List<bool> { false }; //показывает, можем ли мы двигаться по строке далее
																		//Либо там был эпсилон-переход и мы должны остаться на чтении того же символа.
			Dictionary<string, Dictionary<string, (string, int)>> SetDictItem((string, string, string, int)[] edge)
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
			var delta = new Dictionary<int, Dictionary<string, Dictionary<string, (string, int)>>>()
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
			void PrintBranch(int b)
			{
				Console.WriteLine($"Ветка_{b}>> {branches_state[b].q}:{string.Join("", branches_state[b].stack.Select(x => x.ToString()).ToArray())}");
			}
			void PrintBranches()
			{
				for (int b = 0; b < branches_state.Count; b++)
					PrintBranch(b);
				Console.WriteLine();
			}
			void IterateBranch(int branch, string a)
			{
				if (can_branch_read_next_symbol[branch])//если эта ветка уже готова читать следующий символ - ничего не ветвим
					return;
				int q = branches_state[branch].q;//текущее состояние для данной ветки
				string stack = branches_state[branch].stack;//текущий стек для данной ветки
				string stack_top = branches_state[branch].Peek();
				string stack_push;
				int q_next;
				bool have_child = false;//показатель того, что мы пронаследовались хотя бы раз и можно удалять входную ветку branch
										//эпсилон - это либо пустая строка, либо любой символ из алфавита
										//состояние и стек меняются по предыдущему состоянию, входному символу, символу на стеке
				void step_with_pop(string symbol_str, string symbol_stack, bool can_read)
				{
					var dq = delta[q][symbol_str];
					(stack_push, q_next) = dq[symbol_stack];
					var st = new State(q_next, stack);
					st.Pop();
					st.Push(stack_push);
					branches_state.Add(st);
					can_branch_read_next_symbol.Add(can_read);
					have_child = true;
				}
				void step_no_pop(string symbol_str, string symbol_stack, bool can_read)
				{
					var dq = delta[q][symbol_str];
					(stack_push, q_next) = dq[symbol_stack];
					var st = new State(q_next, stack);
					st.Push(stack_push);
					branches_state.Add(st);
					can_branch_read_next_symbol.Add(can_read);
					have_child = true;
				}
				if (a != e && delta[q].ContainsKey(a))
				{
					if (stack_top != e && delta[q][a].ContainsKey(stack_top))
						step_with_pop(a, stack_top, true);
					if (delta[q][a].ContainsKey(e))
						step_no_pop(a, e, true);
				}
				if (delta[q].ContainsKey(e))
				{
					if (stack_top != e && delta[q][e].ContainsKey(stack_top))
						step_with_pop(e, stack_top, false);
					if (delta[q][e].ContainsKey(e))
						step_no_pop(e, e, false);
				}
				if (a != e)
				{//прочитали нормальный символ
					if (have_child)
					{//когда разветвили, вышли из состояния, забываем предыдущее сосотояние
						branches_state.RemoveAt(branch);
						can_branch_read_next_symbol.RemoveAt(branch);
					}
					else
					{//если не нашли правил, по которым ветвиться дальше, считаем узел обработанным
						can_branch_read_next_symbol[branch] = true;
					}
				}
				else
				{//если "читать нечего" - передана пустая строка, то останавливаем алгоритм на этом состоянии, ветку не удаляем
					can_branch_read_next_symbol[branch] = true;
				}
			}

			//List<State> prev_branches_state = branches_state;
			//List<bool> prev_can_branch_read_next_symbol = can_branch_read_next_symbol;
			void StepOnInput(string input_symbol)
			{
				//надо для этого символа пройтись по всем веткам
				bool go_out = false;//прерывание циклов
				while (can_branch_read_next_symbol.Any(x => x == false) && !go_out)
				{//если много эпсилон-переходов, до их делаем до тех пор, пока ветка не готова считать следующий символ
					for (int b = 0; b < can_branch_read_next_symbol.Count; b++)
					{
						if (!can_branch_read_next_symbol[b])
							IterateBranch(b, input_symbol);
						//тут по-хорошему надо обрабатывать любые возможные повторяющиеся циклы - хождения по кругу любой длины.
						//Это задача сложная, на графы, поэтому обрабатываю только "хождение в одной вершине"
						if (branches_state.Count > 1 && !branches_state[branches_state.Count - 1].Equals(branches_state[branches_state.Count - 2]))
						{
							go_out = true;
							break;
						}
					}
					PrintBranches();
					//Не обрабатывается случай зацикливания, когда ничего не изменяется за проход, трудно отловить - тест 1011.
					//if (prev_branches_state.Equals(branches_state) && prev_can_branch_read_next_symbol.Equals(can_branch_read_next_symbol))
					//{
					//	go_out = true;
					//	break;
					//}
					//prev_branches_state = branches_state;
					//prev_can_branch_read_next_symbol = can_branch_read_next_symbol;
				}
			}
			try
			{
				PrintBranches();
				for (int i = 0; i < input.Length; i++)
				{
					string a = input[i].ToString();
					if (a != O && a != I)//если символ не входит в алфавит
						throw new Exception(ans_exeption);
					StepOnInput(a);
					can_branch_read_next_symbol = can_branch_read_next_symbol.Select(x => x = false).ToList();
				}
				//когда строка слова закончилась, то надо проверить, остались ли в конечном состоянии непройденные эпсилон-переходы
				//для этого надо предположить, что мы "не можем прочитать след. символ строки" ввиду того, что остались ещё ветки с эпсилонами
				StepOnInput(e);
				for (int b = 0; b < branches_state.Count; b++)
				{
					ans += $"Ветка_{b}>>";
					if (admissible_states.Contains(branches_state[b].q))
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
