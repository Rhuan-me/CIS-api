using CisApi.API.DTOs;
using FluentValidation;

namespace CisApi.API.Validators;

public class CreateTopicDtoValidator : AbstractValidator<CreateTopicDto>
{
    public CreateTopicDtoValidator()
    {
        // Regra de validação baseada nos critérios de aceitação da User Story
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("O título é obrigatório.")
            .MaximumLength(255).WithMessage("O título não pode exceder 255 caracteres.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("A descrição não pode exceder 1000 caracteres.");
    }
}