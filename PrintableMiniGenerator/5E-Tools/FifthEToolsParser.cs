using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Drawing;
using System.Collections.Generic;
using System;
using System.ComponentModel;

namespace PrintableMiniGenerator
{
    public partial class FifthEToolsParser : INotifyPropertyChanged
    {
        private static readonly HttpClient _httpClient = new();
        private readonly List<Monster> _monsters = new();

        private List<Monster> _filteredMonsterList;
        public List<Monster> FilteredMonsterList
        {
            get => _filteredMonsterList;
            set
            {
                _filteredMonsterList = value;
                NotifyPropertyChanged(nameof(FilteredMonsterList));
            }
        }

        private bool _isLoaded;
        public bool IsLoaded
        {
            get => _isLoaded;
            set
            {
                _isLoaded = value;
                NotifyPropertyChanged(nameof(IsLoaded));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public FifthEToolsParser()
        {
            Initialize();
        }

        public void FilterMonsterList(string filter)
        {
            FilteredMonsterList = _monsters.FindAll(f => { return f.Name.Contains(filter.ToLowerInvariant()); });
        }

        private async void Initialize()
        {
            JsonDocument index = await GetMonsterIndex();

            foreach (JsonProperty element in index.RootElement.EnumerateObject())
            {
                List<Monster> monsters = await GetMonstersFromSource(element.Value.ToString());
                if (monsters != null)
                {
                    _monsters.AddRange(monsters);
                }
            }

            Monster.ResolveDependentMonsters(_monsters);
            Monster.RemoveNoImageMonsters(_monsters);
            FilterMonsterList("");

            IsLoaded = true;
        }

        private static async Task<JsonDocument> GetMonsterIndex()
        {
            HttpResponseMessage sourcesResponse = await _httpClient.GetAsync("https://5e.tools/data/bestiary/index.json");
            return sourcesResponse.IsSuccessStatusCode
                ? await JsonDocument.ParseAsync(await sourcesResponse.Content.ReadAsStreamAsync())
                : throw new HttpRequestException("Monster index could not be loaded");
        }

        private static async Task<List<Monster>> GetMonstersFromSource(string source)
        {
            Uri uri = new("https://5e.tools/data/bestiary/" + source);
            HttpResponseMessage monstersResponse = await _httpClient.GetAsync(uri);
            if (monstersResponse.IsSuccessStatusCode)
            {
                JsonDocument monsters = await JsonDocument.ParseAsync(await monstersResponse.Content.ReadAsStreamAsync());
                return Monster.GenerateList(monsters.RootElement.GetProperty("monster"));
            }
            else
            {
                throw new HttpRequestException("Monster list could not be loaded from source: " + source);
            }
        }

    }
}
