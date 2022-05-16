using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public Transform gunSprite;
    public bool canShoot;
    public float speed = 12f;
    public float rotationSpeed = 0.2f;

    public Transform nextBubblePosition;
    public GameObject currentBubble;
    public GameObject nextBubble;

    public void Update()
    {
        if (Input.GetKey("left")) gunSprite.Rotate(0.0f, 0.0f, rotationSpeed);

        if (Input.GetKey("right")) gunSprite.Rotate(0.0f, 0.0f, rotationSpeed * -1);
    }

    public void Shoot()
    {
        transform.rotation = gunSprite.rotation;
        currentBubble.transform.rotation = transform.rotation;
        currentBubble.GetComponent<Rigidbody2D>().AddForce(currentBubble.transform.up * speed, ForceMode2D.Impulse);
        currentBubble = null;
    }

    [ContextMenu("CreateNextBubble")]
    public void CreateNextBubble()
    {
        var bubblesInScene = LevelManager.instance.bubblesInScene;
        var colors = LevelManager.instance.colorsInScene;

        if (nextBubble == null)
        {
            nextBubble = InstantiateNewBubble(bubblesInScene);
        }
        else
        {
            if (!colors.Contains(nextBubble.GetComponent<Bubble>().bubbleColor.ToString()))
            {
                Destroy(nextBubble);
                nextBubble = InstantiateNewBubble(bubblesInScene);
            }
        }

        if (currentBubble == null)
        {
            currentBubble = nextBubble;
            currentBubble.transform.position = new Vector2(transform.position.x, transform.position.y);
            nextBubble = InstantiateNewBubble(bubblesInScene);
        }
    }

    private GameObject InstantiateNewBubble(List<GameObject> bubblesInScene)
    {
        var newBubble =
            Instantiate(bubblesInScene[(int) (Random.Range(0, bubblesInScene.Count * 1000000f) / 1000000f)]);
        newBubble.transform.position = new Vector2(nextBubblePosition.position.x, nextBubblePosition.position.y);
        newBubble.GetComponent<Bubble>().isFixed = false;
        var rb2d = newBubble.AddComponent(typeof(Rigidbody2D)) as Rigidbody2D;
        rb2d.gravityScale = 0f;

        return newBubble;
    }
}