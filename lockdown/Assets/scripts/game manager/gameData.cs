
using UnityEngine;

public class gameData : MonoBehaviour
{
    public static GameObject player;
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

}
