# AniListing Full-Stack Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build the complete AniListing web application featuring ASP.NET Core Web API with SQLite persistence, JWT authentication, MyAnimeList API v2 proxy with smart offline mock fallback, and Blazor WebAssembly frontend with custom Sakura Flowers styling and separate Anime/Manga lists with sorting.

**Architecture:** ASP.NET Core 10 backend exposing REST API endpoints for authentication, MyAnimeList search/details, and user list persistence via EF Core SQLite. Blazor WebAssembly 10 frontend managing authentication state via JWT tokens in localStorage, calling the API through a typed ApiClient, and presenting an aesthetic sakura-themed UI with dedicated search, details, anime list, and manga list pages.

**Tech Stack:** C# 13, .NET 10.0, ASP.NET Core Web API, Blazor WebAssembly, Entity Framework Core SQLite, Microsoft.AspNetCore.Authentication.JwtBearer, xUnit, bUnit / Moq / Assertions.

**Spec:** `docs/superpowers/specs/2026-09-11-anilisting-design.md`

## Global Constraints
- Target Framework: `net10.0`
- Database: SQLite (`anilisting.db`)
- Authentication: Salted PBKDF2 hashing via `PasswordHasher<User>`, signed JWT Bearer tokens with 7-day expiration
- MyAnimeList API: v2 with `X-MAL-CLIENT-ID` header; automatic fallback to offline mock data when Client ID is empty or demo
- Lists: Strictly separate pages and views for Anime (`/anime-list`) and Manga (`/manga-list`)
- Theme: Sakura Flowers palette with cherry blossom tones (`#FFB7C5`, `#E85D75`, `#FFF8FA`, `#2E1C2B`)

---

### Task 1: Scaffolding, Dependencies & .gitignore

**Files:**
- Create: `.gitignore`
- Modify: `AniListingAPI/AniListingAPI.csproj`
- Create: `AniListingAPI.Tests/AniListingAPI.Tests.csproj`

**Interfaces:**
- Produces: Project references and test runner configuration.

- [ ] **Step 1: Write `.gitignore`**
Define standard rules ignoring build directories (`bin/`, `obj/`), SQLite files (`*.db`, `*.db-shm`, `*.db-wal`), IDE files (`.vs/`, `.idea/`, `.vscode/`), and OS files.

- [ ] **Step 2: Add NuGet packages to `AniListingAPI.csproj`**
Add:
- `Microsoft.EntityFrameworkCore.Sqlite` (v10.0.12 or compatible 10.x)
- `Microsoft.AspNetCore.Authentication.JwtBearer` (v10.0.12 or compatible 10.x)

- [ ] **Step 3: Create `AniListingAPI.Tests.csproj`**
Setup xUnit test project with reference to `AniListingAPI` and `Microsoft.EntityFrameworkCore.InMemory`.

- [ ] **Step 4: Verify build of solution**
Run: `dotnet build AniListingAPI/AniListingAPI.csproj`
Expected: Build succeeded.

---

### Task 2: Data Layer & Entities (EF Core SQLite)

**Files:**
- Create: `AniListingAPI/Data/Entities/User.cs`
- Create: `AniListingAPI/Data/Entities/UserMediaItem.cs`
- Create: `AniListingAPI/Data/AniListingDbContext.cs`
- Create: `AniListingAPI.Tests/DataTests.cs`

**Interfaces:**
- Produces: `AniListingDbContext`, `User`, `UserMediaItem`

- [ ] **Step 1: Write the failing unit test for DbContext**
In `AniListingAPI.Tests/DataTests.cs`, verify that `AniListingDbContext` can create a user and associate a `UserMediaItem` with uniqueness on `(UserId, MalId, MediaType)`.

- [ ] **Step 2: Run test to verify it fails**
Run: `dotnet test AniListingAPI.Tests --filter "FullyQualifiedName~DataTests"`
Expected: FAIL (types not found).

