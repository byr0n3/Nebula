using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Elegance.AspNet.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShipmentTracker.Extensions;
using ShipmentTracker.Services;

namespace ShipmentTracker.Models.Requests
{
	internal sealed class EditAccountModel : IValidatableObject
	{
		[Required] public string? Username { get; set; }

		[Required] public string? Email { get; set; }

		public string? Password { get; set; }

		public string? PasswordConfirmation { get; set; }

		public bool IsValid
		{
			[MemberNotNullWhen(true, nameof(this.Username), nameof(this.Email))]
			get => !string.IsNullOrEmpty(this.Username) && !string.IsNullOrEmpty(this.Email);
		}

		public bool HasPassword
		{
			[MemberNotNullWhen(true, nameof(this.Password), nameof(this.PasswordConfirmation))]
			get => !string.IsNullOrEmpty(this.Password) && !string.IsNullOrEmpty(this.PasswordConfirmation);
		}

		// @todo Localization
		public IEnumerable<ValidationResult> Validate(ValidationContext context)
		{
			Debug.Assert(this.IsValid);

			foreach (var result in this.ValidatePassword())
			{
				yield return result;
			}

			var httpContext = context.GetRequiredService<IHttpContextAccessor>().HttpContext;

			Debug.Assert(httpContext is not null);

			var userId = httpContext.User.GetClaimValue<int>(UserClaim.Id);

			var dbFactory = context.GetRequiredService<IDbContextFactory<ShipmentDbContext>>();
			var db = dbFactory.CreateDbContext();

			using (db)
			{
				var usernameTaken = db.Users.Any((u) => (u.Id != userId) && (u.Username == this.Username));
				var emailTaken = db.Users.Any((u) => (u.Id != userId) && (u.Email == this.Email));

				if (usernameTaken)
				{
					yield return new ValidationResult("Username already taken", [nameof(this.Username)]);
				}

				if (emailTaken)
				{
					yield return new ValidationResult("E-mail already taken", [nameof(this.Email)]);
				}
			}
		}

		// @todo Localization
		private IEnumerable<ValidationResult> ValidatePassword()
		{
			if (!string.IsNullOrEmpty(this.Password))
			{
				if (!PasswordStrength.ValidateStrength(this.Password))
				{
					yield return new ValidationResult(
						$"Password must be at least {PasswordStrength.MinLength} characters long.",
						[nameof(this.Password)]
					);
				}

				if (string.IsNullOrEmpty(this.PasswordConfirmation))
				{
					yield return new ValidationResult("Required", [nameof(this.PasswordConfirmation)]);
				}
			}
		}
	}
}
