using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    Sprite[] ShapeSprites = new Sprite[3];

    [SerializeField]
    AudioClip ChangeSound;

    AudioSource audioSource;

    bool change = false;

    enum ShapeState {
        Square,
        Circle,
        Triangle
    }
    ShapeState currentState = ShapeState.Square;

    void Awake()
    {
        audioSource = GameObject.Find("SceneManager").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        change = Input.GetKeyDown ("space") | Input.GetButtonDown ("Fire1") | Input.GetButtonDown ("Fire2");
        if (Globals.CurrentGameState == Globals.GameState.Playing && change)
        {
            ChangeState();
        }
    }

    public void ChangeState()
    {
        audioSource.PlayOneShot(ChangeSound, 1f);
        currentState = (int)currentState == 2 ? ShapeState.Square : (ShapeState)((int)currentState + 1);
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = ShapeSprites[(int)currentState];
        this.gameObject.layer = Globals.BaseLayer + (int)currentState;
    }
}
