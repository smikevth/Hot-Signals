using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "Scriptable Objects/Dialogue")]
public class Dialogue : ScriptableObject
{
    public int Speaker; //0 for Captain 1 for Admiral
    [TextAreaAttribute]
    public string Text;
}
