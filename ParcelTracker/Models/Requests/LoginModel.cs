using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Elegance.AspNet.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ParcelTracker.Database;
using ParcelTracker.Database.Models;

namespace ParcelTracker.Models.Requests
{
	internal sealed class LoginModel : IValidatableObject
	{
		[Required] public string? User { get; set; }

		[Required] public string? Password { get; set; }

		public bool Persistent { get; set; }

		public bool IsValid
		{
			[MemberNotNullWhen(true, nameof(this.User), nameof(this.Password))]
			get => (this.User is not null) && (this.Password is not null);
		}

		public IEnumerable<ValidationResult> Validate(ValidationContext context)
		{
			Debug.Assert(this.IsValid);

			var dbFactory = context.GetRequiredService<IDbContextFactory<ParcelDbContext>>();
			var db = dbFactory.CreateDbContext();

			using (db)
			{
				var password = this.GetQuery(db)
								   .Select(static (u) => u.Password)
								   .FirstOrDefault();

				if ((password is null) || !Hashing.Verify(password, this.Password))
				{
					// @todo Localization
					yield return new ValidationResult("Invalid user/password", [nameof(this.User)]);
				}
			}
		}

		public IQueryable<User> GetQuery(ParcelDbContext db) =>
			db.Users
			  .Where((u) =>
						 ((u.Username == this.User) || (u.Email == this.User)) &&
						 (u.Flags & UserFlags.Active) != UserFlags.None
			   );
	}
}
