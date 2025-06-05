using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Elegance.AspNet.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShipmentTracker.Database;

namespace ShipmentTracker.Models.Requests
{
	internal sealed class RegisterModel : IValidatableObject
	{
		[Required] public string? Username { get; set; }

		[Required] [EmailAddress] public string? Email { get; set; }

		[Required] public string? Password { get; set; }

		[Required] public string? PasswordConfirmation { get; set; }

		public bool Error { get; set; }

		public bool Valid
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			[MemberNotNullWhen(true,
							   nameof(this.Username), nameof(this.Email),
							   nameof(this.Password), nameof(this.PasswordConfirmation))]
			get => (this.Username is not null) && (this.Email is not null) &&
				   (this.Password is not null) && (this.PasswordConfirmation is not null);
		}

		// @todo Localization
		public IEnumerable<ValidationResult> Validate(ValidationContext context)
		{
			Debug.Assert(this.Valid);

			if (!string.Equals(this.Password, this.PasswordConfirmation, System.StringComparison.Ordinal))
			{
				yield return new ValidationResult(
					"Password and confirmation must match.",
					[nameof(this.Password)]
				);
			}

			if (!PasswordStrength.ValidateStrength(this.Password))
			{
				yield return new ValidationResult(
					$"Password must be at least {PasswordStrength.MinLength} characters long.",
					[nameof(this.Password)]
				);
			}

			var dbFactory = context.GetRequiredService<IDbContextFactory<ShipmentDbContext>>();
			var db = dbFactory.CreateDbContext();

			using (db)
			{
				var usernameTaken = db.Users.Any((u) => u.Username == this.Username);
				var emailTaken = db.Users.Any((u) => u.Email == this.Email);

				if (usernameTaken)
				{
					yield return new ValidationResult("Username is taken.", [nameof(this.Username)]);
				}

				if (emailTaken)
				{
					yield return new ValidationResult("Email is taken.", [nameof(this.Email)]);
				}
			}
		}
	}
}
