using Newtonsoft.Json;
using StringEncoding.App.Models;
using System.IO;

namespace StringEncoding.App.Helpers
{
    public static class LocalStorage
    {
        private static string folderPath = ".\\Storage";
        private static string filePath;

        static LocalStorage()
        {
            filePath = $"{folderPath}\\Profiles.json";
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            if (!File.Exists(filePath))
            {
                using StreamWriter sw = new StreamWriter(filePath);
                sw.WriteLine("[]");
            }
        }

        public static async Task<List<Profile>> GetProfiles()
        {
            using StreamReader sr = new StreamReader(filePath);
            string profilesString = await sr.ReadToEndAsync();
            return JsonConvert.DeserializeObject<List<Profile>>(profilesString)?? new List<Profile>();
        }

        public static async Task AddProfile(Profile profile)
        {
            List<Profile> profiles = new List<Profile>();
            using (StreamReader sr = new StreamReader(filePath))
            {
                string profilesString = await sr.ReadToEndAsync();
                profiles = JsonConvert.DeserializeObject<List<Profile>>(profilesString)?? profiles;
            }

            if (profiles.Any(p => p.Name == profile.Name))
            {
                int index = profiles.FindIndex(p => p.Name == profile.Name);
                profiles[index].MinLenght = profile.MinLenght;
                profiles[index].Salt = profile.Salt;
            }
            else
            {
                profiles.Add(profile);
            }

            using StreamWriter sw = new StreamWriter(filePath);
            await sw.WriteAsync(JsonConvert.SerializeObject(profiles));
        }

        public static async Task DeleteProfile(Profile profile)
        {
            List<Profile> profiles = new List<Profile>();
            using (StreamReader sr = new StreamReader(filePath))
            {
                string profilesString = await sr.ReadToEndAsync();
                profiles = JsonConvert.DeserializeObject<List<Profile>>(profilesString) ?? profiles;
            }

            Profile fromStorage = profiles.FirstOrDefault(p => p.Name == profile.Name);
            profiles.Remove(fromStorage);

            using StreamWriter sw = new StreamWriter(filePath);
            await sw.WriteAsync(JsonConvert.SerializeObject(profiles));
        }
    }
}
