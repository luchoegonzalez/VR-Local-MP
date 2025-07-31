using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using Newtonsoft.Json;

// FROM https://github.com/jitbit/JsonIgnoreProps/

/// <summary>
/// Helper to ignore some properties from serialization
/// </summary>
public class IgnorePropertiesResolver : DefaultContractResolver
{
    private HashSet<string> _propsToIgnore;

    public IgnorePropertiesResolver(IEnumerable<string> propNamesToIgnore)
    {
        _propsToIgnore = new HashSet<string>(propNamesToIgnore);
    }

    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        JsonProperty property = base.CreateProperty(member, memberSerialization);

        if (_propsToIgnore.Contains(property.PropertyName))
        {
            property.ShouldSerialize = (x) => { return false; };
        }

        return property;
    }
}
