using UnityEngine;

public interface IStorable
{
    Texture2D Icon { get; }
    string Name { get; }
    int StackMax { get; }
    int StackCount { get; }

    bool IsSameAs(IStorable storable);
}
