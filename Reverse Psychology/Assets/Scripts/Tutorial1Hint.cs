using UnityEngine;

public class Tutorial1Hint : MonoBehaviour
{
    public GameObject moveLeftText;
    public GameObject moveRightText;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            moveLeftText.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            moveRightText.SetActive(false);
        }
    }
}
