using CisApi.API.DTOs;
using FluentValidation;

namespace CisApi.API.Validators;

public class CreateTopicValidator : AbstractValidator<CreateTopicDto>
{
    public CreateTopicValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("O título é obrigatório.")
            .MinimumLength(5).WithMessage("O título deve ter pelo menos 5 caracteres.")
            .MaximumLength(150).WithMessage("O título não pode exceder 150 caracteres.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("A descrição não pode exceder 500 caracteres.");
    }
}