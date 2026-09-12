# AniListing Design Specification

**Date**: 2026-09-11  
**Status**: Approved  
**Author**: Krzysztof & Antigravity  

---

## 1. Overview & Goals

AniListing is a full-stack web application designed for anime and manga enthusiasts to search, explore, track, and score their favorite anime and manga titles.

The application integrates with the official **MyAnimeList API v2**, features a delicate and aesthetic **Sakura Flowers** color palette, implements secure user authentication with password hashing and JWT authorization, and provides separate personal lists for Anime and Manga with sorting and filtering capabilities.

Because the user does not currently have an active MyAnimeList API Client ID, the application includes a **smart offline mock fallback** for dry testing. Once a client key is added to `appsettings.json`, it seamlessly switches to live MyAnimeList API v2 queries.

---

## 2. Technology Stack & Project Structure

- **Backend**: ASP.NET Core Web API (.NET 10)
  - **ORM**: Entity Framework Core with SQLite (`Microsoft.EntityFrameworkCore.Sqlite`)
  - **Authentication**: JWT Bearer (`Microsoft.AspNetCore.Authentication.JwtBearer`)
  - **Password Hashing**: ASP.NET Core `PasswordHasher<T>` (PBKDF2 HMAC-SHA512)
  - **HTTP Client**: Typed `HttpClient` targeting MyAnimeList API v2
- **Frontend**: Blazor WebAssembly (.NET 10)
  - **State / Auth**: Custom `AuthenticationStateProvider` using browser `localStorage`
  - **Styling**: Custom CSS design system implementing the **Sakura Flowers** theme
- **Solution Layout**:
  ```
  AniListing/
  ├── AniListingAPI/                      # ASP.NET Core Web API
  │   ├── Controllers/
  │   │   ├── AuthController.cs
  │   │   ├── MediaController.cs
  │   │   └── UserListController.cs
  │   ├── Data/
  │   │   ├── AniListingDbContext.cs
  │   │   └── Entities/
  │   │       ├── User.cs
  │   │       └── UserMediaItem.cs
  │   ├── DTOs/
  │   │   ├── AuthDtos.cs
  │   │   ├── MediaDtos.cs
  │   │   └── UserListDtos.cs
  │   ├── Services/
  │   │   ├── IMyAnimeListService.cs
  │   │   ├── MyAnimeListService.cs
  │   │   └── MockMediaCatalog.cs
  │   ├── Program.cs
  │   ├── appsettings.json
  │   └── AniListingAPI.csproj
  ├── AniListingFront/                    # Blazor WebAssembly
  │   ├── Layout/
  │   │   ├── MainLayout.razor
  │   │   ├── MainLayout.razor.css
  │   │   └── NavMenu.razor
  │   ├── Pages/
  │   │   ├── Home.razor (Search & Discover)
  │   │   ├── MediaDetails.razor (/media/{Type}/{Id})
  │   │   ├── AnimeList.razor (/anime-list)
  │   │   ├── MangaList.razor (/manga-list)
  │   │   ├── Login.razor (/login)
  │   │   └── Register.razor (/register)
  │   ├── Services/
  │   │   ├── IApiClient.cs
  │   │   ├── ApiClient.cs
  │   │   └── CustomAuthStateProvider.cs
  │   ├── wwwroot/
  │   │   ├── css/
  │   │   │   └── app.css             # Sakura theme styles & animations
  │   │   └── index.html
  │   ├── App.razor
  │   ├── Program.cs
  │   └── AniListingFront.csproj
  ├── .gitignore
  └── README.md
  ```

---

## 3. Data Models & Database Architecture

### 3.1 Entity Framework Core Configuration
SQLite database file: `anilisting.db` (auto-created via EF Core `EnsureCreatedAsync` on backend startup).

### 3.2 Entities

#### `User`
| Property | Type | Details |
|----------|------|---------|
| `Id` | `int` | Primary Key, Auto-increment |
| `Username` | `string` | Unique, Indexed, 3–30 characters |
| `PasswordHash` | `string` | Salted PBKDF2 HMAC-SHA512 hash |
| `CreatedAt` | `DateTime` | UTC timestamp |
| `MediaItems` | `ICollection<UserMediaItem>` | Navigation collection |

