using FluentValidation;

namespace Skanaus.Data.Dtos;

public record CreateKitchenDto(string Name, string Description);
public record UpdateKitchenDto(string Description);

public class CreateKitchenDtoValidator : AbstractValidator<CreateKitchenDto>
{
    public CreateKitchenDtoValidator()
    {
        RuleFor(dto => dto.Name).NotEmpty().NotNull().Length(min: 2, max: 100);
        RuleFor(dto => dto.Description).NotEmpty().NotNull().Length(min: 10, max: 300);
    }
}
public class UpdateKitchenDtoValidator : AbstractValidator<UpdateKitchenDto>
{
    public UpdateKitchenDtoValidator()
    {
        RuleFor(dto => dto.Description).NotEmpty().NotNull().Length(min: 10, max: 300);
    }
}