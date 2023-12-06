using FluentValidation;

namespace Skanaus.Data.Dtos;

public record CreateCommentDto(string Content);
public record UpdateCommentDto(string Content);

public class CreateCommentDtoValidator : AbstractValidator<CreateCommentDto>
{
    public CreateCommentDtoValidator()
    {
        RuleFor(dto => dto.Content).NotEmpty().NotNull().Length(min: 10, max: 300);
    }
}
public class UpdateCommentDtoValidator : AbstractValidator<UpdateCommentDto>
{
    public UpdateCommentDtoValidator()
    {
        RuleFor(dto => dto.Content).NotEmpty().NotNull().Length(min: 10, max: 300);
    }
}