- [ ] **Step 3: Implement `User.cs`, `UserMediaItem.cs`, and `AniListingDbContext.cs`**
- `User`: `Id`, `Username`, `PasswordHash`, `CreatedAt`, `MediaItems`.
- `UserMediaItem`: `Id`, `UserId`, `MalId`, `MediaType` ("anime" or "manga"), `Title`, `JapaneseTitle`, `PosterUrl`, `Status`, `Score`, `CreatedAt`, `UpdatedAt`.
- `AniListingDbContext`: Model configuration with indexes and unique constraint.

- [ ] **Step 4: Run test to verify it passes**
Run: `dotnet test AniListingAPI.Tests --filter "FullyQualifiedName~DataTests"`
Expected: PASS.

---

### Task 3: Security & Authentication (Password Hashing, JWT, AuthController)

**Files:**
- Create: `AniListingAPI/DTOs/AuthDtos.cs`
- Create: `AniListingAPI/Services/ITokenService.cs`
- Create: `AniListingAPI/Services/TokenService.cs`
- Create: `AniListingAPI/Controllers/AuthController.cs`
- Create: `AniListingAPI.Tests/AuthTests.cs`
- Modify: `AniListingAPI/Program.cs`
- Modify: `AniListingAPI/appsettings.json`

**Interfaces:**
- Consumes: `AniListingDbContext`, `User`
- Produces:
  - `POST /api/auth/register`
  - `POST /api/auth/login`
  - `GET /api/auth/me`

- [ ] **Step 1: Write failing tests for Auth & Password hashing**
In `AniListingAPI.Tests/AuthTests.cs`, test registering a new user, password hashing validation, rejecting duplicate usernames, authenticating valid credentials, and rejecting invalid passwords.

- [ ] **Step 2: Run test to verify failure**
Run: `dotnet test AniListingAPI.Tests --filter "FullyQualifiedName~AuthTests"`
Expected: FAIL.

- [ ] **Step 3: Implement Auth DTOs, TokenService, and AuthController**
- Register DTOs: `RegisterRequest`, `LoginRequest`, `AuthResponse`, `UserDto`.
- Configure JWT Bearer and authorization in `Program.cs`.
- Implement `PasswordHasher<User>` logic.

- [ ] **Step 4: Run test to verify it passes**
Run: `dotnet test AniListingAPI.Tests --filter "FullyQualifiedName~AuthTests"`
Expected: PASS.

---

### Task 4: MyAnimeList API v2 Integration & Smart Mock Media Catalog

**Files:**
- Create: `AniListingAPI/DTOs/MediaDtos.cs`
- Create: `AniListingAPI/Services/IMyAnimeListService.cs`
- Create: `AniListingAPI/Services/MockMediaCatalog.cs`
- Create: `AniListingAPI/Services/MyAnimeListService.cs`
- Create: `AniListingAPI/Controllers/MediaController.cs`
- Create: `AniListingAPI.Tests/MediaServiceTests.cs`

**Interfaces:**
- Produces:
  - `IMyAnimeListService.SearchMediaAsync(string type, string query)`
  - `IMyAnimeListService.GetMediaDetailsAsync(string type, int id)`
  - `GET /api/media/search?type={anime|manga}&q={query}`
  - `GET /api/media/{type}/{id}`

- [ ] **Step 1: Write failing tests for Media Service & Mock Catalog**
Test that searching for English and Japanese names ("Frieren", "葬送のフリーレン", "Berserk") in mock mode returns rich matching items, and querying details by ID returns complete media metadata (synopsis, score, posters).

- [ ] **Step 2: Run test to verify failure**
Run: `dotnet test AniListingAPI.Tests --filter "FullyQualifiedName~MediaServiceTests"`
Expected: FAIL.

- [ ] **Step 3: Implement `MockMediaCatalog`, `MyAnimeListService`, and `MediaController`**
- Curated top-tier catalog for Anime (*Frieren*, *Fullmetal Alchemist*, *Spirited Away*, *Attack on Titan*, *Demon Slayer*, *Spy x Family*) and Manga (*Berserk*, *One Piece*, *Chainsaw Man*, *Monster*, *Vinland Saga*).
- Fallback logic: If `ClientId` is blank/demo, query `MockMediaCatalog`. Otherwise, execute HTTP request to `https://api.myanimelist.net/v2/`.