#### `UserMediaItem`
| Property | Type | Details |
|----------|------|---------|
| `Id` | `int` | Primary Key, Auto-increment |
| `UserId` | `int` | Foreign Key to `User` |
| `MalId` | `int` | MyAnimeList identifier |
| `MediaType` | `string` | `"anime"` or `"manga"` |
| `Title` | `string` | Title (English/Romaji) |
| `JapaneseTitle` | `string?` | Original Japanese kanji/kana title |
| `PosterUrl` | `string?` | Image URL |
| `Status` | `string` | Anime: `Watching`, `Completed`, `PlanToWatch`, `Dropped`<br>Manga: `Reading`, `Completed`, `PlanToRead`, `Dropped` |
| `Score` | `int?` | Personal rating (1–10) |
| `CreatedAt` | `DateTime` | UTC timestamp |
| `UpdatedAt` | `DateTime` | UTC timestamp updated on edits |

Unique constraint on `(UserId, MalId, MediaType)`.

---

## 4. Security & Authentication

### 4.1 Registration & Login Security
- Passwords are never stored in plain text. Hashed using ASP.NET Core `IPasswordHasher<User>`.
- Client and server-side validation:
  - Username: 3 to 30 characters, alphanumeric and basic punctuation (`_`, `-`).
  - Password: Minimum 6 characters.
- Login timing attack mitigation: Password verification is performed consistently even if the user lookup fails, or constant-time comparison is used.

### 4.2 JWT Token Generation & Protection
- Key: Configurable 256-bit symmetric secret in `appsettings.json` (`Jwt:Key`), with sensible fallback for local development.
- Issuer & Audience: `AniListingAPI` / `AniListingFront`.
- Expiry: 7 days.
- Claims:
  - `ClaimTypes.NameIdentifier`: User ID
  - `ClaimTypes.Name`: Username
- Protected endpoints require `[Authorize]`.

---

## 5. MyAnimeList API v2 Integration & Smart Mock Fallback

### 5.1 MyAnimeList API Client (`MyAnimeListService`)
- Base URL: `https://api.myanimelist.net/v2`
- Authorization: HTTP Header `X-MAL-CLIENT-ID: <ClientId>`
- Config section in `appsettings.json`:
  ```json
  "MyAnimeList": {
    "ClientId": ""
  }
  ```

### 5.2 Smart Offline Fallback Mode
When `ClientId` is empty, whitespace, or `"YOUR_MAL_CLIENT_ID"`, `MyAnimeListService` operates in **Dry-Run / Mock Mode**:
- Provides a curated catalog of top anime and manga:
  - **Anime**: *Frieren: Beyond Journey's End* (葬送のフリーレン), *Fullmetal Alchemist: Brotherhood* (鋼の錬金術師), *Spirited Away* (千と千尋の神隠し), *Attack on Titan* (進撃の巨人), *Demon Slayer: Kimetsu no Yaiba* (鬼滅の刃), *Spy x Family* (SPY×FAMILY).
  - **Manga**: *Berserk* (ベルセルク), *One Piece* (ONE PIECE), *Chainsaw Man* (チェンソーマン), *Monster* (MONSTER), *Vinland Saga* (ヴィンランド・サガ).
- Supports searching across English, Romaji, and Japanese titles (case-insensitive substring match).
- Returns the exact same DTO structures as the live MAL API v2 so frontend code is identical in both modes.

---

## 6. Backend REST API Endpoints

### 6.1 Authentication (`AuthController`)
- `POST /api/auth/register` — Body: `{ username, password }` -> Returns: `{ token, username, userId }`
- `POST /api/auth/login` — Body: `{ username, password }` -> Returns: `{ token, username, userId }`
- `GET /api/auth/me` [Authorize] — Returns current user identity

### 6.2 Media Discovery (`MediaController`)
- `GET /api/media/search?type={anime|manga}&q={query}` — Search anime or manga by English/Japanese title.
- `GET /api/media/{type}/{id}` — Get full details (poster, English & Japanese titles, score, synopsis, episodes/chapters, genres).

