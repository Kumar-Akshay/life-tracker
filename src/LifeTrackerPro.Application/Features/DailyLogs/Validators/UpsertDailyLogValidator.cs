using FluentValidation;
using LifeTrackerPro.Shared.Constants;

namespace LifeTrackerPro.Application.Features.DailyLogs.Commands;

public class UpsertDailyLogCommandValidator : AbstractValidator<UpsertDailyLogCommand>
{
    public UpsertDailyLogCommandValidator()
    {
        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Date is required.")
            .Must(d => d <= DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Date cannot be in the future.");

        RuleFor(x => x.HourSlots)
            .Must(h => h.Count <= AppConstants.MaxHourSlots)
            .WithMessage($"Hour slots cannot exceed {AppConstants.MaxHourSlots}.");

        RuleFor(x => x.MoodScore)
            .InclusiveBetween(AppConstants.MinMoodScore, AppConstants.MaxMoodScore)
            .When(x => x.MoodScore.HasValue)
            .WithMessage($"Mood score must be between {AppConstants.MinMoodScore} and {AppConstants.MaxMoodScore}.");

        RuleFor(x => x.ProductivityScore)
            .InclusiveBetween(AppConstants.MinProductivityScore, AppConstants.MaxProductivityScore)
            .When(x => x.ProductivityScore.HasValue);

        RuleFor(x => x.Weight)
            .GreaterThan(0).When(x => x.Weight.HasValue)
            .WithMessage("Weight must be positive.");

        RuleFor(x => x.AmountSpent)
            .GreaterThanOrEqualTo(0).When(x => x.AmountSpent.HasValue)
            .WithMessage("Amount spent cannot be negative.");
    }
}
