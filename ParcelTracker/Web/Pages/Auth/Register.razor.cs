using System.Diagnostics;
using System.Threading.Tasks;
using Elegance.AspNet.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ParcelTracker.Database;
using ParcelTracker.Database.Models;
using ParcelTracker.Models.Requests;

namespace ParcelTracker.Web.Pages.Auth
{
	public sealed partial class Register : ComponentBase
	{
		private const string formName = "register";

		[Inject] public required IDbContextFactory<ParcelDbContext> DbFactory { get; init; }

		[Inject] public required NavigationManager Navigation { get; init; }

		[Inject] public required ILogger<Register> Logger { get; init; }

		[SupplyParameterFromForm(FormName = Register.formName)]
		private RegisterModel Model { get; set; } = new();

		private async Task RegisterAsync()
		{
			var db = await this.DbFactory.CreateDbContextAsync();
			var transaction = await db.Database.BeginTransactionAsync();

			await using (db)
			await using (transaction)
			{
				try
				{
					var model = this.Model;

					Debug.Assert(model.Valid);

					db.Users.Add(new User
					{
						Username = model.Username,
						Email = model.Email,
						Password = Hashing.Hash(model.Password),
					});

					var saved = await db.SaveChangesAsync();

					if (saved != 1)
					{
						throw new System.Exception("Failed to saved new user");
					}

					await transaction.CommitAsync();
				}
				catch (System.Exception ex)
				{
					this.Logger.LogError(ex, "Exception while creating new user.");

					await transaction.RollbackAsync();

					this.Model.Error = true;
					return;
				}
			}

			this.Navigation.NavigateTo("/login", true);
		}
	}
}
