using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowAndShrink : MonoBehaviour
{
    public enum JuicyState {
        Normal,
        Grow,
        Shrink,
    };
    JuicyState state = JuicyState.Normal;

    [SerializeField]
    public float GrowTo = 1.1f;
    [SerializeField]
    float ShrinkTo = 1f;
    [SerializeField]
    float Rate = 7f;
    [SerializeField]
    float JuiceTime = 3f;
    float timer;

    void Start ()
    {
        timer = JuiceTime;
    }

    // Update is called once per frame
    void Update ()
    {
        if (state == JuicyState.Normal)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                StartEffect();
                timer = JuiceTime;
            }
        }
        else if (state == JuicyState.Grow)
        {
            float newScale = Mathf.Min(GrowTo, this.transform.localScale.x + Time.deltaTime * Rate);
            this.transform.localScale = new Vector3(newScale, newScale, newScale);
            if (newScale == GrowTo)
            {
                state =  JuicyState.Shrink;
            }
        }
        else if (state == JuicyState.Shrink)
        {
            float newScale = Mathf.Max(ShrinkTo, this.transform.localScale.x - Time.deltaTime * Rate);
            this.transform.localScale = new Vector3(newScale, newScale, newScale);
            if (newScale == ShrinkTo)
            {
                state =  JuicyState.Normal;
            }
        }
    }

    public void StartEffect()
    {
        state = JuicyState.Grow;
    }
}
