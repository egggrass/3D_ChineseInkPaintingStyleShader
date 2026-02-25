using UnityEngine;

public class DisplaySwitcher : MonoBehaviour
{
    public Camera camera1;
    public Camera camera2;

    private bool swapped = false;

    public void Swap()
    {
        if (Display.displays.Length < 2)
            return;

        swapped = !swapped;

        if (swapped)
        {
            camera1.targetDisplay = 1; // 去 Display2
            camera2.targetDisplay = 0; // 去 Display1
        }
        else
        {
            camera1.targetDisplay = 0;
            camera2.targetDisplay = 1;
        }
    }
}