using UnityEngine;


public static class HelperUtility
{
    public static Color GetColorByType(KnoxColorType knoxColorType)
    {
        return knoxColorType switch
        {
            KnoxColorType.Red => Color.red,
            KnoxColorType.Green => Color.green,
            KnoxColorType.Blue => Color.blue,
            KnoxColorType.Yellow => Color.yellow,
            KnoxColorType.Orange => Color.orange,
            KnoxColorType.Cyan => Color.cyan,
            KnoxColorType.Pink => Color.deepPink,
            KnoxColorType.Brown => Color.brown,
            KnoxColorType.Purple => Color.mediumPurple,
            KnoxColorType.White => Color.white,
            _ => Color.black,
        };
    }
}
