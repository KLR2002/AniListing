using AniListingAPI.DTOs;

namespace AniListingAPI.Services;

public static class MockMediaCatalog
{
    public static readonly List<MediaDetailsDto> Catalog = new()
    {
        // --- ANIME ---
        new MediaDetailsDto(
            Id: 52991,
            MediaType: "anime",
            Title: "Sousou no Frieren",
            JapaneseTitle: "葬送のフリーレン",
            EnglishTitle: "Frieren: Beyond Journey's End",
            PosterUrl: "https://cdn.myanimelist.net/images/anime/1015/138006.jpg",
            MeanScore: 9.38,
            Rank: 1,
            Popularity: 180,
            Status: "finished_airing",
            Synopsis: "During their decade-long quest to defeat the Demon King, the members of the hero's party—Himmel, Eisen, Heiter, and Frieren—bound themselves together through countless battles. After bringing peace to the land, they part ways, but for the elf Frieren, decades are merely a passing instant. Decades later, she attends the funeral of Himmel and regrets not taking the time to understand humans. Thus begins her journey towards the resting place of souls.",
            EpisodeCount: 28,
            ChapterCount: null,
            VolumeCount: null,
            StartDate: "2023-09-29",
            EndDate: "2024-03-22",
            Genres: new List<string> { "Adventure", "Drama", "Fantasy" },
            StudioOrAuthor: "Madhouse"
        ),
        new MediaDetailsDto(
            Id: 5114,
            MediaType: "anime",
            Title: "Fullmetal Alchemist: Brotherhood",
            JapaneseTitle: "鋼の錬金術師 FULLMETAL ALCHEMIST",
            EnglishTitle: "Fullmetal Alchemist: Brotherhood",
            PosterUrl: "https://cdn.myanimelist.net/images/anime/1208/94745.jpg",
            MeanScore: 9.10,
            Rank: 2,
            Popularity: 3,
            Status: "finished_airing",
            Synopsis: "After a horrific alchemy experiment goes wrong in the Elric household, brothers Edward and Alphonse are left in a catastrophic new reality. Ignoring the alchemical restriction against human transmutation, the boys attempted to bring their recently deceased mother back to life. Instead, they suffered brutal personal loss: Alphonse's body disintegrated while Edward lost a leg and then sacrificed an arm to keep Alphonse's soul in the physical realm by binding it to a hulking suit of armor.",
            EpisodeCount: 64,
            ChapterCount: null,
            VolumeCount: null,
            StartDate: "2009-04-05",
            EndDate: "2010-07-04",
            Genres: new List<string> { "Action", "Adventure", "Drama", "Fantasy" },
            StudioOrAuthor: "Bones"
        ),
        new MediaDetailsDto(
            Id: 199,
            MediaType: "anime",
            Title: "Sen to Chihiro no Kamikakushi",
            JapaneseTitle: "千と千尋の神隠し",
            EnglishTitle: "Spirited Away",
            PosterUrl: "https://cdn.myanimelist.net/images/anime/8/73559.jpg",
            MeanScore: 8.78,
            Rank: 38,
            Popularity: 45,
            Status: "finished_airing",
            Synopsis: "Stubborn, spoiled, and naive, 10-year-old Chihiro Ogino is less than pleased when she and her parents discover an abandoned amusement park on the way to their new house. Cautiously venturing inside, she realizes there is more to this place than meets the eye, as strange things begin to happen once dusk falls: ghostly apparitions and a food stall turning her parents into pigs.",
            EpisodeCount: 1,
            ChapterCount: null,
            VolumeCount: null,
            StartDate: "2001-07-20",
            EndDate: "2001-07-20",
            Genres: new List<string> { "Adventure", "Award Winning", "Supernatural" },
            StudioOrAuthor: "Studio Ghibli"
        ),
        new MediaDetailsDto(
            Id: 16498,
            MediaType: "anime",
            Title: "Shingeki no Kyojin",
            JapaneseTitle: "進撃の巨人",
            EnglishTitle: "Attack on Titan",
            PosterUrl: "https://cdn.myanimelist.net/images/anime/10/47347.jpg",
            MeanScore: 8.55,
            Rank: 104,
            Popularity: 1,
            Status: "finished_airing",
            Synopsis: "Centuries ago, mankind was slaughtered to near extinction by monstrous humanoid creatures called Titans, forcing humans to hide in fear behind enormous concentric walls. What makes these giants truly terrifying is that their taste for human flesh is not born out of hunger but what appears to be out of pleasure.",
            EpisodeCount: 25,
            ChapterCount: null,
            VolumeCount: null,
            StartDate: "2013-04-07",
            EndDate: "2013-09-29",
            Genres: new List<string> { "Action", "Award Winning", "Suspense" },
            StudioOrAuthor: "Wit Studio"
        ),
        new MediaDetailsDto(
            Id: 38000,
            MediaType: "anime",
            Title: "Kimetsu no Yaiba",
            JapaneseTitle: "鬼滅の刃",
            EnglishTitle: "Demon Slayer: Kimetsu no Yaiba",
            PosterUrl: "https://cdn.myanimelist.net/images/anime/1286/99889.jpg",
            MeanScore: 8.48,
            Rank: 130,
            Popularity: 4,
            Status: "finished_airing",
            Synopsis: "Ever since the death of his father, the burden of supporting the family has fallen upon Tanjirou Kamado's shoulders. Although living impoverished on a remote mountain, the Kamado family are able to enjoy a relatively peaceful and happy life. One day, Tanjirou decides to go down to the local village to make a little money selling charcoal. On his way back, night falls, forcing Tanjirou to take shelter in the house of a strange man, who warns him of the existence of flesh-eating demons that lurk in the woods at night.",
            EpisodeCount: 26,
            ChapterCount: null,
            VolumeCount: null,
            StartDate: "2019-04-06",
            EndDate: "2019-09-28",
            Genres: new List<string> { "Action", "Fantasy" },
            StudioOrAuthor: "ufotable"
        ),
        new MediaDetailsDto(
            Id: 50265,
            MediaType: "anime",
            Title: "Spy x Family",
            JapaneseTitle: "SPY×FAMILY",
            EnglishTitle: "Spy x Family",
            PosterUrl: "https://cdn.myanimelist.net/images/anime/1441/122795.jpg",
            MeanScore: 8.50,
            Rank: 120,
            Popularity: 48,
            Status: "finished_airing",
            Synopsis: "Corrupt politicians, frenzied nationalists, and other warmongering forces constantly jeopardize the thin veneer of peace between neighboring countries Ostania and Westalis. In order to stop extremist Desmond Donovan, master spy Twilight adopts an orphan girl named Anya and marries a woman named Yor, unknowingly creating a family of a spy, telepath, and assassin.",
            EpisodeCount: 12,
            ChapterCount: null,
            VolumeCount: null,
            StartDate: "2022-04-09",
            EndDate: "2022-06-25",
            Genres: new List<string> { "Action", "Comedy" },
            StudioOrAuthor: "Wit Studio & CloverWorks"
        ),

        // --- MANGA ---
        new MediaDetailsDto(
            Id: 2,
            MediaType: "manga",
            Title: "Berserk",
            JapaneseTitle: "ベルセルク",
            EnglishTitle: "Berserk",
            PosterUrl: "https://cdn.myanimelist.net/images/manga/1/157897.jpg",
            MeanScore: 9.47,
            Rank: 1,
            Popularity: 1,
            Status: "publishing",
            Synopsis: "Guts, a former mercenary now known as the 'Black Swordsman,' is out for revenge. After a tumultuous childhood, he finally finds someone he respects and believes he can trust in Griffith, the charismatic leader of the Band of the Hawk mercenary group. But tragedy and betrayal seal Guts' dark path.",
            EpisodeCount: null,
            ChapterCount: null,
            VolumeCount: null,
            StartDate: "1989-08-25",
            EndDate: null,
            Genres: new List<string> { "Action", "Adventure", "Award Winning", "Drama", "Fantasy", "Horror" },
            StudioOrAuthor: "Miura, Kentarou"
        ),
        new MediaDetailsDto(
            Id: 13,
            MediaType: "manga",
            Title: "One Piece",
            JapaneseTitle: "ONE PIECE",
            EnglishTitle: "One Piece",
            PosterUrl: "https://cdn.myanimelist.net/images/manga/2/253146.jpg",
            MeanScore: 9.22,
            Rank: 3,
            Popularity: 3,
            Status: "publishing",
            Synopsis: "Gol D. Roger, a man referred to as the 'King of the Pirates,' is poised to be executed by the World Government. But just before his demise, he confirms the existence of a great treasure, One Piece, hidden somewhere within the vast ocean expanse known as the Grand Line. Monkey D. Luffy sets sail to become the new Pirate King!",
            EpisodeCount: null,
            ChapterCount: null,
            VolumeCount: null,
            StartDate: "1997-07-22",
            EndDate: null,
            Genres: new List<string> { "Action", "Adventure", "Fantasy" },
            StudioOrAuthor: "Oda, Eiichiro"
        ),
        new MediaDetailsDto(
            Id: 116778,
            MediaType: "manga",
            Title: "Chainsaw Man",
            JapaneseTitle: "チェンソーマン",
            EnglishTitle: "Chainsaw Man",
            PosterUrl: "https://cdn.myanimelist.net/images/manga/3/216464.jpg",
            MeanScore: 8.70,
            Rank: 42,
            Popularity: 2,
            Status: "publishing",
            Synopsis: "Denji has a simple dream—to live a happy and peaceful life, spending time with a girl he likes. This is a far cry from reality, however, as Denji is forced by the yakuza into killing devils in order to pay off his crushing debts. Using his pet devil Pochita as a weapon, he is ready to do anything for a bit of cash.",
            EpisodeCount: null,
            ChapterCount: null,
            VolumeCount: null,
            StartDate: "2018-12-03",
            EndDate: null,
            Genres: new List<string> { "Action", "Supernatural" },
            StudioOrAuthor: "Fujimoto, Tatsuki"
        ),
        new MediaDetailsDto(
            Id: 1,
            MediaType: "manga",
            Title: "Monster",
            JapaneseTitle: "MONSTER",
            EnglishTitle: "Monster",
            PosterUrl: "https://cdn.myanimelist.net/images/manga/3/258224.jpg",
            MeanScore: 9.15,
            Rank: 5,
            Popularity: 25,
            Status: "finished",
            Synopsis: "Kenzou Tenma, an elite brain surgeon recently engaged to his hospital director's daughter, is well on his way to ascending the hospital hierarchy. That is until one night, a small boy named Johan Liebert is rushed into his clinic requiring immediate cranial surgery. Tenma chooses to operate on the boy over the town's mayor, completely transforming his life.",
            EpisodeCount: null,
            ChapterCount: 162,
            VolumeCount: 18,
            StartDate: "1994-12-05",
            EndDate: "2001-12-20",
            Genres: new List<string> { "Award Winning", "Drama", "Mystery", "Psychological" },
            StudioOrAuthor: "Urasawa, Naoki"
        ),
        new MediaDetailsDto(
            Id: 642,
            MediaType: "manga",
            Title: "Vinland Saga",
            JapaneseTitle: "ヴィンランド・サガ",
            EnglishTitle: "Vinland Saga",
            PosterUrl: "https://cdn.myanimelist.net/images/manga/2/188040.jpg",
            MeanScore: 9.06,
            Rank: 8,
            Popularity: 15,
            Status: "publishing",
            Synopsis: "Thorfinn, son of one of the Vikings' greatest warriors, among a band of mercenaries led by the cunning Askeladd, an assassin who killed his father. Thorfinn swears to take his revenge in a fair duel, but is caught in a war for the crown of England.",
            EpisodeCount: null,
            ChapterCount: null,
            VolumeCount: null,
            StartDate: "2005-04-13",
            EndDate: null,
            Genres: new List<string> { "Action", "Adventure", "Drama" },
            StudioOrAuthor: "Yukimura, Makoto"
        )
    };

