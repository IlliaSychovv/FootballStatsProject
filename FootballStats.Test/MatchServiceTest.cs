using FootballStats.Application.DTO.Match;
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
            Id = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            Team1 = "Liverpool",
            Team2 = "Hunter",
            Score = "2:3"
        };

        _mockRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(match);
        
        var result = await _matchService.GetMatchByIdAsync(match.Id);
        
        result.ShouldNotBeNull();
        result.Date.ShouldBe(match.Date);
        _mockRepository.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task AddMatchAsync_ShouldAddMatch_WhenCalled()
    {
        var createDto = new CreateMatchDto
        {
            Date = DateTime.UtcNow,
            Team1 = "Liverpool",
            Team2 = "Hunter",
            Score = "2:3"
        };

        _mockRepository
            .Setup(x => x.AddAsync(It.IsAny<DomainMatch>()))
            .Returns(Task.CompletedTask);
        
        var result = await _matchService.AddMatchAsync(createDto);
        
        result.ShouldNotBeNull();
        result.Date.ShouldBe(createDto.Date);
        result.Team1.ShouldBe(createDto.Team1); 
        result.Score.ShouldBe(createDto.Score);
        _mockRepository.Verify(x => x.AddAsync(It.IsAny<DomainMatch>()), Times.Once);
    }
    
    [Fact]
    public async Task GetAsync_ShouldReturnPagedResponse_WhenMatchesExist()
    { 
        var matches = new List<DomainMatch>
        {
            new DomainMatch
            {
                Id = Guid.NewGuid(),
                Date = DateTime.UtcNow,
                Team1 = "Liverpool",
                Team2 = "Chelsea",
                Score = "2:1"
            },
            new DomainMatch
            {
                Id = Guid.NewGuid(),
                Date = DateTime.UtcNow.AddDays(-1),
                Team1 = "Arsenal",
                Team2 = "Tottenham",
                Score = "3:3"
            }
        };

        _mockRepository
            .Setup(r => r.GetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(matches);

        _mockRepository
            .Setup(r => r.CountAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
            .ReturnsAsync(matches.Count);

        var result = await _matchService.GetAsync(pageNumber: 1, pageSize: 10);
        
        result.ShouldNotBeNull();
        result.Items.Count.ShouldBe(2);
        result.TotalCount.ShouldBe(2);
        result.CurrentPage.ShouldBe(1);
        result.PageSize.ShouldBe(10);

        result.Items[0].Team1.ShouldBe("Liverpool");
        result.Items[1].Team2.ShouldBe("Tottenham");

        _mockRepository.Verify(r => r.GetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), 0, 10), Times.Once);
        _mockRepository.Verify(r => r.CountAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()), Times.Once);
    }
    
    [Fact]
    public async Task GetTeamNamesAsync_ShouldReturnListOfTeamNames()
    {
        var teamNames = new List<string> { "Liverpool", "Chelsea", "Arsenal" };

        _mockRepository
            .Setup(r => r.GetTeamNamesAsync())
            .ReturnsAsync(teamNames);

        var result = await _matchService.GetTeamNamesAsync();
        
        result.ShouldNotBeNull();
        result.Count.ShouldBe(3);
        result.ShouldContain("Liverpool");
        result.ShouldContain("Chelsea");
        result.ShouldContain("Arsenal");
        _mockRepository.Verify(r => r.GetTeamNamesAsync(), Times.Once);
    }
}