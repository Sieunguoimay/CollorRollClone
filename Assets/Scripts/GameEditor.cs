using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEditor : MonoBehaviour
{
    [SerializeField] GameObject carpetPrefab;

    [SerializeField] bool createNewCarpet = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (createNewCarpet)
        {
            createNewCarpet = false;
            Instantiate(carpetPrefab, transform);
        }
    }
}
