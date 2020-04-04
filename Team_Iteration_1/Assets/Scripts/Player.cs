﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float nukeCD = 15f;
    public float nukeStart = 0f;
    public bool nuked = false;
    public float blankCD = 3f;
    public float blankStart = 0f;
    public bool blanked = false;
    public float forceCD = 1.5f;
    public float forceStart = 0f;
    public bool forced = false;
    public Animator animator;
    Vector2 direction;
    Vector2 mousePosition;
    float horDir;
    float verDir;
    Vector2 horMove = new Vector2(1f,0f);
    Vector2 verMove = new Vector2(0f,1f);
    public Rigidbody2D player;
    public float speed;
    public Transform firePoint;
    public LineRenderer lr;
    public static int hitPoints = 20;

    public AudioClip shooting;
    public AudioClip enemyDeath;
    // Start is called before the first frame update
    void Start()
    {
        hitPoints = 20;
        lr.enabled = false;
        GetComponent<AudioSource>().clip = shooting;
        GetComponent<AudioSource>().clip = enemyDeath;
        GetComponent<AudioSource>().playOnAwake = false;
    }

    // Update is called once per frame
    void Update()
    {

        mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        horDir = Input.GetAxisRaw("Horizontal");
        verDir = Input.GetAxisRaw("Vertical");
        Aiming();
        StartCoroutine(Shooting());
        Moving();
        EndGame();
        StartCoroutine(Nuke());
        StartCoroutine(Blank());
        StartCoroutine(Force());
    }
    IEnumerator Nuke()
    {
        if (Input.GetKeyDown("e"))
        {
            if(Time.time > nukeStart + nukeCD)
            {
                if (nukeCD > 10)
                {
                    nukeCD--;
                }
                nuked = true;
                yield return 0;
                nuked = false;
                nukeStart = Time.time;
            }
        }
    }
    IEnumerator Blank()
    {
        if (Input.GetKeyDown("space"))
        {
            if(Time.time > blankStart + blankCD)
            {
                if(blankCD < 7)
                {
                    blankCD++;
                }
                blanked = true;
                yield return 0;
                blanked = false;
                blankStart = Time.time;
            }
        }
    }
    IEnumerator Force()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (Time.time > forceStart + forceCD)
            {
                forced = true;
                forceStart = Time.time;
                yield return 0;
                forced = false;
            }
        }
    }
    void Aiming()
    {
        direction = new Vector2(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y);

        transform.up = direction;
    }
    IEnumerator Shooting()
    {
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Shoot");
           RaycastHit2D hitInfo = Physics2D.Raycast(firePoint.position, firePoint.up);
            GetComponent<AudioSource>().PlayOneShot(shooting);
            if (hitInfo)
            {
                lr.SetPosition(0, firePoint.position);
                lr.SetPosition(1, hitInfo.point);


               enemyA enemyA = hitInfo.transform.GetComponent<enemyA>();
                if( enemyA != null)
                {
                    enemyA.DestroySelf();
                    hitPoints++;
                    GetComponent<AudioSource>().PlayOneShot(enemyDeath);
                }
                enemyB enemyB = hitInfo.transform.GetComponent<enemyB>();
                if (enemyB != null)
                {
                    enemyB.DestroySelf();
                    hitPoints++;
                    GetComponent<AudioSource>().PlayOneShot(enemyDeath);
                }
                bullets bullet = hitInfo.transform.GetComponent<bullets>();
                if (bullet != null)
                {
                    bullet.DestroySelf();
                }

            }
            else
            {
                lr.SetPosition(0, firePoint.position);
                lr.SetPosition(1, firePoint.position + firePoint.up * 100);
            }
            lr.enabled = true;
            yield return 0.02f;
            lr.enabled = false;

        }
    }
    void Moving()
    {

        player.position += (horMove * horDir * speed * Time.deltaTime);
        player.position += (verMove * verDir * speed * Time.deltaTime);
    }

    public void TakeDamage()
    {
        hitPoints-= 4;
    }

    void EndGame()
    {
        if(hitPoints <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
