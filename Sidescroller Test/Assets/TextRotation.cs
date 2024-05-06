using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextRotation : MonoBehaviour
{
    private float count;
    private float count1;
    private float count2;
    //because text starts at 0 degrees, we want it to turn the normal desired rotation, then from there on double it since it needs the desired rotation to
    //return back to 0, AND the desired rotation again to turn into the fourth quadrant. From then on we need desired rotation twice again to turn it to the 
    //2nd quadrant
    private bool firstRotate;
    //this is so rotation doesnt exponentially increase since it's being called on update
    private float countHinderance;
    [SerializeField] private float maxCount;
    [SerializeField] private float minCount;
    [SerializeField] private float num;
   
    // Start is called before the first frame update
    void Start()
    {
        count = 0;  
        count1 = 0;
        count2 = 0;
        firstRotate = true;
        countHinderance = maxCount;
        maxCount *= 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (count1 > maxCount - countHinderance && count2 == 0)
        {
            count = 0;
        }
        if (count1 > maxCount - countHinderance)
        {
            firstRotate = false;
        }
        if (count1 < maxCount - countHinderance)
        {
            count += num;
            count1 += num;
            transform.Rotate(0, 0, count);

        }
        else if (count2 < maxCount)
        {
            count -= num;
            count2 += num;
            transform.Rotate(0, 0, count);
        }
        //this is to make the text rotate past 0 into the negatives, and then rotate all the way back from the negatives to original first rotation
        if (firstRotate == false)
        {
            count1 += countHinderance;
            countHinderance = 0;
        }
        if (count1 > maxCount && count2 > maxCount)
        {
            count1 = 0;
            count2 = 0;
            count = 0;
        }
    }
}