- [ ] **Step 4: Run test to verify it passes**
Run: `dotnet test AniListingAPI.Tests --filter "FullyQualifiedName~MediaServiceTests"`
Expected: PASS.

---

### Task 5: User Media List Management (CRUD & Sorting)

**Files:**
- Create: `AniListingAPI/DTOs/UserListDtos.cs`
- Create: `AniListingAPI/Controllers/UserListController.cs`
- Create: `AniListingAPI.Tests/UserListTests.cs`

**Interfaces:**
- Consumes: `AniListingDbContext`, JWT Auth context
- Produces:
  - `GET /api/userlist?mediaType={anime|manga}&status={}&sortBy={name|score|date}&sortOrder={asc|desc}`
  - `GET /api/userlist/check/{mediaType}/{malId}`
  - `POST /api/userlist`
  - `DELETE /api/userlist/{mediaType}/{malId}`

- [ ] **Step 1: Write failing tests for UserList operations**
Test:
1. Adding an anime to list with status "Watching" and score 9.
2. Checking presence and score.
3. Sorting by Name, Score, and Date Updated.
4. Removing an item.

- [ ] **Step 2: Run test to verify failure**
Run: `dotnet test AniListingAPI.Tests --filter "FullyQualifiedName~UserListTests"`
Expected: FAIL.

- [ ] **Step 3: Implement `UserListController`**
Full implementation with validation, authorized user claim extraction, EF Core updates, and dynamic sorting.

- [ ] **Step 4: Run test to verify it passes**
Run: `dotnet test AniListingAPI.Tests --filter "FullyQualifiedName~UserListTests"`
Expected: PASS.

---

### Task 6: Blazor Frontend Styling & Sakura Theme Setup

**Files:**
- Modify: `AniListingFront/wwwroot/css/app.css`
- Modify: `AniListingFront/wwwroot/index.html`
- Modify: `AniListingFront/Layout/NavMenu.razor`
- Modify: `AniListingFront/Layout/MainLayout.razor`
- Modify: `AniListingFront/Layout/MainLayout.razor.css`

**Interfaces:**
- Produces: Sakura Flowers design system tokens, responsive navbar with Anime List and Manga List links, user profile pill, and toast notifications.

- [ ] **Step 1: Add Sakura Theme styling in `app.css`**
- Set up CSS custom properties: cherry blossom palette (`--sakura-50` through `--sakura-900`), petal shadows, typography, cards, badges, button styles, star rating inputs, and petal keyframe animation.

- [ ] **Step 2: Update `index.html`**
Set page title to "🌸 AniListing", add Google Fonts (Poppins / Noto Sans JP for Japanese titles), and link favicon.

- [ ] **Step 3: Update `NavMenu.razor` and `MainLayout.razor`**
- Brand header with sakura icon and AniListing title.
- Navigation links: **Discover (Search)**, **🌸 Anime List**, **🌸 Manga List**.
- Auth indicator displaying logged-in username or **Login / Register** buttons.

---

### Task 7: Blazor Services & JWT Authentication State

**Files:**
- Create: `AniListingFront/Models/AuthModels.cs`
- Create: `AniListingFront/Models/MediaModels.cs`
- Create: `AniListingFront/Services/CustomAuthStateProvider.cs`
- Create: `AniListingFront/Services/IApiClient.cs`
- Create: `AniListingFront/Services/ApiClient.cs`
- Modify: `AniListingFront/Program.cs`
- Create: `AniListingFront/Pages/Login.razor`
- Create: `AniListingFront/Pages/Register.razor`

**Interfaces:**
- Consumes: Backend `/api/auth/*`
- Produces: Reactive `AuthenticationState`, `IApiClient` service injected into Blazor components.

