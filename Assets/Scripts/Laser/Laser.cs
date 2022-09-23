using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private bool isPlayers = false;
    [SerializeField] private bool hasExploded = false;


    private Vector2 bounds;
    private Vector3 direction;
    private Animator animator;
    private AudioSource audioSource;
    private Sprite originalSprite;
    

    void Start()
    {
        direction = isPlayers ? Vector3.up : Vector3.down;
        audioSource = GetComponent<AudioSource>();
        originalSprite = GetComponent<SpriteRenderer>().sprite;
        animator = GetComponent<Animator>();
        bounds = Game.GetBounds();
        gameObject.SetActive(false);
    }
    public Sprite GetSprite => originalSprite;
    public bool GetIsPlayers => isPlayers;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !isPlayers)
        {
            //GameOver
            Game.Instance.ChangeState(Game.State.GameOver);
        }
        else if (collision.gameObject.tag == "Enemy" && isPlayers)
        {
            collision.GetComponent<Enemy>().hitPoints--;
            Boom();
        }
        else if (collision.transform.parent.tag == "Obstacles")
            Boom();

            
    }

    void Update()
    {
        //If laser is out of the screen then disable it
        if (transform.position.y <= bounds.y * -1 || transform.position.y >= bounds.y)
            gameObject.SetActive(false);

        if (!hasExploded)
            transform.position += direction * speed * Time.deltaTime;
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Boom"))
        {
            float animTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            if (animTime > 1.0f)
            {
                GetComponent<SpriteRenderer>().sprite = originalSprite;
                hasExploded = false;
                gameObject.SetActive(false);
            }
        }
           
    }

    public void Boom()
    {
        if (!hasExploded)
        {
            animator.Play("Boom");
            //Play boom audio here
            hasExploded = true;
        }
    }
}