    public static List<MediaItemDto> Search(string mediaType, string query)
    {
        var targetType = mediaType.ToLower();
        var q = query.Trim().ToLower();

        var queryList = Catalog.Where(m => m.MediaType.Equals(targetType, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(q))
        {
            queryList = queryList.Where(m =>
                m.Title.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                (m.EnglishTitle != null && m.EnglishTitle.Contains(q, StringComparison.OrdinalIgnoreCase)) ||
                (m.JapaneseTitle != null && m.JapaneseTitle.Contains(q, StringComparison.OrdinalIgnoreCase)) ||
                (m.StudioOrAuthor != null && m.StudioOrAuthor.Contains(q, StringComparison.OrdinalIgnoreCase)) ||
                m.Genres.Any(g => g.Contains(q, StringComparison.OrdinalIgnoreCase))
            );
        }

        return queryList.Select(ToMediaItemDto).ToList();
    }

    public static MediaDetailsDto? GetDetails(string mediaType, int id)
    {
        return Catalog.FirstOrDefault(m =>
            m.Id == id &&
            m.MediaType.Equals(mediaType, StringComparison.OrdinalIgnoreCase));
    }

    public static MediaItemDto ToMediaItemDto(MediaDetailsDto d)
    {
        return new MediaItemDto(
            Id: d.Id,
            MediaType: d.MediaType,
            Title: d.Title,
            JapaneseTitle: d.JapaneseTitle,
            EnglishTitle: d.EnglishTitle,
            PosterUrl: d.PosterUrl,
            MeanScore: d.MeanScore,
            Status: d.Status,
            EpisodeCount: d.EpisodeCount,
            ChapterCount: d.ChapterCount,
            VolumeCount: d.VolumeCount,
            Synopsis: d.Synopsis,
            Genres: d.Genres
        );
    }
}
