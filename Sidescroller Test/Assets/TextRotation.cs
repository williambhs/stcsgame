using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextRotation : MonoBehaviour
{
    private float count;
    [SerializeField] private float maxCount;
    [SerializeField] private float minCount;
    [SerializeField] private float num;
    // Start is called before the first frame update
    void Start()
    {
        count = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (count < maxCount)
        {
            count += num;
            transform.Rotate(0, 0, count);

        }
        else if (count > minCount)
        {

            count -= num;
            transform.Rotate(0, 0, count);
        }
    }
}
