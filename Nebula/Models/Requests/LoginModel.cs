using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Elegance.AspNet.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Nebula.Models.Database;
using Nebula.Resources;
using Nebula.Services;

namespace Nebula.Models.Requests
{
	internal sealed class LoginModel : IValidatableObject
	{
		[Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "required")]
		public string? Email { get; set; }

		[Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "required")]
		public string? Password { get; set; }

		public bool Persistent { get; set; }

		public bool IsValid
		{
			[MemberNotNullWhen(true, nameof(this.Email), nameof(this.Password))]
			get => (this.Email is not null) && (this.Password is not null);
		}

		public IEnumerable<ValidationResult> Validate(ValidationContext context)
		{
			Debug.Assert(this.IsValid);

			var localizer = context.GetRequiredService<IStringLocalizer<ErrorMessages>>();
			var dbFactory = context.GetRequiredService<IDbContextFactory<ShipmentDbContext>>();
			var db = dbFactory.CreateDbContext();

			using (db)
			{
				var password = this.GetQuery(db)
								   .Select(static (u) => u.Password)
								   .FirstOrDefault();

				if ((password is null) || !Hashing.Verify(password, this.Password))
				{
					yield return new ValidationResult(localizer["sign-in-failed"], [nameof(this.Email)]);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IQueryable<User> GetQuery(ShipmentDbContext db) =>
			db.Users.Where((u) => (u.Email == this.Email) && ((u.Flags & UserFlags.Active) != UserFlags.None));
	}
}
