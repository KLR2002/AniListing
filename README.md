# 🌸 AniListing

A modern, aesthetic anime and manga tracking application with a **Sakura Flowers** theme, built with **Blazor WebAssembly** on the frontend and **ASP.NET Core Web API** on the backend.

AniListing integrates with the official **MyAnimeList API v2** to search and view anime and manga details, while providing a secure personal tracking and rating system saved in a local SQLite database.

---

## 🌸 Features

- **Secure User Authentication**:
  - Register and login with username and password.
  - Safe password protection using salted PBKDF2 hashing (`PasswordHasher<User>`).
  - Stateless JSON Web Token (JWT) authorization with automatic token injection in API calls.
- **Anime & Manga Discovery**:
  - Search anime and manga by English or Japanese names (e.g. *Frieren*, *葬送のフリーレン*, *Berserk*, *ベルセルク*).
  - High-resolution poster previews, mean community scores, genres, and alternative titles.
- **Detailed Media Pages**:
  - Full synopsis, alternative titles (English, Romaji, Japanese), studio/author, and release stats.
  - Interactive personal tracking box to assign status (*Watching/Reading, Completed, Plan to Watch/Read, Dropped*) and personal rating (1 to 10 stars).
  - Quick update or removal from personal lists.
- **Separate Anime & Manga Catalogs**:
  - Dedicated **🌸 My Anime List** (`/anime-list`) and **🌸 My Manga List** (`/manga-list`).
  - Status filters tailored to each medium.
  - Multi-criteria sorting by **Title (A-Z)**, **Personal Score (10–1)**, or **Date Updated (Recent first)**.
- **Sakura Flowers Theme**:
  - Delicate floral aesthetic inspired by cherry blossoms with soft ivory-pink backgrounds (`#FFF7FA`), floral accent buttons (`#E04373`), petal badges, and responsive layouts.
- **Smart Dry-Run / Mock Fallback**:
  - Test the entire application immediately without needing a MyAnimeList API key!
  - When no Client ID is configured, the backend seamlessly provides a rich catalog of popular anime and manga.
  - Once you get your official MAL API key, simply paste it into `appsettings.json` and the app automatically switches to live MyAnimeList API v2 queries.

---

## 🚀 Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

### 1. Run the Backend API

In a terminal, navigate to `AniListingAPI` and start the server:

```bash
cd AniListingAPI
dotnet run --urls "http://localhost:5000"
```

The API will automatically create the SQLite database `anilisting.db` on first run.

### 2. Run the Frontend (Blazor WebAssembly)

In a second terminal, navigate to `AniListingFront` and run:

```bash
cd AniListingFront
dotnet run
```

Then open your browser at the URL shown in the terminal (usually `http://localhost:5084` or `https://localhost:7084`).

---

## 🔑 Configuring Your MyAnimeList API Key

When you are ready to use the live MyAnimeList API:

1. Go to [MyAnimeList API Config](https://myanimelist.net/apiconfig) and create an API client.
2. Copy your **Client ID**.
3. Open `AniListingAPI/appsettings.json` and paste your Client ID:

```json
"MyAnimeList": {
  "ClientId": "YOUR_ACTUAL_CLIENT_ID_HERE"
}
```

4. Restart `AniListingAPI`. The app will now query live MyAnimeList API v2 endpoints!

> **Note**: As long as `ClientId` is empty, the app operates in **Smart Mock Mode** with offline data so you can test all features right away.

---

## 🧪 Running Tests

To run the automated test suite (covering authentication, password hashing, JWT claims, media mock search, and user list sorting):

```bash
dotnet test
```

---

## 📂 Project Structure

```
AniListing/
├── AniListingAPI/             # ASP.NET Core Web API (.NET 10)
│   ├── Controllers/           # AuthController, MediaController, UserListController
│   ├── Data/                  # EF Core SQLite DbContext & Entities (User, UserMediaItem)
│   ├── DTOs/                  # Request & Response data transfer contracts
│   ├── Services/              # TokenService, MyAnimeListService, MockMediaCatalog
│   └── appsettings.json       # Connection strings, JWT configuration, MAL ClientId
├── AniListingFront/           # Blazor WebAssembly (.NET 10)
│   ├── Layout/                # MainLayout, NavMenu (Sakura top navbar & auth view)
│   ├── Pages/                 # Home (Search), MediaDetails, AnimeList, MangaList, Login, Register
│   ├── Services/              # ApiClient, CustomAuthStateProvider (JWT in localStorage)
│   └── wwwroot/css/app.css    # Custom Sakura Flowers design system
├── AniListingAPI.Tests/       # xUnit unit test suite
├── .gitignore                 # Git ignore configuration
├── AniListing.slnx            # Solution file
└── README.md                  # This documentation
```
