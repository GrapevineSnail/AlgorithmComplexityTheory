using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AlgorithmComplexityTheory
{
	class Program1
	{
		static Stopwatch watch = new Stopwatch();//таймер для измерения времени выполнения программы

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
			var can_branch_read_next_symbol = new List<bool> { false }; //показывает, можем ли мы двигаться по строке далее - читать следующий символ
																		//Либо там был эпсилон-переход и мы должны остаться на уже прочитанном символе.
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
			var delta = new Dictionary<int, Dictionary<string, Dictionary<string, (string, int)>>>()//набор правил перехода
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
			int[] admissible_states = new int[] { 0, 3, 4, 8 };//допустимые состояния
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
			void IterateBranch(int branch, string a)//осуществляем ветвление по правилам перехода
			{
				if (can_branch_read_next_symbol[branch])//если эта ветка уже готова читать следующий символ - ничего не ветвим
					return;
				int q = branches_state[branch].q;//текущее состояние для данной ветки
				string stack = branches_state[branch].stack;//текущий стек для данной ветки
				string stack_top = branches_state[branch].Peek();
				bool have_child = false;//показатель того, что мы пронаследовались хотя бы раз и можно удалять входную ветку branch
				void step(string str_symbol, string stack_symbol, bool can_read)
				{
					string push_stack_symbol;
					int q_next;
					(push_stack_symbol, q_next) = delta[q][str_symbol][stack_symbol];
					var state = new State(q_next, stack);
					if (stack_symbol != e)
						state.Pop();
					state.Push(push_stack_symbol);
					branches_state.Add(state);
					can_branch_read_next_symbol.Add(can_read);
					have_child = true;
				}
				//эпсилон - это либо пустая строка, либо любой символ из алфавита
				//состояние и стек меняются по предыдущему состоянию, входному символу, символу на стеке
				if (a != e && delta[q].ContainsKey(a))
				{
					if (stack_top != e && delta[q][a].ContainsKey(stack_top))
						step(a, stack_top, true);
					if (delta[q][a].ContainsKey(e))
						step(a, e, true);
				}
				if (delta[q].ContainsKey(e))
				{
					if (stack_top != e && delta[q][e].ContainsKey(stack_top))
						step(e, stack_top, false);
					if (delta[q][e].ContainsKey(e))
						step(e, e, false);
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
				{//если "читать нечего" - передана пустая строка (обычно конец)
					if (have_child)
					{//ветвиться можем только по пустому символу. если потомок найден и он равен родителю - состояние+стек,
					 //то мы понимаем, что циклимся по одной вершине в эпсилон-переходе. тогда удаляем эту ветку и завершаем алгоритм
						while (branches_state.Count != 1
							&& branches_state[branch].q == branches_state[branches_state.Count - 1].q
							&& branches_state[branch].stack == branches_state[branches_state.Count - 1].stack)
						{//удалим одинакового родителя
							branches_state.RemoveAt(branch);
							can_branch_read_next_symbol.RemoveAt(branch);
						}//потомок единственный уникальный выйдет из цикла
					}
					//"читать нечего" - передана пустая строка (обычно конец), то останавливаем алгоритм на этом состоянии, ветку не удаляем
					can_branch_read_next_symbol[branch] = true;
				}
			}
			void StepOnInputWord(string input_symbol)
			{
				//надо для этого символа пройтись по всем веткам
				can_branch_read_next_symbol = can_branch_read_next_symbol.Select(x => x = false).ToList();
				while (can_branch_read_next_symbol.Any(x => x == false))
				{//если много эпсилон-переходов, до их делаем до тех пор, пока ветка не готова считать следующий символ
					for (int b = 0; b < can_branch_read_next_symbol.Count; b++)
					{
						if (!can_branch_read_next_symbol[b])
							IterateBranch(b, input_symbol);
					}
					PrintBranches();
				}
				//циклится по бесконечному эпсилон-переходу на
				//1010, 1011, 01010, 01011 - где для строка недопустима и есть подстрока "1+0+1" и что-то после неё
				//НКА верен - он и должен циклиться.
			}
			try
			{
				PrintBranches();
				for (int i = 0; i < input.Length; i++)
				{
					string a = input[i].ToString();
					if (a != O && a != I)//если символ не входит в алфавит
						throw new Exception(ans_exeption);
					StepOnInputWord(a);
				}
				//когда строка слова закончилась, то надо проверить, остались ли в конечном состоянии непройденные эпсилон-переходы
				//для этого надо предположить, что мы "не можем прочитать след. символ строки" ввиду того, что остались ещё ветки с эпсилонами
				StepOnInputWord(e);
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

		static string HW3(string input)
		{
			string ans_no = " False\n";
			string ans_yes = " True\n";
			string ans_exeption = " Неверная входная строка\n";
			string ans = "";
			char O = '0';
			char I = '1';
			string[] Q;//состояния
			int n;//число состояний
			char[] Sigma = new char[] { O, I };//алфавит
			Dictionary<(string, char), (string, bool)> delta =
				new Dictionary<(string, char), (string, bool)>() { };//набор правил перехода - дельта
			string q0;//начальное состояние
			string[] admissible_states_F;//допустимые состояния
			void SetDictTriple((string, char, string) triple)
			{
				string q_input = triple.Item1;
				char symbol = triple.Item2;
				string q_next = triple.Item3;
				delta[(q_input, symbol)] = (q_next, false);//пока ещё не проверенное
			}
			try
			{
				string[] M = input.Split(new string[] { "}{" }, StringSplitOptions.RemoveEmptyEntries);
				if (M.Length != 5)
					throw new Exception(ans_exeption);
				M[0] = M[0].TrimStart('{');
				M[4] = M[4].TrimEnd('}');
				foreach (var item in M)
					if (item.Contains("{") || item.Contains("}"))
						throw new Exception(ans_exeption);

				//парсим состояния
				Q = M[0].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
				n = Q.Length;

				//парсим алфавит
				string[] alphabet = M[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
				if (alphabet.Length != 2)
					throw new Exception(ans_exeption);
				char a;
				for (int i = 0; i <= 1; i++)
					if (!char.TryParse(alphabet[i], out a) || (a != O && a != I))//!Sigma.Contains(a)) //если символ не входит в алфавит
						throw new Exception(ans_exeption);

				//парсим набор правил перехода
				var Rules = M[2].Trim(new char[] { '<', '>' }).Split(new string[] { "><" }, StringSplitOptions.RemoveEmptyEntries);
				if (Rules.Length != 2 * n)
					throw new Exception(ans_exeption);
				foreach (var r in Rules)
				{
					var rr = r.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
					if (!char.TryParse(rr[1], out a) || (a != O && a != I))//!Sigma.Contains(a)) //если символ не входит в алфавит
						throw new Exception(ans_exeption);
					if (!Q.Contains(rr[0]) || !Q.Contains(rr[2]))
						throw new Exception(ans_exeption);
					if (delta.ContainsKey((rr[0], a)))
						throw new Exception(ans_exeption);
					SetDictTriple((rr[0], a, rr[2]));

				}

				//парсим начальное состояние
				if (!Q.Contains(M[3]))
					throw new Exception(ans_exeption);
				q0 = M[3];

				//парсим допустимые состояния
				admissible_states_F = M[4].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
				if (admissible_states_F.Length > n)
					throw new Exception(ans_exeption);
				foreach (var state in admissible_states_F)
				{
					if (!Q.Contains(state))
						throw new Exception(ans_exeption);
				}

				//далее основной алгоритм
				var v = q0;//текущее состояние
				int ones = 0;//счётчик единиц
				while (!delta.All(x => x.Value.Item2 == true))
				{
					var is_checked = delta[(v, O)].Item2;
					var q_next = delta[(v, O)].Item1;
					if (is_checked == false && ones % 2 == 1 && admissible_states_F.Contains(q_next))
						return ans + ans_no;
					delta[(v, O)] = (q_next, true);//пометим как проверенное правило

					is_checked = delta[(v, I)].Item2;
					q_next = delta[(v, I)].Item1;
					if (is_checked == false)
					{
						ones++;
						if (ones % 2 == 1 && admissible_states_F.Contains(q_next))
							return ans + ans_no;
					}
					delta[(v, I)] = (q_next, true);//пометим как проверенное правило
					v = q_next;
				}
				ans += ans_yes;
			}
			catch (Exception ex)
			{
				ans = ex.Message;
			}
			return ans;
		}

		static string HW4(string filename)
		{
			string ans_no = " False\n";
			string ans_yes = " True\n";
			string ans_exeption = " Неверный ввод\n";
			string ans = "";
			int n;//число вершин
			int[,] M;//матрица смежности графа
			try
			{
				//чтение из файла
				string[] lines = File.ReadAllLines("HW4/" + filename).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
				ans += string.Join('\n', lines) + '\n';
				n = lines.First().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Count();
				if (n == 0)
					throw new ArgumentException("Пустой файл");
				M = new int[n, n];
				for (int i = 0; i < n; i++)
				{
					string[] numbers = lines[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
					if (numbers.GetLength(0) != n)
						throw new ArgumentException("Неверное количество символов");
					int res;
					for (int j = 0; j < n; j++)
					{
						if (int.TryParse(numbers[j], out res))
							M[i, j] = res;
						else
							throw new ArgumentException("Неверный символ");
						if (i > j && M[i, j] != M[j, i])
							throw new ArgumentException("Матрица не симметричная");
					}
				}

				//основной алгоритм
				Queue<int> Q = new Queue<int>();//очередь для работы алгоритма (BFS)
				int[] colors = new int[n];
				for (int i = 0; i < n; i++)
				{
					if (M[i, i] == 1)
					{//когда есть петля
						ans += ans_no;
						return ans;
					}
					colors[i] = 0;
				}
				int point = 0;//индекс остановки в массиве цветов
				int v;
				while (point < n)
				{
					if (colors[point] == 0)
					{
						colors[point] = 1;
						Q.Enqueue(point);
						while (Q.Count() > 0)
						{
							v = Q.Dequeue();
							var col = (colors[v] == 1) ? 2 : 1;
							for (int j = 0; j < n; j++)
							{
								if (M[v, j] == 1)
								{
									if (colors[j] == 0)
									{
										colors[j] = col;
										Q.Enqueue(j);
									}
									else if ((colors[j] == 1 && col == 2) || (colors[j] == 2 && col == 1))
									{
										ans += ans_no;
										return ans;
									}
								}
							}
						}
					}
					else
						point++;
				}
				ans += ans_yes;
			}
			catch (Exception ex)
			{
				ans = ex.Message;
			}
			return ans;
		}

		static string HW5(string filename)
		{
			string ans_no = " False\n";
			string ans_yes = " True\n";
			string ans_exeption = " Неверный ввод\n";
			string ans = "";
			int n;//число вершин
			int[,] M;//матрица смежности графа
			int[] parse_line(string line)
			{
				string[] numbers_str = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
				int[] numbers_int = new int[numbers_str.GetLength(0)];
				for (int j = 0; j < numbers_int.GetLength(0); j++)
				{
					if (!int.TryParse(numbers_str[j], out numbers_int[j]))
						throw new ArgumentException("Неверный символ");
				}
				return numbers_int;
			}
			try
			{
				//чтение матрицы из файла
				string[] lines = File.ReadAllLines("HW5/" + filename).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
				ans += string.Join('\n', lines) + '\n';
				n = parse_line(lines.First()).Count();
				if (n == 0)
					throw new ArgumentException("Пустой файл");
				M = new int[n, n];
				for (int i = 0; i < n; i++)
				{
					int[] numbers = parse_line(lines[i]);
					if (numbers.GetLength(0) != n)
						throw new ArgumentException("Неверное количество символов");
					for (int j = 0; j < n; j++)
					{
						M[i, j] = numbers[j];
						if (i > j && M[i, j] != M[j, i])
							throw new ArgumentException("Матрица не симметричная");
					}
				}


				//чтение списка удаляемых вершин
				int[] certificate = parse_line(Console.ReadLine()).Select(x => x - 1).ToArray();

				//основной алгоритм
				var M_new = new int[n - certificate.Length, n - certificate.Length];
				int[] colors = Enumerable.Repeat(0, n).ToArray();
				int ii = 0;
				for (int i = 0; i < n; i++)
					if (!certificate.Contains(i))
					{
						int jj = 0;
						for (int j = 0; j < n; j++)
							if (!certificate.Contains(j))
							{
								M_new[ii, jj] = M[i, j];
								jj++;
							}
						ii++;
					}

				M = M_new;
				n = M.GetLength(0);

				bool dfs(int v, int predecessor)
				{
					bool ans = true;
					colors[v] = 1;
					for (int j = 0; j < n; j++)
						if (M[v, j] > 0 && j != predecessor)
						{
							if (colors[j] != 1)
								ans = dfs(j, v);
							else//есть цикл
								ans = false;
							if (ans == false)
								break; 
						}
					colors[v] = 2;
					return ans;
				}

				watch.Restart();

				int point = 0;//индекс остановки в массиве цветов
				while (point < n)
				{
					if (colors[point] == 0)
					{
						if (dfs(point, -1) == false)
						{
							ans += ans_no;
							return ans;
						}
					}
					else
						point++;
				}
				ans += ans_yes;
			}
			catch (Exception ex)
			{
				ans = ans_exeption + ex.Message;
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

				watch.Restart();
				Console.WriteLine(f(input));
				watch.Stop();
				Console.WriteLine($"Время выполнения: {watch.Elapsed.TotalSeconds} сек.\n");
			}
		}

		static void Main(string[] args)
		{
			//ConsoleInputCycle("Вариант 13. A = { w: w содержит ровно три 0 или ровно три 1 }", HW1);
			//ConsoleInputCycle("Вариант 13. B = {w: w содержит подстроку 01 столько же раз, сколько в начале w расположено 0}", HW2);
			//ConsoleInputCycle("Вариант 13. A = {<M> : M – ДКА, который не допускает строки, содержащие нечётное число 1}", HW3);
			//ConsoleInputCycle("Вариант 13. Проверка, двудольный ли граф.", HW4);
			ConsoleInputCycle("Вариант 13. Верификация графа: проверка на цикличность.", HW5);

			//Console.WriteLine("Press any key...");
		}
	}
}
