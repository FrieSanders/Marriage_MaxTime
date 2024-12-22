using System.Diagnostics;

namespace Marriage_MaxTime
{
	internal class Program
	{
		public static void Main(string[] args)
		{
			// Участники и их предпочтения
			var men = new List<string>();
			var women = new List<string>();

			const int n = 1000; // Увеличиваем количество участников для демонстрации максимального времени выполнения
			for (int i = 1; i <= n; i++)
			{
				men.Add($"M{i}");
				women.Add($"W{i}");
			}

			var menPreferences = new Dictionary<string, List<string>>();
			var womenPreferences = new Dictionary<string, List<string>>();

			var rnd = new Random();

			foreach (var man in men)
			{
				var shuffledWomen = new List<string>(women);
				ShuffleList(shuffledWomen, rnd);
				menPreferences[man] = shuffledWomen;
			}

			foreach (var woman in women)
			{
				var shuffledMen = new List<string>(men);
				ShuffleList(shuffledMen, rnd);
				womenPreferences[woman] = shuffledMen;
			}

			var stopwatch = Stopwatch.StartNew(); // Начинаем измерение времени

			var stableMatches = FindStableMatches(men, women, menPreferences, womenPreferences);

			stopwatch.Stop(); // Останавливаем измерение времени

			Console.WriteLine("Stable Matches:");
			foreach (var match in stableMatches)
			{
				Console.WriteLine($"{match.Key} is paired with {match.Value}");
			}

			Console.WriteLine($"Execution Time: {stopwatch.ElapsedMilliseconds} ms");
		}

		public static Dictionary<string, string> FindStableMatches(
			List<string> men,
			List<string> women,
			Dictionary<string, List<string>> menPreferences,
			Dictionary<string, List<string>> womenPreferences)
		{
			// Инициализация
			var freeMen = new Queue<string>(men); // Все мужчины изначально свободны
			var womenPartners = new Dictionary<string, string>(); // Текущие партнеры женщин
			var menNextProposal = new Dictionary<string, int>(); // Индекс следующего предложения для каждого мужчины

			foreach (var man in men)
			{
				menNextProposal[man] = 0; // Все мужчины начинают с первого предпочтения
			}

			// Алгоритм предложений
			while (freeMen.Count > 0)
			{
				var man = freeMen.Dequeue();
				var womanIndex = menNextProposal[man];
				var woman = menPreferences[man][womanIndex];
				menNextProposal[man]++;

				if (!womenPartners.ContainsKey(woman))
				{
					// Женщина свободна, формируем пару
					womenPartners[woman] = man;
				}
				else
				{
					// Женщина уже имеет партнера, проверяем предпочтения
					var currentPartner = womenPartners[woman];
					if (PrefersNewPartner(woman, man, currentPartner, womenPreferences))
					{
						// Женщина выбирает нового партнера, освобождаем старого
						freeMen.Enqueue(currentPartner);
						womenPartners[woman] = man;
					}
					else
					{
						// Женщина остается с текущим партнером
						freeMen.Enqueue(man);
					}
				}
			}

			// Формируем результат
			var matches = new Dictionary<string, string>();
			foreach (var pair in womenPartners)
			{
				matches[pair.Value] = pair.Key;
			}

			return matches;
		}

		private static bool PrefersNewPartner(string woman, string newMan, string currentMan, Dictionary<string, List<string>> womenPreferences)
		{
			var preferences = womenPreferences[woman];
			return preferences.IndexOf(newMan) < preferences.IndexOf(currentMan);
		}

		private static void ShuffleList<T>(List<T> list, Random rnd)
		{
			for (int i = list.Count - 1; i > 0; i--)
			{
				int j = rnd.Next(i + 1);
				(list[i], list[j]) = (list[j], list[i]);
			}
		}
	}
}
