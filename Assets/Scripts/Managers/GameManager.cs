using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GamePauser pauser;
    // Start is called before the first frame update
    void Start()
    {
        pauser.PauseGame(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
