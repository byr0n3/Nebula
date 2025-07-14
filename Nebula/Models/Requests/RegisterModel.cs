using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using Elegance.AspNet.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Nebula.Resources;
using Nebula.Services;

namespace Nebula.Models.Requests
{
	internal sealed class RegisterModel : IValidatableObject
	{
		[Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "required")]
		[EmailAddress(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "email")]
		public string? Email { get; set; }

		[Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "required")]
		public string? Password { get; set; }

		[Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "required")]
		[Compare(nameof(RegisterModel.Password),
				 ErrorMessageResourceType = typeof(ErrorMessages),
				 ErrorMessageResourceName = "password_confirmation")]
		public string? PasswordConfirmation { get; set; }

		public bool Error { get; set; }

		public bool Valid
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			[MemberNotNullWhen(true, nameof(this.Email), nameof(this.Password), nameof(this.PasswordConfirmation))]
			get => (this.Email is not null) && (this.Password is not null) && (this.PasswordConfirmation is not null);
		}

		// @todo Localization
		public IEnumerable<ValidationResult> Validate(ValidationContext context)
		{
			Debug.Assert(this.Valid);

			var localizer = context.GetRequiredService<IStringLocalizer<ErrorMessages>>();

			if (!PasswordStrength.ValidateStrength(this.Password))
			{
				yield return new ValidationResult(
					string.Format(CultureInfo.InvariantCulture, localizer["weak-password"], PasswordStrength.MinLength),
					[nameof(this.Password)]
				);
			}

			var dbFactory = context.GetRequiredService<IDbContextFactory<ShipmentDbContext>>();
			var db = dbFactory.CreateDbContext();

			using (db)
			{
				var emailTaken = db.Users.Any((u) => u.Email == this.Email);

				if (emailTaken)
				{
					yield return new ValidationResult(localizer["email-taken"], [nameof(this.Email)]);
				}
			}
		}
	}
}
