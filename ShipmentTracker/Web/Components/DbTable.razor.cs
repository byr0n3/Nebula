using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.QuickGrid;
using Microsoft.EntityFrameworkCore;
using ShipmentTracker.Database;

namespace ShipmentTracker.Web.Components
{
	[CascadingTypeParameter("TGridItem")]
	public sealed partial class DbTable<TGridItem> : ComponentBase
	{
		[Inject] public required IDbContextFactory<ShipmentDbContext> DbFactory { get; init; }

		[Parameter] [EditorRequired] public required System.Func<ShipmentDbContext, IQueryable<TGridItem>> GetQuery { get; set; }

		[Parameter] [EditorRequired] public required RenderFragment ChildContent { get; set; }

		private readonly PaginationState pagination = new();

		private async ValueTask<GridItemsProviderResult<TGridItem>> LoadAsync(GridItemsProviderRequest<TGridItem> request)
		{
			int count;
			TGridItem[] items;

			var db = await this.DbFactory.CreateDbContextAsync(request.CancellationToken);

			await using (db)
			{
				var query = this.GetQuery(db);

				count = await query.CountAsync(request.CancellationToken);

				query = DbTable<TGridItem>.ApplyRequest(query, request);

				items = await query.ToArrayAsync(request.CancellationToken);
			}

			return new GridItemsProviderResult<TGridItem>
			{
				Items = items,
				TotalItemCount = count,
			};
		}

		private static IQueryable<TGridItem> ApplyRequest(IQueryable<TGridItem> query, GridItemsProviderRequest<TGridItem> request)
		{
			query = request.ApplySorting(query);

			if (request.Count.HasValue)
			{
				query = query.Take(request.Count.GetValueOrDefault());
			}

			return query.Skip(request.StartIndex);
		}
	}
}
