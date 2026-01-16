# PubQuiz

A real-time multiplayer pub quiz game platform built with Blazor Server and SignalR.

## Overview

PubQuiz is a full-stack web application where a host creates and controls quiz games while teams compete by answering diverse question types in real-time. The platform features automatic scoring, live leaderboards, and seamless reconnection support.

## Features

- **Multi-team gameplay** - Multiple teams compete simultaneously
- **Real-time updates** - Live scoring and leaderboards via SignalR
- **6 question types** - Multiple choice, open-ended, Wordle, estimate, real or fake, and Dino Run
- **Host controls** - Full game flow control with question reveal and answer review
- **Session persistence** - Teams can rejoin games after disconnection
- **Predefined teams** - Optionally set up team names before the game
- **Direct join links** - Share URLs for quick team joining (`/join?Code=ABC123&Team=MyTeam`)

## Tech Stack

| Component | Technology |
|-----------|------------|
| Backend | ASP.NET Core 10.0 |
| Frontend | Blazor Server |
| Database | PostgreSQL |
| ORM | Entity Framework Core 10.0 |
| Real-time | SignalR |
| Security | BCrypt |

## Question Types

| Type | Description | Scoring |
|------|-------------|---------|
| **Multiple Choice** | Select from 4 options | 10 points if correct |
| **Open Ended** | Text answer reviewed by host | 10 points if approved |
| **Wordle** | Guess a hidden word in up to 6 attempts | 10 + 2 × (attempts saved) |
| **Estimate** | Guess a numerical value | 0-10 based on accuracy |
| **Real or Fake** | Select the correct image from 4 options | 10 points if correct |
| **Dino Run** | Mini-game where teams compete for high scores | Points based on ranking |

## Project Structure

```
PubQuiz/
├── PubQuiz.sln
└── src/PubQuiz.Web/
    ├── Components/
    │   ├── Pages/           # Page components (Home, Admin, DirectJoin)
    │   │   ├── Host/        # Host control pages (CreateGame, Control, Login)
    │   │   └── Game/        # Player pages (Lobby, Play, Results)
    │   ├── Game/            # Question type components
    │   ├── Host/            # Host-specific panels
    │   └── Layout/          # App layout and modals
    ├── Models/              # Data entities (Game, Team, Question, Answer)
    ├── Services/            # Business logic (GameService, ScoringService, WordleService)
    ├── Data/                # Database context and seed data
    ├── Hubs/                # SignalR GameHub
    ├── Migrations/          # EF Core migrations
    └── wwwroot/             # Static assets
```

## Getting Started

### Prerequisites

- .NET 10.0 SDK
- PostgreSQL database

### Configuration

Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=pubquiz;Username=your_user;Password=your_password"
  }
}
```

### Running the Application

```bash
cd src/PubQuiz.Web
dotnet ef database update
dotnet run
```

The application will be available at `https://localhost:5001`.

## Game Flow

1. **Host creates a game** - Optionally sets a password and predefined teams
2. **Teams join** - Using a 6-character game code
3. **Lobby** - Teams wait for the host to start
4. **Questions** - Host reveals questions one at a time
5. **Answers** - Teams submit answers within time limits
6. **Review** - Host reviews open-ended answers
7. **Results** - Final leaderboard and winner announcement

## User Roles

- **Host**: Creates games, controls question flow, reviews answers, authenticates with password
- **Teams**: Join games, answer questions, view scores and results

## License

This project is private and not licensed for public use.
