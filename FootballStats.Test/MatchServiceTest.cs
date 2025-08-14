using FootballStats.Application.DTO;
using FootballStats.Application.Interfaces.Repositories;
using FootballStats.Application.Services;
using DomainMatch = FootballStats.Domain.Entity.Match;
using Moq;
using Shouldly;

namespace FootballStats.Test;

public class MatchServiceTest
{
    private readonly Mock<IMatchRepository> _mockRepository;
    private readonly MatchService _matchService;

    public MatchServiceTest()
    {
        _mockRepository = new Mock<IMatchRepository>();
        _matchService = new MatchService(_mockRepository.Object);
    }
    
    [Fact]
    public async Task GetMatchByIdAsync_ShouldReturnMappedDto_WhenMatchExists()
    {
        var match = new DomainMatch
        {
            Id = 1,
            Date = DateTime.UtcNow,
            Team1 = "Liverpool",
            Team2 = "Hunter",
            Score = "2:3"
        };

        _mockRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(match);
        
        var result = await _matchService.GetMatchByIdAsync(match.Id);
        
        result.ShouldNotBeNull();
        result.Date.ShouldBe(match.Date);
        _mockRepository.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task AddMatchesAsync_ShouldAddMatch_WhenWeCallMethod()
    {
        var dto = new MatchDto
        {
            Id = 1,
            Date = DateTime.UtcNow,
            Team1 = "Liverpool",
            Team2 = "Hunter",
            Score = "2:3"
        };
        
        _mockRepository
            .Setup(x => x.AddAsync(It.IsAny<DomainMatch>()))
            .Returns(Task.CompletedTask);
        
        var result = await _matchService.AddMatchAsync(dto);
        
        result.ShouldNotBeNull();
        result.Date.ShouldBe(dto.Date);
        result.Team1.ShouldBe(dto.Team1);
        _mockRepository.Verify(x => x.AddAsync(It.IsAny<DomainMatch>()), Times.Once);
    }

    [Fact]
    public async Task GetMatchesForMVC_ShouldReturnPagedMatches_WhenMatchExists()
    {
        var matches = new List<DomainMatch>
        {
            new DomainMatch { Id = 1, Date = DateTime.UtcNow, Team1 = "Liverpool", Team2 = "Hunter", Score = "2:3" },
            new DomainMatch { Id = 2, Date = DateTime.UtcNow, Team1 = "Barca", Team2 = "Paris", Score = "3:2" }
        };
        
        _mockRepository
            .Setup(x => x.GetAsync(It.IsAny<string?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), 0, 50))
            .ReturnsAsync(matches);

        var result = await _matchService.GetMatchesForMVC("Liverpool", DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));
        
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
        _mockRepository.Verify(x => x.GetAsync(It.IsAny<string?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), 0, 50), Times.Once); 
    }

    [Fact]
    public async Task GetAsync_ShouldReturnPagedMatches_WhenMatchExists()
    {
        var matches = new List<DomainMatch>
        {
            new DomainMatch { Id = 1, Date = DateTime.UtcNow, Team1 = "Liverpool", Team2 = "Hunter", Score = "2:3" },
            new DomainMatch { Id = 2, Date = DateTime.UtcNow, Team1 = "Barca", Team2 = "Paris", Score = "3:2" }
        };
        
        _mockRepository
            .Setup(x => x.GetAsync(It.IsAny<string?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), 0, 50))
            .ReturnsAsync(matches);

        _mockRepository
            .Setup(x => x.CountAsync(It.IsAny<string?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
            .ReturnsAsync(matches.Count);
        
        var result = await _matchService.GetAsync("Liverpool", DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));
        
        result.ShouldNotBeNull();
        result.Items.Count.ShouldBe(2);
        result.CurrentPage.ShouldBe(1);
        result.PageSize.ShouldBe(50);
        result.Items[0].Id.ShouldBe(matches[0].Id);
        result.Items[0].Team1.ShouldBe(matches[0].Team1);
        
        _mockRepository.Verify(x => x.GetAsync(It.IsAny<string?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), 0, 50), Times.Once);
        _mockRepository.Verify(x => x.CountAsync(It.IsAny<string?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()), Times.Once);
    }
}