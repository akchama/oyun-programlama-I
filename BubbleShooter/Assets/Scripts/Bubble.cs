using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    // Balon renkleri
    public enum BubbleColor
    {
        BLUE,
        YELLOW,
        RED,
        GREEN
    }

    public float raycastRange = 0.7f;
    public float raycastOffset = 0.51f;

    public bool isFixed;
    public bool isConnected;

    public BubbleColor bubbleColor;

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bubble" && collision.gameObject.GetComponent<Bubble>().isFixed)
            if (!isFixed)
                HasCollided();

        if (collision.gameObject.tag == "Limit")
            if (!isFixed)
                HasCollided();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
    }

    private void HasCollided()
    {
        var rb = GetComponent<Rigidbody2D>();
        Destroy(rb);
        isFixed = true;
        
        // Balonların arasına ekle
        LevelManager.instance.SetAsBubbleAreaChild(transform);
        
        
        GameManager.instance.ProcessTurn(transform);
    }

    public List<Transform> GetNeighbors()
    {
        var hits = new List<RaycastHit2D>();
        var neighbors = new List<Transform>();

        hits.Add(Physics2D.Raycast(new Vector2(transform.position.x - raycastOffset, transform.position.y),
            Vector3.left, raycastRange));
        hits.Add(Physics2D.Raycast(new Vector2(transform.position.x + raycastOffset, transform.position.y),
            Vector3.right, raycastRange));
        hits.Add(Physics2D.Raycast(
            new Vector2(transform.position.x - raycastOffset, transform.position.y + raycastOffset),
            new Vector2(-1f, 1f), raycastRange));
        hits.Add(Physics2D.Raycast(
            new Vector2(transform.position.x - raycastOffset, transform.position.y - raycastOffset),
            new Vector2(-1f, -1f), raycastRange));
        hits.Add(Physics2D.Raycast(
            new Vector2(transform.position.x + raycastOffset, transform.position.y + raycastOffset),
            new Vector2(1f, 1f), raycastRange));
        hits.Add(Physics2D.Raycast(
            new Vector2(transform.position.x + raycastOffset, transform.position.y - raycastOffset),
            new Vector2(1f, -1f), raycastRange));

        foreach (var hit in hits)
            if (hit.collider != null && hit.transform.tag.Equals("Bubble"))
                neighbors.Add(hit.transform);

        return neighbors;
    }
}