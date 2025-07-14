using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Elegance.AspNet.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Nebula.Extensions;
using Nebula.Resources;
using Nebula.Services;

namespace Nebula.Models.Requests
{
	internal sealed class EditAccountModel : IValidatableObject
	{
		[Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "required")]
		[EmailAddress(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "email")]
		public string? Email { get; set; }

		public string? Password { get; set; }

		[Compare(nameof(RegisterModel.Password),
				 ErrorMessageResourceType = typeof(ErrorMessages),
				 ErrorMessageResourceName = "password_confirmation")]
		public string? PasswordConfirmation { get; set; }

		[Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "required")]
		public string? Culture { get; set; }

		public bool IsValid
		{
			[MemberNotNullWhen(true, nameof(this.Email), nameof(this.Culture))]
			get => !string.IsNullOrEmpty(this.Email) && !string.IsNullOrEmpty(this.Culture);
		}

		public bool HasPassword
		{
			[MemberNotNullWhen(true, nameof(this.Password), nameof(this.PasswordConfirmation))]
			get => !string.IsNullOrEmpty(this.Password) && !string.IsNullOrEmpty(this.PasswordConfirmation);
		}

		public IEnumerable<ValidationResult> Validate(ValidationContext context)
		{
			Debug.Assert(this.IsValid);

			var localizer = context.GetRequiredService<IStringLocalizer<ErrorMessages>>();

			if (!string.IsNullOrEmpty(this.Password))
			{
				if (!PasswordStrength.ValidateStrength(this.Password))
				{
					yield return new ValidationResult(
						string.Format(CultureInfo.InvariantCulture, localizer["weak-password"], PasswordStrength.MinLength),
						[nameof(this.Password)]
					);
				}
			}

			if ((this.Culture is not null) && !Cultures.Supported.Contains(new CultureInfo(this.Culture)))
			{
				yield return new ValidationResult(localizer["culture"], [nameof(this.Culture)]);
			}

			var httpContext = context.GetRequiredService<IHttpContextAccessor>().HttpContext;

			Debug.Assert(httpContext is not null);

			var userId = httpContext.User.GetClaimValue<int>(UserClaim.Id);

			var dbFactory = context.GetRequiredService<IDbContextFactory<ShipmentDbContext>>();
			var db = dbFactory.CreateDbContext();

			using (db)
			{
				var emailTaken = db.Users.Any((u) => (u.Id != userId) && (u.Email == this.Email));

				if (emailTaken)
				{
					yield return new ValidationResult(localizer["email-taken"], [nameof(this.Email)]);
				}
			}
		}
	}
}
