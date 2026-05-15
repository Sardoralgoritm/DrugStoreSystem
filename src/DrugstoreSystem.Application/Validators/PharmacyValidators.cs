using DrugstoreSystem.Application.Requests;
using FluentValidation;

namespace DrugstoreSystem.Application.Validators;

public class CreatePharmacyRequestValidator : AbstractValidator<CreatePharmacyRequest>
{
    public CreatePharmacyRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Address).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Latitude).InclusiveBetween(-90, 90);
        RuleFor(x => x.Longitude).InclusiveBetween(-180, 180);
        RuleFor(x => x.Phone).MaximumLength(50).When(x => x.Phone is not null);
        RuleFor(x => x.WorkingHours).MaximumLength(200).When(x => x.WorkingHours is not null);
        RuleFor(x => x.PharmacistEmail).NotEmpty().EmailAddress();
        RuleFor(x => x.PharmacistPassword).NotEmpty().MinimumLength(6);
    }
}

public class UpdatePharmacyRequestValidator : AbstractValidator<UpdatePharmacyRequest>
{
    public UpdatePharmacyRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Address).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Latitude).InclusiveBetween(-90, 90);
        RuleFor(x => x.Longitude).InclusiveBetween(-180, 180);
        RuleFor(x => x.Phone).MaximumLength(50).When(x => x.Phone is not null);
        RuleFor(x => x.WorkingHours).MaximumLength(200).When(x => x.WorkingHours is not null);
    }
}
