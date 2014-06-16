using System;
using System.Collections.Generic;

namespace EntitiesCqrsCommandHandler.Infrastructure
{
	public static class Extensions
	{
		public static void Each<T>(this IEnumerable<T> self, Action<T> action)
		{
			foreach (var item in self)
			{
				action(item);
			}
		}

		public static IEnumerable<T> Apply<T>(this IEnumerable<T> self, Action<T> action)
		{
			foreach (var item in self)
			{
				action(item);
				yield return item;
			}
		}
	}
}
