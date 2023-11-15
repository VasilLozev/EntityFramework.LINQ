namespace MusicHub
{
    using System;
    using System.Text;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;
    using MusicHub.Data.Models;

    public class StartUp
    {
        public static void Main()
        {

            MusicHubDbContext context = new MusicHubDbContext();

            // DbInitializer.ResetDatabase(context);

            Console.WriteLine(ExportAlbumsInfo(context, 9));
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            var albumInfo = context.Producers
                .Include(x=>x.Albums).ThenInclude(a=>a.Songs)
                .ThenInclude(s=>s.Writer)
                .First(x => x.Id == producerId)
                .Albums.Select(x => new
                {
                    AlbumName = x.Name,
                    ReleaseDate = x.ReleaseDate,
                    ProducerName = x.Producer.Name,
                    AlbumSongs = x.Songs.Select(x => new
                    {
                        SongName = x.Name,
                        SongPrice = x.Price,
                        SongWriterName = x.Writer.Name
                    })
                    .OrderByDescending(x => x.SongName)
                    .ThenBy(x => x.SongWriterName),
                    TotalAlbumPrice = x.Price
                }).OrderByDescending(x => x.TotalAlbumPrice).AsEnumerable();

            StringBuilder stringBuilder = new StringBuilder();

            foreach (var album in albumInfo)
            {
                stringBuilder
                    .AppendLine($"-AlbumName: {album.AlbumName}")
                    .AppendLine($"-ReleaseDate: {album.ReleaseDate.ToString("MM/dd/yyyy")}")
                    .AppendLine($"-ProducerName: {album.ProducerName}")
                    .AppendLine($"-Songs:");
                if (album.AlbumSongs.Any())
                {
                    int songNumber = 1;
                    foreach (var song in album.AlbumSongs)
                    {
                        stringBuilder
                            .AppendLine($"---#{songNumber++}")
                            .AppendLine($"---SongName: {song.SongName}")
                            .AppendLine($"---Price: {song.SongPrice:f2}")
                            .AppendLine($"---Writer: {song.SongWriterName}");
                    }
                }
                stringBuilder
                    .AppendLine($"-AlbumPrice: {album.TotalAlbumPrice:f2}");
            }

            string res = stringBuilder.ToString().Trim();
             return res;
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            throw new NotImplementedException();
        }
    }
}
