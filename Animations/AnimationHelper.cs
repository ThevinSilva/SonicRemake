using Newtonsoft.Json;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace SonicRemake.Animations
{
  public class AnimationHelper
  {
    private static readonly Log _log = new(typeof(AnimationHelper));

    public static Dictionary<string, AnimationData> Animations { get; set; } = new();

    public static void LoadAnimationsFromYaml(string yamlFile)
    {
      var deserializer = new DeserializerBuilder()
                  .WithNamingConvention(CamelCaseNamingConvention.Instance)
                  .Build();

      var rawData = deserializer.Deserialize<Dictionary<string, object>>(File.ReadAllText(yamlFile));

      foreach (var keyValue in rawData)
      {
        ProcessYamlValue(keyValue.Key, keyValue.Value);
      }
    }

    private static void ProcessYamlValue(string key, object value)
    {
      if (value is string stringValue)
      {
        AddAnimation(key, stringValue);
      }
      else if (value is List<object> itemList)
      {
        foreach (var item in itemList)
        {
          if (item is Dictionary<object, object> animationItem)
          {
            foreach (var detailKeyValue in animationItem)
            {
              AddAnimation($"{key}_{detailKeyValue.Key}", detailKeyValue.Value.ToString()!);
            }
          }
        }
      }
    }

    private static void AddAnimation(string name, string yamlString)
    {
      try
      {
        var animation = BuildAnimationDataFromYaml(name, yamlString);
        Animations.Add(animation.Name, animation);
        _log.Information($"Added animation: {animation.Name}");
      }
      catch (Exception ex)
      {
        _log.Error(ex, $"Error adding animation: {ex.Message}");
      }
    }

    private static AnimationData BuildAnimationDataFromYaml(string name, string yamlString)
    {
      var values = yamlString.Split(',').Select(x => x.Trim());

      if (values.Count() < 2)
        throw new Exception("Too few values in yaml string, expected at least 2") { Data = { { "name", name }, { "yamlString", yamlString } } };

      if (!int.TryParse(values.ElementAt(0), out int startFrameX))
        throw new Exception("Could not parse startFrameX") { Data = { { "name", name }, { "yamlString", yamlString } } };

      if (!int.TryParse(values.ElementAt(1), out int startFrameY))
        throw new Exception("Could not parse startFrameY") { Data = { { "name", name }, { "yamlString", yamlString } } };

      var numberOfSprites = values.Count() > 2 && int.TryParse(values.ElementAt(2), out int sprites) ? sprites : 1;
      var loops = values.Count() > 3 && int.TryParse(values.ElementAt(3), out int loopCount) ? loopCount : 0;

      return new AnimationData
      {
        Name = name,
        StartFrameX = startFrameX,
        StartFrameY = startFrameY,
        NumberOfSprites = numberOfSprites,
        Loops = loops,
        FramesPerSprite = 6 // TODO: Make this configurable
      };
    }
  }
}
