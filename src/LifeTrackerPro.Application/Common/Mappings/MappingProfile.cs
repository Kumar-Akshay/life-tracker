using AutoMapper;
using LifeTrackerPro.Domain.Entities;
using LifeTrackerPro.Shared.DTOs;
using System.Text.Json;

namespace LifeTrackerPro.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ApplicationUser, UserProfileDto>();

        CreateMap<DailyLog, DailyLogDto>()
            .ForMember(d => d.HourSlots,
                opt => opt.MapFrom(s => JsonSerializer.Deserialize<List<string>>(s.HourSlots, (JsonSerializerOptions?)null) ?? new List<string>()));

        CreateMap<Category, CategoryDto>();

        CreateMap<UserSettings, UserSettingsDto>()
            .ForMember(d => d.TimeGranularity, opt => opt.MapFrom(s => s.TimeGranularity.ToString()))
            .ForMember(d => d.ThemeMode, opt => opt.MapFrom(s => s.ThemeMode.ToString()));
    }
}
