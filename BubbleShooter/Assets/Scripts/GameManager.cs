using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Shooter shootScript;
    public Transform pointerToLastLine;

    // Bitişiğindeki balonları tutan liste
    [SerializeField] private List<Transform> bubbleSequence;

    private readonly int sequenceSize = 3;

    private void Start()
    {
        bubbleSequence = new List<Transform>();

        LevelManager.instance.GenerateLevel();

        shootScript.canShoot = true;
        shootScript.CreateNextBubble();
    }

    private void Update()
    {
        if (shootScript.canShoot
            && Input.GetKeyDown("space")
            && Camera.main.ScreenToWorldPoint(Input.mousePosition).y > shootScript.transform.position.y)
        {
            shootScript.canShoot = false;
            shootScript.Shoot();
        }
    }

    public void ProcessTurn(Transform currentBubble)
    {
        bubbleSequence.Clear();
        CheckBubbleSequence(currentBubble);

        // Eğer bağlantılı balon sayısı istenenden fazla bir sayıya
        // ulaştıysa balonları patlat ve bağlı olmayanları düşür
        if (bubbleSequence.Count >= sequenceSize)
        {
            PlayDestroyAnimation();
            Invoke(nameof(DestroyBubblesInSequence), 1.667f);
            Invoke(nameof(DropDisconectedBubbles), 1.667f);

        }

        LevelManager.instance.UpdateListOfBubblesInScene();

        shootScript.CreateNextBubble();
        shootScript.canShoot = true;
    }

    private void PlayDestroyAnimation()
    {
        foreach (var bubble in bubbleSequence)
        {
            var anim = bubble.GetComponent<Animator>();
            anim.SetBool(Destroy1, true);
        }
    }

    // Bitişiğindeki balonları ara ve bul
    private void CheckBubbleSequence(Transform currentBubble)
    {
        bubbleSequence.Add(currentBubble);

        var bubbleScript = currentBubble.GetComponent<Bubble>();
        var neighbors = bubbleScript.GetNeighbors();

        foreach (var t in neighbors)
            if (!bubbleSequence.Contains(t))
            {
                var bScript = t.GetComponent<Bubble>();

                if (bScript.bubbleColor == bubbleScript.bubbleColor) CheckBubbleSequence(t);
            }
    }

    private void DestroyBubblesInSequence()
    {
        foreach (var t in bubbleSequence)
        {
            Destroy(t.gameObject);
        }
    }

    // Bağlı olmayan balonları düşür
    private void DropDisconectedBubbles()
    {
        SetAllBubblesConnectionToFalse();
        SetConnectedBubblesToTrue();
        SetGravityToDisconectedBubbles();
    }

    #region Singleton

    public static GameManager instance;
    private static readonly int Destroy1 = Animator.StringToHash("Destroyed");

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    #endregion

    #region Drop Disconected Bubbles

    // Balonların bağlantılarını false olarak işaretle
    private void SetAllBubblesConnectionToFalse()
    {
        foreach (Transform bubble in LevelManager.instance.bubblesArea)
            bubble.GetComponent<Bubble>().isConnected = false;
    }

    private void SetConnectedBubblesToTrue()
    {
        bubbleSequence.Clear();

        var hits = Physics2D.RaycastAll(pointerToLastLine.position, pointerToLastLine.right, 15f);

        for (var i = 0; i < hits.Length; i++)
            if (hits[i].transform.gameObject.tag.Equals("Bubble"))
                SetNeighboursConnectionToTrue(hits[i].transform);
    }

    // Bitişiğindeki balonları bağlı olarak işaretle
    private void SetNeighboursConnectionToTrue(Transform bubble)
    {
        var bubbleScript = bubble.GetComponent<Bubble>();
        bubbleScript.isConnected = true;
        bubbleSequence.Add(bubble);

        foreach (var t in bubbleScript.GetNeighbors())
            if (!bubbleSequence.Contains(t))
                SetNeighboursConnectionToTrue(t);
    }

    
    // Bağlı olmayan balonlara yer çekimi ekle
    private void SetGravityToDisconectedBubbles()
    {
        foreach (Transform bubble in LevelManager.instance.bubblesArea)
            if (!bubble.GetComponent<Bubble>().isConnected)
            {
                bubble.gameObject.GetComponent<CircleCollider2D>().enabled = false;
                if (!bubble.GetComponent<Rigidbody2D>())
                {
                    var rb2d = bubble.gameObject.AddComponent(typeof(Rigidbody2D)) as Rigidbody2D;
                }
            }
    }

    #endregion
}