- [ ] **Step 1: Implement `CustomAuthStateProvider` & `ApiClient`**
- `CustomAuthStateProvider` reads JWT token from browser `localStorage` using JS interop, parses claims, and notifies components.
- `ApiClient` attaches `Bearer` token to headers and handles error responses gracefully.

- [ ] **Step 2: Register services in `AniListingFront/Program.cs`**
Register `AuthenticationStateProvider`, `AuthorizationCore`, `IApiClient`, and configure backend `BaseAddress`.

- [ ] **Step 3: Build `Login.razor` and `Register.razor`**
Sakura-styled forms with client-side validation, error handling banner, and automatic redirection on success.

- [ ] **Step 4: Verify compilation**
Run: `dotnet build AniListingFront/AniListingFront.csproj`
Expected: Build succeeded.

---

### Task 8: Media Discovery & Details Pages

**Files:**
- Modify: `AniListingFront/Pages/Home.razor`
- Create: `AniListingFront/Pages/MediaDetails.razor`

**Interfaces:**
- Consumes: `/api/media/search`, `/api/media/{type}/{id}`, `/api/userlist`
- Produces:
  - Discovery search interface with dual English/Japanese name matching
  - Full media details page with interactive tracking card

- [ ] **Step 1: Implement `Home.razor`**
- Hero section with search bar and Anime / Manga switch.
- Search execution on enter or search button click.
- Results grid with high-res poster images, English & Japanese titles, MAL score badge, and quick link to details.

- [ ] **Step 2: Implement `MediaDetails.razor` (`/media/{Type}/{Id:int}`)**
- Left banner: Poster, status, score badge, episodes/chapters.
- Right content: Title, Japanese alternative title, genres chips, synopsis.
- Interactive Tracking Box:
  - If not logged in: Prompts user to log in to track.
  - If logged in: Status selector (*Watching/Reading, Completed, Plan to Watch/Read, Dropped*), Score selector (1 to 10), "Save to My List" button with feedback, and "Remove from List" button.

- [ ] **Step 3: Verify compilation**
Run: `dotnet build AniListingFront/AniListingFront.csproj`
Expected: Build succeeded.

---

### Task 9: Dedicated Anime List & Manga List Pages

**Files:**
- Create: `AniListingFront/Pages/AnimeList.razor` (`/anime-list`)
- Create: `AniListingFront/Pages/MangaList.razor` (`/manga-list`)

**Interfaces:**
- Consumes: `GET /api/userlist?mediaType=anime` and `GET /api/userlist?mediaType=manga`

- [ ] **Step 1: Implement `AnimeList.razor`**
- Header: "🌸 My Anime List" with total item count and average score.
- Status filters: *All*, *Watching*, *Completed*, *Plan to Watch*, *Dropped*.
- Sorting bar: Sort by *Title*, *My Score*, *Date Updated*, with Ascending / Descending toggle.
- Media items grid/cards with poster, title, user status, user rating, and direct remove action.

- [ ] **Step 2: Implement `MangaList.razor`**
- Header: "🌸 My Manga List" with total item count and average score.
- Status filters: *All*, *Reading*, *Completed*, *Plan to Read*, *Dropped*.
- Sorting bar: Sort by *Title*, *My Score*, *Date Updated*, with Ascending / Descending toggle.
- Media items grid/cards with poster, title, user status, user rating, and direct remove action.

- [ ] **Step 3: Verify compilation**
Run: `dotnet build AniListingFront/AniListingFront.csproj`
Expected: Build succeeded.

---

### Task 10: Documentation, Verification & Final Delivery

**Files:**
- Create: `README.md`

- [ ] **Step 1: Write `README.md`**
- Overview of AniListing and features.
- Quickstart instructions: running backend API and frontend WASM app concurrently.
- Explanation of smart dry-run mock mode vs. configuring live `MyAnimeList:ClientId` in `appsettings.json`.
- API endpoint summary and architecture overview.

- [ ] **Step 2: Run all backend tests**
Run: `dotnet test`
Expected: All tests pass.

- [ ] **Step 3: Run full solution build**
Run: `dotnet build`
Expected: Build succeeded with 0 errors.
