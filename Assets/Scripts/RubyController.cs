using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;
    public bool gameOver;
    public bool gameWin;
    
    public int maxHealth = 5;

    public int score;
    public int scoreAmount;
    public int cogs = 4;

    public GameObject projectilePrefab;
    public GameObject pickupEffect;

    public GameObject hitEffect;

    public AudioClip throwSound;
    public AudioClip hitSound;

    public Text scoreText;
    public Text winText;
    public Text cogsText;

    public static int staticLevel = 1;
    
    public int health { get { return currentHealth; }}
    int currentHealth;

    
    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;
    
    Rigidbody2D rigidbody2d;

    float horizontal;
    float vertical;

    Animator animator;
    Vector2 lookDirection = new Vector2(1,0);

    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody2d = GetComponent<Rigidbody2D>();

        currentHealth = maxHealth;

        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);

        cogsText.text = "Cogs: " + cogs;

        if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }

        if(Input.GetKeyDown(KeyCode.C))
        {
            if  (cogs >= 1)
            {
                Launch();
                cogs = cogs - 1;
            }
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f,
            lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    character.DisplayDialog();
                }

                if (scoreAmount >= 4)
                {
                    SceneManager.LoadScene("SceneTwo");
                    staticLevel = 2;
                }
            }
        }

        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }

        if (Input.GetKey(KeyCode.R))
        {
            if (gameWin == true)
            {
                SceneManager.LoadScene("MainGame");
            }

            else if (gameWin == false && gameOver == true)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }


        if (currentHealth <= 0)
        {
            gameOver = true;
            speed = 0;
            winText.text = "You Lost! Press R to restart";
        }

    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;
        
        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            animator.SetTrigger("Hit");
            if (isInvincible)
                return;

                isInvincible = true;
                invincibleTimer = timeInvincible;
                
                GameObject healthDecrease = Instantiate(hitEffect, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
                animator.SetTrigger("Hit");
                PlaySound(hitSound);
        }

        
        if (amount > 0)
        {
            Instantiate(pickupEffect, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }

    public void ChangeScore(int score)
    {
        scoreAmount = scoreAmount + score;
        scoreText.text = "Fixed Robots: " + scoreAmount.ToString();

        if (staticLevel == 1)
        {
            if (scoreAmount == 4)
            {
             winText.text = "Talk to Jambi to visit stage two!";
            }
        }

        if (staticLevel == 2)
        {
           if (scoreAmount == 4)
           {
            gameWin = true;
            winText.text = "You Win! Game created by Derek Justus! Press R to play again!";
            speed = 0.0f;
           }

        }
    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");

        PlaySound(throwSound);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Ammo")
        {
            cogs = cogs + 3;
            Destroy(other.gameObject);
        }
    }
}
