using FluentValidation;

namespace Skanaus.Data.Dtos;

public record CreateRecipeDto(string Name, string Body);
public record UpdateRecipeDto(string Body);

public class CreateRecipeDtoValidator : AbstractValidator<CreateRecipeDto>
{
    public CreateRecipeDtoValidator()
    {
        RuleFor(dto => dto.Name).NotEmpty().NotNull().Length(min: 2, max: 100);
        RuleFor(dto => dto.Body).NotEmpty().NotNull().Length(min: 10, max: 300);
    }
}
public class UpdateRecipeDtoValidator : AbstractValidator<UpdateRecipeDto>
{
    public UpdateRecipeDtoValidator()
    {
        RuleFor(dto => dto.Body).NotEmpty().NotNull().Length(min: 10, max: 300);
    }
}