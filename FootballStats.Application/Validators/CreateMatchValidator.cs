using FluentValidation;
using FootballStats.Application.DTO.Match;

namespace FootballStats.Application.Validators;

public class CreateMatchValidator : AbstractValidator<MatchDto>
{
    public CreateMatchValidator()
    {
        RuleFor(m => m.Date)
            .NotEmpty().WithMessage("Match date is required.")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Match date cannot be in the future.");

        RuleFor(m => m.Team1)
            .NotEmpty().WithMessage("First team name is required.")
            .MaximumLength(100).WithMessage("First team name cannot exceed 100 characters.");

        RuleFor(m => m.Team2)
            .NotEmpty().WithMessage("Second team name is required.")
            .MaximumLength(100).WithMessage("Second team name cannot exceed 100 characters.")
            .NotEqual(m => m.Team1).WithMessage("Teams cannot be the same.");

        RuleFor(m => m.Score)
            .NotEmpty().WithMessage("Score is required.")
            .Matches(@"^\d+:\d+$").WithMessage("Score must be in the format 'X:Y'.");
    }
}