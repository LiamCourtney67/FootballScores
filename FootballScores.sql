drop database if exists FootballScores;
create database if not exists FootballScores;
use FootballScores;

create table Leagues (
	LeagueID int primary key auto_increment,
    LeagueName varchar(50)
);

create table Teams (
	TeamID int primary key auto_increment,
	TeamName varchar(30),
	LeagueID int,
    GamesPlayed int,
    GamesWon int,
    GamesDrawn int,
    GamesLost int,
    GoalsFor int,
    GoalsAgainst int,
    GoalDifference int,
    Points int,
    foreign key(LeagueID) references Leagues(LeagueID)
);

create table Players (
	PlayerID int primary key auto_increment,
    PlayerFirstName varchar(30),
    PlayerLastName varchar(30),
    PlayerAge int,
    PlayerKitNumber int,
    Position varchar(10),
    TeamID int,
    GoalsScored int,
    Assists int,
    CleanSheets int,
    YellowCards int,
    RedCards int,
    foreign key(TeamID) references Teams(TeamID)
);

create table Matches (
	MatchID int primary key auto_increment,
    HomeTeamID int,
    AwayTeamID int,
    LeagueID int,
    DatePlayed datetime,
    HomeGoals int,
    AwayGoals int,
    Result varchar(10),
    foreign key(HomeTeamID) references Teams(TeamID),
	foreign key(AwayTeamID) references Teams(TeamID),
    foreign key(LeagueID) references Leagues(LeagueID)
);

create table MatchStatsDetails (
	MatchID int,
    PlayerID int,
    Goals int,
    Assists int,
    YellowCards int,
    RedCards int,
    primary key(MatchID, PlayerID),
	foreign key(MatchID) references Matches(MatchID),
	foreign key(PlayerID) references Players(PlayerID)
);