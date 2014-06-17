using System;
using System.Collections.Generic;
using System.Data;
using CommandHandler.Entities;
using DapperExtensions;

namespace CommandHandler.Infrastructure
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
		
		public static void Upsert(this IDbConnection connection, IKeyed entity)
		{
			if (entity.ID == Guid.Empty)
			{
				entity.ID = Guid.NewGuid();
				connection.Insert(entity);
			}
			else
			{
				connection.Update(entity);
			}
		}
	}
}