### 6.3 User Lists (`UserListController` - All [Authorize])
- `GET /api/userlist?mediaType={anime|manga}&status={status}&sortBy={name|score|date}&sortOrder={asc|desc}` — Fetch filtered & sorted personal list items for the authenticated user.
- `GET /api/userlist/check/{mediaType}/{malId}` — Check if the current user already has this item saved (returns status and score, or 404 if not found).
- `POST /api/userlist` — Add or update an item in the user's list (with status and score).
- `DELETE /api/userlist/{mediaType}/{malId}` — Remove an item from the user's list.

---

## 7. Blazor WebAssembly Frontend & Sakura Theme

### 7.1 Sakura Flowers Design System
- **Color Variables**:
  - `--sakura-50`: `#FFF8FA` (Background body)
  - `--sakura-100`: `#FCE4EC` (Petal tint / Card borders)
  - `--sakura-200`: `#F8BBD0` (Soft highlight / Active tabs)
  - `--sakura-300`: `#F48FB1` (Subtle accent)
  - `--sakura-500`: `#E85D75` (Primary floral button & brand color)
  - `--sakura-600`: `#D81B60` (Primary hover / Active state)
  - `--sakura-900`: `#4A154B` (Deep plum header / High contrast elements)
  - `--text-primary`: `#2E1C2B` (Dark plum text, WCAG AAA compliant)
  - `--text-muted`: `#7C5C70`
  - `--star-gold`: `#FFA000` (Score ratings)
- **Visual Flourishes**:
  - Subtle floating sakura petal animation / SVG motifs in header banner.
  - Smooth rounded pill-shaped badges for status and media types.
  - Polished responsive grid of media cards with zoom/elevation hover effects.

### 7.2 Separate Lists User Experience
Per requirements, **Anime** and **Manga** lists are separate first-class pages:
1. **🌸 My Anime List** (`/anime-list`):
   - Status filters: *All*, *Watching*, *Completed*, *Plan to Watch*, *Dropped*.
   - Sorting dropdown:
     - Sort by: *Title (Name)*, *Score (High to Low / Low to High)*, *Date Updated*.
   - Anime-specific details: Episodes watched / total count, custom score (1–10).
2. **🌸 My Manga List** (`/manga-list`):
   - Status filters: *All*, *Reading*, *Completed*, *Plan to Read*, *Dropped*.
   - Sorting dropdown:
     - Sort by: *Title (Name)*, *Score (High to Low / Low to High)*, *Date Updated*.
   - Manga-specific details: Chapters/volumes read, custom score (1–10).

### 7.3 Discovery & Media Details
- **Home / Search (`/`)**:
  - Search input with category switch: Anime vs. Manga.
  - Live result card display with poster, dual English/Japanese titles, mean score badge, and quick access.
- **Media Details (`/media/{type}/{id}`)**:
  - Hero banner with poster, high-res details, synopses, alternative titles, genres, and metadata.
  - Interactive "Personal Tracking" box:
    - If unauthenticated: prompt with button directing to `/login`.
    - If authenticated: instant status selection, star/number score rating (1 to 10), "Save to My List", and "Remove" action.

---

## 8. Verification & Testing Strategy

1. **Compilation & Static Checks**:
   - Both `AniListingAPI` and `AniListingFront` compile cleanly with zero errors.
2. **Authentication Flow Verification**:
   - Register a new user (`testuser`, password).
   - Login, receive JWT, and verify claims extraction in Blazor `AuthenticationStateProvider`.
3. **Dry-Run Search & Media Display Verification**:
   - Search for "Frieren" or "葬送のフリーレン" under Anime -> verify mock returns result.
   - Search for "One Piece" or "Berserk" under Manga -> verify result.
   - Click to open Media Details page -> verify poster, description, score, and title rendering.
4. **List Management & Sorting Verification**:
   - Add anime to *Watching* with score 9.
   - Add another anime to *Completed* with score 10.
   - Go to `/anime-list` -> verify both appear.
   - Switch status filter to *Completed* -> verify only the completed anime appears.
   - Change sort order to Score (Descending) vs Name (Ascending) -> verify order updates properly.
   - Repeat for `/manga-list`.
   - Remove item -> verify list updates.
5. **Configuration & Documentation**:
   - Create `.gitignore` ignoring build artifacts, SQLite `.db`, and IDE folders.
   - Create clear `README.md` with startup instructions for both API and Frontend, and instructions on plugging in the real MyAnimeList API key.
