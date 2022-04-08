using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeField : MonoBehaviour
{
    Vector2 movement;

    [SerializeField]
    Sprite[] ShapeSprites = new Sprite[3];

    [SerializeField]
    AudioClip HitSound;
    [SerializeField]
    AudioClip PassSound;

    AudioSource audioSource;

    bool canScore = true;
    float minXPos = -20f;

    void Awake()
    {
        audioSource = GameObject.Find("SceneManager").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update () {
        if (transform.localPosition.x < minXPos)
            Destroy(this.gameObject);

        movement = new Vector2 (Globals.ScrollSpeed.x * Globals.ScrollDirection.x, 0);

        if (Globals.CurrentGameState == Globals.GameState.Playing)
        {
            // update score
            if(transform.localPosition.x < -3 && canScore)
            {
                canScore = false;
                Globals.CurrentScore++;
                audioSource.PlayOneShot(PassSound, 1f);
            }
        }
        if (Globals.CurrentGameState == Globals.GameState.Dead)
        {
            //drop everything off screen
            GetComponent<Rigidbody2D>().gravityScale = 2;
            GetComponent<Rigidbody2D>().isKinematic = false;
        }
        if (Globals.CurrentGameState == Globals.GameState.Score)
        {
            Destroy(this.gameObject);
        }
    }

    void FixedUpdate()
    {
        if (Globals.CurrentGameState == Globals.GameState.Playing)
            GetComponent<Rigidbody2D>().velocity = movement;
    }

    public void InitShapeField()
    {
        int type = Random.Range(0, 3);
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = ShapeSprites[type];
        this.gameObject.layer = Globals.BaseLayer + type;
    }

    // check if the played collided with a field
    void OnTriggerEnter2D(Collider2D collider)
    {
        Player player = collider.gameObject.GetComponent<Player>();
        if (player != null && Globals.CurrentGameState == Globals.GameState.Playing)
        {
            audioSource.PlayOneShot(HitSound, 1f);
            Camera camera = Camera.main;
            camera.GetComponent<CameraShake>().StartShake();
            Globals.CurrentGameState = Globals.GameState.Dead;
        }
    }
}
