using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Nebula.Models.Database;
using Nebula.Services;

namespace Nebula.Web.Components
{
	public sealed partial class Grid<TGridItem> : ComponentBase
		where TGridItem : IEntity
	{
		private const string pageQueryKey = "page";
		private const string perPageQueryKey = "per_page";
		private const int defaultPerPage = 6;

		[Inject] public required IDbContextFactory<ShipmentDbContext> DbFactory { get; init; }

		[Inject] public required NavigationManager Navigation { get; init; }

		[Parameter] [EditorRequired] public required System.Func<ShipmentDbContext, IQueryable<TGridItem>> GetQuery { get; set; }

		[Parameter] [EditorRequired] public required RenderFragment<TGridItem> ItemTemplate { get; set; }

		[Parameter] public RenderFragment? EmptyTemplate { get; set; }

		[SupplyParameterFromQuery(Name = Grid<TGridItem>.pageQueryKey)]
		public required int Page { get; set; } = 1;

		[SupplyParameterFromQuery(Name = Grid<TGridItem>.perPageQueryKey)]
		public required int PerPage { get; set; } = Grid<TGridItem>.defaultPerPage;

		private int LastPage =>
			(int)float.Ceiling(this.count / (float)this.PerPage);

		private int count;
		private List<TGridItem>? items;

		protected override Task OnInitializedAsync()
		{
			this.Page = int.Max(this.Page, 1);

			if (this.PerPage <= 0)
			{
				this.PerPage = Grid<TGridItem>.defaultPerPage;
			}

			return this.LoadDataAsync();
		}

		private async Task LoadDataAsync()
		{
			var db = await this.DbFactory.CreateDbContextAsync();

			await using (db)
			{
				var query = this.GetQuery(db);

				this.count = await query.CountAsync();

				this.Page = int.Min(this.Page, int.Max(this.LastPage, 1));

				query = query.Skip((this.Page - 1) * this.PerPage)
							 .Take(this.PerPage);

				this.items = await query.ToListAsync();
			}
		}

		private string GetPageLink(int page) =>
			this.Navigation.GetUriWithQueryParameter(Grid<TGridItem>.pageQueryKey, (page > 1) && (page <= this.LastPage) ? page : null);
	}
}
