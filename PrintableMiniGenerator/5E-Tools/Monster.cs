using System.Text.Json;
using System.Drawing;
using System.Collections.Generic;

namespace PrintableMiniGenerator
{
    public partial class FifthEToolsParser
    {
        public class Monster
        {
            public readonly string Name;
            public readonly string Source;
            public MonsterSize Size { get; private set; }
            private readonly ImageSource _imageSource;

            public readonly string CopyName;
            public readonly string CopySource;
            private bool _hasUnresolvedDependency;
            private static readonly List<Monster> _dependentMonsters = new();

            public Monster(JsonElement jsonElement, ImageSource imageSource)
            {
                Name = jsonElement.GetProperty("name").GetString();
                Source = jsonElement.GetProperty("source").GetString();
                if (jsonElement.TryGetProperty("_copy", out JsonElement copy))
                {
                    _hasUnresolvedDependency = true;
                    _dependentMonsters.Add(this);
                    CopyName = copy.GetProperty("name").GetString();
                    CopySource = copy.GetProperty("source").GetString();
                }
                else
                {
                    Size = jsonElement.GetProperty("size").GetString() switch
                    {
                        "L" => MonsterSize.Large,
                        "H" => MonsterSize.Huge,
                        "G" => MonsterSize.Gargantuan,
                        _ => MonsterSize.Medium,
                    };
                    _imageSource = imageSource;
                }
            }

            public static void ResolveDependentMonsters(List<Monster> fullMonsterList)
            {
                fullMonsterList.Sort(new MonsterComparer());

                foreach (Monster monster in _dependentMonsters)
                {
                    if (monster._hasUnresolvedDependency)
                    {
                        monster.UpdateSizeFromDependency(fullMonsterList);
                    }
                }
            }

            private void UpdateSizeFromDependency(List<Monster> fullMonsterList)
            {
                int index = fullMonsterList.BinarySearch(this, new DependencyComparer());
                if (fullMonsterList[index]._hasUnresolvedDependency)
                {
                    fullMonsterList[index].UpdateSizeFromDependency(fullMonsterList);
                }

                _hasUnresolvedDependency = false;
                Size = fullMonsterList[index].Size;
            }

            public static List<Monster> GenerateList(JsonElement MonsterArray)
            {
                List<Monster> monsters = new();

                foreach (JsonElement monster in MonsterArray.EnumerateArray())
                {
                    
                    ImageSource source = ImageSource.None;

                    if (monster.TryGetProperty("hasFluffImages", out JsonElement hasFluffImages) && hasFluffImages.GetBoolean())
                    {
                        source = ImageSource.Fluff;
                    }

                    if (monster.TryGetProperty("hasToken", out JsonElement hasToken) && hasToken.GetBoolean())
                    {
                        source = source == ImageSource.None ? ImageSource.Token : ImageSource.Both;
                    }

                    monsters.Add(new Monster(monster, source));
                }
                return monsters;
            }

            public static void RemoveNoImageMonsters(List<Monster> monsters)
            {
                _ = monsters.RemoveAll(e => { return e._imageSource == ImageSource.None; });
            }

            public Bitmap GetImage()
            {
                return null;
            }
        }

        public enum MonsterSize
        {
            Medium,
            Large,
            Huge,
            Gargantuan
        }

        public enum ImageSource
        {
            None,
            Fluff,
            Token,
            Both
        }

        private class DependencyComparer : IComparer<Monster>
        {
            public int Compare(Monster x, Monster y)
            {
                int nameCompare = string.CompareOrdinal(x.Name.ToLowerInvariant(), y.CopyName.ToLowerInvariant());
                return x.Name == y.Name && nameCompare == 0
                    ? string.CompareOrdinal(x.Source.ToLowerInvariant(), y.CopySource.ToLowerInvariant())
                    : nameCompare;
            }
        }

        private class MonsterComparer : IComparer<Monster>
        {
            public int Compare(Monster x, Monster y)
            {
                int nameCompare = string.CompareOrdinal(x.Name.ToLowerInvariant(), y.Name.ToLowerInvariant());
                return nameCompare == 0 ? string.CompareOrdinal(x.Source.ToLowerInvariant(), y.Source.ToLowerInvariant()) : nameCompare;
            }
        }
    }
}
