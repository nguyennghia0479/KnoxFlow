using UnityEngine;

public class Cell : MonoBehaviour
{
    private int coordX;
    private int coordY;

    public void SetPosition(int coordX, int coordY)
    {
        this.coordX = coordX;
        this.coordY = coordY;

        gameObject.name = $"Cell_{coordX}_{coordY}";
    }
}
