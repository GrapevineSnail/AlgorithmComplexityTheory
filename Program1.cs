using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AlgorithmComplexityTheory
{
	class Program1
	{
		static Stopwatch watch = new Stopwatch();//таймер для измерения времени выполнения программы
		static string interrupt_symbol = "q";//символ для выхода из программы

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
		/// для HW6
		/// </summary>
		class BoardState : ICloneable
		{
			private BoardState(int[,] matrix)
			{
				var n = matrix.GetLength(0);
				var m = matrix.GetLength(1);
				Board = new int[n, m];
				for (int i = 0; i < n; i++)
					for (int j = 0; j < m; j++)
						Board[i, j] = matrix[i, j];
			}
			public BoardState(int n, int m, List<(int, int)> used_coords, int empty_sign)
			{
				EMPTY = empty_sign;
				Board = new int[n, m];
				for (int i = 0; i < n; i++)
					for (int j = 0; j < m; j++)
						Board[i, j] = EMPTY;
				empty_cells_cnt = 0;
				foreach ((int i, int j) crd in used_coords)
					if (Board[crd.i, crd.j] == EMPTY)
						empty_cells_cnt += 1;
			}
			public int[,] Board;                                       //матрица поля
			public int empty_cells_cnt;                                //количество пустых клеток
			public int who_made_the_move;                                       //какой игрок сделал ход в этой доске
			public (int i, int j) move;                                         //куда игрок пошёл
			public BoardState parent = null;                                    //предыдущее состояние игровой доски
			public List<BoardState> children = new List<BoardState>();          //следующие состояния игровой доски
			public double win_proportion;                                       //доля выигрышей
			public double draw_proportion;                                      //доля ничьих
			public double losing_proportion;                                    //доля проигрышей
			public bool is_terminating = false;                                 //является ли состояние завершающим игру
			int EMPTY;                                                          //обозначает пустую клетку


			public List<(int, int)> EmptyCells(List<(int, int)> used_coords)
			{
				List<(int, int)> ans = new List<(int, int)>();
				foreach ((int i, int j) crd in used_coords)
					if (Board[crd.i, crd.j] == EMPTY)
						ans.Add(crd);
				return ans;
			}

			public bool Is_equivalent(BoardState other, List<(int, int)> used_coords)
			{
				foreach ((int i, int j) crd in used_coords)
					if (Board[crd.i, crd.j] != other.Board[crd.i, crd.j])
						return false;
				return true;
			}

			public object Clone()
			{
				var ans = new BoardState(Board);
				ans.empty_cells_cnt = empty_cells_cnt;
				ans.who_made_the_move = who_made_the_move;
				ans.move = move;
				ans.win_proportion = win_proportion;
				ans.draw_proportion = draw_proportion;
				ans.losing_proportion = losing_proportion;
				ans.is_terminating = is_terminating;
				ans.EMPTY = EMPTY;
				return ans;
			}
		}
		static string HW6(string _)
		{
			/*
			  | 0 | 1 | 2 | 3 | 4 |
			-     +---+---+---+
			0     |   |   |   |
			- +---+---+---+---+---+
			1 |   |   |   |   |   |
			- +---+---+---+---+---+
			2     |   |   |   |
			-     +---+---+---+
			*/
			string ans_exeption = " Неверный ввод\n";
			string ans_end = " Игра окончена\n";
			string ans = "";
			int n = 3;//количество строк поля
			int m = 5;//количество столбцов поля
			List<(int, int)> used_coords = new List<(int, int)>(); //употребляемые координаты
			for (int i = 0; i < n; i++)
				for (int j = 0; j < m; j++)
					used_coords.Add((i, j));
			//var deleted_cells = new List<(int, int)> { (0, 0), (0, 4), (2, 0), (2, 4) };
			var deleted_cells = new List<(int, int)> { (0, 0), (0, 4), (2, 0), (2, 4), (1,0), (1,4)
			,(2,1),(2,2),(2,3)};
			used_coords = used_coords.Except(deleted_cells).ToList();
			const int EMPTY = 0; //обозначение пустой клеточки в матрице
			const int PLAYER1 = 1; // обозначает знак (крестик, нолик и т.д.) игрока
			const int PLAYER2 = 2;
			BoardState current_state_in_tree; //текущее место в дереве в процессе игры

			int other_player(int s)//вернет знак другого игрока, не того, кто в аргументе
			{
				return s % 2 + 1;
			}

			BoardState MakePlayTree()
			{
				List<(int, int)[]> get_lines(int i, int j)//список линий из трёх клеток в которых участвует клектка i,j
				{
					var existing_lines = new List<(int, int)[]>();
					bool is_line_exist((int, int)[] line)
					{
						foreach (var crd in line)
							if (!used_coords.Contains(crd))
								return false;
						return true;
					}
					for (int k = 0; k < 3; k++)
					{
						var vertical = new (int, int)[] {
							(i, j - 2 + k),
							(i, j - 1 + k),
							(i, j + k    ) };// это |
						var horizontal = new (int, int)[] {
							(i - 2 + k, j),
							(i - 1 + k, j),
							(i + k,     j) };// это -
						var diagonal_backslash = new (int, int)[] {
							(i - 2 + k, j - 2 + k),
							(i - 1 + k, j - 1 + k),
							(i + k,     j + k    ) };// это \
						var diagonal_slash = new (int, int)[] {
							(i + 2 - k, j - 2 + k),
							(i + 1 - k, j - 1 + k),
							(i - k,     j + k    ) };// это /
						foreach (var ln in
							new (int, int)[][] { vertical, horizontal, diagonal_backslash, diagonal_slash })
							if (is_line_exist(ln))
								existing_lines.Add(ln);
					}
					return existing_lines;
				}

				bool is_terminated(BoardState B, int last_move_i, int last_move_j)
				{//при ходе на i,j может возникнуть выигрышное состояние
					bool is_winning_line(int i, int j, int player)
					{//есть ли выигрышная линия с участием клетки i,j
						var lines = get_lines(i, j);
						foreach ((int i, int j)[] ln in lines)
							if (ln.All(x => B.Board[x.i, x.j] == player))
								return true;
						return false;
					}
					//если есть три в ряд - кто-то выиграл, другой - проиграл
					//выигрыш высчитывается для компьютера
					if (is_winning_line(last_move_i, last_move_j, PLAYER1))
					{
						B.win_proportion = 1;
						B.draw_proportion = 0;
						B.losing_proportion = 0;
						return true;
					}
					else if (is_winning_line(last_move_i, last_move_j, PLAYER2))
					{
						B.win_proportion = 0;
						B.draw_proportion = 0;
						B.losing_proportion = 1;
						return true;
					}
					else if (B.empty_cells_cnt == 0)// если нет трёх в ряд, но при этом заполнено всё поле - ничья
					{
						B.win_proportion = 0;
						B.draw_proportion = 1;
						B.losing_proportion = 0;
						return true;
					}
					return false;
				}

				void embranchment(BoardState B)
				{
					if (B.is_terminating)
						return;
					var next_sign = other_player(B.who_made_the_move);
					var empty_coords = B.EmptyCells(used_coords);
					foreach ((int i, int j) crd in empty_coords)
					{
						BoardState child = (BoardState)(B.Clone());
						child.Board[crd.i, crd.j] = next_sign;
						child.who_made_the_move = next_sign;
						child.move = crd;
						child.empty_cells_cnt = B.empty_cells_cnt - 1;
						child.is_terminating = is_terminated(child, crd.i, crd.j);
						child.parent = B;
						B.children.Add(child);
					}
				}
				BoardState root = new BoardState(n, m, used_coords, EMPTY);
				//заполнение дерева с одновременным обходом в глубину и подсчётом долей выигрышных состояний
				Stack<BoardState> stack = new Stack<BoardState>();
				stack.Push(root);
				while (stack.Count != 0)
				{
					var cur_board = stack.Pop();
					if (!cur_board.is_terminating)
					{
						embranchment(cur_board);
						if (cur_board.children.All(x => x.is_terminating == true))
						{
							int children_cnt = cur_board.children.Count;
							cur_board.win_proportion =
								cur_board.children.Select(x => x.win_proportion).Sum() / children_cnt;
							cur_board.draw_proportion =
								cur_board.children.Select(x => x.draw_proportion).Sum() / children_cnt;
							cur_board.losing_proportion =
								cur_board.children.Select(x => x.losing_proportion).Sum() / children_cnt;
						}
						else
						{
							foreach (BoardState b in cur_board.children.Where(x => !x.is_terminating).ToList())
								stack.Push(b);
						}
					}
				}
				return root;
			}

			string PrintBoardState(int[,] B)
			{
				string response = "";
				try
				{
					if (B.GetLength(0) != n || B.GetLength(1) != m)
						throw new ArgumentException("Неправильная матрица доски");
					Dictionary<int, string> sign = new Dictionary<int, string>()
					{
						{ EMPTY, " " },
						{ PLAYER1, "X" },
						{ PLAYER2, "O" }
					};
					string txt_columns = "  | 0 | 1 | 2 | 3 | 4 |\n";
					string[] txt_rows = { "0", "1", "2" };
					string txt_sep1 = "-     +---+---+---+\n";
					string txt_sep2 = "- +---+---+---+---+---+\n";
					string vert_sep = "|";
					string print_cell(int i, int j)
					{
						return vert_sep + String.Format($" {{0,-{1}}} ", sign[B[i, j]]);
					}

					response += txt_columns;
					response += txt_sep1;
					for (int i = 0; i < n; i++)
					{
						response += txt_rows[i] + " ";
						if (i == 1)
							response += print_cell(i, 0);
						else
							response += "    ";
						for (int j = 1; j < m - 1; j++)
							response += print_cell(i, j);
						if (i == 1)
							response += print_cell(i, 4);
						response += vert_sep + "\n";
						if (i == 0 || i == 1)
							response += txt_sep2;
					}
					response += txt_sep1;
				}
				catch (Exception ex)
				{
					response += ex.Message;
				}
				Console.WriteLine(response);
				return response;
			}

			int[] parse_line(string line) // читает строку целых чисел, разделённых пробелами
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

			void MakeAMove(BoardState state, int i, int j, int player)
			{
				if (state.Board[i, j] != EMPTY)
					throw new Exception("Клетка уже занята");
				state.Board[i, j] = player;
				current_state_in_tree = current_state_in_tree.children.Find(
					x => x.Is_equivalent(state, used_coords));
			}

			(int, int) ComputerTurn()
			{
				var children = current_state_in_tree.children;
				//var min_loss = children.Select(x => x.losing_proportion).Min();

				var max_win = children.Select(x => x.win_proportion).Max();
				List<BoardState> max_win_Children = children.Where(x => x.win_proportion == max_win).ToList();
				var max_draw = max_win_Children.Select(x => x.draw_proportion).Max();
				List<BoardState> max_draw_Children = max_win_Children.Where(x => x.draw_proportion == max_draw).ToList();
				var random = new Random();
				var c = max_draw_Children[random.Next(max_draw_Children.Count)];
				return (c.move.i, c.move.j);
			}

			bool IsGameEnd(BoardState B)
			{
				if (B.is_terminating)
				{
					string ans = "";
					ans += ans_end;
					if (B.win_proportion == 1)
						ans += "Выиграл первый игрок";
					else if (B.losing_proportion == 1)
						ans += "Выиграл второй игрок";
					else
						ans += "Ничья";
					Console.WriteLine(ans + "\n");
					return true;
				}
				return false;
			}

			try
			{
				BoardState root = MakePlayTree();
				current_state_in_tree = root;

				BoardState state = new BoardState(n, m, used_coords, EMPTY);
				Console.WriteLine("Игра началась");
				PrintBoardState(state.Board);
				string input = "";
				bool can_player1_make_a_move = true;//потому что компьютер ходит первым
				while (true)
				{

					int i, j;
					if (can_player1_make_a_move)
					{
						(i, j) = ComputerTurn();
						MakeAMove(state, i, j, PLAYER1);
						Console.WriteLine("Ход компьютера:");
						Console.WriteLine($"Частоты (для первого игрока): выигрышей - {state.win_proportion}, ничьих - {state.draw_proportion}, проигрышей - {state.losing_proportion}");
						PrintBoardState(state.Board);
						if (IsGameEnd(current_state_in_tree))
							return ans;
					}
					try
					{
						can_player1_make_a_move = false;
						input = Console.ReadLine();
						if (input == interrupt_symbol)
							break;
						int[] input_coords = parse_line(input).ToArray();
						if (input_coords.Length != 2)
							throw new ArgumentException("Неверное количество координат");
						i = input_coords[0];//строка
						j = input_coords[1];//столбец
						if (!used_coords.Contains((i, j)))
							throw new ArgumentException($"Неверные координаты: i = {i}, j = {j}");

						MakeAMove(state, i, j, PLAYER2);
						can_player1_make_a_move = true;
						PrintBoardState(state.Board);
						if (IsGameEnd(current_state_in_tree))
							return ans;
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
						PrintBoardState(state.Board);
					}

				}
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
			string input;
			Console.WriteLine("Для выхода введите " + interrupt_symbol);

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
			//ConsoleInputCycle("Вариант 13. Верификация графа: проверка на цикличность.", HW5);
			ConsoleInputCycle("Вариант 13. Крестики-нолики на расширенном поле.", HW6);

			//Console.WriteLine("Press any key...");
		}
	}
}
