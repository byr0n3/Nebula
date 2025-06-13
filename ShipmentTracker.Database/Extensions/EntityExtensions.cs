using System.Linq;
using System.Runtime.CompilerServices;
using ShipmentTracker.Database.Interfaces;

namespace ShipmentTracker.Database.Extensions
{
	public static class EntityExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IQueryable<T> WhereId<T>(this IQueryable<T> @this, int id) where T : IEntity =>
			@this.Where((e) => e.Id == id);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IQueryable<T> WhereUserId<T>(this IQueryable<T> @this, int userId) where T : IEntityWithUser =>
			@this.Where((e) => e.UserId == userId);
	}
}
